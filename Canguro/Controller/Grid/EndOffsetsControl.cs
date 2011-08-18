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
    public partial class EndOffsetsControl : UserControl, IPopupGridControl
    {
        long clickTimer;
        const long minClickTime = 5000000;
        PopupCellEditingControl editingControl = null;
        LineEndOffsets value = LineEndOffsets.Empty;
        bool changed = false;

        public EndOffsetsControl()
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
                if (value is LineEndOffsets)
                    this.value = (LineEndOffsets)value;
                changed = false;
                UpdateControl();
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
            if (!settingValue && editingControl != null)
            {
                editingControl.EditingControlValueChanged = true;
                editingControl.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        
        bool cancelClick = false;
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
            float val;
            if (float.TryParse(offITextBox.Text, out val))
                value.EndI = val;
            if (float.TryParse(offJTextBox.Text, out val))
                value.EndJ = val;
            if (float.TryParse(factorTextBox.Text, out val))
                value.Factor = val;
            if (changed)
                notifyCellDirty();
        }

        private void EndOffsetsControl_Load(object sender, EventArgs e)
        {
            offILabel.Text = Culture.Get("LineEndOffsetI") + " (" + Model.Model.Instance.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance) + ")";
            offJLabel.Text = Culture.Get("LineEndOffsetJ") + " (" + Model.Model.Instance.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance) + ")";
            factorLabel.Text = Culture.Get("LineEndOffsetFactor");
            offILabel.Size = new Size(38, 15);
            offJLabel.Size = new Size(38, 15);
            UpdateControl();
        }

        private void UpdateControl()
        {
            offITextBox.Text = value.EndI.ToString("F3");
            offJTextBox.Text = value.EndJ.ToString("F3");
            factorTextBox.Text = value.Factor.ToString("F3");
        }

        private void offITextBox_TextChanged(object sender, EventArgs e)
        {
            changed = true;
        }

        private void offITextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                EndEdit();
            else if (e.KeyData == Keys.Left)
            {
                if (offITextBox.Focused)
                {
                    factorTextBox.Focus();
                    factorTextBox.SelectAll();
                }
                else if (factorTextBox.Focused)
                {
                    offJTextBox.Focus();
                    offJTextBox.SelectAll();
                }
                else
                {
                    offITextBox.Focus();
                    offITextBox.SelectAll();
                }
            }
            else if (e.KeyData == Keys.Right)
            {
                if (offJTextBox.Focused)
                {
                    factorTextBox.Focus();
                    factorTextBox.SelectAll();
                }
                else if (offITextBox.Focused)
                {
                    offJTextBox.Focus();
                    offJTextBox.SelectAll();
                }
                else
                {
                    offITextBox.Focus();
                    offITextBox.SelectAll();
                }
            }
        }
    }
}
