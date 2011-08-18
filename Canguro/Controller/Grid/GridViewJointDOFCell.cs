using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Canguro.Model;

namespace Canguro.Controller.Grid
{
    public class GridViewJointDOFCell : GridViewPopupCell
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
}
