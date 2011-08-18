using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public class PopupCellEditingControl : Control, IDataGridViewEditingControl
    {
        object initialValue = null;
        public PopupCellEditingControl()
        {
        }

        DataGridView dataGridView;
        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public new void Focus()
        {
            base.Focus();
            if (tsdd != null)
                tsdd.Focus();
        }

        ToolStripControlHost tbh;
        ToolStripDropDown tsdd;
        Control popupCtrl = null;
        IPopupGridControl popupValue = null;
        protected override void InitLayout()
        {
            base.InitLayout();

            System.Drawing.Point c = EditingControlDataGridView.CurrentCellAddress;
            string formattedValue = EditingControlDataGridView[c.X, c.Y].FormattedValue.ToString();

            popupCtrl = getPopupCtrl(EditingControlDataGridView.CurrentCell);
            popupValue = popupCtrl as IPopupGridControl;
            System.Diagnostics.Debug.Assert(popupValue != null);

            tbh = new ToolStripControlHost(popupCtrl, "popupCtrl");
            tbh.Padding = new Padding(0);
            tbh.AutoSize = true;
            tsdd = new ToolStripDropDown();

            popupValue.EditingControl = this;
            forwardFocus = true;

            tsdd.AutoSize = true;

            tsdd.Items.Add(tbh);
            tsdd.Padding = new Padding(0);
            tsdd.Closing += new ToolStripDropDownClosingEventHandler(tsdd_Closing);
            tsdd.Show(EditingControlDataGridView, EditingControlDataGridView.GetCellDisplayRectangle(c.X, c.Y, true).Location);
        }

        bool forwardFocus = false;
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (forwardFocus && popupCtrl != null)
            {
                popupCtrl.Focus();
                forwardFocus = false;
            }
        }

        void tsdd_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //this.formattedValue = tsdd.Items[0].Text;
            if (((e.CloseReason != ToolStripDropDownCloseReason.ItemClicked) &&
                (e.CloseReason != ToolStripDropDownCloseReason.Keyboard)) &&
                (popupCtrl.ContainsFocus || popupValue.HoldFocus))
            {
                e.Cancel = true;
                ((ToolStripDropDown)sender).Focus();
            }
            else
            {
                if (!(EditingControlDataGridView.IsDisposed || EditingControlDataGridView.Disposing))
                {
                    EditingControlDataGridView.BeginInvoke((MethodInvoker)delegate()
                    {
                        if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
                        {
                            EditingControlDataGridView.CancelEdit();
                        }
                        EditingControlDataGridView.EndEdit();
                    });
                }
            }
        }

        int rowIndex;
        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        bool valueChanged = false;
        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        public object EditingControlFormattedValue
        {
            get { return Value; }
            set { Value = initialValue; }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        Control getPopupCtrl(DataGridViewCell cell)
        {
            try
            {
                return ((GridViewPopupColumn)EditingControlDataGridView.Columns[cell.ColumnIndex]).PopupCtrl;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ToolStripDropDown DropDown
        {
            get
            {
                return tsdd;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        public object Value
        {
            get { return popupValue.Value; }
            set { popupValue.Value = initialValue = value; }
        }
    }
}
