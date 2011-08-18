namespace Canguro.Controller.Grid
{
    partial class LoadEditFrm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadEditFrm));
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.loadText = new System.Windows.Forms.Label();
            this.loadPicture = new System.Windows.Forms.PictureBox();
            this.loadDescription = new System.Windows.Forms.Label();
            this.loadImageList = new System.Windows.Forms.ImageList(this.components);
            this.properties = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.loadPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ok, "ok");
            this.ok.Name = "ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // loadText
            // 
            resources.ApplyResources(this.loadText, "loadText");
            this.loadText.Name = "loadText";
            // 
            // loadPicture
            // 
            this.loadPicture.Image = global::Canguro.Properties.Resources.FORCELOAD250X125;
            resources.ApplyResources(this.loadPicture, "loadPicture");
            this.loadPicture.Name = "loadPicture";
            this.loadPicture.TabStop = false;
            // 
            // loadDescription
            // 
            this.loadDescription.AutoEllipsis = true;
            resources.ApplyResources(this.loadDescription, "loadDescription");
            this.loadDescription.Name = "loadDescription";
            this.loadDescription.UseMnemonic = false;
            // 
            // loadImageList
            // 
            this.loadImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("loadImageList.ImageStream")));
            this.loadImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.loadImageList.Images.SetKeyName(0, "ConcentratedSpanLoad");
            this.loadImageList.Images.SetKeyName(1, "DistributedSpanLoad");
            this.loadImageList.Images.SetKeyName(2, "ForceLoad");
            this.loadImageList.Images.SetKeyName(3, "GroundDisplacementLoad");
            this.loadImageList.Images.SetKeyName(4, "UniformLineLoadCmd");
            this.loadImageList.Images.SetKeyName(5, "TriangleLineLoadCmd");
            this.loadImageList.Images.SetKeyName(6, "TemperatureLineLoad");
            this.loadImageList.Images.SetKeyName(7, "TemperatureGradientLineLoad");
            // 
            // properties
            // 
            resources.ApplyResources(this.properties, "properties");
            this.properties.Name = "properties";
            this.properties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.properties.ToolbarVisible = false;
            this.properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.properties_PropertyValueChanged);
            // 
            // LoadEditFrm
            // 
            this.AcceptButton = this.ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancel;
            this.Controls.Add(this.loadDescription);
            this.Controls.Add(this.loadPicture);
            this.Controls.Add(this.loadText);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.properties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadEditFrm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.loadPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid properties;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label loadText;
        private System.Windows.Forms.PictureBox loadPicture;
        private System.Windows.Forms.Label loadDescription;
        private System.Windows.Forms.ImageList loadImageList;
    }
}