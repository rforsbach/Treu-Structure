using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Constraint Class. Currently, only Diaphragm constraint is implemented.
    /// </summary>
    [Serializable]
    public class Constraint : INamed
    {
        private string name = "";

        /// <summary>
        /// Constructor. Sets the given name.
        /// </summary>
        /// <param name="name"></param>
        public Constraint(string name)
        {
            Name = name;
            Model.Instance.ConstraintList.ElementRemoved += new ManagedList<Constraint>.ListChangedEventHandler(ConstraintList_ElementRemoved);
        }

        void ConstraintList_ElementRemoved(object sender, ListChangedEventArgs<Constraint> args)
        {
            if (args.ChangedObject.Name.Equals(this.Name))
                foreach (Joint j in GetJoints())
                    j.Constraint = null;
        }

        /// <summary>
        /// Default Constructor. Sets a default name to the constraint.
        /// </summary>
        public Constraint() : this(Culture.Get("DefaultConstraintName"))
        {
        }

        /// <summary>
        /// The name of the constraint.
        /// Checks Model.ConstraintList to make sure it's unique.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                value = value.Trim().Replace("\"", "''");
                value = (value.Length > 0) ? value : Culture.Get("jointConstraintProp");
                string tmp = value;
                bool unique = false;
                int i = 0;
                while (!unique)
                {
                    unique = true;
                    foreach (Constraint cons in Model.Instance.ConstraintList)
                        if (cons != this && tmp.Equals(cons.Name))
                            unique = false;
                    if (!unique)
                        tmp = string.Format("{0} ({1})", value, ++i);
                }
                Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                name = tmp;
            }
        }

        /// <summary>
        /// Allways "GLOBAL"
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public string CoordinateSystem
        {
            get { return "GLOBAL"; }
        }

        /// <summary>
        /// Allways Diaphragm
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public ConstType ConstraintType
        {
            get { return ConstType.Diaphragm; }
        }

        /// <summary>
        /// Allways Z
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public ConstraintAxis Axis
        {
            get { return ConstraintAxis.Z; }
        }

        public override string ToString()
        {
            return Name;
        }

        public enum ConstType
        {
            Body, Diaphragm, Plate, Rod, Beam, Equal, Local, Weld, Line
        }

        public enum ConstraintAxis
        {
            X, Y, Z,
        }

        public enum ConstraintDOF
        {
            Tx, Ty, Tz, Rx, Ry, Rz,
        }

        /// <summary>
        /// Creates and returns a list with all the joints in the constraint
        /// </summary>
        /// <returns></returns>
        public List<Joint> GetJoints()
        {
            List<Joint> list = new List<Joint>();
            foreach (Joint j in Model.Instance.JointList)
                if (j != null && j.Constraint == this)
                    list.Add(j);
            return list;
        }
    }
}
