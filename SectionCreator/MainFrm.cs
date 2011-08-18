using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.SectionCreator.View;

namespace Canguro.SectionCreator
{
    public partial class MainFrm : Form
    {
        // sectionPanel = new SectionPanel();

        public MainFrm()
        {
            InitializeComponent();
        }

        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));

        public void UpdateStatusBar()
        {
            System.Drawing.PointF pos = sectionPanel.View.GetModelPosition(Controller.Instance.MousePosition);
            int scale = (int)Model.Instance.Unit;
            toolStripStatusLabel1.Text = string.Format("({0:G4}, {1:G4})", pos.X, pos.Y);
            statusStrip.Update();
        }

        public void UpdateView()
        {
            List<Contour> contours = Model.Instance.Contours;
            contoursTreeView.Nodes.Clear();
            foreach (Contour con in contours)
            {
                TreeNode node = new TreeNode(con.Name);
                node.Tag = con;
                foreach (Point p in con.Points)
                {
                    TreeNode child = new TreeNode(p.ToString());
                    child.Tag = p;
                    node.Nodes.Add(child);
                }
                contoursTreeView.Nodes.Add(node);
            }
            UpdateTreeSelection();
            UpdateToolbar();
            string path = Model.Instance.CurrentPath;
            path = (string.IsNullOrEmpty(path)) ? "Untitled" : System.IO.Path.GetFileNameWithoutExtension(path);
            if (Model.Instance.Modified)
                path += "*";
            Text = path + " - Treu Section Creator";
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            Controller.Instance.MainFrame = this;
            Controller.Instance.CommandChanged += new EventHandler(CommandChanged);
            Model.Instance.ModelChanged += new EventHandler(ModelChanged);
            Model.Instance.SelectionChanged += new EventHandler(SelectionChanged);
            foreach (LenghtUnits u in Enum.GetValues(typeof(LenghtUnits)))
                unitsComboBox.Items.Add(u);

            Model.Instance.ChangeModel();
        }

        void CommandChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        void SelectionChanged(object sender, EventArgs e)
        {
            UpdateView();
            UpdateTreeSelection();
            UpdatePropertyGrid();
        }

        void ModelChanged(object sender, EventArgs e)
        {
            UpdateView();
            Invalidate();
        }

        void UpdateToolbar()
        {
            bool emptyModel = (Model.Instance.Contours.Count == 0);
            bool modelChanged = Model.Instance.Modified;
            bool executing = Controller.Instance.IsExecuting;
            bool hasCopied = (Clipboard.GetData("SectionCreator") != null);
            bool canUndo = Model.Instance.Undo.CanUndo;
            bool canRedo = Model.Instance.Undo.CanRedo;
            bool hasSelection = Model.Instance.GetSelection().Count > 0;

            newButton.Enabled = true;
            openButton.Enabled = true;
            saveButton.Enabled = modelChanged;
            cutButton.Enabled = hasSelection;
            copyButton.Enabled = hasSelection;
            pasteButton.Enabled = hasCopied;
            drawButton.Enabled = true;
            editButton.Enabled = !emptyModel;
            deleteButton.Enabled = hasSelection;
            undoButton.Enabled = canUndo;
            redoButton.Enabled = canRedo;
            unitsComboBox.Enabled = true;
            selectionButton.Enabled = true;
            panButton.Enabled = true;
            zoomInButton.Enabled = true;
            zoomOutButton.Enabled = true;
            zoomAllButton.Enabled = true;

            newToolStripMenuItem.Enabled = true;
            openToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = modelChanged;
            saveAsToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem.Enabled = hasSelection;
            copyToolStripMenuItem.Enabled = hasSelection;
            pasteToolStripMenuItem.Enabled = hasCopied;
            addContourToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = !emptyModel;
            deleteToolStripMenuItem.Enabled = hasSelection;
            undoToolStripMenuItem.Enabled = canUndo; 
            redoToolStripMenuItem.Enabled = canRedo;
            selectionToolStripMenuItem.Enabled = true;
            panToolStripMenuItem.Enabled = true;
            zoomInToolStripMenuItem.Enabled = true;
            zoomOutToolStripMenuItem.Enabled = true;
            zoomAllToolStripMenuItem.Enabled = true;

            Commands.ViewCommand com = Controller.Instance.CurrentCommand;
            drawButton.Checked = (com is Commands.AddContourCommand);
            addContourToolStripMenuItem.Checked = drawButton.Checked;
            editButton.Checked = (com is Commands.EditCommand);
            editToolStripMenuItem.Checked = editButton.Checked;
            selectionButton.Checked = (com is Commands.SelectionCommand);
            selectionToolStripMenuItem.Checked = selectionButton.Checked;
            panButton.Checked = (com is Commands.PanCommand);
            panToolStripMenuItem.Checked = panButton.Checked;

            unitsComboBox.SelectedItem = Model.Instance.Unit;
        }

        void UpdatePropertyGrid()
        {
            object selection = null;
            int count = 0;
            foreach (Contour con in Model.Instance.Contours)
            {
                if (con.IsSelected)
                {
                    selection = con;
                    count++;
                }
                foreach (Point p in con.Points)
                {
                    if (p.IsSelected)
                    {
                        selection = p;
                        count++;
                    }
                }
            }
            if (count == 1)
                selectionPropertyGrid.SelectedObject = selection;
            else
                selectionPropertyGrid.SelectedObject = null;
        }

        void UpdateTreeSelection()
        {
            foreach (TreeNode node in contoursTreeView.Nodes)
                UpdateTreeSelection(node);
        }

        void UpdateTreeSelection(TreeNode node)
        {
            if (node.Tag is ISelectable)
            {
                node.Checked = ((ISelectable)node.Tag).IsSelected;
                node.Expand();
                foreach (TreeNode n in node.Nodes)
                    UpdateTreeSelection(n);
            }
        }

        internal SectionPanel SectionPanel
        {
            get { return sectionPanel; }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            Model.Instance.Reset();
            Controller.Instance.Execute("New");
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Open");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Save");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("SaveAs");
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Cut");
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Copy");
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Paste");
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Polygon");
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Edit");
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Move");
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Delete");
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Merge");
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Undo");
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Redo");
        }

        private void unitsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Model.Instance.Unit = (LenghtUnits)unitsComboBox.SelectedItem;
        }

        private void selectionButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Selection");
        }

        private void panButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("Pan");
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("ZoomIn");
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("ZoomOut");
        }

        private void zoomAllButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Execute("ZoomAll");
        }

        private void contoursTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is ISelectable)
                ((ISelectable)e.Node.Tag).IsSelected = e.Node.Checked;
            sectionPanel.Invalidate();
            UpdatePropertyGrid();
        }

        private void selectionPropertyGrid_Leave(object sender, EventArgs e)
        {
            Model.Instance.Undo.Commit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            // Triangulation test

            IList<IList<System.Drawing.PointF>> contours = new List<IList<System.Drawing.PointF>>();
            IList<Canguro.Analysis.Sections.Material> materials = new List<Canguro.Analysis.Sections.Material>();
            foreach (Contour con in Model.Instance.Contours)
            {
                IList<System.Drawing.PointF> list = new List<System.Drawing.PointF>();
                foreach (Point p in con.Points)
                    list.Add(new System.Drawing.PointF(p.X, p.Y));
                contours.Add(list);
                materials.Add(con.Material);
            }

            // Get Triangulator Params

            Canguro.Analysis.Sections.Meshing.Quadrangulator quadrangulator = new Canguro.Analysis.Sections.Meshing.Quadrangulator();
            quadrangulator.Quadrangulate(contours, materials);
            Canguro.Analysis.Sections.FemCrossSection femSection = new Canguro.Analysis.Sections.FemCrossSection(quadrangulator.Mesh);
            femSection.GetSectionProperties();
            MessageBox.Show(femSection.ToString());
        }

        private void contoursTreeView_MouseEnter(object sender, EventArgs e)
        {
            contoursTreeView.Focus();
        }

        private void sectionPanel_MouseEnter(object sender, EventArgs e)
        {
            sectionPanel.Focus();
        }
    }
}