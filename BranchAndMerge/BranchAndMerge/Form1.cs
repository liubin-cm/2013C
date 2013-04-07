using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Common;
using BranchAndMerge.lib;
using System.Text.RegularExpressions;
using BranchAndMerge.operation;

namespace BranchAndMerge
{
    public partial class Form1 : Form
    {
        private CTfsFileSupport cfs = new CTfsFileSupport();
        private string[] projectNames;
        private string tfsserver;
        private bool checkunmergeflag = false;

        public Form1()
        {
            InitializeComponent();
        }
       
        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\config";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.configFileTextBox.Text = openFileDialog1.FileName;
                this.ListBoxInit();
            }           
        }

        private void Mergebutton_Click(object sender, EventArgs e)
        {
            if (!this.InputValidationCheck())
            {
                return;
            }
            List<string> projectMainlineParent = this.SelectedItemsConvertToArray(this.projectlistBox.SelectedItems, "$/");

            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要合并" + this.tfsserver + " " + this.cycleCodeComboBox.Text.Trim() + "的分支吗?", "确认", messButton);
            if (dr == DialogResult.OK)
            {
                ScmMerge sm = new ScmMerge(this.tfsserver, projectMainlineParent.ToArray(), this.cycleCodeComboBox.Text.Trim());
                string result = sm.MergeAllProj();
                if (result == string.Empty)
                    MessageBox.Show("合并结束，请查看日志文件！");
                else
                    MessageBox.Show("存在如下的warning：\r\n" + result);
            }
        }

        private bool InputValidationCheck()
        {
            if (this.cycleCodeComboBox.Text.Trim().Equals(string.Empty))//!Regex.IsMatch(this.cycleCodeComboBox.Text.Trim(), @"\d{3}_\d{4}$")) 
            {
                MessageBox.Show("请填写周期数字!格式如098_1214");
                return false;
            }
            if (this.projectlistBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择合并的项目!");
                return false;
            }
            return true;
        }

        private void BranchButton_Click(object sender, EventArgs e)
        {
            if (!this.InputValidationCheck())
            {
                return;
            }
            int releasenumber = int.Parse(this.cycleCodeComboBox.Text.Trim().Substring(0, this.cycleCodeComboBox.Text.Trim().IndexOf('_')));
            int last = releasenumber - 1;
            string lastreleasenumber = last.ToString() + "_" + this.cycleCodeComboBox.Text.Trim().Substring(this.cycleCodeComboBox.Text.Trim().LastIndexOf("_") + 1);
            string result = GetCheckUnmergedChangeSets(lastreleasenumber, checkunmergeflag);
            if (result == string.Empty)
            {
                MessageBox.Show("不存在未合并的变更集！");
            }
            else
            {
                var mergeMessageForm = new ShowMessage("存在未合并的变更集，请先merge以下的变更:\r\n" + result);
                mergeMessageForm.ShowDialog();
            }

            List<string> projectMainlineParent = SelectedItemsConvertToArray(this.projectlistBox.SelectedItems);
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要开出" + this.tfsserver + " "+ this.cycleCodeComboBox.Text.Trim() + "的分支吗?", "确认", messButton);
            if (dr == DialogResult.OK)
            {
                operation.ScmBranch cb = new operation.ScmBranch(projectMainlineParent.ToArray(), this.tfsserver);
                string CreateBranchResult = cb.CreateBranchOperation(this.cycleCodeComboBox.Text.Trim());
                if (CreateBranchResult == string.Empty)
                    MessageBox.Show("所有工程创建分支成功！");
                else
                    MessageBox.Show("创建分支不成功，详细如下：\r\n" + CreateBranchResult);
            }
        }

        private List<string> SelectedItemsConvertToArray(ListBox.SelectedObjectCollection selectionItems, string preStr="", string postStr ="")
        {
            List<string> projectMainlineParent = new List<string>();
            foreach (var item in selectionItems)
            {
                projectMainlineParent.Add(preStr + item.ToString() + postStr);
            }
            return projectMainlineParent;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime dt = new DateTime(2013, 3, 13);
            int cycleInit = 92;
            DateTime now = DateTime.Now;
            int interValue = (now - dt).Days/7;

            for (int i = -1; i < 2; i++)
            {
                DateTime futureDt = dt + new TimeSpan(7 * (interValue + i), 0, 0, 0);
                int cycleCode = cycleInit + interValue + i;
                this.cycleCodeComboBox.Items.Add(CycleToStr(cycleCode) + "_" + MonthDay(futureDt.Month, futureDt.Day));
            }
            //this.cycleCodeComboBox.Items.Add(cycleCodeStr + "_" + (dt + new TimeSpan(7*,0,0,0)));
        }

        private void ListBoxInit()
        {
            if (File.Exists("\"" + this.configFileTextBox.Text.Trim() + "\""))
            {
                MessageBox.Show(this.configFileTextBox.Text.Trim() + "文件不存在!");
                return;
            }

            this.projectNames = this.cfs.GetProjectNames(this.configFileTextBox.Text.Trim());
            this.tfsserver = this.cfs.tfsserverurl;
            this.projectlistBox.Items.Clear();
            this.projectlistBox.Items.AddRange(this.projectNames);
        }

        private string CycleToStr(int cycleCode)
        {
            if (cycleCode < 100)
            {
                return "0" + cycleCode.ToString();
            }
            return cycleCode.ToString();
        }

        private string MonthDay(int month, int day)
        {
            string monthStr = string.Empty;
            string dayStr = string.Empty;
            if (month < 10)
            {
                monthStr = "0" + month.ToString();
            }
            else
            {
                monthStr = month.ToString();
            }
            if (day < 10)
            {
                dayStr = "0" + day.ToString();
            }
            else
            {
                dayStr = day.ToString();
            }
            return monthStr + dayStr;
        }

        private void configFileTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ListBoxInit();
        }

        private void checkUnmergedChangeSetsButton_Click(object sender, EventArgs e)
        {
            if (!this.InputValidationCheck())
            {
                return;
            }
            string result = GetCheckUnmergedChangeSets(this.cycleCodeComboBox.Text.Trim(), this.checkBox1.Checked);
            if (result == string.Empty)
            {
                MessageBox.Show("不存在未合并的变更集！");
            }
            else
            {
                var mergeMessageForm = new ShowMessage("存在未合并的变更集，请先merge以下的变更:\r\n" + result);
                mergeMessageForm.ShowDialog();
            }
        }

        private string GetCheckUnmergedChangeSets(string cycleCode, bool checkunmergeflag)
        {
            List<string> projectMainlineParent = this.SelectedItemsConvertToArray(this.projectlistBox.SelectedItems);
            List<string> changestsResult = new List<string>();
            string result = string.Empty;
            operation.ScmBranch cb = new operation.ScmBranch(projectMainlineParent.ToArray(), this.tfsserver);
            foreach (var item in projectMainlineParent)
            {
                changestsResult.Add(cb.CheckLatestReleaseChangests(item, cycleCode, checkunmergeflag));
            }

            foreach (var item in changestsResult)
            {
                result += item;
            }
            return result;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.Checked)
            {
                checkunmergeflag = true;
            }
        }
   }
}
