namespace Canguro.Controller.Grid
{
    partial class SectionsTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SectionsTreeView));
            this.sectionsImgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // sectionsImgList
            // 
            this.sectionsImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sectionsImgList.ImageStream")));
            this.sectionsImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.sectionsImgList.Images.SetKeyName(0, "ISection");
            this.sectionsImgList.Images.SetKeyName(1, "2CSection");
            this.sectionsImgList.Images.SetKeyName(2, "2LSection");
            this.sectionsImgList.Images.SetKeyName(3, "BSection");
            this.sectionsImgList.Images.SetKeyName(4, "CSection");
            this.sectionsImgList.Images.SetKeyName(5, "LSection");
            this.sectionsImgList.Images.SetKeyName(6, "PSection");
            this.sectionsImgList.Images.SetKeyName(7, "RNSection");
            this.sectionsImgList.Images.SetKeyName(8, "RSection");
            this.sectionsImgList.Images.SetKeyName(9, "TSection");
            // 
            // SectionsTreeView
            // 
            this.LineColor = System.Drawing.Color.Black;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList sectionsImgList;
    }
}
