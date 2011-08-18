using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.Controller.Grid
{
    /// <summary>
    /// Control class for display and edit of joint constraints in the GridView
    /// </summary>
    public partial class JointConstraintControl : UserControl, IPopupGridControl
    {
        PopupCellEditingControl editingControl = null;
        Canguro.Model.Constraint value = null;

        /// <summary>
        /// Default constructor. Calls InitializeComponent.
        /// </summary>
        public JointConstraintControl()
        {
            InitializeComponent();
        }

        #region IPopupGridControl Members

        bool settingValue = false;
        bool askedUnlockModel = false;
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is Model.Constraint) 
                {
                    this.value = (Model.Constraint)value;
                    notifyCellDirty();
                    constraintComboBox.SelectedItem = value;
                }
                Invalidate();
            }
        }

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

        public bool HoldFocus
        {
            get { return false; }
        }

        #endregion

        private void notifyCellDirty()
        {
            //constraintComboBox.Items.Clear();
            //constraintComboBox.Items.Add(Culture.Get("NoConstraint"));
            //foreach (Model.Constraint cons in Model.Model.Instance.ConstraintList)
            //    if (cons != null)
            //        constraintComboBox.Items.Add(cons);
            if (Value == null && constraintComboBox.SelectedIndex != 0)
                constraintComboBox.SelectedIndex = 0;
            else if (constraintComboBox.SelectedItem != Value)
                constraintComboBox.SelectedItem = Value;

            if (!settingValue && editingControl != null)
            {
                editingControl.EditingControlValueChanged = true;
                editingControl.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        private void JointConstraintControl_KeyDown(object sender, KeyEventArgs e)
        {
            constraintComboBox.Focus();
            e.Handled = true;
        }

        bool cancelClick = false;
        long clickTimer;
        const long minClickTime = 5000000;
        private void st_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            cancelClick = false;
            if ((DateTime.Now.Ticks - clickTimer) < minClickTime && !settingValue)
            {
                cancelClick = true;
                return;
            }

            if ((e.Node.FirstNode) == null)
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
        }

        private void st_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Node.FirstNode) == null && !settingValue)
                notifyCellDirty();
        }

        private void st_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void st_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void st_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelClick)
                e.Cancel = true;
        }

        private void button_Clicked(object sender, EventArgs e)
        {
            EndEdit();
        }

        private void EndEdit()
        {
            notifyCellDirty();
            editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);   
        }

        private void JointConstraintControl_Load(object sender, EventArgs e)
        {
        }

        private void constraintComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            value = (constraintComboBox.SelectedItem is Model.Constraint) ? (Model.Constraint)constraintComboBox.SelectedItem : null;
            notifyCellDirty();
        }
    }
}
