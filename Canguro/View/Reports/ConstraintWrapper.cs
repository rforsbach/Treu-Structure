using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.View.Reports
{
    class ConstraintWrapper : ReportData
    {
        string constraint;
        uint jointID;

        public ConstraintWrapper(uint jointID, string constraint)
        {
            this.jointID = jointID;
            this.constraint = constraint;
        }


        public static List<ReportData> GetAllConstraints(Model.Model model)
        {
            List<ReportData> list = new List<ReportData>();
            foreach (Constraint c in model.ConstraintList)
            {
                List<Joint> joints = c.GetJoints();

                foreach (Joint j in joints)
                    if (j != null)
                        list.Add(new ConstraintWrapper(j.Id, c.Name));

                if (joints.Count == 0)
                    list.Add(new ConstraintWrapper(0, c.Name));
            }
            return list;
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 4000)]
        public string Constraint
        {
            get { return constraint; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 4000)]
        public string Joint
        {
            get { return (jointID > 0) ? jointID.ToString() : "NA"; }
            set { }
        }
    }
}
