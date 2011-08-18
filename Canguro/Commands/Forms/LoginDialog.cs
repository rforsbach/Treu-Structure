using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Controller;

namespace Canguro.Commands.Forms
{
    public partial class LoginDialog : Form
    {
        private Credentials credentials;

        public LoginDialog(CommandServices services)
        {
            credentials = services.UserCredentials;
            InitializeComponent();
        }

        private void LoginDialog_Load(object sender, EventArgs e)
        {
            usernameTextBox.Text = credentials.UserName;
            passwordTextBox.Text = credentials.Password;
            rememberMeCheckBox.Checked = (!string.IsNullOrEmpty(credentials.UserName) && !string.IsNullOrEmpty(credentials.UserName));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            credentials.UserName = usernameTextBox.Text;
            credentials.Password = passwordTextBox.Text;

            if (rememberMeCheckBox.Checked)
            {
                credentials.Save();
                credentials.Load();
            }
            else
                credentials.DeleteFile();

            if (credentials.Authenticate())
                DialogResult = DialogResult.OK;
            else
                failedLabel.Visible = true;
        }

        private void registerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.RegistrationURL);
            //start.UseShellExecute = true;
            //System.Diagnostics.Process.Start(start);
            RegistrationForm dlg = new RegistrationForm();
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                usernameTextBox.Text = dlg.Email;
                passwordTextBox.Text = dlg.Password;
            }
        }
    }
}