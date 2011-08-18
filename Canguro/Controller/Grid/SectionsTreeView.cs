using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.Controller.Grid
{
    public partial class SectionsTreeView : TreeView
    {
        private bool rebuildTree;
        private Section section;

        public SectionsTreeView()
        {
            InitializeComponent();
            ImageList = sectionsImgList;
            SectionManager.Instance.CatalogChanged += new EventHandler(Sections_CatalogChanged);
            rebuildTree = true;
            section = null;
        }

        void Sections_CatalogChanged(object sender, EventArgs e)
        {
            rebuildTree = true;
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Section Section
        {
            get
            {
                TreeNode tn = SelectedNode;
                if (tn != null)
                {
                    if (tn.FirstNode == null)
                        return (Section)SelectedNode.Tag;
                }
                return section;
            }
            set
            { 
                section = value;

                if (rebuildTree)
                {
                    Nodes.Clear();
                    Nodes.Add(Canguro.Model.Section.SectionManager.Instance.Tree);
                    SelectedNode = setImages(Nodes);
                    rebuildTree = false;
                }
                else
                {
                    SelectedNode = findSelectedNode(Nodes);
                }
            }
        }

        private TreeNode findSelectedNode(TreeNodeCollection tnc)
        {
            TreeNode selectedNode = null, tmpNode = null;
            foreach (TreeNode tn in tnc)
            {
                if (tn.FirstNode == null)
                {
                    if (tn.Tag == section)
                        selectedNode = tn;
                }
                else
                {
                    tmpNode = findSelectedNode(tn.Nodes);
                    if (selectedNode == null)
                        selectedNode = tmpNode;
                }
            }

            return selectedNode;
        }

        private TreeNode setImages(TreeNodeCollection tnc)
        {
            TreeNode selectedNode = null, tmpNode = null;
            foreach (TreeNode tn in tnc)
            {
                if (tn.FirstNode == null)
                {
                    if (tn.Tag == section)
                        selectedNode = tn;
                    if (tn != null && tn.Tag != null && ((FrameSection)tn.Tag).Shape != null)
                        tn.SelectedImageKey = tn.ImageKey = ((FrameSection)tn.Tag).Shape + "Section";
                }
                else
                {
                    tn.SelectedImageKey = tn.ImageKey = tn.Text + "Section";
                    tmpNode = setImages(tn.Nodes);
                    if (selectedNode == null)
                        selectedNode = tmpNode;
                }
            }

            return selectedNode;
        }
    }
}
