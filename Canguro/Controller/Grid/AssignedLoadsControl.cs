using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Load;

namespace Canguro.Controller.Grid
{
    public partial class AssignedLoadsControl : UserControl, IPopupGridControl
    {
        Canguro.Model.Load.AssignedLoads value;
        public AssignedLoadsControl()
        {
            InitializeComponent();
        }

        #region IPopupGridControl Members

        public object Value
        {
            get
            {
                // Set value according to control

                return value;
            }
            set
            {
                this.value = value as Canguro.Model.Load.AssignedLoads;
                if (this.value == null) return;

                // Initialize AddLoad DropDownButton
                addLoadButton.Enabled = true;
                
                if (this.value.GetType() == typeof(AssignedJointLoads))
                    addLoadButton.DropDown = addJointLoadMenu;
                else if (this.value.GetType() == typeof(AssignedLineLoads))
                    addLoadButton.DropDown = addLineLoadMenu;
                else if (this.value.GetType() == typeof(AssignedAreaLoads))
                    addLoadButton.DropDown = addAreaLoadMenu;
                else
                    addLoadButton.Enabled = false;

                // Add Loads to the list view
                addLoads();

                updateControl();
            }
        }

        PopupCellEditingControl editingControl = null;
        public PopupCellEditingControl EditingControl
        {
            get
            {
                return editingControl;
            }
            set
            {
                editingControl = value;
            }
        }

        bool holdFocus = false;
        public bool HoldFocus
        {
            get { return holdFocus; }
        }

        #endregion

        private void addLoads()
        {
            // Initialize ListView with Loads
            lv1.Items.Clear();
            LoadCase lc = Canguro.Model.Model.Instance.ActiveLoadCase;
            loadCaseLabel.Text = lc.Name;

            ItemList<Load> loads = this.value[lc];
            if (loads != null)
                foreach (Load l in loads)
                    if (l != null)
                        addLoadToListView(l);
        }

        void updateControl()
        {
            if (lv1.Items.Count > 2)
                this.MinimumSize = new Size(366, 84);
            this.Refresh();
        }

        ListViewItem oldItem = null;
        private void lv1_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = lv1.GetItemAt(e.X, e.Y);
            if (item != oldItem)
            {
                oldItem = item;
                if (item != null)
                {
                    toolTip1.SetToolTip(lv1, item.Text);
                    //lv1.Invalidate();
                }
                else
                    toolTip1.RemoveAll();
            }
        }

        private void lv1_ItemActivate(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                Load l = (Load)lv1.SelectedItems[0].Tag;
                if (l != null)
                {
                    editLoad(l);
                }
            }
        }

        private void addJointLoadMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            editingControl.Focus();
            lv1.Focus();
        }

        ListViewItem addLoadToListView(Load l)
        {
            ListViewItem lv = new ListViewItem(l.ToString(), l.GetType().Name);
            lv.Tag = l;
            return lv1.Items.Add(lv);
        }

        void addNewLoad(Load l)
        {
            try
            {
                value.Add(l);
                ListViewItem lv = addLoadToListView(l);
                lv.Selected = true;
                updateControl();

                editLoad(l);
            }
            catch (ModelIsLockedException) { } // Ignore the change
        }

        void editLoad(Load l)
        {
            holdFocus = true;
            LoadEditFrm.EditLoad(l, EditingControl.DropDown);
            holdFocus = false;
            addLoads();
        }

        private void forceLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new ForceLoad());
        }

        private void groundDisplacementLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new GroundDisplacementLoad());
        }

        private void lv1_SelectedIndexChanged(object sender, EventArgs e)
        {
            editLoadButton.Enabled = deleteLoadButton.Enabled = (lv1.SelectedItems.Count > 0);
        }

        private void deleteLoadButton_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                ListViewItem lv = lv1.SelectedItems[0];
                Load l = (Load)lv.Tag;
                if (l != null)
                {
                    //DialogResult r = MessageBox.Show(Culture.Get("deleteLoadFromAssignedLoadsQuestion"), 
                    //    Culture.Get("deleteLoadFromAssignedLoadsQuestionTitle"), 
                    //    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    //if (r == DialogResult.Yes)
                    //{
                        value.Remove(l);
                        lv1.Items.Remove(lv);
                        updateControl();
                        Canguro.Model.Model.Instance.Undo.Commit();
                    //}
                }
            }
        }

        private void editLoadButton_Click(object sender, EventArgs e)
        {
            if (lv1.SelectedItems.Count > 0)
            {
                Load l = (Load)lv1.SelectedItems[0].Tag;
                if (l != null)
                {
                    editLoad(l);
                }
            }
        }

        private void concentratedSpanLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new ConcentratedSpanLoad());
        }

        private void distributedSpanLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new DistributedSpanLoad());
        }

        private void temperatureLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new TemperatureLineLoad());
        }

        private void temperatureGradientLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewLoad(new TemperatureGradientLineLoad());
        }

        private void lv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteLoadButton_Click(this, EventArgs.Empty);
        }
    }
}
