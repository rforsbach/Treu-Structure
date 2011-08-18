using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public class GridViewSingleArrayCell : DataGridViewTextBoxCell
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
}
