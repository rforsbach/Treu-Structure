namespace Canguro.Commands.Forms
{
    partial class ReportOptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportOptionsDialog));
            this.optionsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.onlySelectedCheckBox = new System.Windows.Forms.CheckBox();
            this.authorLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.authorTextBox = new System.Windows.Forms.TextBox();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.checkAllLink = new System.Windows.Forms.LinkLabel();
            this.checkNoneLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // optionsCheckedListBox
            // 
            this.optionsCheckedListBox.CheckOnClick = true;
            resources.ApplyResources(this.optionsCheckedListBox, "optionsCheckedListBox");
            this.optionsCheckedListBox.FormattingEnabled = true;
            this.optionsCheckedListBox.Name = "optionsCheckedListBox";
            this.optionsCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.optionsCheckedListBox_SelectedIndexChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // onlySelectedCheckBox
            // 
            resources.ApplyResources(this.onlySelectedCheckBox, "onlySelectedCheckBox");
            this.onlySelectedCheckBox.Name = "onlySelectedCheckBox";
            this.onlySelectedCheckBox.UseVisualStyleBackColor = true;
            // 
            // authorLabel
            // 
            resources.ApplyResources(this.authorLabel, "authorLabel");
            this.authorLabel.Name = "authorLabel";
            // 
            // titleLabel
            // 
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.Name = "titleLabel";
            // 
            // authorTextBox
            // 
            resources.ApplyResources(this.authorTextBox, "authorTextBox");
            this.authorTextBox.Name = "authorTextBox";
            // 
            // titleTextBox
            // 
            resources.ApplyResources(this.titleTextBox, "titleTextBox");
            this.titleTextBox.Name = "titleTextBox";
            // 
            // checkAllLink
            // 
            resources.ApplyResources(this.checkAllLink, "checkAllLink");
            this.checkAllLink.Name = "checkAllLink";
            this.checkAllLink.TabStop = true;
            this.checkAllLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.checkAllLink_LinkClicked);
            // 
            // checkNoneLink
            // 
            resources.ApplyResources(this.checkNoneLink, "checkNoneLink");
            this.checkNoneLink.Name = "checkNoneLink";
            this.checkNoneLink.TabStop = true;
            this.checkNoneLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.checkNoneLink_LinkClicked);
            // 
            // ReportOptionsDialog
            // 
            this.AcceptButton = this.OKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.checkNoneLink);
            this.Controls.Add(this.checkAllLink);
            this.Controls.Add(this.titleTextBox);
            this.Controls.Add(this.authorTextBox);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.authorLabel);
            this.Controls.Add(this.onlySelectedCheckBox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.optionsCheckedListBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportOptionsDialog";
            this.Load += new System.EventHandler(this.ReportOptionsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox optionsCheckedListBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.CheckBox onlySelectedCheckBox;
        private System.Windows.Forms.Label authorLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TextBox authorTextBox;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.LinkLabel checkAllLink;
        private System.Windows.Forms.LinkLabel checkNoneLink;
    }
}