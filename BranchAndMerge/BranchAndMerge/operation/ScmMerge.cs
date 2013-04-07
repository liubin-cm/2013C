using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BranchAndMerge.lib;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Text.RegularExpressions;
using log4net;

namespace BranchAndMerge.operation
{
    class ScmMerge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScmMerge));
        
        public ScmMerge(string collectionUrl, string[] mainlineParent, string cycleCode)
        {
            this.ctpc = new CTfsTeamProjectCollection(collectionUrl);
            this.cvc = new CTfsVersionControl(this.ctpc);
            string collectionName = collectionUrl.Substring(collectionUrl.LastIndexOf("/")+1);
            this.ctws = this.cvc.GetWorkSpace(@"d:\merge\" + collectionName);
            if (this.ctws == null)
            {
                this.cvc.CreateWorkSpace(collectionName, @"d:\merge\" + collectionName);
                this.ctws = this.cvc.CWorkSpace;
            }
            this.mainlineParent = mainlineParent;
            this.cycleCode = cycleCode;
        }

        public string MergeAllProj()
        {
            string result = string.Empty;
            foreach (string item in mainlineParent)
            {
                try
                {
                    List<string> tagPaths = MergeReleaseToMainlineOfProj(item + "/mainline");
                    if (tagPaths.Count > 1)
                    {
                        this.cvc.CreateLabel(item.Substring(item.LastIndexOf("/") + 1) + "release" + this.cycleCode, tagPaths.ToArray(), "for release " + this.cycleCode);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
            }
            foreach (var items in warninglog)
            {
                result += items;
            }
            return result;
        }

        public List<string> MergeReleaseToMainlineOfProj(string mainlinePath)
        {
            List<string> tagContents = new List<string>();
            if (this.cvc.IsBranch(mainlinePath))
            {
                GetMergedPath(mainlinePath, tagContents);
            }
            else
            {
                string[] childrenOfMainline = this.cvc.GetItems(mainlinePath);
                foreach (var childOfMainline in childrenOfMainline)
                {
                    if (this.cvc.IsBranch(childOfMainline))
                    {
                        GetMergedPath(childOfMainline,tagContents);
                    }
                }
            }
            tagContents.Add(mainlinePath);
            return tagContents;
        }

        private void GetMergedPath(string mainlinePath, List<string> tagContents)
        {
            string releasePath = MergeOneDir(mainlinePath);
            if (!releasePath.Equals(string.Empty))
            {
                tagContents.Add(releasePath);
            }
        }

        private string MergeOneDir(string mainlinePath)
        {
            List<string> childBranches = this.cvc.GetAllChildBranches(mainlinePath);
            foreach (var child in childBranches)
            {
                if (Regex.IsMatch(child, @".*R" + this.cycleCode + "/.*") || 
                    Regex.IsMatch(child, @".*Release/" + this.cycleCode + "$") || 
                    Regex.IsMatch(child, @".*Release" + this.cycleCode + "/.*"))
                {
                    this.cvc.SetCheckinPermission(child, false);
                    this.ctws.Undo(this.ctws.GetLocalItemForServerItme(mainlinePath));
                    this.ctws.Get(new string[] { mainlinePath }, VersionSpec.Latest);
                    int conflict = this.ctws.Merge(child, this.ctws.GetLocalItemForServerItme(mainlinePath));
                    if (conflict == 0 )
                    {
                        log.Info(child + " is equal to " + mainlinePath);
                        if (!IsMainlineEqualRelease(this.ctws.GetLocalItemForServerItme(mainlinePath), child))
                        {
                            log.Warn(mainlinePath + "there is a little different with " + child);
                            warninglog.Add(mainlinePath + "there is a little different with " + child);
                        }
                        this.ctws.CheckIn(this.ctws.GetLocalItemForServerItme(mainlinePath), "merge " + this.cycleCode);
                        this.cvc.SetCheckinPermission(mainlinePath, true);
                        log.Info(mainlinePath + " merge success!");
                        return child;
                    }
                    else
                    {
                        this.cvc.SetCheckinPermission(child, true);
                        log.Error(child + " and " + mainlinePath + " have conflicts, please check and solve!");
                    }
                    break;
                }
            }
            log.Warn(mainlinePath + "没有相应周期的release分支！");
            return string.Empty;
        }

        public bool IsMainlineEqualRelease(string mainline, string release)
        {
            string output = this.ctws.FolderDiff(release, mainline);
            Match mt = Regex.Match(output, @"Summary:.*folders,.*files, 0 source, 0 target, 0 different, 0 with errors");
            if (mt.Value != string.Empty)
            {
                return true;
            }
            return false;
        }

        private CTfsTeamProjectCollection ctpc;
        private CTfsVersionControl cvc;
        private CTfsWorkSpace ctws;
        private string[] mainlineParent;
        private string cycleCode;
        public List<string> warninglog = new List<string>();
    }
}
