using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using Canguro.Model;
using Canguro.View;
using Canguro.Model.Load;
using Canguro.Model.ModelAttributes;

namespace Canguro.Controller.Grid
{
    public class GridView<Titem> : DataGridView, ModelObserver where Titem : Item
    {
        private bool enabled;
        private ItemList<Titem> source;
        private readonly EventHandler updateEventHandler;

        // Declare a List to serve as the data cache, to point to the selected Items only
        List<Titem> selectedItems = new List<Titem>();
        
        // Declare a Customer object to store data for a row being edited.
        private Titem itemInEdit;

        // Declare a variable to store the index of a row being edited. 
        // A value of -1 indicates that there is no row currently in edit. 
        private int rowInEdit = -1;

        // Declare a variable to indicate the commit scope. 
        // Set this value to false to use cell-level commit scope. 
        private bool rowScopeCommit = true;

        // Declare Property List to store accessors to the actual object
        // properties using Reflection
        private List<PropertyDescriptor> propList = new List<PropertyDescriptor>();

        // Declare a variable to indicate whether the grid should use ID as the rowheader
        private bool useIdAsRowHeader = false;

        public GridView()
        {
            // Link updateModel to the eventHandler for Model.ModelChange
            updateEventHandler = updateModel;
            enabled = false;

            // Enable virtual mode.
            VirtualMode = true;
        }

        private void setDataSource(ItemList<Titem> source)
        {
            this.source = source;
            propList.Clear();

            // Add columns to the DataGridView.
            Type theType = typeof(Titem);
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(theType).Sort(new GridPositionComparer());
            
            foreach (PropertyDescriptor pd in pdc)
            {
                if (pd.IsBrowsable)
                {
                    // Initialize and add a text box column.
                    DataGridViewColumn column;
                    if (pd.Name.Equals("Id"))
                    {
                        useIdAsRowHeader = true;
                        RowHeadersWidth = 23; // 56;
                        idStringFormat.Alignment = System.Drawing.StringAlignment.Far;
                    }
                    else
                    {
                        if (pd.PropertyType == typeof(JointDOF))
                            column = new DataGridViewPopupColumn(new DataGridViewJointDOFCell(), new JointDOFControl());
                        else if (pd.PropertyType == typeof(AssignedLoads))
                            column = new DataGridViewPopupColumn(new AssignedLoadsControl());
                        else if (pd.PropertyType.Name.Equals("Single[]"))
                            column = new DataGridViewSingleArrayColumn();
                        else if (pd.PropertyType == typeof(bool))
                            column = new DataGridViewCheckBoxColumn();
                        else
                            column = new DataGridViewTextBoxColumn();
                        column.HeaderText = Culture.Get(pd.DisplayName);
                        column.Name = pd.Name;
                        //column.ReadOnly = pd.IsReadOnly;
                        column.ValueType = pd.PropertyType;
                        column.Width = ((GridPositionAttribute)pd.Attributes[typeof(GridPositionAttribute)]).Width;
                        Columns.Add(column);
                        propList.Add(pd);
                    }
                    
                    //AttributeCollection attttt = pd.Attributes;
                }
            }

            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            fillSelectedItems();
            Enabled = true;
        }

        public new ItemList<Titem> DataSource
        {
            get { return source; }
            set
            {
                setDataSource(value);
            }
        }

        #region ModelObserver Interface
        public new bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value ^ enabled)
                {
                    if (value)
                    {
                        Model.Model.Instance.ModelChanged += updateEventHandler;
                        ReadOnly = false;
                    }
                    else
                    {
                        Model.Model.Instance.ModelChanged -= updateEventHandler;
                        ReadOnly = true;
                    }
                    enabled = value;
                }
            }
        }

        private void updateModel(object sender, System.EventArgs e)
        {
            fillSelectedItems();
        }
        #endregion

        #region DataGridView event overrides for Virtual Mode
        /// <summary>
        /// Used by the control to retrieve a cell value from the data cache for display. 
        /// This event occurs whenever the DataGridView control needs to paint a cell.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == RowCount - 1) return;

            Titem itemTmp;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
                itemTmp = itemInEdit;
            else
                itemTmp = (Titem)selectedItems[e.RowIndex];

            if (itemTmp == null)
                return;

            // Set the cell value to paint using the object retrieved.
            e.Value = propList[e.ColumnIndex].GetValue(itemTmp);

            base.OnCellValueNeeded(e);
        }

        /// <summary>
        /// Used by the control to commit user input for a cell to the data cache. 
        /// This event occurs whenever the user commits a cell value change.
        /// 
        /// Call the UpdateCellValue method when changing a cached value outside of 
        /// a CellValuePushed event handler to ensure that the current value is displayed
        /// in the control and to apply any automatic sizing modes currently in effect.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValuePushed(DataGridViewCellValueEventArgs e)
        {
            Titem itemTmp;
            bool commitChange = false;

            // Store a reference to the object for the row being edited.
            if (e.RowIndex < selectedItems.Count)
            {
                // If the user is editing a new row, get the corresponding object.
                rowInEdit = e.RowIndex;
                if (itemInEdit == null)
                    itemInEdit = selectedItems[rowInEdit];

                commitChange = true;
            }
            itemTmp = itemInEdit;

            // Set the appropriate property to the cell value entered.
            propList[e.ColumnIndex].SetValue(itemTmp, e.Value);

            if (commitChange)
                Canguro.Model.Model.Instance.Undo.Commit();
            
            base.OnCellValuePushed(e);
        }

        /// <summary>
        /// Used by the control to indicate the need for a new row in the data cache.
        /// This event occurs whenever the user enters the row for new records.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNewRowNeeded(DataGridViewRowEventArgs e)
        {
            // Create a new object when the user edits
            // the row for new records.

            // TODO: Fábrica de Titem's, falta alguna forma genérica de crear
            // objetos de tipo Titem. Opción 1) usar new constraint
            // (http://msdn2.microsoft.com/en-us/library/ms379608(vs.80).aspx)
            // o usar el patrón Prototype, poniéndole un método abstracto
            // Clone() a Item. Por el momento llamo a una función que hace un
            // switch y según el tipo crea y devuelve un objeto, como una
            // mini-fábrica aquí en GridView
            
            itemInEdit = getNewTitem();
            rowInEdit = Rows.Count - 1;
            
            base.OnNewRowNeeded(e);
        }

        /// <summary>
        /// Event that saves new or modified rows to the data store. 
        /// This event occurs whenever the user changes the current row.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowValidated(DataGridViewCellEventArgs e)
        {
            // Save row changes if any were made and release the edited 
            // object if there is one.
            if (e.RowIndex >= selectedItems.Count && e.RowIndex != Rows.Count - 1)
            {
                // Add the new object to the data store.
                itemInEdit.IsSelected = true;
                source.Add(itemInEdit);
                Model.Model.Instance.Undo.Commit();

                itemInEdit = null;
                rowInEdit = -1;
            }
            else if (ContainsFocus || (itemInEdit != null && e.RowIndex < selectedItems.Count))
            {
                itemInEdit = null;
                this.rowInEdit = -1;
            }

            base.OnRowValidated(e);
        }

        /// <summary>
        /// Used by the control to determine whether a row has any uncommitted changes.
        ///
        /// Event that indicates whether the CancelRowEdit event will occur when the user 
        /// signals row reversion by pressing ESC twice in edit mode or once outside of 
        /// edit mode. By default, CancelRowEdit occurs upon row reversion when any cells
        /// in the current row have been modified unless the 
        /// System.Windows.Forms.QuestionEventArgs.Response property is set to true in the 
        /// RowDirtyStateNeeded event handler. This event is useful when the commit scope 
        /// is determined at run time.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDirtyStateNeeded(QuestionEventArgs e)
        {
            if (!rowScopeCommit)
            {
                // In cell-level commit scope, indicate whether the value
                // of the current cell has been modified.
                e.Response = IsCurrentCellDirty;
            }

            base.OnRowDirtyStateNeeded(e);
        }

        /// <summary>
        /// Used by the control to indicate that a row should revert to its cached values.
        /// 
        /// Event that discards the values of the object representing the current row. 
        /// This event occurs when the user signals row reversion by pressing ESC twice 
        /// in edit mode or once outside of edit mode. This event does not occur if no 
        /// cells in the current row have been modified or if the value of the 
        /// System.Windows.Forms.QuestionEventArgs.Response property has been set to 
        /// false in a RowDirtyStateNeeded event handler.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCancelRowEdit(QuestionEventArgs e)
        {
            if (rowInEdit == Rows.Count - 2 && rowInEdit == selectedItems.Count)
            {
                // If the user has canceled the edit of a newly created row, 
                // replace the corresponding object with a new, empty one.
                itemInEdit = getNewTitem();
            }
            else
            {
                // If the user has canceled the edit of an existing row, 
                // release the corresponding object.
                itemInEdit = null;
                rowInEdit = -1;
            }
            
            base.OnCancelRowEdit(e);
        }

        /// <summary>
        /// Event that deletes an existing object from the data store or discards an 
        /// unsaved object representing a newly created row. This event occurs whenever 
        /// the user deletes a row by clicking a row header and pressing the DELETE key.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Index < selectedItems.Count)
            {
                // If the user has deleted an existing row, remove the 
                // corresponding object from the data store.
                source.RemoveAt((int)selectedItems[e.Row.Index].Id);
                Model.Model.Instance.Undo.Commit();
            }

            if (e.Row.Index == rowInEdit)
            {
                // If the user has deleted a newly created row, release
                // the corresponding object. 
                rowInEdit = -1;
                itemInEdit = null;
            }

            base.OnUserDeletingRow(e);
        }
        #endregion

        #region Pintar ID's (OnRowPostPaint)
        System.Drawing.StringFormat idStringFormat = new System.Drawing.StringFormat();
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            if (useIdAsRowHeader)
            {
                // Paint the time on the row header.
                // The using statement automatically disposes the brush.
                using (System.Drawing.SolidBrush b = new System.Drawing.SolidBrush(RowHeadersDefaultCellStyle.ForeColor))
                {
                    try
                    {
                        if ((e.RowIndex + 1) < RowCount)

                            e.Graphics.DrawString(selectedItems[e.RowIndex].Id.ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + RowHeadersWidth - 3, e.RowBounds.Location.Y + 4, idStringFormat);
                    }
                    catch (Exception) { }
                }
            }
        }
        #endregion

        #region Editar con doble click
        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnCellDoubleClick(e);

            try
            {
                BeginEdit(true);
            }
            catch (Exception) { }
        }
        #endregion

        #region Helper methods
        private void fillSelectedItems()
        {
            selectedItems.Clear();
            foreach (Titem i in source)
                if ((i != null) && (i.IsSelected))
                    selectedItems.Add(i);

            if (selectedItems.Count > 0)
                RowHeadersWidth = 23 + 6 * (selectedItems[selectedItems.Count - 1].Id.ToString().Length - 1);
            else
                RowHeadersWidth = 23;

            // Set the row count, including the row for new records.
            // If RowCount matches the last RowCount, just Invalidate data
            if (RowCount != selectedItems.Count + 1)
                RowCount = selectedItems.Count + 1;
            else
                Invalidate();
        }

        private Titem getNewTitem()
        {
            Type t = typeof(Titem);
            if (t == typeof(Joint))
                return (Titem)(object)new Joint();
            else if (t == typeof(LineElement))
                return (Titem)(object)new LineElement(new StraightFrameProps());

            return null;
        }
        #endregion

    }

    #region SingleArrayCell + Column
    public class DataGridViewSingleArrayCell : DataGridViewTextBoxCell
    {
        protected override object GetValue(int rowIndex)
        {
            object value = base.GetValue(rowIndex);

            if (value != null)
            {
                string tmp = "";
                System.Collections.IList theList = (System.Collections.IList)value;
                foreach (object o in theList)
                {
                    tmp += o + ", ";
                }
                return tmp.Substring(0, tmp.Length - 2);
            }
            return "";

        }

        protected override bool SetValue(int rowIndex, object value)
        {
            float[] array = null;
            char[] separators = { ' ', ',', ';', '\t', '\n', '\r', '/', '\\', '-', '_', ':'};
            try
            {
                if (value != null)
                {
                        string val = (string)value;
                        string[] vals = val.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        array = new float[vals.Length];
                        for (int i = 0; i < vals.Length; i++)
                            array[i] = float.Parse(vals[i]);
                }
                return base.SetValue(rowIndex, array);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }
    }
    
    public class DataGridViewSingleArrayColumn : DataGridViewColumn
    {
        public DataGridViewSingleArrayColumn()
        {
            CellTemplate = new DataGridViewSingleArrayCell();
        }
    }
    #endregion

    #region DataGridViewPopupCell + Column + EditingControl
    public class DataGridViewPopupCell : DataGridViewTextBoxCell
    {
        public override Type EditType
        {
            get
            {
                return typeof(PopupCellEditingControl);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            PopupCellEditingControl ctl = (PopupCellEditingControl)DataGridView.EditingControl;
            ctl.Value = Value;
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return null;
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return base.ValueType;
            }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value != null)
                return value.ToString();
            else
                return "";
        }
    }

    public class DataGridViewPopupColumn : DataGridViewTextBoxColumn
    {
        Control popupCtrl;
        public DataGridViewPopupColumn(Control popupCtrl) : this(null, popupCtrl) { }
        public DataGridViewPopupColumn(DataGridViewPopupCell cellTemplate, Control popupCtrl)
        {
            if (cellTemplate == null)
                CellTemplate = new DataGridViewPopupCell();
            else
                CellTemplate = cellTemplate;

            this.popupCtrl = popupCtrl;
        }

        public Control PopupCtrl
        {
            get { return popupCtrl; }
        }
    }

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
            // Let the DateTimePicker handle the keys listed.
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
                return ((DataGridViewPopupColumn)EditingControlDataGridView.Columns[cell.ColumnIndex]).PopupCtrl;
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
    #endregion

    #region JointDOFCell
    public class DataGridViewJointDOFCell : DataGridViewPopupCell
    {
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            JointDOF dof = (JointDOF)value;
            Type doftype = typeof(JointDOF.DofType);
            string tmp = "";
            if (dof != null)
            {
                tmp += Enum.GetName(doftype, dof.T1).Substring(0, 1) + " ";
                tmp += Enum.GetName(doftype, dof.T2).Substring(0, 1) + " ";
                tmp += Enum.GetName(doftype, dof.T3).Substring(0, 1) + " ";
                tmp += Enum.GetName(doftype, dof.R1).Substring(0, 1) + " ";
                tmp += Enum.GetName(doftype, dof.R2).Substring(0, 1) + " ";
                tmp += Enum.GetName(doftype, dof.R3).Substring(0, 1);
            }

            return tmp;
        }
    }
    #endregion
}
