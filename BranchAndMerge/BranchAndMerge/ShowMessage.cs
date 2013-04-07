using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BranchAndMerge
{
    public partial class ShowMessage : Form
    {
        public ShowMessage(string message)
        {
            InitializeComponent();
            this.messageTextBox.Text = message;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
