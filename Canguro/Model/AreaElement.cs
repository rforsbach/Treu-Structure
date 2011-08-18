using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class AreaElement : Element
    {
        private float angle;
        private Joint[] joints;
        private float mass;
        private float materialTemperature;
        private float[] offsets;
        //private ManagedList<AreaSprings> springs;
        private AreaProps props;
        private int flipJoints;
    
        /// <summary>
        /// Al construir un AreaElement se necesita un prototipo de Propiedades, que define el tipo de elemento que
        /// se forma. De esta manera, después de construir el elemento no se le puede cambiar el tipo, además de que
        /// se pueden crear varios elementos con las mismas propiedades. La constructora clona al prototipo, de manera
        /// que un objeto externo puede crear varios elementos con el mismo objeto de propiedades.
        /// </summary>
        /// <param name="prototype">El prototipo de propiedades.</param>
        public AreaElement(AreaProps prototype)
        {
            props = (AreaProps)prototype.Clone();
            joints = new Joint[4];
            angle = 0;
            mass = 0;
            materialTemperature = 0;
            offsets = new float[4];
            //springs = new ManagedList<AreaSprings>();
            flipJoints = 0;

            Joint j = Model.Instance.JointList[1];
            int i = 1;
            while (j == null && i < Model.Instance.JointList.Count)
                j = Model.Instance.JointList[i++];
            if (j != null)
            {
                J1 = J2 = J3 = j;
                joints[3] = null;
            }
            else
                throw new NullReferenceException(Culture.Get("EM0020"));
        }

        /// <summary>
        /// Constructor de copia que clona un AreaElement a partir de src
        /// </summary>
        /// <param name="src">El AreaElement a copiar.</param>
        internal AreaElement(AreaElement src, Joint j1, Joint j2, Joint j3, Joint j4) : base(src)
        {
            props = (AreaProps)src.Properties.Clone();
            joints = new Joint[4];
            angle = src.Angle;
            mass = src.Mass;
            materialTemperature = src.MaterialTemperature;
            offsets = src.Offsets;
            //springs = a.Springs;
            //springs = new ManagedList<AreaSprings>();
            flipJoints = src.FlipJoints;
            
            joints[0] = j1;
            joints[1] = j2;
            joints[2] = j3;
            joints[3] = j4;
        }

        /// <summary>
        /// Constructora que inicializa el AreaElement con un clon de las propiedades indicadas y los nodos indicados.
        /// Si los nodos no están en la lista correspondiente, en la posición adecuada, se lanza una NullReferenceException
        /// Se recomienda usar esta constructora para minimizar el consumo de memoria en Model.Undo
        /// </summary>
        /// <param name="prototype">Prototype properties to copy from</param>
        /// <param name="jointList">Joints to be added to this AreaElement</param>
        public AreaElement(AreaProps prototype, Joint j1, Joint j2, Joint j3, Joint j4)
        {
            props = (AreaProps)prototype.Clone();
            joints = new Joint[4];
            angle = 0;
            mass = 0;
            materialTemperature = 0;
            offsets = new float[4];
            //springs = new ManagedList<AreaSprings>();

            if (j1 == null || j2 == null || j3 == null)
                throw new NullReferenceException(Culture.Get("EM0021"));

            J1 = j1;
            J2 = j2;
            J3 = j3;
            J4 = j4;
        }
    
        /// <summary>
        /// Regresa el objeto de propiedades del elemento. Para hacer cambios el elemento es responsable de avisar
        /// a UndoManager y de hacer cambios de unidades.
        /// No se permite volver a asignar, por lo que el elemento es del mismo tipo durante todo su ciclo de vida.
        /// </summary>
        [ModelAttributes.GridPosition(5, 100)]
        [System.ComponentModel.DisplayName("areaProps")]
        public AreaProps Properties
        {
            get
            {
                return props;
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
                ItemList<AreaElement> list = Model.Instance.AreaList;
                if (id == 0 || list[(int)value] == this)
                    id = value;
                else
                    throw new InvalidIndexException();
            }
        }

        /// <summary>
        /// Angulo de rotación del elemento sobre su eje principal (1)
        /// </summary>
        [ModelAttributes.GridPosition(6, 73)]
        [System.ComponentModel.DisplayName("areaAngle")]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public float Angle
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(angle, Canguro.Model.UnitSystem.Units.Angle);
            }
            set
            {
                Model.Instance.Undo.Change(this, Angle, GetType().GetProperty("Angle"));
                angle = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Angle);
            }
        }

        /// <summary>
        /// This property gets/sets the Local Axes as a right handed coordinate system.
        /// This Local Axes form a normalized orthogonal basis.
        /// 
        /// The default orientation of the Local Axes is as follows:
        ///     Local Axis 1 and 2: Director axes of the plane of which all Joints are elemts of
        ///     Local Axis 3: Normal vector, perpendicular to 1 and 2. L3 = L1 x L2;
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Microsoft.DirectX.Vector3[] LocalAxes
        {
            get
            {
                Microsoft.DirectX.Vector3[] axes = new Microsoft.DirectX.Vector3[3];

                // First, find the normal to the plane, this would be axis number 3
                if (joints[3] != null)
                    // Cross product of the midpoints of opposite edges
                    axes[2] = Microsoft.DirectX.Vector3.Cross(
                        Microsoft.DirectX.Vector3.Scale(joints[1].Position + joints[2].Position, 0.5f) - 
                        Microsoft.DirectX.Vector3.Scale(joints[3].Position + joints[0].Position, 0.5f),
                        Microsoft.DirectX.Vector3.Scale(joints[2].Position + joints[3].Position, 0.5f) - 
                        Microsoft.DirectX.Vector3.Scale(joints[0].Position + joints[1].Position, 0.5f));
                else
                    axes[2] = Microsoft.DirectX.Vector3.Cross(
                        joints[1].Position - joints[0].Position, 
                        joints[2].Position - joints[1].Position);

                axes[2].Normalize();

                // Second, find the sine of the angle between local 3 axis and Z axis, if this angle is less than 10e-3 the area element is horizontal
                float cosine = Microsoft.DirectX.Vector3.Dot(axes[2], Canguro.Utility.CommonAxes.GlobalAxes[2]);

                // sin^2 = 1-cos^2(theta) => sqrt(1-cos^2(theta)) < 0.001 => 1-cos^2(theta) < 0.000001 => cos^2(theta) > 0.999999
                if (Math.Abs(cosine) > 0.999999) 
                {
                    // Area element is horizontal  and local axis 2 is +Y
                    axes[1] = new Microsoft.DirectX.Vector3(0, 1, 0);
                    // Local Axis 2 follos +Z
                    axes[2] = new Microsoft.DirectX.Vector3(0, 0, Math.Sign(cosine));
                    axes[0] = new Microsoft.DirectX.Vector3(Math.Sign(cosine), 0, 0);
                }
                else
                {
                    // Axis 2 have an upward (+Z) sense...
                    axes[1] = Microsoft.DirectX.Vector3.Normalize(Canguro.Utility.CommonAxes.GlobalAxes[2] - Microsoft.DirectX.Vector3.Scale(axes[2], cosine));
                    axes[0] = Microsoft.DirectX.Vector3.Cross(axes[1], axes[2]);
                    //axes[2] = Microsoft.DirectX.Vector3.Cross(axes[1], axes[0]);
                }


                return axes;
            }
        }

        /// <summary>
        /// This method builds a rotation matrix from the global coordinate system
        /// to the local coordinate system formed by the local axes
        /// </summary>
        /// <param name="m">The rotation matrix to build</param>
        public void RotationMatrix(out Microsoft.DirectX.Matrix m)
        {
            m = Microsoft.DirectX.Matrix.Identity;

            //Obtain local axes
            Microsoft.DirectX.Vector3[] axes = LocalAxes;

            // Build rotation matrix
            m.M11 = axes[0].X; m.M12 = axes[0].Y; m.M13 = axes[0].Z;
            m.M21 = axes[1].X; m.M22 = axes[1].Y; m.M23 = axes[1].Z;
            m.M31 = axes[2].X; m.M32 = axes[2].Y; m.M33 = axes[2].Z;
        }

        [System.ComponentModel.Browsable(false)]
        public float Area
        {
            get
            {
                float area = Microsoft.DirectX.Vector3.Cross(joints[1].Position - joints[0].Position, joints[2].Position - joints[1].Position).Length();
                if (joints[3] != null)
                    area += Microsoft.DirectX.Vector3.Cross(joints[2].Position - joints[0].Position, joints[3].Position - joints[2].Position).Length();
                area *= 0.5f;

                return Model.Instance.UnitSystem.FromInternational(area, Canguro.Model.UnitSystem.Units.AreaBig);
            }
        }

        /// <summary>
        /// Regresa (asigna) los offsets del área.
        /// Clona el arreglo para asegurar consistencia (no permite que alguien más tenga la referencia al arreglo bueno)
        /// Realiza los cambios de unidades necesarios.
        /// </summary>
        [ModelAttributes.GridPosition(8, 80)]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float[] Offsets
        {
            get
            {
                float[] tmp = new float[4];
                if (offsets != null)
                {
                    for (int i = 0; i < 4; i++)
                        tmp[i] = Model.Instance.UnitSystem.FromInternational(offsets[i], Canguro.Model.UnitSystem.Units.SmallDistance);
                }
                return tmp;
            }
            set
            {
                Model.Instance.Undo.Change(this, Offsets, this.GetType().GetProperty("Offsets"));
                if (value == null)
                    offsets = null;
                else
                {
                    if (offsets == null)
                        offsets = new float[4];
                    for (int i = 0; i < 4; i++)
                        offsets[i] = (i < value.Length) ? Model.Instance.UnitSystem.ToInternational(value[i], Canguro.Model.UnitSystem.Units.SmallDistance) : 0;
                }
            }
        }

        internal Joint this[int index]
        {
            get { return joints[index]; }
            set
            {
                switch (index)
                {
                    case 0:
                        J1 = value;
                        break;
                    case 1:
                        J2 = value;
                        break;
                    case 2:
                        J3 = value;
                        break;
                    case 3:
                        J4 = value;
                        break;
                    default:
                        throw new InvalidIndexException();
                }
            }
        }

        [ModelAttributes.GridPosition(1)]
        public Joint J1
        {
            get { return joints[0]; }
            set 
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[0])
                    {
                        if (joints[0] != null)
                            Model.Instance.Undo.Change(this, joints[0], GetType().GetProperty("J1"));
                        if (joints[0] != joints[3] && joints[0] != joints[1] && joints[0] != joints[2] && joints[0] != null)
                            joints[0].RemoveDependent(this);
                        joints[0] = value;
                        joints[0].AddDependent(this);
                    }
                }
                else if (value == null)
                    throw new System.NullReferenceException("AreaElement can not have any of the first three joints as null");
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        [ModelAttributes.GridPosition(2)]
        public Joint J2
        {
            get { return joints[1]; }
            set
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[1])
                    {
                        if (joints[1] != null)
                            Model.Instance.Undo.Change(this, joints[1], GetType().GetProperty("J2"));
                        if (joints[1] != joints[0] && joints[1] != joints[3] && joints[1] != joints[2] && joints[1] != null)
                            joints[1].RemoveDependent(this);
                        joints[1] = value;
                        joints[1].AddDependent(this);
                    }
                }
                else if (value == null)
                    throw new System.NullReferenceException("AreaElement can not have any of the first three joints as null");
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        [ModelAttributes.GridPosition(3)]
        public Joint J3
        {
            get { return joints[2]; }
            set
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[2])
                    {
                        if (joints[2] != null)
                            Model.Instance.Undo.Change(this, joints[2], GetType().GetProperty("J3"));
                        if (joints[2] != joints[0] && joints[2] != joints[1] && joints[2] != joints[3] && joints[2] != null)
                            joints[2].RemoveDependent(this);
                        joints[2] = value;
                        joints[2].AddDependent(this);
                    }
                }
                else if (value == null)
                    throw new System.NullReferenceException("AreaElement can not have any of the first three joints as null");
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        [ModelAttributes.GridPosition(4)]
        public Joint J4
        {
            get { return joints[3]; }
            set
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[3])
                    {
                        Model.Instance.Undo.Change(this, joints[3], GetType().GetProperty("J4"));
                        if (joints[3] != joints[0] && joints[3] != joints[1] && joints[3] != joints[2] && joints[3] != null)
                            joints[3].RemoveDependent(this);
                        joints[3] = value;
                        joints[3].AddDependent(this);
                    }
                }
                else if (value == null)
                {
                    if (joints[3] != null)
                    {
                        Model.Instance.Undo.Change(this, joints[3], GetType().GetProperty("J4"));
                        joints[3].RemoveDependent(this);
                    }
                    joints[3] = value;
                }
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        [System.ComponentModel.Browsable(false)]
        public ManagedList<AreaSprings> Springs
        {
            get
            {
                throw new NotImplementedException();
                //return springs;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsPlanar
        {
            get
            {
                if (joints[3] == null) return true;

                Microsoft.DirectX.Vector3 j0 = joints[0].Position, j3 = joints[3].Position;
                Microsoft.DirectX.Vector3 n = Microsoft.DirectX.Vector3.Cross(joints[1].Position - j0, j3 - j0);
                float distanceSq = Utility.GeometricUtils.DistancePoint2Plane(ref j0, ref n, ref j3);

                distanceSq = distanceSq * distanceSq / n.LengthSq();
                if (distanceSq > Utility.GeometricUtils.Epsilon)
                    return false;

                return true;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsConvex
        {
            get 
            {
                return ConcaveJointIndex == -1;
            }
        }

        internal int ConcaveJointIndex
        {
            get
            {
                if (joints[3] != null && IsPlanar)
                {
                    Microsoft.DirectX.Vector3 vLeft, vRight;

                    // Check for each vertex if the normal at that vertex points in the same direction as the plane normal. If not, then the vertex forms a concavitie
                    for (int index = 0; index < 4; ++index)
                    {
                        vLeft = joints[index].Position - joints[index - 1 < 0 ? 3 : index - 1].Position;
                        vRight = joints[index + 1 > 3 ? 0 : index + 1].Position - joints[index].Position;
                        if (Microsoft.DirectX.Vector3.Dot(Microsoft.DirectX.Vector3.Cross(vLeft, vRight), LocalAxes[2]) < 0) // We found the vertex that make this quad a concave polygon
                            return index;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Flips the joints of the AreaElement by shifting each joint to the next position 
        /// and the last one to the first.
        /// </summary>
        public int FlipJoints
        {
            get
            {
                return flipJoints;
            }
            set
            {
                int flipDelta = (value % 4) - flipJoints;

                for (int i = 0; i < Math.Abs(flipDelta); i++)
                {
                    if (flipDelta > 0)
                    {
                        Joint j3 = joints[3];

                        if (j3 == null)
                            j3 = joints[2];
                        else
                            joints[3] = joints[2];

                        joints[2] = joints[1];
                        joints[1] = joints[0];
                        joints[0] = j3;
                    }
                    else if (flipDelta < 0)
                    {
                        Joint j0 = joints[0];

                        joints[0] = joints[1];
                        joints[1] = joints[2];
                        if (joints[3] == null)
                            joints[2] = j0;
                        else
                        {
                            joints[2] = joints[3];
                            joints[3] = j0;
                        }
                    }
                }

                Model.Instance.Undo.Change(this, flipJoints, GetType().GetProperty("FlipJoints"));
                flipJoints = Math.Abs(value % 4);
            }
        }

        [ModelAttributes.GridPosition(7)]
        public float Mass
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(mass, Canguro.Model.UnitSystem.Units.Mass2D);
            }
            set
            {
                Model.Instance.Undo.Change(this, mass, this.GetType().GetProperty("Mass"));
                mass = Model.Instance.UnitSystem.ToInternational(Math.Max(0f, value), Canguro.Model.UnitSystem.Units.Mass2D);
            }
        }

        [System.ComponentModel.Browsable(false)]
        [ModelAttributes.GridPosition(9)]
        public float MaterialTemperature
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(materialTemperature, Canguro.Model.UnitSystem.Units.Temperature);
            }
            set
            {
                Model.Instance.Undo.Change(this, materialTemperature, this.GetType().GetProperty("MaterialTemperature"));
                materialTemperature = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Temperature);
            }
        }
    }
}
