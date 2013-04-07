namespace BranchAndMerge
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cycleCodeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.configFileTextBox = new System.Windows.Forms.TextBox();
            this.fileSelectButton = new System.Windows.Forms.Button();
            this.Mergebutton = new System.Windows.Forms.Button();
            this.projectlistBox = new System.Windows.Forms.ListBox();
            this.projectLabel = new System.Windows.Forms.Label();
            this.BranchButton = new System.Windows.Forms.Button();
            this.checkUnmergedChangeSetsButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cycleCodeComboBox
            // 
            this.cycleCodeComboBox.FormattingEnabled = true;
            this.cycleCodeComboBox.Location = new System.Drawing.Point(78, 12);
            this.cycleCodeComboBox.Name = "cycleCodeComboBox";
            this.cycleCodeComboBox.Size = new System.Drawing.Size(103, 20);
            this.cycleCodeComboBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "周期";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "config";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "配置文件";
            // 
            // configFileTextBox
            // 
            this.configFileTextBox.Location = new System.Drawing.Point(78, 49);
            this.configFileTextBox.Name = "configFileTextBox";
            this.configFileTextBox.ReadOnly = true;
            this.configFileTextBox.Size = new System.Drawing.Size(293, 21);
            this.configFileTextBox.TabIndex = 6;
            this.configFileTextBox.TextChanged += new System.EventHandler(this.configFileTextBox_TextChanged);
            // 
            // fileSelectButton
            // 
            this.fileSelectButton.Location = new System.Drawing.Point(377, 49);
            this.fileSelectButton.Name = "fileSelectButton";
            this.fileSelectButton.Size = new System.Drawing.Size(25, 23);
            this.fileSelectButton.TabIndex = 7;
            this.fileSelectButton.Text = "..";
            this.fileSelectButton.UseVisualStyleBackColor = true;
            this.fileSelectButton.Click += new System.EventHandler(this.fileSelectButton_Click);
            // 
            // Mergebutton
            // 
            this.Mergebutton.Location = new System.Drawing.Point(325, 184);
            this.Mergebutton.Name = "Mergebutton";
            this.Mergebutton.Size = new System.Drawing.Size(76, 23);
            this.Mergebutton.TabIndex = 8;
            this.Mergebutton.Text = "合并";
            this.Mergebutton.UseVisualStyleBackColor = true;
            this.Mergebutton.Click += new System.EventHandler(this.Mergebutton_Click);
            // 
            // projectlistBox
            // 
            this.projectlistBox.FormattingEnabled = true;
            this.projectlistBox.ItemHeight = 12;
            this.projectlistBox.Location = new System.Drawing.Point(78, 83);
            this.projectlistBox.Name = "projectlistBox";
            this.projectlistBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.projectlistBox.Size = new System.Drawing.Size(151, 124);
            this.projectlistBox.TabIndex = 9;
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(18, 83);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(53, 12);
            this.projectLabel.TabIndex = 10;
            this.projectLabel.Text = "Projects";
            // 
            // BranchButton
            // 
            this.BranchButton.Location = new System.Drawing.Point(327, 155);
            this.BranchButton.Name = "BranchButton";
            this.BranchButton.Size = new System.Drawing.Size(75, 23);
            this.BranchButton.TabIndex = 11;
            this.BranchButton.Text = "开分支";
            this.BranchButton.UseVisualStyleBackColor = true;
            this.BranchButton.Click += new System.EventHandler(this.BranchButton_Click);
            // 
            // checkUnmergedChangeSetsButton
            // 
            this.checkUnmergedChangeSetsButton.Location = new System.Drawing.Point(327, 126);
            this.checkUnmergedChangeSetsButton.Name = "checkUnmergedChangeSetsButton";
            this.checkUnmergedChangeSetsButton.Size = new System.Drawing.Size(75, 23);
            this.checkUnmergedChangeSetsButton.TabIndex = 12;
            this.checkUnmergedChangeSetsButton.Text = "合并检查";
            this.checkUnmergedChangeSetsButton.UseVisualStyleBackColor = true;
            this.checkUnmergedChangeSetsButton.Click += new System.EventHandler(this.checkUnmergedChangeSetsButton_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(206, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(60, 16);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "源路径";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 221);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkUnmergedChangeSetsButton);
            this.Controls.Add(this.BranchButton);
            this.Controls.Add(this.projectLabel);
            this.Controls.Add(this.projectlistBox);
            this.Controls.Add(this.Mergebutton);
            this.Controls.Add(this.fileSelectButton);
            this.Controls.Add(this.configFileTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cycleCodeComboBox);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scm流程";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cycleCodeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox configFileTextBox;
        private System.Windows.Forms.Button fileSelectButton;
        private System.Windows.Forms.Button Mergebutton;
        private System.Windows.Forms.ListBox projectlistBox;
        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.Button BranchButton;
        private System.Windows.Forms.Button checkUnmergedChangeSetsButton;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

