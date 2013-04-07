

namespace BranchAndMerge.lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.Server;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    /// <summary>
    /// for team project collection
    /// </summary>
    public class CTfsTeamProjectCollection
    {
        /// <summary>
        /// TfsTeamProjectCollection实例对象
        /// </summary>
        private TfsTeamProjectCollection tfsTeamProjectCollection;

        /// <summary>
        /// 团队项目集合版本控制对象
        /// </summary>
        private VersionControlServer vcs;

        /// <summary>
        /// 团队项目集合权限用户组管理对象
        /// </summary>
        private IIdentityManagementService identityManagementService;

        /// <summary>
        /// 使用windows帐号验证
        /// </summary>
        /// <param name="uri">collection地址，如http://192.168.83.70:8080/tfs/system</param>
        public CTfsTeamProjectCollection(string uri)
        {
            Uri tfsUri = new Uri(uri);
            this.tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsUri);
            this.tfsTeamProjectCollection.EnsureAuthenticated();
            this.vcs = this.tfsTeamProjectCollection.GetService<VersionControlServer>();
            this.identityManagementService = this.tfsTeamProjectCollection.GetService<IIdentityManagementService>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uri">collection地址，如http://192.168.83.70:8080/tfs/system</param>
        /// <param name="username">用户名</param>
        /// <param name="passwd">密码</param>
        /// <param name="domain">域名</param>
        public CTfsTeamProjectCollection(string uri, string username, string passwd, string domain)
        {
            this.tfsTeamProjectCollection = new TfsTeamProjectCollection(new Uri(uri), new NetworkCredential(username, passwd, domain));
            this.tfsTeamProjectCollection.EnsureAuthenticated();
            this.vcs = this.tfsTeamProjectCollection.GetService<VersionControlServer>();
            this.identityManagementService = this.tfsTeamProjectCollection.GetService<IIdentityManagementService>();
        }

        /// <summary>
        /// 获得团队项目集合对象
        /// </summary>
        public TfsTeamProjectCollection TPC
        {
            get { return this.tfsTeamProjectCollection; }
        }

        /// <summary>
        /// 获得版本控制对象
        /// </summary>
        public VersionControlServer VCS
        {
            get { return this.vcs; }
        }

        /// <summary>
        /// 获取所有团队项目对象
        /// </summary>
        /// <returns></returns>
        public List<TeamProject> GetProjects()
        {
            List<TeamProject> teamProjects = new List<TeamProject>(this.vcs.GetAllTeamProjects(false));
            return teamProjects;
        }

        /// <summary>
        /// 获取某个团队项目对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TeamProject GetProject(string name)
        {
            List<TeamProject> teamProjects = this.GetProjects();
            for (int i = 0; i < teamProjects.Count; i++)
            {
                if (teamProjects[i].Name == name)
                {
                    return teamProjects[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get group's name list
        /// </summary>
        /// <returns></returns>
        public List<string> GetSecurityGroupNames()
        {
            List<string> groupNames = new List<string>();
            List<TeamFoundationIdentity> securityGroups = this.GetSecurityGroups();
            foreach (TeamFoundationIdentity item in securityGroups)
            {
                groupNames.Add(item.DisplayName);
            }
            return groupNames;
        }

        /// <summary>
        /// 获取组成员
        /// </summary>
        /// <param name="groupName">GUI中显示的名称</param>
        /// <returns>返回的是域用户的显示名称,而不是登录名称, 比如cn1\lbin的域用户名称为vlb刘斌</returns>
        public List<string> GetMembersofGroup(string groupName)
        {
            List<string> namesOfMembers = new List<string>();

            List<TeamFoundationIdentity> tfiSet = this.GetSecurityGroups();

            //通过先前vcs.GetAllTeamProjects只能获得组名,没有组成员信息
            TeamFoundationIdentity groupIdentity = this.GetSecurityGroup(groupName);
            TeamFoundationIdentity groupIdentityWithMembers = this.identityManagementService.ReadIdentity(
                                                                                        IdentitySearchFactor.AccountName,
                                                                                        groupIdentity.DisplayName, 
                                                                                        MembershipQuery.Direct, 
                                                                                        ReadIdentityOptions.None);
            if (!groupIdentityWithMembers.IsContainer)
            {
                return null;
            }

            TeamFoundationIdentity[] identityObjectsOfMembers = this.identityManagementService.ReadIdentities(groupIdentityWithMembers.Members, 
                                                                                                MembershipQuery.Direct, 
                                                                                                ReadIdentityOptions.None);
            foreach (TeamFoundationIdentity member in identityObjectsOfMembers)
                namesOfMembers.Add(member.DisplayName);
            return namesOfMembers;
        }

        /// <summary>
        /// refrence http://msdn.microsoft.com/zh-cn/library/ff734144.aspx
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="memberNames">只能增加cn1域名或者已定义的TFS群组, 使用的名称为登录名</param>
        public void AddMembersToGroup(string groupName, string[] memberNames, bool flag=true)
        {
            foreach (var member in memberNames)
            {
                TeamFoundationIdentity id = this.GetSecurityGroup(member);
                string searchValue;
                if (id != null)
                {
                    searchValue = id.DisplayName;
                }
                else
                {
                    searchValue = "cn1\\" + member;
                }

                TeamFoundationIdentity identity = this.identityManagementService.ReadIdentity(IdentitySearchFactor.AccountName, searchValue, MembershipQuery.None, ReadIdentityOptions.None);
                if (flag)
                {
                    this.identityManagementService.AddMemberToApplicationGroup(this.GetSecurityGroup(groupName).Descriptor, identity.Descriptor);
                }
                else
                {
                    this.identityManagementService.RemoveMemberFromApplicationGroup(this.GetSecurityGroup(groupName).Descriptor, identity.Descriptor);
                }
            }
        }

        /// <summary>
        /// 从组中删除用户
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="memberNames"></param>
        public void DeleteMembersToGroup(string groupName, string[] memberNames)
        {
            AddMembersToGroup(groupName, memberNames, false);
        }


        /// <summary>
        /// Get TeamFoundationIdentity objects of project's groups
        /// </summary>
        /// <returns></returns>
        private List<TeamFoundationIdentity> GetSecurityGroups()
        {
            List<TeamFoundationIdentity> allSecurityGroups = new List<TeamFoundationIdentity>();

            ////第一个参数为空,则是团队项目集合,如果是团队项目名称,则是团队项目
            TeamFoundationIdentity[] tfiSet = this.identityManagementService.ListApplicationGroups(string.Empty, ReadIdentityOptions.TrueSid);

            foreach (TeamFoundationIdentity item in tfiSet)
            {
                allSecurityGroups.Add(item);
            }
            return allSecurityGroups;
        }

        /// <summary>
        /// Get TeamFoundationIdentity object by group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private TeamFoundationIdentity GetSecurityGroup(string groupName)
        {
            List<TeamFoundationIdentity> securityGroups = this.GetSecurityGroups();
            foreach (TeamFoundationIdentity item in securityGroups)
            {
                string fullGroupName = item.DisplayName;
                if (fullGroupName.Substring(fullGroupName.LastIndexOf("\\") + 1) == groupName)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
