using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Section;

namespace Canguro.Controller.PropertyGrid
{
    public partial class FrameSectionFrm : Form
    {
        private System.Windows.Forms.Design.IWindowsFormsEditorService wfes;

        public FrameSectionFrm()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        public void SetDropDownParams(Section lastSection, System.Windows.Forms.Design.IWindowsFormsEditorService wfes)
        {
            this.wfes = wfes;
            sectionsTree.Section = lastSection;
        }

        public Section Result
        {
            get { return sectionsTree.Section; }
        }

        private void sectionsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.FirstNode == null)
            {
                wfes.CloseDropDown();
                Visible = false;
            }
        }
    }
}