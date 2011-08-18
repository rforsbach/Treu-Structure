namespace Canguro.Controller.Grid
{
    partial class StraightFrameControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.st = new Canguro.Controller.Grid.SectionsTreeView();
            this.SuspendLayout();
            // 
            // st
            // 
            this.st.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.st.HideSelection = false;
            this.st.HotTracking = true;
            this.st.ImageIndex = 0;
            this.st.Location = new System.Drawing.Point(0, 0);
            this.st.Margin = new System.Windows.Forms.Padding(0);
            this.st.Name = "st";
            this.st.SelectedImageIndex = 0;
            this.st.ShowNodeToolTips = true;
            this.st.Size = new System.Drawing.Size(264, 160);
            this.st.TabIndex = 0;
            this.st.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.st_BeforeExpand);
            this.st.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.st_BeforeCollapse);
            this.st.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.st_AfterSelect);
            this.st.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.st_NodeMouseClick);
            this.st.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.st_BeforeSelect);
            this.st.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StraightFrameControl_KeyDown);
            // 
            // StraightFrameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.st);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "StraightFrameControl";
            this.Size = new System.Drawing.Size(264, 160);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StraightFrameControl_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private SectionsTreeView st;
    }
}
