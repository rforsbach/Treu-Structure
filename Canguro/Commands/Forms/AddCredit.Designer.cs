namespace Canguro.Commands.Forms
{
    partial class AddCredit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCredit));
            this.creditLabel = new System.Windows.Forms.Label();
            this.addCreditButton = new System.Windows.Forms.Button();
            this.buyCreditLink = new System.Windows.Forms.LinkLabel();
            this.addCreditLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // creditLabel
            // 
            this.creditLabel.AccessibleDescription = null;
            this.creditLabel.AccessibleName = null;
            resources.ApplyResources(this.creditLabel, "creditLabel");
            this.creditLabel.Font = null;
            this.creditLabel.Name = "creditLabel";
            // 
            // addCreditButton
            // 
            this.addCreditButton.AccessibleDescription = null;
            this.addCreditButton.AccessibleName = null;
            resources.ApplyResources(this.addCreditButton, "addCreditButton");
            this.addCreditButton.BackgroundImage = null;
            this.addCreditButton.Font = null;
            this.addCreditButton.Name = "addCreditButton";
            this.addCreditButton.UseVisualStyleBackColor = true;
            // 
            // buyCreditLink
            // 
            this.buyCreditLink.AccessibleDescription = null;
            this.buyCreditLink.AccessibleName = null;
            resources.ApplyResources(this.buyCreditLink, "buyCreditLink");
            this.buyCreditLink.Font = null;
            this.buyCreditLink.Name = "buyCreditLink";
            this.buyCreditLink.TabStop = true;
            this.buyCreditLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buyCreditLink_LinkClicked);
            // 
            // addCreditLink
            // 
            this.addCreditLink.AccessibleDescription = null;
            this.addCreditLink.AccessibleName = null;
            resources.ApplyResources(this.addCreditLink, "addCreditLink");
            this.addCreditLink.Font = null;
            this.addCreditLink.Name = "addCreditLink";
            this.addCreditLink.TabStop = true;
            this.addCreditLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.addCreditLink_LinkClicked);
            // 
            // AddCredit
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.addCreditLink);
            this.Controls.Add(this.buyCreditLink);
            this.Controls.Add(this.addCreditButton);
            this.Controls.Add(this.creditLabel);
            this.Font = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCredit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label creditLabel;
        private System.Windows.Forms.Button addCreditButton;
        private System.Windows.Forms.LinkLabel buyCreditLink;
        private System.Windows.Forms.LinkLabel addCreditLink;
    }
}