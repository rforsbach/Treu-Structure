namespace Canguro.Commands.Forms
{
    partial class RegistrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistrationForm));
            this.introLabel = new System.Windows.Forms.Label();
            this.registerSerialCheckBox = new System.Windows.Forms.CheckBox();
            this.keyTextBox = new System.Windows.Forms.MaskedTextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.keyLabel = new System.Windows.Forms.Label();
            this.registerGroupBox = new System.Windows.Forms.GroupBox();
            this.registerUserCheckBox = new System.Windows.Forms.CheckBox();
            this.confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.confirmPasswordLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.EmailTextBox = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.keyGroupBox = new System.Windows.Forms.GroupBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.registerGroupBox.SuspendLayout();
            this.keyGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // introLabel
            // 
            resources.ApplyResources(this.introLabel, "introLabel");
            this.introLabel.Name = "introLabel";
            // 
            // registerSerialCheckBox
            // 
            resources.ApplyResources(this.registerSerialCheckBox, "registerSerialCheckBox");
            this.registerSerialCheckBox.Name = "registerSerialCheckBox";
            this.registerSerialCheckBox.UseVisualStyleBackColor = true;
            this.registerSerialCheckBox.CheckedChanged += new System.EventHandler(this.registerSerialCheckBox_CheckedChanged);
            // 
            // keyTextBox
            // 
            resources.ApplyResources(this.keyTextBox, "keyTextBox");
            this.keyTextBox.Name = "keyTextBox";
            this.keyTextBox.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.maskedTextBox1_MaskInputRejected);
            this.keyTextBox.Leave += new System.EventHandler(this.keyTextBox_Leave);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // keyLabel
            // 
            resources.ApplyResources(this.keyLabel, "keyLabel");
            this.keyLabel.Name = "keyLabel";
            // 
            // registerGroupBox
            // 
            this.registerGroupBox.Controls.Add(this.registerUserCheckBox);
            this.registerGroupBox.Controls.Add(this.confirmPasswordTextBox);
            this.registerGroupBox.Controls.Add(this.passwordTextBox);
            this.registerGroupBox.Controls.Add(this.lastNameTextBox);
            this.registerGroupBox.Controls.Add(this.nameTextBox);
            this.registerGroupBox.Controls.Add(this.confirmPasswordLabel);
            this.registerGroupBox.Controls.Add(this.passwordLabel);
            this.registerGroupBox.Controls.Add(this.lastNameLabel);
            this.registerGroupBox.Controls.Add(this.nameLabel);
            resources.ApplyResources(this.registerGroupBox, "registerGroupBox");
            this.registerGroupBox.Name = "registerGroupBox";
            this.registerGroupBox.TabStop = false;
            // 
            // registerUserCheckBox
            // 
            resources.ApplyResources(this.registerUserCheckBox, "registerUserCheckBox");
            this.registerUserCheckBox.Checked = true;
            this.registerUserCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.registerUserCheckBox.Name = "registerUserCheckBox";
            this.registerUserCheckBox.UseVisualStyleBackColor = true;
            this.registerUserCheckBox.CheckedChanged += new System.EventHandler(this.registerUserCheckBox_CheckedChanged);
            // 
            // confirmPasswordTextBox
            // 
            resources.ApplyResources(this.confirmPasswordTextBox, "confirmPasswordTextBox");
            this.confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            this.confirmPasswordTextBox.UseSystemPasswordChar = true;
            this.confirmPasswordTextBox.Leave += new System.EventHandler(this.confirmPasswordTextBox_Leave);
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.Leave += new System.EventHandler(this.passwordTextBox_Leave);
            // 
            // lastNameTextBox
            // 
            resources.ApplyResources(this.lastNameTextBox, "lastNameTextBox");
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Leave += new System.EventHandler(this.lastNameTextBox_Leave);
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // confirmPasswordLabel
            // 
            resources.ApplyResources(this.confirmPasswordLabel, "confirmPasswordLabel");
            this.confirmPasswordLabel.Name = "confirmPasswordLabel";
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.Name = "passwordLabel";
            // 
            // lastNameLabel
            // 
            resources.ApplyResources(this.lastNameLabel, "lastNameLabel");
            this.lastNameLabel.Name = "lastNameLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            resources.ApplyResources(this.EmailTextBox, "EmailTextBox");
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Leave += new System.EventHandler(this.EmailTextBox_Leave);
            // 
            // emailLabel
            // 
            resources.ApplyResources(this.emailLabel, "emailLabel");
            this.emailLabel.Name = "emailLabel";
            // 
            // keyGroupBox
            // 
            this.keyGroupBox.Controls.Add(this.keyLabel);
            this.keyGroupBox.Controls.Add(this.registerSerialCheckBox);
            this.keyGroupBox.Controls.Add(this.keyTextBox);
            resources.ApplyResources(this.keyGroupBox, "keyGroupBox");
            this.keyGroupBox.Name = "keyGroupBox";
            this.keyGroupBox.TabStop = false;
            // 
            // errorLabel
            // 
            this.errorLabel.ForeColor = System.Drawing.Color.DarkRed;
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.Name = "errorLabel";
            // 
            // RegistrationForm
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.introLabel);
            this.Controls.Add(this.registerGroupBox);
            this.Controls.Add(this.keyGroupBox);
            this.Controls.Add(this.emailLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationForm";
            this.ShowInTaskbar = false;
            this.registerGroupBox.ResumeLayout(false);
            this.registerGroupBox.PerformLayout();
            this.keyGroupBox.ResumeLayout(false);
            this.keyGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label introLabel;
        private System.Windows.Forms.CheckBox registerSerialCheckBox;
        private System.Windows.Forms.MaskedTextBox keyTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label keyLabel;
        private System.Windows.Forms.GroupBox registerGroupBox;
        private System.Windows.Forms.TextBox confirmPasswordTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox EmailTextBox;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label confirmPasswordLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.GroupBox keyGroupBox;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.CheckBox registerUserCheckBox;
    }
}