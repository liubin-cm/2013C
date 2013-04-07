using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Server;

namespace BranchAndMerge.lib
{

    class CTfsTeamProject
    {
        private VersionControlServer version;
        private IGroupSecurityService vag;
        private Identity[] appGroups;
        private Identity memberInfo;

        /// <summary>
        /// 获取VersionControlServer对象
        /// </summary>
        /// <param name="configurationServer"></param>
        public CTfsTeamProject(TfsTeamProjectCollection configurationServer)
        {
            version = configurationServer.GetService(typeof(VersionControlServer)) as VersionControlServer;
        }

        /// <summary>
        /// 获取文件的详细信息
        /// </summary>
        /// <param name="projectName">file目录</param>
        /// <returns>file的属性、类型等详细信息</returns>
        public Item GetProjectsItemSet(string projectName)
        {
            Item items = version.GetItem(projectName, VersionSpec.Latest,
                DeletedState.NonDeleted, GetItemsOptions.IncludeBranchInfo);
            return items;  
        }

        /// <summary>
        /// 根据project的名字获取工程的详细信息
        /// </summary>
        /// <param name="projectName">project的名字</param>
        /// <returns>TeamProject对象</returns>
        public TeamProject GetProjectDetailsByName(string projectName)
        {
            if (projectName != string.Empty)
            {
                return version.TryGetTeamProject(projectName);
            }
            else
                return null;
        }
        
        /// <summary>
        /// 获取工程的group membership
        /// </summary>
        /// <param name="configurationServer">TfsTeamProjectCollection对象</param>
        /// <returns>Identity类型数组</returns>
        public Identity[] GetApplicationGroup(TfsTeamProjectCollection configurationServer)
        {
            List<TeamProject> teamProjects = new List<TeamProject>(version.GetAllTeamProjects(false));
            vag = configurationServer.GetService<IGroupSecurityService>();
            
            foreach (TeamProject teamProject in teamProjects)
            {
                appGroups = vag.ListApplicationGroups(teamProject.ArtifactUri.AbsoluteUri);
            }
            return appGroups;
        }

        /// <summary>
        /// 获取工程的group membership中group里的成员信息
        /// </summary>
        /// <param name="appGroups">Identity类型数组</param>
        /// <returns></returns>
        public List<GroupMember> GetGroupMM(Identity[] appGroups)
        {
            GroupMember groupMM = new GroupMember();
            List<GroupMember> groupMCollection = new List<GroupMember>();
            foreach (Identity group in appGroups)
            {
                Identity[] groupMembers = vag.ReadIdentities(SearchFactor.Sid, new string[] { group.Sid }, QueryMembership.Expanded);

                foreach (Identity member in groupMembers)
                {
                    GroupMembership groupM = new GroupMembership { GroupName = member.DisplayName, GroupSid = member.Sid };

                    if (member.Members != null)
                    {
                        
                        foreach (string memberSid in member.Members)
                        {
                            memberInfo = vag.ReadIdentity(SearchFactor.Sid, memberSid, QueryMembership.Expanded);
                            
                            groupMM.MemberName = memberInfo.AccountName;
                            groupMM.MemberSid = memberInfo.Sid;
                            groupMM.Domain = memberInfo.Domain;
                            groupMM.Email = memberInfo.MailAddress;
                        }
                        groupMCollection.Add(groupMM);
                    }
                }
            }
            return groupMCollection;

        }
        
        /// <summary>
        /// 获取成员在某个工程中的权限值
        /// </summary>
        /// <param name="teamProject">TeamProject对象</param>
        /// <param name="memeber">GroupMember对象</param>
        /// <returns></returns>
        public List<VersionControlPermission> GetGroupMMPermission(TeamProject teamProject, GroupMember memeber)
        {
            List<VersionControlPermission> versionControlPermissions = new List<VersionControlPermission>();
            if (memeber != null)
            {
                string userName = memeber.Domain + "\\" + memeber.MemberName;
                string[] permissions = version.GetEffectivePermissions(userName, teamProject.ServerItem);

                foreach (var permission in permissions)
                {
                    versionControlPermissions.Add(new VersionControlPermission() { Name = permission });
                }
            }
            return versionControlPermissions;
        }

        /// <summary>
        /// 获取工程所有的策略名称
        /// </summary>
        /// <param name="teamProject">teamProject对象</param>
        /// <returns>策略名称数组</returns>
        public string[] GetCheckinPolicies(TeamProject teamProject)
        {
            PolicyEnvelope[] strPolicy = teamProject.GetCheckinPolicies();
            List<string> strList = new List<string>();
            for (int i = 0; i < strPolicy.Length; i++)
            {
                strList.Add(strPolicy[i].Policy.ToString());
            }
            return strList.ToArray();
        }

        /// <summary>
        /// 设置策略
        /// </summary>
        /// <param name="teamProject">teamProject对象</param>
        /// <param name="policies"></param>
        public void SetCheckinPolicies(TeamProject teamProject, PolicyEnvelope[] policies)
        {
            teamProject.SetCheckinPolicies(policies);
        }

        /// <summary>
        /// 清除策略
        /// </summary>
        /// <param name="teamProject">teamProject对象</param>
        public void ClearCheckinPolicies(TeamProject teamProject)
        {
            teamProject.SetCheckinPolicies(null);
        }

        public class GroupMembership
        {
            public string GroupName { get; set; }
            public string GroupSid { get; set; }
            public List<GroupMember> GroupMember { get; set; }
        }

        public class VersionControlPermission
        {
            public string Name { get; set; }
        }

        public class GroupMember
        {
            public string MemberName { get; set; }
            public string MemberSid { get; set; }
            public string Domain { get; set; }
            public string Email { get; set; }
            public List<VersionControlPermission> VersionControlPermissions { get; set; }
        }

    }
}
