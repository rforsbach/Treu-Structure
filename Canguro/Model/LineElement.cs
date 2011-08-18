using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    
    /// <summary>
    /// La clase LineElement representa un elemento finito unidimensional. 
    /// Se define principalmente con dos nodos.
    /// El elemento puede ser una barra, cable o tensor.
    /// En la primera versión sólo la barra de sección uniforme está soportada.
    /// </summary>
    [Serializable]
    public class LineElement : Element
    {
        private LineProps props;
        private Joint[] joints;
        private float angle;
        private JointDOF dofI;
        private JointDOF dofJ;
        private CardinalPoint cardinalPoint = CardinalPoint.Centroid;
        private LineEndOffsets offsets = LineEndOffsets.Empty;

        internal void GetEndOffsets(ref float endI, ref float endJ)
        {
            endI = offsets.EndIInternational;
            endJ = offsets.EndJInternational;
        }

        /// <summary>
        /// Al construir un LineElement se necesita un prototipo de Propiedades, que define el tipo de elemento que
        /// se forma. De esta manera, después de construir el elemento no se le puede cambiar el tipo, además de que
        /// se pueden crear varios elementos con las mismas propiedades. La constructora clona al prototipo, de manera
        /// que un objeto externo puede crear varios elementos con el mismo objeto de propiedades.
        /// </summary>
        /// <param name="prototype">El prototipo de propiedades.</param>
        public LineElement(LineProps prototype)
        {
            props = (LineProps)prototype.Clone();
            joints = new Joint[2];
            angle = 0;
            dofI = new JointDOF(true);
            dofJ = new JointDOF(true);

            Joint j = Model.Instance.JointList[1];
            int i = 1;
            while (j == null && i < Model.Instance.JointList.Count)
                j = Model.Instance.JointList[i++];
            if (j != null)
            {
                I = j;
                J = j;
            }
            else
                throw new NullReferenceException(Culture.Get("EM0020"));
        }

        /// <summary>
        /// Constructora que inicializa el LineElement con un clon de las propiedades indicadas y los nodos indicados.
        /// Si los nodos no están en la lista correspondiente, en la posición adecuada, se lanza una NullReferenceException
        /// Se recomienda usar esta constructora para minimizar el consumo de memoria en Model.Undo
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public LineElement(LineProps prototype, Joint i, Joint j)
        {
            props = (LineProps)prototype.Clone();
            joints = new Joint[2];
            angle = 0;
            dofI = new JointDOF(true);
            dofJ = new JointDOF(true);

            if (i == null || j == null || 
                Model.Instance.JointList[i.Id] != i || Model.Instance.JointList[j.Id] != j)
                throw new NullReferenceException(Culture.Get("EM0021"));
            I = i;
            J = j;
        }

        /// <summary>
        /// Constructora de copia que inicializa el LineElement como un clon de otro.
        /// </summary>
        /// <param name="src">El LineElement a copiar</param>
        internal LineElement(LineElement src, Joint i, Joint j) : base(src)
        {
            props = (LineProps)src.Properties.Clone();
            joints = new Joint[2];
            angle = src.Angle;
            cardinalPoint = src.CardinalPoint;
            offsets = src.EndOffsets;
            dofI = src.DoFI;
            dofJ = src.DoFJ;
            joints[0] = i;
            joints[1] = j;
        }

        /// <summary>
        /// Regresa el objeto de propiedades del elemento. Para hacer cambios el elemento es responsable de avisar
        /// a UndoManager y de hacer cambios de unidades.
        /// No se permite volver a asignar, por lo que el elemento es del mismo tipo durante todo su ciclo de vida.
        /// </summary>
        [ModelAttributes.GridPosition(3, 100, true)]
        [System.ComponentModel.DisplayName("lineProps")]
        public LineProps Properties
        {
            get
            {
                return props;
            }
        }

        /// <summary>
        /// El nodo inicial (I) del elemento.
        /// Para hacer la asignación es necesario revisar que el nodo está en la lista correspondiente.
        /// </summary>
        [ModelAttributes.GridPosition(1)]
        public Joint I
        {
            get
            {
                return joints[0];
            }
            set
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[0])
                    {
                        if (joints[0] != null)
                            Model.Instance.Undo.Change(this, joints[0], GetType().GetProperty("I"));
                        if (joints[0] != joints[1] && joints[0] != null)
                            joints[0].RemoveDependent(this);
                        joints[0] = value;
                        joints[0].AddDependent(this);
                    }
                }
                else if (value == null)
                    throw new System.NullReferenceException("LineElement can not have null joints");
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        /// <summary>
        /// El nodo final (J) del elemento.
        /// Para hacer la asignación es necesario revisar que el nodo está en la lista correspondiente.
        /// </summary>
        [ModelAttributes.GridPosition(2)]
        public Joint J
        {
            get
            {
                return joints[1];
            }
            set
            {
                if (value != null && Model.Instance.JointList.Contains(value))
                {
                    if (value != joints[1])
                    {
                        if (joints[1] != null)
                            Model.Instance.Undo.Change(this, joints[1], GetType().GetProperty("J"));
                        if (joints[0] != joints[1] && joints[1] != null)
                            joints[1].RemoveDependent(this);
                        joints[1] = value;
                        joints[1].AddDependent(this);
                    }
                }
                else if (value == null)
                    throw new System.NullReferenceException("LineElement can not have null joints");
                else
                    throw new InvalidIndexException("Joint " + value.ToString() + " is not in JointList");
            }
        }

        /// <summary>
        /// Calcula la longitud del elemento y la convierte al sistema de unidades activo.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Length
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational((joints[1].Position - joints[0].Position).Length(), Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Calcula la longitud del elemento en sistema de unidades internacional.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public float LengthInt
        {
            get
            {
                return (joints[1].Position - joints[0].Position).Length();
            }
        }

        /// <summary>
        /// Angulo de rotación del elemento sobre su eje principal (1)
        /// </summary>
        [ModelAttributes.GridPosition(4, 73)]
        [System.ComponentModel.DisplayName("lineAngle")]
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
                angle = value;
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
                ItemList<LineElement> list = Model.Instance.LineList;
                if (id == 0 || list[(int)value] == this)
                    id = value;
                else
                    throw new InvalidIndexException();
            }
        }

        /// <summary>
        /// This property gets/sets the Local Axes as a right handed coordinate system.
        /// This Local Axes form a normalized orthogonal basis.
        /// 
        /// The default orientation of the Local Axes is as follows:
        ///     Local Axis 1: Longitudinal, follows the path from joint I to joint J
        ///     Local Axis 2: Upward sense, trying to follow +Z global axis
        ///         Expection: if cos(angle) between local 1 and Z > 0.999999, local 2 = global X
        ///     Local Axis 3: Perpendicular to local 1 and 2
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Microsoft.DirectX.Vector3[] LocalAxes
        {
            // Cortesía del Hippie Come Flores
            get
            {
                Microsoft.DirectX.Vector3[] axes = new Microsoft.DirectX.Vector3[3];
                Microsoft.DirectX.Vector3 zvec = new Microsoft.DirectX.Vector3(0, 0, 1);
                
                // Longitudinal axis
                axes[0] = Microsoft.DirectX.Vector3.Normalize(J.Position - I.Position);

                // Calculate projection of Z and n. Try to find local 2 axis following Z
                float zn = Microsoft.DirectX.Vector3.Dot(zvec, axes[0]);

                // Axis parallel to Z (Dot product near maximum value equals parallelism)
                if (Math.Abs(zn) > 0.999999)
                {
                    // Local Axis 2 = X and 3 = +/- Y
                    axes[1] = new Microsoft.DirectX.Vector3(1, 0, 0);
                    axes[2] = new Microsoft.DirectX.Vector3(0, Math.Sign(zn), 0);
                    //axes[2] = new Microsoft.DirectX.Vector3(1, 0, 0);
                    //axes[1] = new Microsoft.DirectX.Vector3(0, Math.Sign(zn), 0);
                    axes[0] = new Microsoft.DirectX.Vector3(0, 0, Math.Sign(zn));
                }
                else
                {
                    // Local 2 equals the difference between the projected Z at n and Z
                    axes[1] = Microsoft.DirectX.Vector3.Normalize(zvec - Microsoft.DirectX.Vector3.Scale(axes[0], zn));
                    axes[2] = Microsoft.DirectX.Vector3.Cross(axes[0], axes[1]);
                    //axes[2] = Microsoft.DirectX.Vector3.Cross(axes[1], axes[0]);
                }

                if (angle != 0f)
                {
                    Microsoft.DirectX.Matrix rotMatrix = Microsoft.DirectX.Matrix.RotationAxis(axes[0], (float)(angle * Math.PI / 180.0));
                    axes[1].TransformCoordinate(rotMatrix);
                    axes[2].TransformCoordinate(rotMatrix);
                }

                return axes;
            }
        }

        /// <summary>
        /// This method builds a rotation matrix from the global coordinate system
        /// to the local coordinate system formed by the local axes
        /// </summary>
        /// <param name="m">The rotation matrix to build</param>
        public void RotationMatrix (out Microsoft.DirectX.Matrix m)
        {
            m = Microsoft.DirectX.Matrix.Identity;

            //Obtain local axes
            Microsoft.DirectX.Vector3[] axes = LocalAxes;

            // Build rotation matrix
            m.M11 = axes[0].X; m.M12 = axes[0].Y; m.M13 = axes[0].Z;
            m.M21 = axes[1].X; m.M22 = axes[1].Y; m.M23 = axes[1].Z;
            m.M31 = axes[2].X; m.M32 = axes[2].Y; m.M33 = axes[2].Z;
        }

        /// <summary>
        /// Regresa (asigna) el objeto de grados de libertad en el extremo I.
        /// JointDoF se encarga de hacer los cambios de unidades necesarios.
        /// </summary>
        [ModelAttributes.GridPosition(5, 80)]
        public JointDOF DoFI
        {
            get
            {
                return (JointDOF)dofI.Clone();
            }
            set
            {
                Model.Instance.Undo.Change(this, dofI, this.GetType().GetProperty("DoFI"));
                dofI = value.Clone();
                dofJ.T1 = (dofI.T1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T1;
                dofJ.T2 = (dofI.T2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T2;
                dofJ.T3 = (dofI.T3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T3;
                dofJ.R1 = (dofI.R1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.R1;
                dofJ.T2 = (dofI.R3 == JointDOF.DofType.Free && dofJ.R3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T2;
                dofJ.T3 = (dofI.R2 == JointDOF.DofType.Free && dofJ.R2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T3;
            }
        }

        /// <summary>
        /// Regresa (asigna) el objeto de grados de libertad en el extremo J.
        /// JointDoF se encarga de hacer los cambios de unidades necesarios.
        /// </summary>
        [ModelAttributes.GridPosition(6, 80)]
        public JointDOF DoFJ
        {
            get
            {
                return (JointDOF)dofJ.Clone();
            }
            set
            {
                Model.Instance.Undo.Change(this, dofJ, this.GetType().GetProperty("DoFJ"));
                dofJ = value.Clone();
                dofI.T1 = (dofJ.T1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.T1;
                dofI.T2 = (dofJ.T2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.T2;
                dofI.T3 = (dofJ.T3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.T3;
                dofI.R1 = (dofJ.R1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.R1;
                dofI.T2 = (dofI.R3 == JointDOF.DofType.Free && dofJ.R3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.T2;
                dofI.T3 = (dofI.R2 == JointDOF.DofType.Free && dofJ.R2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofI.T3;
            }
        }

        /// <summary>
        /// Gets or Sets the cardinal point for the LineElement.
        /// Represents the point in the selected Section where the element's axis is.
        /// </summary>
        [ModelAttributes.GridPosition(7, 90)]
        public CardinalPoint CardinalPoint
        {
            get
            {
                return cardinalPoint;
            }
            set
            {
                Model.Instance.Undo.Change(this, cardinalPoint, this.GetType().GetProperty("CardinalPoint"));
                cardinalPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the End Length Offsets for the Line Element.
        /// The value is cloned when read or written.
        /// </summary>
        [ModelAttributes.GridPosition(8, 100)]
#if !DEBUG
        [System.ComponentModel.Browsable(false)]
#endif
        public LineEndOffsets EndOffsets
        {
            get
            {
                return (LineEndOffsets)offsets.Clone();
            }
            set
            {
                if (!offsets.Equals(value))
                {
                    Model.Instance.Undo.Change(this, EndOffsets, this.GetType().GetProperty("EndOffsets"));
                    if (LineEndOffsets.Empty.Equals(value))
                        offsets = LineEndOffsets.Empty;
                    else
                        offsets = (LineEndOffsets)value.Clone();
                }
            }
        }
    }
}
