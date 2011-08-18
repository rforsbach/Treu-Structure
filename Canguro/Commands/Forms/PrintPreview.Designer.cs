namespace Canguro.Commands.Forms
{
    partial class PrintPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPreview));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.printButton = new System.Windows.Forms.ToolStripButton();
            this.pageSettingsButton = new System.Windows.Forms.ToolStripButton();
            this.orientButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeButton = new System.Windows.Forms.ToolStripButton();
            this.printPreviewControl = new System.Windows.Forms.PrintPreviewControl();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printButton,
            this.pageSettingsButton,
            this.orientButton,
            this.toolStripSeparator1,
            this.closeButton});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // printButton
            // 
            resources.ApplyResources(this.printButton, "printButton");
            this.printButton.Name = "printButton";
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // pageSettingsButton
            // 
            resources.ApplyResources(this.pageSettingsButton, "pageSettingsButton");
            this.pageSettingsButton.Name = "pageSettingsButton";
            // 
            // orientButton
            // 
            resources.ApplyResources(this.orientButton, "orientButton");
            this.orientButton.Name = "orientButton";
            this.orientButton.Click += new System.EventHandler(this.orientButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // printPreviewControl
            // 
            resources.ApplyResources(this.printPreviewControl, "printPreviewControl");
            this.printPreviewControl.Name = "printPreviewControl";
            // 
            // PrintPreview
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.printPreviewControl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PrintPreview";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton printButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton closeButton;
        private System.Windows.Forms.PrintPreviewControl printPreviewControl;
        private System.Windows.Forms.ToolStripButton orientButton;
        private System.Windows.Forms.ToolStripButton pageSettingsButton;
    }
}