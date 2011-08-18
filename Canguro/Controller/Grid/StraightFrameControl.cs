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
    public partial class StraightFrameControl : UserControl, IPopupGridControl
    {
        long clickTimer;
        const long minClickTime = 5000000;
        PopupCellEditingControl editingControl = null;
        StraightFrameProps props = null;
        public StraightFrameControl()
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
                FrameSection sec = props.Section;
                try
                {
                    if (!Canguro.Model.Model.Instance.IsLocked || !askedUnlockModel)
                    {
                        askedUnlockModel = true;
                        props.Section = (FrameSection)st.Section;
                    }
                }
                catch (ModelIsLockedException) { }
                
                return props;
            }
            set
            {
                askedUnlockModel = false;
                settingValue = true;
                props = value as StraightFrameProps;
                if (props != null)
                {
                    //props = (StraightFrameProps)props.Clone();
                    st.Section = props.Section;
                    settingValue = false;
                    clickTimer = DateTime.Now.Ticks;
                }
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

        private void StraightFrameControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                editingControl.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
                e.Handled = true;
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
    }
}
