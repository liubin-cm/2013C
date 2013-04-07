// -----------------------------------------------------------------------
// <copyright file="CTfsVersionControl.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BranchAndMerge.lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CTfsVersionControl
    {
        private VersionControlServer vcs;
        private CTfsWorkSpace cWorkSpace;
         
        public CTfsWorkSpace CWorkSpace
        {
            get { return cWorkSpace; }
        }

        public CTfsVersionControl(CTfsTeamProjectCollection ctpc)
        {
            this.vcs = ctpc.VCS;
        }

        /// <summary>
        /// 创建分支
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="version">有五个子类,分别代表LabelVersionSpec, 时间DateVersionSpec, 最新LatestVersionSpec, 工作空间WorkspaceVersionSpec, 变更集版本ChangesetVersionSpec</param>
        public int CreateBranch(
                                string sourcePath,
                                string targetPath,
                                VersionSpec version
                                )
        {   
            ////CreateBranchObject类似GUI中的转换为分支
            ////this.vcs.CreateBranchObject(new BranchProperties(new ItemIdentifier(sourcePath)));
            try
            {
                return this.vcs.CreateBranch(sourcePath, targetPath, version);
            }
            catch (Exception e)
            {
                throw new Exception("创建分支失败:" + e.Message + "\r\n源路径:" +  sourcePath + "\r\n目录路径:" + targetPath);
            }             
        }

        /// <summary>
        /// 创建工作空间, 只能是映射团队项目集合的根目录
        /// </summary>
        /// <param name="workSpaceName">工作空间名字</param>
        /// <param name="localPath">本地绝对路径</param>
        public void CreateWorkSpace(string workSpaceName, string localPath)
        {
            WorkingFolder[] mappings = new WorkingFolder[] { new WorkingFolder("$\\", localPath) };
            Workspace workSpace = this.vcs.CreateWorkspace(workSpaceName, this.vcs.AuthorizedUser, string.Empty, mappings);
            this.cWorkSpace = new CTfsWorkSpace(workSpace);
        }

        /// <summary>
        /// 判断服务器路径是否为
        /// </summary>
        /// <param name="serverPath">服务器路径</param>
        /// <returns></returns>
        public bool IsBranch(string serverPath)
        {
            try
            {
                Item item = vcs.GetItem(serverPath, VersionSpec.Latest, DeletedState.Any, GetItemsOptions.IncludeBranchInfo);
                return item.IsBranch;
            }
            catch (Exception e)
            {
                throw new Exception("Function IsBranch Error: " + e.Message);
            }
        }

        /// <summary>
        /// 根据本地路径获取工作空间
        /// </summary>
        /// <param name="localPath">工作空间本地路径</param>
        /// <returns></returns>
        public CTfsWorkSpace GetWorkSpace(string localPath)
        {
            try
            {
                Workspace ws = vcs.GetWorkspace(localPath);
                return new CTfsWorkSpace(ws);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定目录下的item集合
        /// </summary>
        /// <param name="serverPath">服务器端绝对路径</param>
        /// <returns></returns>
        public string[] GetItems(string serverPath)
        {
            ItemSet items = this.vcs.GetItems(serverPath, VersionSpec.Latest, RecursionType.OneLevel, DeletedState.NonDeleted, ItemType.Any);
            if (items.Items.Length > 1)
            {
                List<string> itemsname = new List<string>();
                for (int i = 1; i < items.Items.Length; i++)
                {
                    itemsname.Add(items.Items[i].ServerItem);
                }
                return itemsname.ToArray();
            }
            else
            {
                return null;
            }
        }

        public Item GetItem(string serverPath)
        {
           return this.vcs.GetItem(serverPath);
        }

        public PermissionChange[] GetPermission(string serverPath)
        {
            ItemSecurity[] itemSecurities = this.vcs.GetPermissions(new string[] { serverPath }, RecursionType.None);
            if (itemSecurities.Length != 1)
            {
                throw new Exception(serverPath + "权限获取失败. ");
            }

            AccessEntry[] accessEntries = itemSecurities[0].Entries;
            List<PermissionChange> permissionChangeList = new List<PermissionChange>();
            foreach (AccessEntry item in accessEntries)
            {
                string[] allow = CombileTwoArray(item.Allow, item.AllowInherited);
                string[] deny = CombileTwoArray(item.Deny, item.DenyInherited);
                PermissionChange pc = new PermissionChange(serverPath, item.IdentityName, allow, deny, null);
                permissionChangeList.Add(pc);
            }

            return permissionChangeList.ToArray();
        }

        /// <summary>
        /// add or move checkin permission
        /// </summary>
        /// <param name="serverPath">文件路径</param>
        /// <param name="flag">true:add checkin；false:move checkin</param>
        public void SetCheckinPermission(string serverPath, bool flag)
        {
            bool inherit = IsInherit(serverPath);
            string[] defaultgroup = new string[]{"Project Administrators", "Project Collection Administrators",
                                                "Project Collection Build Service Accounts", "Project Collection Service Accounts",
                                                "ReadAll", "TRDC", "Builders", "Contributors", "Readers"};
            if (inherit)
            {
                SetInherit(serverPath, false);
            }
            ItemSecurity[] itemSecurities = this.vcs.GetPermissions(new string[] { serverPath }, RecursionType.None);
            AccessEntry[] accessEntries = itemSecurities[0].Entries;

            foreach (AccessEntry item in accessEntries)
            {
                if (!defaultgroup.Contains(item.IdentityName.Substring(item.IdentityName.LastIndexOf("\\") + 1)))
                {
                   if (flag)
                   { vcs.SetPermissions(new SecurityChange[] { new PermissionChange(serverPath, item.IdentityName, new string[] { "Checkin" }, null, null) }); }
                    else
                   { vcs.SetPermissions(new SecurityChange[] { new PermissionChange(serverPath, item.IdentityName, null, null, new string[] { "Checkin" }) }); }
                }
            }
        }

        private string[] CombileTwoArray(string[] a, string[] b)
        {
            string[] c = new string[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }

        /// <summary>
        /// 设置添加\删除继承
        /// </summary>
        /// <param name="serverPath">文件的路径</param>
        /// <param name="inherit">true:添加继承；false:删除继承</param>
        public void SetInherit(string serverPath, bool inherit)
        {
            if (serverPath != string.Empty)
            {
                PermissionChange[] pc = GetPermission(serverPath);
                SecurityChange[] sc = this.vcs.SetPermissions(new SecurityChange[] { new InheritanceChange(serverPath, inherit) });
                if (!inherit)
                {
                    this.vcs.SetPermissions(pc);
                }
            }
        }

        /// <summary>
        /// 判断某个目录是否继承
        /// </summary>
        /// <param name="serverPath">目录的路径</param>
        /// <returns>true:继承则返回;false:不继承则返回</returns>
        public bool IsInherit(string serverPath)
        {
            ItemSecurity[] itemSecurities = this.vcs.GetPermissions(new string[] { serverPath }, RecursionType.None);
            if (itemSecurities.Length != 0)
            {
                return itemSecurities[0].Inherit;
            }
            else
            {
                throw new Exception(serverPath + "权限列表为空");
            }
        }

        /// <summary>
        /// 创建label
        /// </summary>
        /// <param name="labelName">label的名字，具有唯一性，必填项</param>
        /// <param name="itemSpecArray">打标签的filepath 字符串数组</param>
        /// <param name="labelComment">label的注释</param>
        /// <returns>true:创建成功返回；false:创建失败返回</returns>
        public bool CreateLabel(string labelName, string[] itemSpecArray, string labelComment)
        {
            if (labelName != string.Empty && itemSpecArray.Length != 0)
            {
                VersionControlLabel labelToCreate = new VersionControlLabel(this.vcs, labelName, null, null, labelComment);

                LabelItemSpec[] labelItemSpecs = new LabelItemSpec[itemSpecArray.Length];
                for (int i = 0; i < itemSpecArray.Length; i++)
                {
                    ItemSpec itemSpec = new ItemSpec(itemSpecArray[i], RecursionType.Full);
                    labelItemSpecs[i] = new LabelItemSpec(itemSpec, VersionSpec.Latest, false);
                }

                LabelResult[] result = vcs.CreateLabel(labelToCreate, labelItemSpecs, LabelChildOption.Replace);
                if (result.Length > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获得某个分支的所有子分支路径
        /// </summary>
        /// <param name="parentBranch">父分支服务器端路径</param>
        /// <returns></returns>
        public List<string> GetAllChildBranches(string parentBranch)
        {
            var i = new ItemIdentifier(parentBranch);
            var branches = new List<string>();
            if (IsBranch(parentBranch))
            {
                var allBranches = this.vcs.QueryBranchObjects(i, RecursionType.None);

                foreach (var branchObject in allBranches)
                {
                    foreach (var childBranche in branchObject.ChildBranches)
                    {
                        if (!childBranche.IsDeleted)
                        {
                            branches.Add(childBranche.Item);
                        }
                    }
                }
            }
            return branches;
        }

        private static void DisplayAllBranches(BranchObject bo, VersionControlServer vcs)
        {
            //0.Prepare display indentation
            for (int tabcounter = 0; tabcounter < recursionlevel; tabcounter++)
                Console.Write("\t");
            //1.Display the current branch
            Console.WriteLine(string.Format("{0}", bo.Properties.RootItem.Item));

            //2.Query all child branches (one level deep)
            BranchObject[] childBos = vcs.QueryBranchObjects(bo.Properties.RootItem, RecursionType.OneLevel);

            //3.Display all children recursively
            recursionlevel++;
            foreach (BranchObject child in childBos)
            {
                if (child.Properties.RootItem.Item == bo.Properties.RootItem.Item)
                    continue;

                DisplayAllBranches(child, vcs);
            }
            recursionlevel--;
        }

        private static int recursionlevel = 0;

        public class SecurityGroup
        {
            string groupName;
            string[] permissions;

            public string GroupName
            {
                get { return this.groupName; }
            }
            public string[] Permissions
            {
                get { return permissions; }
            }
            public SecurityGroup(string group, string[] permissions)
            {
                this.groupName = group;
                this.permissions = permissions;
            }
        }

        public class ItemPermission
        {
            public static string Read
            {
                get { return PermissionChange.ItemPermissionRead; }
            }

            public static string CheckOut
            {
                get { return PermissionChange.ItemPermissionPendChange; }
            }

            public static string Checkin
            {
                get { return PermissionChange.ItemPermissionCheckin; }
            }

            public static string Label
            {
                get { return PermissionChange.ItemPermissionLabel; }
            }

            public static string Lock
            {
                get { return PermissionChange.ItemPermissionLock; }
            }

            public static string Reviseother
            {
                get { return PermissionChange.ItemPermissionReviseOther; }
            }

            public static string Unlockother
            {
                get { return PermissionChange.ItemPermissionUnlockOther; }
            }

            public static string Undoother
            {
                get { return PermissionChange.ItemPermissionUndoOther; }
            }

            public static string Administerlabels
            {
                get { return PermissionChange.ItemPermissionLabelOther; }
            }

            public static string ManagePermissions
            {
                get { return PermissionChange.ItemPermissionAdminProjectRights; }
            }

            public static string Checkinother
            {
                get { return PermissionChange.ItemPermissionCheckinOther; }
            }

            public static string Merge
            {
                get { return PermissionChange.ItemPermissionMerge; }
            }

            public static string ManageBranch
            {
                get { return PermissionChange.ItemPermissionManageBranch; }
            }
        }
    }
}
