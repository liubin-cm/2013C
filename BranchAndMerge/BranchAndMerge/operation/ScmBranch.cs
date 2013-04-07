

namespace BranchAndMerge.operation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using BranchAndMerge.lib;
    using System.Text.RegularExpressions;
    using log4net;
    using log4net.Config;

    class ScmBranch
    {
        public string[] projectnames;
        private string tfsserver;

        private CTfsTeamProjectCollection ctpc;
        private CTfsVersionControl ctvc;

        private static readonly ILog log = LogManager.GetLogger(typeof(ScmBranch));

        public ScmBranch(string[] projectNames, string tfsserverurl)
        {
            this.projectnames = projectNames;
            this.tfsserver = tfsserverurl;
            this.ctpc = new CTfsTeamProjectCollection(tfsserver);
            this.ctvc = new CTfsVersionControl(ctpc);
        }

        public string CreateBranchOperation(string cyclecode)
        {
            List<string> result = new List<string>();
            string failresult = null;
            foreach (var mailineParent in this.projectnames)
            {
                try
                {
                    string mainlinePath = "$/" + mailineParent + "/mainline";
                    if (this.ctvc.IsBranch(mainlinePath))
                    {
                        BranchAtomicOperation("$/" + mailineParent + "/Release" + cyclecode, mainlinePath);
                    }
                    else
                    {
                        string[] branchs = GetBranchesUnderMainline(mainlinePath);
                        if (branchs.Length == 1)
                        {
                            if (branchs[0] == "$/Booking/MainLine/Pub")
                            {
                                BranchAtomicOperation("$/" + mailineParent + "/R" + cyclecode + "/pub", branchs[0]);
                            }
                            else
                            {
                                BranchAtomicOperation("$/" + mailineParent + "/Release/" + cyclecode, branchs[0]);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < branchs.Length; j++)
                            {
                                string chileItem = branchs[j].Substring(branchs[j].LastIndexOf("/") + 1);
                                if (mailineParent == "AffiliateMarketing")
                                {
                                    BranchAtomicOperation("$/" + mailineParent + "/R" + cyclecode + "/" + chileItem, branchs[j]);
                                }
                                else
                                {
                                    BranchAtomicOperation("$/" + mailineParent + "/Release" + cyclecode + "/" + chileItem, branchs[j]);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    result.Add(mailineParent);
                    log.Error(mailineParent + "branch failed!\r\nDetails: " + e.Message);
                }
            }
            foreach (var items in result)
            {
                failresult += items;
            }
            return failresult;
        }

        public void BranchAtomicOperation(string targetPath, string sourcePath)
        {
            try
            {
                ctvc.SetCheckinPermission(sourcePath, false);
                int number = ctvc.CreateBranch(sourcePath, targetPath, VersionSpec.Latest);
                ctvc.SetCheckinPermission(targetPath, true); 
                log.Info("Branch from" + sourcePath + " to "+ targetPath + " has been done successfully.");
            }
            catch (Exception e)
            {
                log.Error("Branch " + sourcePath +  " failed. \r\nDetails: " + e.Message);
            }
        }

        public string[] GetBranchesUnderMainline(string sourcePath)
        {
            string[] itemsnames = ctvc.GetItems(sourcePath);
            List<string> itembranch = new List<string>();
            for (int i = 0; i < itemsnames.Length; i++)
            {
                if (ctvc.IsBranch(itemsnames[i]))
                {
                    itembranch.Add(itemsnames[i]);
                }
            }
            return itembranch.ToArray();
        }

        public string GetLatestReleasePath(string[] releasePath, string cycleCode)
        {
            int releasenumber = int.Parse(cycleCode.Substring(0, cycleCode.IndexOf('_')));
            for (int i = 0; i < releasePath.Length; i++)
            {
                if (Regex.IsMatch(releasePath[i], releasenumber.ToString() + "_" + @"\d{4,}" + ".*"))
                {
                    return releasePath[i];
                }
             }
            return string.Empty;
        }

        public MergeCandidate[] GetLatestReleaseChangests(string sourcePath, string cycleCode, bool checkunmergeflag)
        {
            List<string> releasePath = this.ctvc.GetAllChildBranches(sourcePath);
            string tagetPath = GetLatestReleasePath(releasePath.ToArray(), cycleCode);
            if (tagetPath != string.Empty)
            {
                if (checkunmergeflag)
                {
                    MergeCandidate[] mc = this.ctpc.VCS.GetMergeCandidates(tagetPath, sourcePath, RecursionType.Full);
                    return mc;
                }
                else
                {
                    MergeCandidate[] mc = this.ctpc.VCS.GetMergeCandidates(sourcePath, tagetPath, RecursionType.Full);
                    return mc;
                }
            }
            return new MergeCandidate[] { };
        }

        public string CheckLatestReleaseChangests(string projectNames, string cycleCode, bool checkunmergeflag)
        {
            string mainlinePath = "$/" + projectNames + "/mainline";
            List<string> changestsResult = new List<string>();
            string result = string.Empty;
            if (this.ctvc.IsBranch(mainlinePath))
            {
                changestsResult.Add(GetUnCheckinChangestsInfo(GetLatestReleaseChangests(mainlinePath, cycleCode, checkunmergeflag)));
            }
            else
            {
                string[] branchs = GetBranchesUnderMainline(mainlinePath);
                for (int i = 0; i < branchs.Length; i++)
                {
                    changestsResult.Add(GetUnCheckinChangestsInfo(GetLatestReleaseChangests(branchs[i], cycleCode, checkunmergeflag)));
                }
            }
            foreach (var item in changestsResult)
                result += item;
            return result;
        }

        public string GetUnCheckinChangestsInfo(MergeCandidate[] mc)
        {
            List<string> changestsResult = new List<string>();
            string result = string.Empty;
            for (int k = 0; k < mc.Length; k++)
            {
                changestsResult.Add("Date: "+ mc[k].Changeset.CreationDate.ToString()+"\r\n");
                changestsResult.Add("Owner: " + mc[k].Changeset.Owner + "\r\n");
                changestsResult.Add("Comment: " + mc[k].Changeset.Comment + "\r\n");
                Changeset changesets = this.ctpc.VCS.GetChangeset(mc[k].Changeset.ChangesetId);
                foreach(var item in changesets.Changes)
                {
                    changestsResult.Add("File: " + item.Item.ServerItem + "\r\n"); 
                }
                changestsResult.Add("\r\n"); 
            }
            foreach (var item in changestsResult)
                result += item;
            return result;
        }
    }
}
