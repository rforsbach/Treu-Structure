using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Command to add a Frame based grid
    /// </summary>
    public class AddGridCmd : Canguro.Commands.ModelCommand
    {
        static protected float dx = 5, dy = 5, dz = 3;
        static protected int nx = 3, ny = 3, nz = 2;
        static Canguro.Model.Section.FrameSection section = null;

        /// <summary>
        /// The Frame Section to use in all the Line Elements of the Grid
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Editor(typeof(Canguro.Controller.PropertyGrid.FrameSectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Canguro.Model.Section.FrameSection Section
        {
            get { return AddGridCmd.section; }
            set { AddGridCmd.section = value; }
        }

        /// <summary>
        /// Bay width in the X direction.
        /// </summary>
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Dx
        {
            get { return dx; }
            set { dx = (value <= 0) ? 0.001F : value; }
        }

        /// <summary>
        /// Bay width in the Y direction
        /// </summary>
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Dy
        {
            get { return dy; }
            set { dy = (value <= 0) ? 0.001F : value; }
        }

        /// <summary>
        /// Story height (in Z the axis) [0 - 100m]
        /// </summary>
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Dz
        {
            get { return dz; }
            set { dz = (value <= 0) ? 0.001F : value; }
        }

        /// <summary>
        /// Number of bays in the X axis. [1 - 500]
        /// </summary>
        public int Nx
        {
            get { return nx; }
            set { nx = (value < 1) ? 1 : (value > 500) ? 500 : value; }
        }

        /// <summary>
        /// Number of bays in the Y axis. [1 - 500]
        /// </summary>
        public int Ny
        {
            get { return ny; }
            set { ny = (value < 1) ? 1 : (value > 500) ? 500 : value; }
        }

        /// <summary>
        /// Number of stories (in the Z axis) [1 - 500]
        /// </summary>
        public int Nz
        {
            get { return nz; }
            set { nz = (value < 1) ? 1 : (value > 500) ? 500 : value; }
        }

        /// <summary>
        /// Executes the command. 
        /// Gets the parameters and calls beamGrid3D() to make the grid.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (section == null)
                section = Canguro.Model.Section.SectionManager.Instance.DefaultFrameSection as Canguro.Model.Section.FrameSection;
            services.GetProperties(Culture.Get("gridCmdTitle"), this);

            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("selectGridOrigin"));
            if (m == null) return;
            Microsoft.DirectX.Vector3 o = m.SnapPosition;

            StraightFrameProps props = new StraightFrameProps();
            props.Section = section;
            beamGrid3D(services.Model, o.X, o.Y, o.Z, dx, 0, 0, 0, dy, 0, 0, 0, dz, nx + 1, ny + 1, nz + 1, true, props);
        }

        /// <summary>
        /// Creates a Grid with frames given an origin, 3 vectors and number of bays in the 3 directions.
        /// Joints at the base are not connected and have restricted translation.
        /// </summary>
        /// <param name="model">The Model object to add the Grid to.</param>
        /// <param name="x0">X Component of the Origin point</param>
        /// <param name="y0">Y Component of the Origin point</param>
        /// <param name="z0">Z Component of the Origin point</param>
        /// <param name="ux">X Component of the U directional vector</param>
        /// <param name="uy">Y Component of the U directional vector</param>
        /// <param name="uz">Z Component of the U directional vector</param>
        /// <param name="vx">X Component of the V directional vector</param>
        /// <param name="vy">Y Component of the V directional vector</param>
        /// <param name="vz">Z Component of the V directional vector</param>
        /// <param name="wx">X Component of the W directional vector</param>
        /// <param name="wy">Y Component of the W directional vector</param>
        /// <param name="wz">Z Component of the W directional vector</param>
        /// <param name="nu">Number of bays in the U direction</param>
        /// <param name="nv">Number of bays in the V direction</param>
        /// <param name="nw">Number of bays in the W direction</param>
        /// <param name="doLines">If set to false, only the Joints are created</param>
        /// <param name="props">Frame properties to use in all the Line Elements created</param>
        private static void beamGrid3D(Canguro.Model.Model model, float x0, float y0, float z0, float ux, float uy, float uz,
    float vx, float vy, float vz, float wx, float wy, float wz, int nu, int nv, int nw, bool doLines, StraightFrameProps props)
        {
            Joint joint;
            Joint[] joints = new Joint[nu * nv * nw];
            Stack<Joint> jStack = new Stack<Joint>();
            JointDOF baseDoF = new JointDOF();
            baseDoF.T1 = baseDoF.T2 = baseDoF.T3 = JointDOF.DofType.Restrained;

            for (int i = 0; i < nw; i++)
                for (int j = 0; j < nv; j++)
                    for (int k = 0; k < nu; k++)
                    {
                        model.JointList.Add(joints[i * nu * nv + j * nu + k] =
                            joint = new Joint(x0 + k * ux + j * vx + i * wx,
                                                    y0 + j * uy + j * vy + i * wy,
                                                    z0 + i * uz + i * vz + i * wz));
                        if (i == 0)
                            joints[i * nu * nv + j * nu + k].DoF = baseDoF;
                    }

            if (doLines)
            {
                LineElement beam;
                for (int i = 0; i < nw; i++)
                    for (int j = 0; j < nv; j++)
                        for (int k = 0; k < nu; k++)
                        {
                            jStack.Push(joints[i * nu * nv + j * nu + k]);
                            if (i > 0)
                            {
                                jStack.Push(joints[(i - 1) * nu * nv + j * nu + k]);
                                model.LineList.Add(beam = new LineElement(props));
                                beam.I = jStack.Pop();
                                beam.J = jStack.Peek();
                                if (j > 0)
                                {
                                    jStack.Push(joints[i * nu * nv + (j - 1) * nu + k]);
                                    model.LineList.Add(beam = new LineElement(props));
                                    beam.I = jStack.Pop();
                                    beam.J = jStack.Peek();
                                }
                                if (k > 0)
                                {
                                    jStack.Push(joints[i * nu * nv + j * nu + k - 1]);
                                    model.LineList.Add(beam = new Canguro.Model.LineElement(props));
                                    beam.I = jStack.Pop();
                                    beam.J = jStack.Peek();
                                }
                            }
                            jStack.Pop();
                        }
            }
        }
    }
}
