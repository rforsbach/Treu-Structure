namespace Canguro.Controller.PropertyGrid
{
    partial class FrameSectionFrm
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
            this.sectionsTree = new Canguro.Controller.Grid.SectionsTreeView();
            this.SuspendLayout();
            // 
            // sectionsTree
            // 
            this.sectionsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectionsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionsTree.HideSelection = false;
            this.sectionsTree.HotTracking = true;
            this.sectionsTree.Location = new System.Drawing.Point(0, 0);
            this.sectionsTree.Name = "sectionsTree";
            this.sectionsTree.ShowNodeToolTips = true;
            this.sectionsTree.Size = new System.Drawing.Size(264, 160);
            this.sectionsTree.TabIndex = 0;
            this.sectionsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.sectionsTree_NodeMouseClick);
            // 
            // FrameSectionFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 160);
            this.ControlBox = false;
            this.Controls.Add(this.sectionsTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameSectionFrm";
            this.ShowInTaskbar = false;
            this.Text = "Secciones";
            this.ResumeLayout(false);

        }

        #endregion

        private Canguro.Controller.Grid.SectionsTreeView sectionsTree;
    }
}