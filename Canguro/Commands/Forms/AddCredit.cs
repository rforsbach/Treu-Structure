using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Commands.Forms
{
    public partial class AddCredit : Form
    {
        public AddCredit()
        {
            InitializeComponent();
        }

        private void buyCreditLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.BuyCreditURL);
            start.UseShellExecute = true;
            System.Diagnostics.Process.Start(start);
            this.Close();
        }

        private void addCreditLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.AddCreditURL);
            start.UseShellExecute = true;
            System.Diagnostics.Process.Start(start);
            this.Close();
        }
    }
}