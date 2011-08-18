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
        private ItemList<Titem> source = null;

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

        // Last unit system used for display of units in the header row
        private Canguro.Model.UnitSystem.UnitSystem lastUnitSystem;

        // Value to avoid validating rows when this.Rows.Clear is called
        private bool clearingRows = false;

        public GridView()
        {
            // Link updateModel to the eventHandler for Model.ModelChange
            enabled = false;
            DoubleBuffered = true;
            lastUnitSystem = null;
            // Enable virtual mode.
            VirtualMode = true;
        }

        private void setDataSource(ItemList<Titem> source)
        {
            bool createColumns = true;
            //if (this.source != null) createColumns = false;

            this.source = source;

            if (createColumns)
            {
                Columns.Clear();
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
                            GridPositionAttribute gridPosition = ((GridPositionAttribute)pd.Attributes[typeof(GridPositionAttribute)]);
                            UnitsAttribute gridUnits = ((UnitsAttribute)pd.Attributes[typeof(UnitsAttribute)]);

                            if (gridPosition.Expand)
                                // Esto se debe modificar para soportar columnas expandibles a
                                // partir de metadatos. Está fijo con StraightFrameProps
                                // porque es la única propiedad que la necesita por el momento.
                                // Será necesario cuando aparezcan más tipos de LineProp's
                                column = new GridViewPopupColumn(new GridViewStraightFrameCell(), new StraightFrameControl());
                            else if (pd.PropertyType == typeof(JointDOF))
                                column = new GridViewPopupColumn(new JointDOFControl());
                            else if (pd.PropertyType == typeof(Joint))
                                column = new GridViewTextBoxCellTemplateColumn(
                                    new GridViewItemCell(), Canguro.Model.Model.Instance.JointList);
                            else if (pd.PropertyType == typeof(LineElement))
                                column = new GridViewTextBoxCellTemplateColumn(
                                    new GridViewItemCell(), Canguro.Model.Model.Instance.LineList);
                            else if (pd.PropertyType == typeof(AreaElement))
                                column = new GridViewTextBoxCellTemplateColumn(
                                    new GridViewItemCell(), Canguro.Model.Model.Instance.AreaList);
                            else if (pd.PropertyType == typeof(AssignedLoads))
                                column = new GridViewPopupColumn(new AssignedLoadsControl());
                            else if (pd.PropertyType.Name.Equals("Single[]"))
                                column = new GridViewSingleArrayColumn();
                            else if (pd.PropertyType == typeof(float))
                                column = new GridViewSingleColumn();
                            else if (pd.PropertyType == typeof(bool))
                                column = new DataGridViewCheckBoxColumn();
                            else if (pd.PropertyType == typeof(CardinalPoint))
                                column = new GridViewPopupColumn(new CardinalPointControl());
                            else if (pd.PropertyType == typeof(Model.Constraint))
                                column = new GridViewConstraintComboColumn();
                            else if (pd.PropertyType == typeof(LineEndOffsets))
                                column = new GridViewPopupColumn(new EndOffsetsControl());
                            //else if (pd.PropertyType == typeof(Joint.Bla))
                            //    column = new GridViewLocalizedEnumComboColumn<Joint.Bla>();
                            else
                                column = new DataGridViewTextBoxColumn();

                            string units = "";
                            if (gridUnits.Units != Canguro.Model.UnitSystem.Units.NoUnit)
                                units = " [" + Canguro.Model.Model.Instance.UnitSystem.UnitName(gridUnits.Units)  +"]";
                            column.HeaderText = Culture.Get(pd.DisplayName) + units;
                            column.Name = pd.Name;
                            //column.ReadOnly = pd.IsReadOnly;
                            column.ValueType = pd.PropertyType;
                            column.Width = gridPosition.Width;
                            column.Tag = pd;
                            Columns.Add(column);
                            propList.Add(pd);
                        }
                    }
                }

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                lastUnitSystem = Canguro.Model.Model.Instance.UnitSystem;
            }

            fillSelectedItems(null, true);
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

        public Titem SelectedObject
        {
            get { return (CurrentRow != null) ? getObjectFromRow(CurrentRow.Index) : null; }
        }

        public List<Titem> SelectedObjects
        {
            get 
            {
                DataGridViewSelectedCellCollection cells = SelectedCells;
                List<int> indices = new List<int>();
                List<Titem> list = new List<Titem>();
                Titem item;
                foreach (DataGridViewCell cell in cells)
                    if (cell != null && !indices.Contains(cell.RowIndex) &&
                        (item = getObjectFromRow(cell.RowIndex)) != null)
                    {
                        indices.Add(cell.RowIndex);
                        list.Add(item);
                    }
                return list;
            }
        }

        private Titem getObjectFromRow(int rowIndex)
        {
            if (rowIndex < selectedItems.Count)
                return (Titem)selectedItems[rowIndex];
            return null;
        }

        public void FillDown()
        {
            //DataGridViewSelectedCellCollection cells = this.SelectedCells;
            //if (cells.Count == 1)
            //{
            //    DataGridViewCell cell = cells[0];
            DataGridViewCell cell = CurrentCell;
            if (cell != null && cell.Value != null)
            {
                int colIndex = cell.ColumnIndex;
                object value = cell.Value;

                // If it's a float[], then parse the string
                if (propList[colIndex].PropertyType == typeof(float[]))
                {
                    float[] array = null;
                    char[] separators = { ' ', ',', ';', '\t', '\n', '\r', '/', '\\', '-', '_', ':' };
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
                    }
                    catch (FormatException) { }
                    value = array;
                }

                // Assign value to every row
                foreach (DataGridViewRow row in Rows)
                {
                    Titem obj = getObjectFromRow(row.Index);

                    if (value is StraightFrameProps)
                    {
                        if (obj != null)
                        {
                            if (cell.Value is StraightFrameProps)
                                ((StraightFrameProps)propList[colIndex].GetValue(obj)).Section = ((StraightFrameProps)value).Section;
                        }
                    }
                    else if (value is ICloneable)
                    {
                        if (obj != null)
                            propList[colIndex].SetValue(obj, ((ICloneable)value).Clone());
                    }
                    else
                    {
                        if (obj != null)
                            propList[colIndex].SetValue(obj, value);
                    }
                }

                Model.Model.Instance.Undo.Commit();
                Refresh();
            }
        }

        public void Copy()
        {
            DataObject clip = GetClipboardContent();
            string clipT = clip.GetText();
            string[] clipRows = clipT.Split('\n');

            List<int> selectedRows = new List<int>(SelectedCells.Count);
            
            foreach (DataGridViewCell cell in SelectedCells)
                selectedRows.Add(cell.RowIndex);

            selectedRows.Sort();

            int i = -1, currentRow = -1;
            foreach (int row in selectedRows)
            {
                if (row != currentRow && row < selectedItems.Count)
                {
                    ++i;
                    currentRow = row;
                    clipRows[i] = selectedItems[currentRow].Id.ToString() + clipRows[i];
                }
            }

            clipT = string.Join("\n", clipRows);
            Clipboard.SetText(clipT);
            //Clipboard.SetDataObject(clip, true);
        }
        
        public void Paste(string[][] values)
        {
            DataGridViewCell startCell = null;
            int maxColumn = -1;

            // Find startCell and end column to paste
            foreach (DataGridViewCell cell in SelectedCells)
            {
                if (startCell == null || cell.ColumnIndex < startCell.ColumnIndex)
                    startCell = cell;
                if (startCell == null || cell.ColumnIndex > maxColumn)
                    maxColumn = cell.ColumnIndex;
            }
            if (startCell == null)
                startCell = Rows[0].Cells[0];
            if (maxColumn == -1)
                maxColumn = 0;

            // Now paste the data            
            int numColumns = maxColumn - startCell.ColumnIndex;
            if (numColumns == 0) numColumns = 999999;

            bool pasteWithErrors = false;

            Cursor = Cursors.WaitCursor;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                Rows.Add();
                for (int j = 0; j <= numColumns && j < values[i].Length; j++)
                {
                    try
                    {
                        object val = CurrentCell.ParseFormattedValue(values[i][j], CurrentCell.Style, null, null);
                        CurrentCell = Rows[RowCount - 2].Cells[startCell.ColumnIndex + j];
                        CurrentCell.Value = val;
                    }
                    catch
                    {
                        pasteWithErrors = true;
                    }
                }
                this.ProcessDownKey(Keys.Down);
            }
            Cursor = Cursors.Default;

            if (pasteWithErrors)
                MessageBox.Show(Culture.Get("strPasteWithErrors"));

            CurrentCell = startCell;
        }

        public List<PropertyDescriptor> Properties
        {
            get { return propList; }
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
                        Model.Model.Instance.ModelChanged += updateModel;
                        Model.Model.Instance.SelectionChanged += new Canguro.Model.Model.SelectionChangedEventHandler(updateSelection);
                        ReadOnly = false;
                    }
                    else
                    {
                        Model.Model.Instance.ModelChanged -= updateModel;
                        Model.Model.Instance.SelectionChanged -= new Canguro.Model.Model.SelectionChangedEventHandler(updateSelection);
                        ReadOnly = true;
                    }
                    enabled = value;
                }
            }
        }

        void updateSelection(object sender, Canguro.Model.Model.SelectionChangedEventArgs e)
        {
            fillSelectedItems(e.Picked, true);
        }

        private void updateModel(object sender, System.EventArgs e)
        {
            if (lastUnitSystem != Canguro.Model.Model.Instance.UnitSystem)
            {
                foreach (DataGridViewColumn column in Columns)
                {
                    PropertyDescriptor pd = (PropertyDescriptor)column.Tag;
                    UnitsAttribute gridUnits = ((UnitsAttribute)pd.Attributes[typeof(UnitsAttribute)]);

                    string units = "";
                    if (gridUnits.Units != Canguro.Model.UnitSystem.Units.NoUnit)
                        units = " [" + Canguro.Model.Model.Instance.UnitSystem.UnitName(gridUnits.Units) + "]";

                    if (!string.IsNullOrEmpty(units))
                    {
                        //int i1;//, i2;
                        //i2 = column.HeaderText.LastIndexOf(']');

                        //i1 = column.HeaderText.LastIndexOf('[');
                        //column.HeaderText = column.HeaderText.Substring(0, i1) + units;
                        column.HeaderText = Culture.Get(pd.DisplayName) + units;
                    }
                }
                lastUnitSystem = Canguro.Model.Model.Instance.UnitSystem;
            }

            fillSelectedItems(null, false);
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

            Titem itemTmp = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
                itemTmp = itemInEdit;
            else if (selectedItems.Count > e.RowIndex)
                itemTmp = (Titem)selectedItems[e.RowIndex];
            else
            {
                if ((itemInEdit = getNewTitem()) != null)
                    rowInEdit = Rows.Count - 2;
                else
                    rowInEdit = -1;
            }

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
            if (itemTmp == null) return;

            // Set the appropriate property to the cell value entered.
            try
            {
                propList[e.ColumnIndex].SetValue(itemTmp, e.Value);

                if (commitChange)
                    Canguro.Model.Model.Instance.Undo.Commit();

                base.OnCellValuePushed(e);
            }
            catch (ModelIsLockedException) { }  // Ignore the change if Model is Locked
            catch (ArgumentException) { } // Ignore change if the value is of the correct type
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

            if (changingToSize == -1 || Math.Abs(Rows.Count - changingToSize) <= 1)
            {
                if ((itemInEdit = getNewTitem()) != null)
                    rowInEdit = Rows.Count - 1;
                else
                    rowInEdit = -1;
                changingToSize = -1;
            }
            else
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
            if (e.RowIndex >= selectedItems.Count && e.RowIndex != Rows.Count - 1 && !clearingRows)
            {
                if (itemInEdit == null)
                {
                    CancelEdit();
                    return;
                }

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

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException)
            {
                MessageBox.Show(Culture.Get("gridFormatException"), Application.ProductName);
                displayErrorDialogIfNoHandler = false;
                e.Cancel = true;
            }

            base.OnDataError(displayErrorDialogIfNoHandler, e);
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
        private int changingToSize = -1;
        private void fillSelectedItems(Item picked, bool newSelection)
        {
            if (picked != null && !(picked is Titem)) return;
            if (IsDisposed || Disposing) return;

            int pickedIndex = -1, index = 0;
            selectedItems.Clear();
            foreach (Titem i in source)
                if ((i != null) && (i.IsSelected))
                {
                    selectedItems.Add(i);
                    if (i == picked)
                        pickedIndex = index;
                    index++;
                }

            if (selectedItems.Count > 0)
                RowHeadersWidth = 23 + 6 * (selectedItems[selectedItems.Count - 1].Id.ToString().Length - 1);
            else
                RowHeadersWidth = 23;

            // Set the row count, including the row for new records.
            // If RowCount matches the last RowCount, just Invalidate data
            if (RowCount != selectedItems.Count + 1)
            {
                //if (Rows.Count > 0)
                //    CurrentCell = this.Rows[0].Cells[0];
                changingToSize = selectedItems.Count + 1;
                clearingRows = true;
                try
                {
                    this.Rows.Clear();
                    RowCount = changingToSize;
                }
                finally
                {
                    clearingRows = false;
                    itemInEdit = null;
                    rowInEdit = -1;
                }
            }
            else
                Invalidate();

            if (Rows.Count > 0 && pickedIndex >= 0)
                CurrentCell = this.Rows[pickedIndex].Cells[0];
        }

        private Titem getNewTitem()
        {
            try
            {

                Type t = typeof(Titem);
                if (t == typeof(Joint))
                    return (Titem)(object)new Joint();
                else if (t == typeof(LineElement))
                    return (Titem)(object)new LineElement(new StraightFrameProps());
            }
            catch (Exception) { }

            return null;
        }
        #endregion

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // GridView
            // 
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
