// -----------------------------------------------------------------------
// <copyright file="CTfsWorkSpace.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BranchAndMerge.lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using log4net;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CTfsWorkSpace
    {
        private Workspace workSpace;
        private static readonly ILog log = LogManager.GetLogger(typeof(CTfsWorkSpace));

        //private VersionControlServer vcs;
        public CTfsWorkSpace(Workspace ws)
        {
            this.workSpace = ws;
            //this.vcs = ws.VersionControlServer;
        }

        /// <summary>
        /// 获取最新代码
        /// </summary>
        /// <param name="Paths">可以是本地路径, 也可以是服务器路径, 服务器路径需要是映射路径</param>
        /// <param name="versionSpec"></param>
        public void Get(string[] Paths, VersionSpec versionSpec)
        {
            if (Paths.Length > 0)
            {
                GetStatus gs = this.workSpace.Get(Paths, versionSpec, RecursionType.Full, GetOptions.GetAll | GetOptions.Overwrite);
                if (gs.NumFailures > 0)
                {
                    Failure[] fls = gs.GetFailures();
                    foreach (var item in fls)
                    {
                        log.Error(item.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 合并操作
        /// </summary>
        /// <param name="sourcePath">源路径,可以是服务器路径</param>
        /// <param name="targetPath">目标路径,只能是本地路径</param>
        /// <returns>如果merge没有冲突, 则返回true,否则返回false</returns>
        public int Merge(string sourcePath, string targetPath)
        {
            GetStatus gs = this.workSpace.Merge(sourcePath, targetPath, null, null, LockLevel.None, RecursionType.Full, MergeOptions.None);
            return gs.NumConflicts;
        }

        /// <summary>
        /// check in变更集
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="comment"></param>
        public void CheckIn(string localPath, string comment)
        {
            PendingChange[] pc = this.workSpace.GetPendingChanges(localPath, RecursionType.Full);
            if (pc.Length > 0)
            {
                this.workSpace.CheckIn(pc, comment);
            }
        }

        /// <summary>
        /// 前置条件需要先执行GetWorkSpace或者CreateWorkSpace
        /// </summary>
        /// <param name="sourcePath">源路径，可以是服务器或者本地路径, 如果sourcePath为空, 则本地路径与服务器最新版本相比较</param>
        /// <param name="targetPath">目标路径，为本地路径</param>
        /// <returns>返回比较结果，与命令行一致</returns>
        public string FolderDiff(string sourcePath, string targetPath)
        {
            CProcess cProcess = new CProcess();
            string para = string.Empty;
            if (sourcePath.Equals(string.Empty))
            {
                para = " folderdiff \"" + targetPath + "\" /recursive";
            }
            else
            {
                para = " folderdiff \"" + sourcePath + "\" \"" + targetPath + "\" /recursive";
            }
            cProcess.Run("tf", para , targetPath);
            if (cProcess.HasError)
            {
                throw new Exception("FolderDiff:\r\nsourcePath:" + sourcePath + "\r\ntargetPath:" + targetPath + "\r\n" + cProcess.Error);
            }

            return cProcess.Output;
        }

        /// <summary>
        /// 获得服务器路径的本地路径
        /// </summary>
        /// <param name="serverItem">服务器路径</param>
        /// <returns>本地路径</returns>
        public string GetLocalItemForServerItme(string serverItem)
        {
            return this.workSpace.GetLocalItemForServerItem(serverItem);
        }

        public void Undo(string localPath)
        {
            //CProcess cp = new CProcess();
            //cp.Run("tf", "undo /recursive /noprompt \"" + localPath + "\"", localPath);
            PendingChange[] pendingChanges = this.workSpace.GetPendingChanges(localPath, RecursionType.Full);
            if (pendingChanges.Length > 0)
            {
                int num = this.workSpace.Undo(pendingChanges);
            }
        }
    }
}
