using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Canguro.Model;

namespace Canguro.Controller.Grid
{
    public class GridViewConstraintComboCell : DataGridViewComboBoxCell
    {
        protected Dictionary<string, Constraint> allValues = null;
        protected readonly ConstraintList datasource = new ConstraintList();
        private EventHandler indexChangedEventHandler = null;

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            Control c = DataGridView.EditingControl;
            if (c is ComboBox)
            {
                if (indexChangedEventHandler == null)
                    indexChangedEventHandler = new EventHandler(GridViewConstraintComboCell_SelectedIndexChanged);
                ((ComboBox)c).SelectedIndexChanged -= indexChangedEventHandler;
                ((ComboBox)c).SelectedIndexChanged += indexChangedEventHandler;
            }

        }

        void GridViewConstraintComboCell_SelectedIndexChanged(object sender, EventArgs e)
        {
            string val = ((Control)sender).Text;
            Console.Write(". ");
            if (ConstraintList.NewConstraint.Equals(val) && DataGridView != null && this.IsInEditMode)
            {
                NewConstraintDialog dlg = new NewConstraintDialog();
                if (dlg.ShowDialog(this.DataGridView) == DialogResult.OK)
                {
                    Constraint cons = dlg.Constraint;
                    base.SetValue(RowIndex, cons);
                    DataGridView.NotifyCurrentCellDirty(false);
                    //base.Selected = false;
                    this.DataGridView.EndEdit();
                    if (sender is ComboBox)
                        ((ComboBox)sender).SelectedIndexChanged -= indexChangedEventHandler;
                }
            }
        }

        protected Dictionary<string, Constraint> AllValues
        {
            get
            {
                if (allValues == null)
                {
                    allValues = new Dictionary<string, Constraint>();
                    foreach (Constraint val in Model.Model.Instance.ConstraintList)
                        allValues.Add(val.ToString(), val);
                    allValues.Add(ConstraintList.NewConstraint, null);
                    allValues.Add(ConstraintList.NoConstraint, null);
                }
                return allValues;
            }
        }

        protected override object GetValue(int rowIndex)
        {
            object value = base.GetValue(rowIndex);
            if (value is Constraint)
                return value;
            if (value != null)
                return AllValues[value.ToString()];
            return null;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            //if (value is EnumType)
            //{
            //    string str = null;
            //    foreach (string key in AllValues.Keys)
            //        if (AllValues[key].Equals(value))
            //        {
            //            str = key;
            //            break;
            //        }
            //    if (!string.IsNullOrEmpty(str))
            //        return base.SetValue(rowIndex, str);
            //}

            if (value != null && AllValues.ContainsKey(value.ToString()))
                return base.SetValue(rowIndex, AllValues[value.ToString()]);

            return base.SetValue(rowIndex, null);
        }

        

        public override object DataSource
        {
            get
            {
                //if (datasource == null)
                //{
                //    datasource = new string[AllValues.Count];
                //    int i = 0;
                //    foreach (string key in AllValues.Keys)
                //        datasource[i++] = key;
                //}
                //return datasource;
                List<object> list = new List<object>();
                list.Add(ConstraintList.NoConstraint);
                foreach (Constraint cons in Model.Model.Instance.ConstraintList)
                    list.Add(cons);
                list.Add(ConstraintList.NewConstraint);
                return list;
            }
            set
            {
//                base.DataSource = value;
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(object);
            }
        }


        public override Type FormattedValueType
        {
            get
            {
                return typeof(string);
            }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value is Constraint)
            {
                return value.ToString();
            }
            return string.Empty;
        }

        protected class ConstraintList : IEnumerable<string>
        {
            #region IEnumerable<Constraint> Members

            public static readonly string NoConstraint = Culture.Get("NoConstraint");
            public static readonly string NewConstraint = Culture.Get("NewConstraint");

            public string this[int index]
            {
                get
                {
                    IList<Constraint> constraints = Model.Model.Instance.ConstraintList;
                    return (index == 0) ? NoConstraint : (index < constraints.Count && constraints[index - 1] != null) ? constraints[index - 1].ToString() : NewConstraint;
                }
            }

            public int Count
            {
                get
                {
                    return Model.Model.Instance.ConstraintList.Count + 2;
                }
            }

            IEnumerator<string> enumerator = null;
            public IEnumerator<string> GetEnumerator()
            {
                if (enumerator == null)
                    enumerator = new Enumerator(this);
                return enumerator;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            public struct Enumerator : IEnumerator<string>
            {

                #region IEnumerator<string> Members

                ConstraintList list;
                private int current;

                public Enumerator(ConstraintList list)
                {
                    current = 0;
                    this.list = list;
                }

                public string Current
                {
                    get { return list[current]; }
                }

                #endregion

                #region IDisposable Members

                public void Dispose()
                {
                }

                #endregion

                #region IEnumerator Members

                object System.Collections.IEnumerator.Current
                {
                    get { return list[current]; }
                }

                public bool MoveNext()
                {
                    current++;
                    return list.Count > current;
                }

                public void Reset()
                {
                    current = -1;
                }

                #endregion
            }
        }
    }
}
