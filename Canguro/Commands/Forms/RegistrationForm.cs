using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Canguro.Commands.Forms
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            errorLabel.Text = "";
            errorLabel.Visible = false;
            registerSerialCheckBox.Checked = false;
            UpdateDialog();
        }

        private void UpdateDialog()
        {
            keyLabel.Enabled = registerSerialCheckBox.Checked;
            keyTextBox.Enabled = registerSerialCheckBox.Checked;

            bool regUser = registerUserCheckBox.Checked;
            nameLabel.Enabled = regUser;
            nameTextBox.Enabled = regUser;
            lastNameLabel.Enabled = regUser;
            lastNameTextBox.Enabled = regUser;
            passwordLabel.Enabled = regUser;
            passwordTextBox.Enabled = regUser;
            confirmPasswordLabel.Enabled = regUser;
            confirmPasswordTextBox.Enabled = regUser;
        }

        /// <summary>
        /// Validates all fields. 
        /// Returns an error string if any field is invalid.
        /// Returns string.Empty if everything is OK.
        /// </summary>
        /// <returns></returns>
        private string ValidateData()
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (!re.IsMatch(EmailTextBox.Text))
                return Culture.Get("RegistrationErrorInvalidEmail");
            if (registerUserCheckBox.Checked)
            {
                if (string.IsNullOrEmpty(nameTextBox.Text))
                    return Culture.Get("RegistrationErrorNameIsEmpty");
                if (string.IsNullOrEmpty(lastNameTextBox.Text))
                    return Culture.Get("RegistrationErrorLastNameIsEmpty");
                if (!passwordTextBox.Text.Equals(confirmPasswordTextBox.Text))
                    return Culture.Get("RegistrationErrorPasswordMismatch");
            }

            if (registerSerialCheckBox.Checked && !keyTextBox.MaskCompleted)
                return Culture.Get("RegistrationErrorInvalidKey");

            return string.Empty;
        }

        private bool Register()
        {
            try
            {
                CanguroServer.Analysis ws = new CanguroServer.Analysis();

                string res = "";
                if (registerUserCheckBox.Checked)
                    res = ws.RegisterUser(EmailTextBox.Text, passwordTextBox.Text, nameTextBox.Text, lastNameTextBox.Text);

                if (string.IsNullOrEmpty(res))
                {
                    if (registerSerialCheckBox.Checked)
                    {
                        res = ws.AddSellFromSerial(EmailTextBox.Text, keyTextBox.Text);
                        if (!string.IsNullOrEmpty(res))
                        {
                            errorLabel.Text = Culture.Get(res);
                            errorLabel.Visible = true;
                            return false;
                        }
                    }
                }
                else
                {
                    errorLabel.Text = Culture.Get(res);
                    errorLabel.Visible = true;
                    return false;
                }                    

                return true;
            }
            catch (Exception ex)
            {
                errorLabel.Text = Culture.Get("RegistrationErrorServerError");
            }
            return false;
        }

        private void registerSerialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDialog();
        }

        private void registerUserCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDialog();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string err = ValidateData();
            if (string.IsNullOrEmpty(err))
            {
                if (Register())
                    DialogResult = DialogResult.OK;
                else
                    DialogResult = DialogResult.None;
            }
            else
            {
                errorLabel.Text = err;
                errorLabel.Visible = true;
                DialogResult = DialogResult.None;
            }
        }

        private void nameTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                errorLabel.Text = Culture.Get("RegistrationErrorNameIsEmpty");
                errorLabel.Visible = true;
            }
            else
                errorLabel.Visible = false;
        }

        private void lastNameTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastNameTextBox.Text))
            {
                errorLabel.Text = Culture.Get("RegistrationErrorLastNameIsEmpty");
                errorLabel.Visible = true;
            }
            else
                errorLabel.Visible = false;
        }

        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (!re.IsMatch(EmailTextBox.Text))
            {
                errorLabel.Text = Culture.Get("RegistrationErrorInvalidEmail");
                errorLabel.Visible = true;
            }
            else
                errorLabel.Visible = false;
        }

        private void passwordTextBox_Leave(object sender, EventArgs e)
        {
            errorLabel.Visible = false;
        }

        private void confirmPasswordTextBox_Leave(object sender, EventArgs e)
        {
            if (!passwordTextBox.Text.Equals(confirmPasswordTextBox.Text))
            {
                errorLabel.Text = Culture.Get("RegistrationErrorPasswordMismatch");
                errorLabel.Visible = true;
            }
            else
                errorLabel.Visible = false;
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            errorLabel.Text = Culture.Get("RegistrationErrorInvalidKey");
            errorLabel.Visible = true;
        }

        private void keyTextBox_Leave(object sender, EventArgs e)
        {
            if (keyTextBox.MaskCompleted)
                errorLabel.Visible = false;
            else
            {
                errorLabel.Text = Culture.Get("RegistrationErrorInvalidKey");
                errorLabel.Visible = true;
            }
        }

        internal string Email
        {
            get { return EmailTextBox.Text; }
        }

        internal string Password
        {
            get { return passwordTextBox.Text; }
        }        
    }
}