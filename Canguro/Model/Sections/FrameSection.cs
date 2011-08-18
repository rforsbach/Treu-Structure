using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Clase abstracta que representa la sección de una barra (columna o viga)
    /// </summary>
    [Serializable]
    public abstract class FrameSection : Utility.GlobalizedObject, Section
    {
        #region Fields
        /// <summary>Width</summary>
        protected float t3;
        /// <summary>Height</summary>
        protected float t2;
        /// <summary>Hzt plate width</summary>
        protected float tf;
        /// <summary>Vertical plate width</summary>
        protected float tw;
        /// <summary>Other dimension</summary>
        protected float t2b;
        /// <summary>Other dimension</summary>
        protected float tfb;
        /// <summary>Distance between section parts</summary>
        protected float dis;
        /// <summary>Cross area</summary>
        protected float area;
        /// <summary>Torsional constant</summary>
        protected float torsConst;
        /// <summary>Moment of Inertia about 3 axis</summary>
        protected float i33;
        /// <summary>Moment of Inertia about 2 axis</summary>
        protected float i22;
        /// <summary>Shear area in 2 direction</summary>
        protected float as2;
        /// <summary>Shear area in 3 direcion</summary>
        protected float as3;
        /// <summary>Section modulus about 3 axis</summary>
        protected float s33;
        /// <summary>Section modulus about 2 axis</summary>
        protected float s22;
        /// <summary>Plastic modulus about 3 axis</summary>
        protected float z33;
        /// <summary>Plastic modulus about 2 axis</summary>
        protected float z22;
        /// <summary>Radius of gyration about 3 axis</summary>
        protected float r33;
        /// <summary>Radius of gyration about 2 axis</summary>
        protected float r22;

        protected Microsoft.DirectX.Vector2[][] contour;
        private ConcreteSectionProps concreteProps;
        private string name = "";
        private string shape = "";
        private Material.Material material;
        [NonSerialized]
        protected float lodSize;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructora que asigna valores deafault a todas las variables.
        /// </summary>
        public FrameSection() : this(Culture.Get("defaultSectionName"), SectionManager.Instance.DefaultShape, Canguro.Model.Material.MaterialManager.Instance.DefaultSteel, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0) { }

        /// <summary>
        /// Constructora que asigna nombre, forma, material y propiedades de concreto, y asigna valores default a las demás variables.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shape"></param>
        /// <param name="material"></param>
        /// <param name="concreteProperties"></param>
        public FrameSection(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties) : this(name, shape, material, concreteProperties, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0) { }

        /// <summary>
        /// Constructora que asigna todas las variables que definen la sección en unidades internacionales.
        /// </summary>
        /// <param name="name">Section name</param>
        /// <param name="shape">Shape name</param>
        /// <param name="material">Material</param>
        /// <param name="concreteProperties">Concrete properties (for concrete sections only)</param>
        /// <param name="t3">Width</param>
        /// <param name="t2">Height</param>
        /// <param name="tf">Hzt plate width</param>
        /// <param name="tw">Vertical plate width</param>
        /// <param name="t2b">Other dimension</param>
        /// <param name="tfb">Other dimension</param>
        /// <param name="dis">Distance between section parts</param>
        /// <param name="area">Cross area</param>
        /// <param name="torsConst">Torsional constant</param>
        /// <param name="i33">Moment of Inertia about 3 axis</param>
        /// <param name="i22">Moment of Inertia about 2 axis</param>
        /// <param name="as2">Shear area in 2 direction</param>
        /// <param name="as3">Shear area in 3 direcion</param>
        /// <param name="s33">Section modulus about 3 axis</param>
        /// <param name="s22">Section modulus about 2 axis</param>
        /// <param name="z33">Plastic modulus about 3 axis</param>
        /// <param name="z22">Plastic modulus about 2 axis</param>
        /// <param name="r33">Radius of gyration about 3 axis</param>
        /// <param name="r22">Radius of gyration about 2 axis</param>
        public FrameSection(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
        {
            this.shape = shape;
            this.material = material;
            this.concreteProps = concreteProperties;
            contour = new Microsoft.DirectX.Vector2[2][];

            this.t3 = t3;
            this.t2 = t2;
            this.tf = tf;
            this.tw = tw;
            this.t2b = t2b;
            this.tfb = tfb;
            this.dis = dis;
            this.area = area;
            this.torsConst = torsConst;
            this.i33 = i33;
            this.i22 = i22;
            this.as2 = as2;
            this.as3 = as3;
            this.s33 = s33;
            this.s22 = s22;
            this.z33 = z33;
            this.z22 = z22;
            this.r33 = r33;
            this.r22 = r22;
            Name = name;

            initContourAndLOD();
        }

        /// <summary>
        /// Constructora que calcula las propiedades de una sección general a partir del contorno
        /// No está implementada en la primera versión.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shape"></param>
        /// <param name="material"></param>
        /// <param name="concreteProperties"></param>
        /// <param name="contour"></param>
        public FrameSection(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, Microsoft.DirectX.Vector2[] contour)
        {
            throw new System.NotImplementedException("Falta un SectionDesigner que calcule las propiedades");
        }
        #endregion

        #region Physical and Geometrical properties
        /// <summary>
        /// Plastic modulus about 3 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ShearModulus)]
        public float Z33
        {
            get { return Model.Instance.UnitSystem.FromInternational(z33, Canguro.Model.UnitSystem.Units.ShearModulus); }
            set {
                if (value != Z33)
                {
                    Model.Instance.Undo.Change(this, Z33, GetType().GetProperty("Z33"));
                    z33 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ShearModulus);
                }
            }
        }

        /// <summary>
        /// Plastic modulus about 2 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ShearModulus)]
        public float Z22
        {
            get { return Model.Instance.UnitSystem.FromInternational(z22, Canguro.Model.UnitSystem.Units.ShearModulus); }
            set {
                if (value != Z22)
                {
                    Model.Instance.Undo.Change(this, Z22, GetType().GetProperty("Z22"));
                    z22 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ShearModulus);
                }
            }
        }

        /// <summary>
        /// Vertical plate width
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float Tw
        {
            get { return Model.Instance.UnitSystem.FromInternational(tw, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != Tw)
                {
                    Model.Instance.Undo.Change(this, Tw, GetType().GetProperty("Tw"));
                    tw = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Torsional Constant J. 
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.AreaInertia)]
        public float TorsConst
        {
            get { return Model.Instance.UnitSystem.FromInternational(torsConst, Canguro.Model.UnitSystem.Units.AreaInertia); }
            set
            {
                if (value != torsConst)
                {
                    Model.Instance.Undo.Change(this, torsConst, GetType().GetProperty("TorsConst"));
                    torsConst = value;
                }
            }
        }

        /// <summary>
        /// Other dimension
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float Tfb
        {
            get { return Model.Instance.UnitSystem.FromInternational(tfb, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != Tfb)
                {
                    Model.Instance.Undo.Change(this, Tfb, GetType().GetProperty("Tfb"));
                    tfb = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Distance between section parts
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float Dis
        {
            get { return Model.Instance.UnitSystem.FromInternational(dis, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != Dis)
                {
                    Model.Instance.Undo.Change(this, Dis, GetType().GetProperty("Dis"));
                    dis = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Hzt plate width
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float Tf
        {
            get { return Model.Instance.UnitSystem.FromInternational(tf, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != Tf)
                {
                    Model.Instance.Undo.Change(this, Tf, GetType().GetProperty("Tf"));
                    tf = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Section width
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float T3
        {
            get { return Model.Instance.UnitSystem.FromInternational(t3, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != T3)
                {
                    Model.Instance.Undo.Change(this, T3, GetType().GetProperty("T3"));
                    t3 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Other dimension
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float T2b
        {
            get { return Model.Instance.UnitSystem.FromInternational(t2b, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != T2b)
                {
                    Model.Instance.Undo.Change(this, T2b, GetType().GetProperty("T2b"));
                    t2b = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Section height
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        [System.ComponentModel.Browsable(false)]
        public virtual float T2
        {
            get { return Model.Instance.UnitSystem.FromInternational(t2, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != T2)
                {
                    Model.Instance.Undo.Change(this, T2, GetType().GetProperty("T2"));
                    t2 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                    initContourAndLOD();
                }
            }
        }

        /// <summary>
        /// Section modulus about 3 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ShearModulus)]
        public float S33
        {
            get { return Model.Instance.UnitSystem.FromInternational(s33, Canguro.Model.UnitSystem.Units.ShearModulus); }
            set
            {
                if (value != S33)
                {
                    Model.Instance.Undo.Change(this, S33, GetType().GetProperty("S33"));
                    s33 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ShearModulus);
                }
            }
        }

        /// <summary>
        /// Section modulus about 2 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ShearModulus)]
        public float S22
        {
            get { return Model.Instance.UnitSystem.FromInternational(s22, Canguro.Model.UnitSystem.Units.ShearModulus); }
            set
            {
                if (value != S22)
                {
                    Model.Instance.Undo.Change(this, S22, GetType().GetProperty("S22"));
                    s22 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ShearModulus);
                }
            }
        }

        /// <summary>
        /// Radius of gyration about 3 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float R33
        {
            get { return Model.Instance.UnitSystem.FromInternational(r33, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != R33)
                {
                    Model.Instance.Undo.Change(this, R33, GetType().GetProperty("R33"));
                    r33 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Radius of gyration about 2 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float R22
        {
            get { return Model.Instance.UnitSystem.FromInternational(r22, Canguro.Model.UnitSystem.Units.SmallDistance); }
            set
            {
                if (value != R22)
                {
                    Model.Instance.Undo.Change(this, R22, GetType().GetProperty("R22"));
                    r22 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Moment of Inertia about 3 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.AreaInertia)]
        public float I33
        {
            get { return Model.Instance.UnitSystem.FromInternational(i33, Canguro.Model.UnitSystem.Units.AreaInertia); }
            set
            {
                if (value != i33)
                {
                    Model.Instance.Undo.Change(this, I33, GetType().GetProperty("I33"));
                    i33 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.AreaInertia);
                }
            }
        }

        /// <summary>
        /// Moment of Inertia about 2 axis
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.AreaInertia)]
        public float I22
        {
            get { return Model.Instance.UnitSystem.FromInternational(i22, Canguro.Model.UnitSystem.Units.AreaInertia); }
            set
            {
                if (value != I22)
                {
                    Model.Instance.Undo.Change(this, I22, GetType().GetProperty("I22"));
                    i22 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.AreaInertia);
                }
            }
        }

        /// <summary>
        /// Shear area in 3 direcion
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.AreaInertia)]
        public float As3
        {
            get { return Model.Instance.UnitSystem.FromInternational(as3, Canguro.Model.UnitSystem.Units.AreaInertia); }
            set
            {
                if (value != As3)
                {
                    Model.Instance.Undo.Change(this, As3, GetType().GetProperty("As3"));
                    as3 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.AreaInertia);
                }
            }
        }

        /// <summary>
        /// Shear area in 2 direcion
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.AreaInertia)]
        public float As2
        {
            get { return Model.Instance.UnitSystem.FromInternational(as2, Canguro.Model.UnitSystem.Units.AreaInertia); }
            set
            {
                if (value != As2)
                {
                    Model.Instance.Undo.Change(this, As2, GetType().GetProperty("As2"));
                    as2 = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.AreaInertia);
                }
            }
        }

        /// <summary>
        /// Cross Section area
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Area)]
        public float Area
        {
            get { return Model.Instance.UnitSystem.FromInternational(area, Canguro.Model.UnitSystem.Units.Area); }
            set
            {
                if (value != area)
                {
                    Model.Instance.Undo.Change(this, Area, GetType().GetProperty("Area"));
                    area = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Area);
                    initContourAndLOD();
                }
            }
        }
        #endregion

        /// <summary>
        /// Propiedad que expresa todas las propiedades de tipo real (float), en unidades internacionales.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public float[] Properties
        {
            get
            {
                float[] props = new float[18];

                props[0] = t3;
                props[1] = t2;
                props[2] = tf;
                props[3] = tw;
                props[4] = t2b;
                props[5] = tfb;
                props[6] = area;
                props[7] = torsConst;
                props[8] = i33;
                props[9] = i22;
                props[10] = as2;
                props[11] = as3;
                props[12] = s33;
                props[13] = s22;
                props[14] = z33;
                props[15] = z22;
                props[16] = r33;
                props[17] = r22;

                return props;
            }
            set
            {
                Model.Instance.Undo.Change(this, Properties, GetType().GetProperty("Properties"));
                t3 = value[0];
                t2 = value[1];
                tf = value[2];
                tw = value[3];
                t2b = value[4];
                tfb = value[5];
                area = value[6];
                torsConst = value[7];
                i33 = value[8];
                i22 = value[9];
                as2 = value[10];
                as3 = value[11];
                s33 = value[12];
                s22 = value[13];
                z33 = value[14];
                z22 = value[15];
                r33 = value[16];
                r22 = value[17];
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                value = value.Trim().Replace("\"", "''");
                value = (value.Length > 0) ? value : Culture.Get("Section");
                string aux = value;
                int i = 0;
                Catalog<Section> cat = Model.Instance.Sections;
                if (cat != null && !(name.Equals(value) && cat[name] == this))
                {
                    while (cat[aux] != null)
                    {
                        aux = value + "(" + ++i + ")";
                    }
                    Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                    if (cat[name] == this)
                        cat.MoveValue(name, aux);
                    name = aux;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        [System.ComponentModel.Browsable(false)]
        public Material.Material Material
        {
            get
            {
                return material;
            }
            set
            {
                if (value != null)
                {
                    Model.Instance.Undo.Change(this, material, GetType().GetProperty("Material"));
                    material = value;
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public virtual string Shape
        {
            get { return "G"; }
        }

        [System.ComponentModel.Browsable(false)]
        public virtual bool FaceNormals
        {
            get { return true; }
        }

        [System.ComponentModel.Browsable(false)]
        public ConcreteSectionProps ConcreteProperties
        {
            get
            {
                return (concreteProps != null)? (ConcreteSectionProps)concreteProps.Clone() : null;
            }
            set
            {
                Model.Instance.Undo.Change(this, concreteProps, GetType().GetProperty("ConcreteProperties"));
                concreteProps = (value != null)? (ConcreteSectionProps)value.Clone() : null;
            }
        }

        /// <summary>
        /// Propiedad de sólo lectura que regresa el contorno de la sección y la normal en cada punto.
        /// No hace una copia por motivos de eficiencia. Es responsabilidad del usuario no alterar la información.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Microsoft.DirectX.Vector2[][] Contour
        {
            get
            {
                return contour;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public abstract short[][] ContourIndices
        {
            get;
        }

        private Vector2[] boundingBox = new Vector2[2];

        protected void calculateBB()
        {
            boundingBox[0] = boundingBox[1] = Vector2.Empty;

            short[][] contourIndices = ContourIndices;
            if (contourIndices != null)
            {
                short[] indices = contourIndices[0];
                //blCorner = trCorner = contour[0][indices[0]];
                boundingBox[0] = boundingBox[1] = contour[0][indices[0]];

                // Bounding Box
                for (int i = 0; i < indices.Length - 1; i++)
                {
                    boundingBox[0] = Microsoft.DirectX.Vector2.Minimize(contour[0][indices[i]], boundingBox[0]);
                    boundingBox[1] = Microsoft.DirectX.Vector2.Maximize(contour[0][indices[i]], boundingBox[1]);
                }
            }
        }

        protected void calcLODSize()
        {
            // Calculate LOD Size based on contour
            //float sizeX = 0, sizeY = 0;
            //short[] indices = ContourIndices[0];
            //for (int i = 0; i < indices.Length - 1; i++)
            //{
            //    sizeX += Math.Abs(contour[0][indices[i]].X);
            //    sizeY += Math.Abs(contour[0][indices[i]].Y);
            //}

            //lodSize = sizeX * sizeX + sizeY * sizeY;

            // Upcoming code was moved to calculateBB

            //short[][] contourIndices = ContourIndices;
            //if (contourIndices != null)
            //{
            //    short[] indices = contourIndices[0];
            //    //blCorner = trCorner = contour[0][indices[0]];
            //    boundingBox[0] = boundingBox[1] = contour[0][indices[0]];

            //    // Bounding Box
            //    for (int i = 0; i < indices.Length - 1; i++)
            //    {
            //        boundingBox[0] = Microsoft.DirectX.Vector2.Minimize(contour[0][indices[i]], boundingBox[0]);
            //        boundingBox[1] = Microsoft.DirectX.Vector2.Maximize(contour[0][indices[i]], boundingBox[1]);
            //    }

            lodSize = (boundingBox[1] - boundingBox[0]).Length();
            //}
        }

        [System.ComponentModel.Browsable(false)]
        public float LODSize
        {
            get { return lodSize; }
        }

        protected void initContourAndLOD()
        {
            initContour();
            calculateBB();
            calcLODSize();
        }

        /// <summary>
        /// Función virtual que inicializa el contorno y las normales.
        /// </summary>
        virtual protected void initContour()
        {
            contour[0] = new Microsoft.DirectX.Vector2[8];
            contour[1] = new Microsoft.DirectX.Vector2[8];
            int i = 0;
            contour[0][i++] = new Microsoft.DirectX.Vector2(-1, -1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0, -1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(1, -1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(1, 0);
            contour[0][i++] = new Microsoft.DirectX.Vector2(1, 1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0, 1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(-1, 1);
            contour[0][i++] = new Microsoft.DirectX.Vector2(-1, 0);

            i = 0;
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(-1, 0);

            buildHighStressCover();
            buildHighLODCover();
        }

        protected short[] coverHighStress = null;
        protected short[] coverHigh = null;

        [System.ComponentModel.Browsable(false)]
        public short[] CoverHighstress
        {
            get 
            {
                if (coverHighStress == null)
                    buildHighStressCover();
                
                return coverHighStress;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public short[] CoverHigh
        {
            get
            {
                if (coverHigh == null)
                    buildHighLODCover();

                return coverHigh;
            }
        }

        virtual protected void buildHighStressCover()
        {
            coverHighStress = new short[6 * 3];

            // First triangle
            coverHighStress[0] = 0; coverHighStress[1] = 1; coverHighStress[2] = 7;
            // Second triangle
            coverHighStress[3] = 1; coverHighStress[4] = 3; coverHighStress[5] = 7;
            // Third triangle
            coverHighStress[6] = 1; coverHighStress[7] = 2; coverHighStress[8] = 3;
            // Fourth triangle
            coverHighStress[9] = 3; coverHighStress[10] = 4; coverHighStress[11] = 5;
            // Fifth triangle
            coverHighStress[12] = 5; coverHighStress[13] = 7; coverHighStress[14] = 3;
            // Sixth triangle
            coverHighStress[15] = 5; coverHighStress[16] = 6; coverHighStress[17] = 7;
        }


        public void GetOffsetForCardinalPoint(CardinalPoint cardinalPoint, ref Vector2 offset)
        {
            switch (cardinalPoint)
            {
                case CardinalPoint.BottomCenter:
                    offset = new Vector2(boundingBox[0].X + (boundingBox[1].X - boundingBox[0].X) / 2f, boundingBox[0].Y);
                    break;
                case CardinalPoint.BottomLeft:
                    offset = boundingBox[0];
                    break;
                case CardinalPoint.BottomRight:
                    offset = new Vector2(boundingBox[1].X, boundingBox[0].Y);
                    break;
                case CardinalPoint.Centroid:
                    offset = Vector2.Empty;
                    break;
                case CardinalPoint.MiddleCenter:
                    offset = new Vector2(boundingBox[0].X + (boundingBox[1].X - boundingBox[0].X) / 2f, boundingBox[0].Y + (boundingBox[1].Y - boundingBox[0].Y) / 2f);
                    break;
                case CardinalPoint.MiddleLeft:
                    offset = new Vector2(boundingBox[0].X, boundingBox[0].Y + (boundingBox[1].Y - boundingBox[0].Y) / 2f);
                    break;
                case CardinalPoint.MiddleRight:
                    offset = new Vector2(boundingBox[1].X, boundingBox[0].Y + (boundingBox[1].Y - boundingBox[0].Y) / 2f);
                    break;
                case CardinalPoint.ShearCenter:
                    offset = Vector2.Empty;
                    break;
                case CardinalPoint.TopCenter:
                    offset = new Vector2(boundingBox[0].X + (boundingBox[1].X - boundingBox[0].X) / 2f, boundingBox[1].Y);
                    break;
                case CardinalPoint.TopLeft:
                    offset = new Vector2(boundingBox[0].X , boundingBox[1].Y);
                    break;
                case CardinalPoint.TopRight:
                    offset = boundingBox[1];
                    break;
            }
        }

        virtual protected void buildHighLODCover()
        {
            coverHigh = new short[2 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 2;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 2; coverHigh[5] = 3;
        }

        [System.ComponentModel.Browsable(false)]
        public string Description
        {
            get
            {
                string unit = UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.SmallDistance);
                if (T2 > 0)
                    return string.Format("{3}:  {0:#,0.#} {4} X {1:#,0.#} {4} ({2})", T2, T3, material.Name, Name, unit);
                return string.Format("{2}:  {0:#,0.#} {3} ({1})", T3, material.Name, Name, unit);
            }
        }

        /// <summary>
        /// Calculates Area, I33 and I22
        /// </summary>
        protected void CalcProps()
        {
            if (contour != null && contour.Length > 0 && contour[0] != null)
            {
                float r1 = 0;
                float r2 = 0;
                float x1 = 0;
                float y1 = 0;
                float x2 = 0;
                float y2 = 0;
                area = 0;

                int count = contour[0].Length;
                for (int i = 0; i < count; i++)
                {
                    x1 = contour[0][i].X;
                    y1 = contour[0][i].Y;
                    x2 = contour[0][(i + 1) % count].X;
                    y2 = contour[0][(i + 1) % count].Y;
                    // Area
                    float s = (x1 * y2 - x2 * y1) / 2f;
                    area += s;
                    // Centroid
                    float z1 = (x1 + x2) / 3f;
                    float z2 = (y1 + y2) / 3f;
                    // Inertia Moments
                    r1 = r1 + s * (y1 * y1 + y2 * y2 + y1 * y2) / 6f;
                    r2 = r2 + s * (x1 * x1 + x2 * x2 + x1 * x2) / 6f;
                }
                area = Math.Abs(area);
                i22 = Math.Abs(r1);
                i33 = Math.Abs(r2);
            }
        }
    }
}
