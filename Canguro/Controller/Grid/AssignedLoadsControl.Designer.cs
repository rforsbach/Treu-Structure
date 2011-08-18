namespace Canguro.Controller.Grid
{
    partial class AssignedLoadsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignedLoadsControl));
            this.loadImgList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addLoadButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.editLoadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteLoadButton = new System.Windows.Forms.ToolStripButton();
            this.loadCaseLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lv1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.addJointLoadMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.forceLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groundDisplacementLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLineLoadMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.concentratedSpanLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distributedSpanLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temperatureLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temperatureGradientLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAreaLoadMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStrip1.SuspendLayout();
            this.addJointLoadMenu.SuspendLayout();
            this.addLineLoadMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadImgList
            // 
            this.loadImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("loadImgList.ImageStream")));
            this.loadImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.loadImgList.Images.SetKeyName(0, "ThumbnailView.png");
            this.loadImgList.Images.SetKeyName(1, "ForceLoad");
            this.loadImgList.Images.SetKeyName(2, "GroundDisplacementLoad");
            this.loadImgList.Images.SetKeyName(3, "ConcentratedSpanLoad");
            this.loadImgList.Images.SetKeyName(4, "DistributedSpanLoad");
            this.loadImgList.Images.SetKeyName(5, "TemperatureLineLoad");
            this.loadImgList.Images.SetKeyName(6, "TemperatureGradientLineLoad");
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLoadButton,
            this.editLoadButton,
            this.toolStripSeparator1,
            this.deleteLoadButton,
            this.loadCaseLabel});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Stretch = true;
            // 
            // addLoadButton
            // 
            this.addLoadButton.Image = global::Canguro.Properties.Resources.all_loads;
            resources.ApplyResources(this.addLoadButton, "addLoadButton");
            this.addLoadButton.Name = "addLoadButton";
            // 
            // editLoadButton
            // 
            this.editLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editLoadButton.Image = global::Canguro.Properties.Resources.edit;
            resources.ApplyResources(this.editLoadButton, "editLoadButton");
            this.editLoadButton.Name = "editLoadButton";
            this.editLoadButton.Click += new System.EventHandler(this.editLoadButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // deleteLoadButton
            // 
            this.deleteLoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteLoadButton.Image = global::Canguro.Properties.Resources.delete;
            resources.ApplyResources(this.deleteLoadButton, "deleteLoadButton");
            this.deleteLoadButton.Name = "deleteLoadButton";
            this.deleteLoadButton.Click += new System.EventHandler(this.deleteLoadButton_Click);
            // 
            // loadCaseLabel
            // 
            this.loadCaseLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.loadCaseLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loadCaseLabel.Name = "loadCaseLabel";
            this.loadCaseLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            resources.ApplyResources(this.loadCaseLabel, "loadCaseLabel");
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            // 
            // lv1
            // 
            resources.ApplyResources(this.lv1, "lv1");
            this.lv1.AutoArrange = false;
            this.lv1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lv1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv1.LargeImageList = this.loadImgList;
            this.lv1.MultiSelect = false;
            this.lv1.Name = "lv1";
            this.lv1.ShowGroups = false;
            this.lv1.SmallImageList = this.loadImgList;
            this.lv1.TileSize = new System.Drawing.Size(150, 40);
            this.lv1.UseCompatibleStateImageBehavior = false;
            this.lv1.View = System.Windows.Forms.View.Tile;
            this.lv1.ItemActivate += new System.EventHandler(this.lv1_ItemActivate);
            this.lv1.SelectedIndexChanged += new System.EventHandler(this.lv1_SelectedIndexChanged);
            this.lv1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lv1_MouseMove);
            this.lv1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lv1_KeyDown);
            // 
            // addJointLoadMenu
            // 
            this.addJointLoadMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forceLoadToolStripMenuItem,
            this.groundDisplacementLoadToolStripMenuItem});
            this.addJointLoadMenu.Name = "addJointLoadMenu";
            resources.ApplyResources(this.addJointLoadMenu, "addJointLoadMenu");
            this.addJointLoadMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.addJointLoadMenu_Closed);
            // 
            // forceLoadToolStripMenuItem
            // 
            this.forceLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.forceload;
            this.forceLoadToolStripMenuItem.Name = "forceLoadToolStripMenuItem";
            resources.ApplyResources(this.forceLoadToolStripMenuItem, "forceLoadToolStripMenuItem");
            this.forceLoadToolStripMenuItem.Click += new System.EventHandler(this.forceLoadToolStripMenuItem_Click);
            // 
            // groundDisplacementLoadToolStripMenuItem
            // 
            this.groundDisplacementLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.grounddisplacementload;
            this.groundDisplacementLoadToolStripMenuItem.Name = "groundDisplacementLoadToolStripMenuItem";
            resources.ApplyResources(this.groundDisplacementLoadToolStripMenuItem, "groundDisplacementLoadToolStripMenuItem");
            this.groundDisplacementLoadToolStripMenuItem.Click += new System.EventHandler(this.groundDisplacementLoadToolStripMenuItem_Click);
            // 
            // addLineLoadMenu
            // 
            this.addLineLoadMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.concentratedSpanLoadToolStripMenuItem,
            this.distributedSpanLoadToolStripMenuItem,
            this.temperatureLoadToolStripMenuItem,
            this.temperatureGradientLoadToolStripMenuItem});
            this.addLineLoadMenu.Name = "addLineLoadMenu";
            resources.ApplyResources(this.addLineLoadMenu, "addLineLoadMenu");
            this.addLineLoadMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.addJointLoadMenu_Closed);
            // 
            // concentratedSpanLoadToolStripMenuItem
            // 
            this.concentratedSpanLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.concentrated_load;
            this.concentratedSpanLoadToolStripMenuItem.Name = "concentratedSpanLoadToolStripMenuItem";
            resources.ApplyResources(this.concentratedSpanLoadToolStripMenuItem, "concentratedSpanLoadToolStripMenuItem");
            this.concentratedSpanLoadToolStripMenuItem.Click += new System.EventHandler(this.concentratedSpanLoadToolStripMenuItem_Click);
            // 
            // distributedSpanLoadToolStripMenuItem
            // 
            this.distributedSpanLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.distributedexpandload;
            this.distributedSpanLoadToolStripMenuItem.Name = "distributedSpanLoadToolStripMenuItem";
            resources.ApplyResources(this.distributedSpanLoadToolStripMenuItem, "distributedSpanLoadToolStripMenuItem");
            this.distributedSpanLoadToolStripMenuItem.Click += new System.EventHandler(this.distributedSpanLoadToolStripMenuItem_Click);
            // 
            // temperatureLoadToolStripMenuItem
            // 
            this.temperatureLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.temperatureLineLoad;
            this.temperatureLoadToolStripMenuItem.Name = "temperatureLoadToolStripMenuItem";
            resources.ApplyResources(this.temperatureLoadToolStripMenuItem, "temperatureLoadToolStripMenuItem");
            this.temperatureLoadToolStripMenuItem.Click += new System.EventHandler(this.temperatureLoadToolStripMenuItem_Click);
            // 
            // temperatureGradientLoadToolStripMenuItem
            // 
            this.temperatureGradientLoadToolStripMenuItem.Image = global::Canguro.Properties.Resources.temperatureGradientLineLoad;
            this.temperatureGradientLoadToolStripMenuItem.Name = "temperatureGradientLoadToolStripMenuItem";
            resources.ApplyResources(this.temperatureGradientLoadToolStripMenuItem, "temperatureGradientLoadToolStripMenuItem");
            this.temperatureGradientLoadToolStripMenuItem.Click += new System.EventHandler(this.temperatureGradientLoadToolStripMenuItem_Click);
            // 
            // addAreaLoadMenu
            // 
            this.addAreaLoadMenu.Name = "addAreaLoadMenu";
            resources.ApplyResources(this.addAreaLoadMenu, "addAreaLoadMenu");
            // 
            // AssignedLoadsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lv1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(366, 67);
            this.Name = "AssignedLoadsControl";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.addJointLoadMenu.ResumeLayout(false);
            this.addLineLoadMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList loadImgList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton deleteLoadButton;
        private System.Windows.Forms.ToolStripDropDownButton addLoadButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton editLoadButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripLabel loadCaseLabel;
        private System.Windows.Forms.ListView lv1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip addJointLoadMenu;
        private System.Windows.Forms.ToolStripMenuItem forceLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groundDisplacementLoadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip addLineLoadMenu;
        private System.Windows.Forms.ToolStripMenuItem concentratedSpanLoadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip addAreaLoadMenu;
        private System.Windows.Forms.ToolStripMenuItem distributedSpanLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temperatureLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temperatureGradientLoadToolStripMenuItem;
    }
}
