using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.DirectX;

namespace Canguro.Model
{
    /// <summary>
    /// La clase Joint representa un nodo en la estructura. Contiene la posición 3D, 
    /// los grados de libertad y las masas en los 6 grados de libertad
    /// </summary>
    [Serializable]
    public class Joint : Element
    {
        private Microsoft.DirectX.Vector3 position;
        private JointDOF dof = new JointDOF();
        private float[] masses = null;
        private Constraint constraint = null;

        [NonSerialized]
        private List<Element> dependents = new List<Element>();

        /// <summary>
        /// Constructora que crea un nodo en la posición indicada.
        /// Se recomienda usar esta constructora para evitar ecceso de memoria en Model.Undo
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Joint(float x, float y, float z)
        {
            position.X = (float.IsNaN(x) || float.IsInfinity(x)) ? 0 : Model.Instance.UnitSystem.ToInternational(x, Canguro.Model.UnitSystem.Units.Distance);
            position.Y = (float.IsNaN(y) || float.IsInfinity(y)) ? 0 : Model.Instance.UnitSystem.ToInternational(y, Canguro.Model.UnitSystem.Units.Distance);
            position.Z = (float.IsNaN(z) || float.IsInfinity(z)) ? 0 : Model.Instance.UnitSystem.ToInternational(z, Canguro.Model.UnitSystem.Units.Distance);
        }

        /// <summary>
        /// Constructora que inicializa los valores con el default (0, 0, 0)
        /// </summary>
        public Joint()
        {
        }

        /// <summary>
        /// Regresa la posición del nodo en el sistema internacional de unidades.
        /// Se usa principalmente para hacer más rápido y consistente el despliegue.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Microsoft.DirectX.Vector3 Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Regresa (asigna) el valor a la componente X de la posición.
        /// Antes de la operación hace la conversión al(del) sistema de unidades activo.
        /// Para el setter, avisa a UndoManager, si hay algún problema, éste avienta una ModelLockedException.
        /// </summary>
        [ModelAttributes.GridPosition(1)]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float X
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(position.X, Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, X, this.GetType().GetProperty("X"));
                position.X = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Regresa (asigna) el valor a la componente Y de la posición.
        /// Antes de la operación hace la conversión al(del) sistema de unidades activo.
        /// Para el setter, avisa a UndoManager, si hay algún problema, éste avienta una ModelLockedException.
        /// </summary>
        [ModelAttributes.GridPosition(2)]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Y
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(position.Y, Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, Y, this.GetType().GetProperty("Y"));
                position.Y = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Regresa (asigna) el valor a la componente Z de la posición.
        /// Antes de la operación hace la conversión al(del) sistema de unidades activo.
        /// Para el setter, avisa a UndoManager, si hay algún problema, éste avienta una ModelLockedException.
        /// </summary>
        [ModelAttributes.GridPosition(3)]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Z
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(position.Z, Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, Z, this.GetType().GetProperty("Z"));
                position.Z = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Genera una lista de solo lectura de Constraints.
        /// Implementación retrasada a la 2a versión.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public System.Collections.ObjectModel.ReadOnlyCollection<Canguro.Model.Constraint> Constraints
        {
            get
            {
                throw new System.NotImplementedException();
/*                List<Constraint> cList = Model.Instance.ConstraintList;
                List<Constraint> retList = new List<Constraint>();
                foreach (Constraint c in cList)
                    if (c.Joints.Contains(this))
                        retList.Add(c);
                return retList;
 */
            }
        }

        /// <summary>
        /// Regresa (asigna) el objeto de grados de libertad.
        /// JointDoF se encarga de hacer los cambios de unidades necesarios.
        /// </summary>
        [ModelAttributes.GridPosition(4, 80)]
        [System.ComponentModel.DisplayName("jointDoFProp")]
        public JointDOF DoF
        {
            get
            {
                return dof;
            }
            set
            {
                Model.Instance.Undo.Change(this, dof, this.GetType().GetProperty("DoF"));
                dof = value.Clone();
            }
        }

        /// <summary>
        /// Regresa (asigna) las masas del nodo.
        /// Clona el arreglo para asegurar consistencia (no permite que alguien más tenga la referencia al arreglo bueno)
        /// Realiza los cambios de unidades necesarios.
        /// </summary>
        [ModelAttributes.GridPosition(30, 100)]
        [System.ComponentModel.DisplayName("jointMassesProp")]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public float[] Masses
        {
            get
            {
                float[] tmp = new float[6];
                if (masses != null)
                {
                    for (int i = 0; i < 6; i++)
                        tmp[i] = Model.Instance.UnitSystem.FromInternational(masses[i], Canguro.Model.UnitSystem.Units.Mass);
                }
                return tmp;
            }
            set
            {
                Model.Instance.Undo.Change(this, Masses, this.GetType().GetProperty("Masses"));
                if (value == null)
                    masses = null;
                else
                {
                    if (masses == null)
                        masses = new float[6];
                    for (int i = 0; i < 6; i++)
                        masses[i] = (i < value.Length)? Model.Instance.UnitSystem.ToInternational(value[i], Canguro.Model.UnitSystem.Units.Mass) : 0;
                }
            }
        }

        /// <summary>
        /// Regresa el identificador del Item.
        /// Sólo permite la asignación cuando el nodo ya está en la lista registrada en el modelo y está en la
        /// posición id. Se usa para renumerar y compactar la lista.
        /// </summary>
        public override uint Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    ItemList<Joint> list = Model.Instance.JointList;
                    if (id == 0 || list[(int)value] == this)
                        id = value;
                    else
                        throw new InvalidIndexException();
                }
            }
        }

        internal void AddDependent(Element elem)
        {
            if (!dependents.Contains(elem))
                dependents.Add(elem);
        }

        internal void RemoveDependent(Element elem)
        {
            if (dependents.Contains(elem))
                dependents.Remove(elem);
        }

        [System.ComponentModel.Browsable(false)]
        internal bool HasDependents
        {
            get { return dependents.Count > 0; }
        }

        [System.ComponentModel.Browsable(false)]
        public bool HassMass
        {
            get
            {
                for (int i = 0; masses != null && i < 6; i++)
                    if (masses[i] != 0)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// The joint might be part of a Constraint, which has to be in Model.ConstraintList.
        /// By default, this is null.
        /// </summary>
        [ModelAttributes.GridPosition(40, 100)]
        [System.ComponentModel.DisplayName("jointConstraintProp")]
        public Constraint Constraint
        {
            get { return constraint; }
            set
            {
                if (value != null && !Model.Instance.ConstraintList.Contains(value))
                    throw new InvalidItemException("Model.ConstraintList does not contain item " + value);
                Model.Instance.Undo.Change(this, constraint, this.GetType().GetProperty("Constraint"));
                constraint = value;
            }
        }

//#if DEBUG
//        // Testing Enum
//        public enum Bla { x, y, z }
//        private Bla blah;

//        [ModelAttributes.GridPosition(100, 100)]
//        public Bla Blah
//        {
//            get { return blah; }
//            set { blah = value; }
//        }
//#endif
    }
}
