using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Load;
using Canguro.Properties;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Load renderer. Base class for load rendering. According to the different kinds of loads, this class intends to 
    /// set up nice symbols for displaying them accordingly to the magnitude they represent
    /// </summary>
    public abstract class LoadRenderer : ItemRenderer
    {

        #region JointLoad Renderer related...
        /// <summary>
        /// Render any load over joints
        /// </summary>
        /// <param name="joints"> Collection of joints </param>
        /// <param name="options"> Rendering options </param>
        protected void renderJointLoads(List<Item> itemsInView, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            Vector3 jointPos;
            Joint element;

            // Sweep joint list
            foreach ( Item item in itemsInView)
            {
                if (item is Joint)
                {
                    element = (Joint)item;

                    // Loads are displayed only if joint is selected
                    if (element != null && element.IsSelected == true)
                    {
                        jointPos = new Vector3(element.X, element.Y, element.Z);
                        // Get load list for the current joint
                        AssignedJointLoads jointLoads = (AssignedJointLoads)element.Loads;

                        ItemList<Load> loadList = jointLoads[Canguro.Model.Model.Instance.ActiveLoadCase];

                        if (loadList != null)
                        {
                            foreach (JointLoad load in loadList)
                            {
                                // When force load is detected...
                                if (load is ForceLoad)
                                {
                                    drawForceLoad(jointPos, (ForceLoad)load, options, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
                                }
                                // When ground displacement load is detected...
                                if (load is GroundDisplacementLoad)
                                {
                                    drawGroundDisplacementLoad(jointPos, ((GroundDisplacementLoad)load).Displacements, options, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int drawForceLoad(Vector3 jointPos, ForceLoad fl, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            string unit, jointLabel;

            float[] flInt = fl.Force;
            float[] forceValues = new float[4];
            float forceValue = 0.0f;

            bool isForceLoad = true;

            // Compute a vector representing the force and scale it for having a best display
            Vector3 force = new Vector3(flInt[0], flInt[1], flInt[2]);
            float scale = (float)Math.Sqrt(force.Length());
            force.Normalize();
            force.Scale(scale);

            // Do the same with moments
            Vector3 momentX = new Vector3(1.0f, 0.0f, 0.0f);
            momentX.Scale((float)Math.Sqrt(Math.Abs(flInt[3])) * Math.Sign(flInt[3]));

            Vector3 momentY = new Vector3(0.0f, 1.0f, 0.0f);
            momentY.Scale((float)Math.Sqrt(Math.Abs(flInt[4])) * Math.Sign(flInt[4]));

            Vector3 momentZ = new Vector3(0.0f, 0.0f, 1.0f);
            momentZ.Scale((float)Math.Sqrt(Math.Abs(flInt[5])) * Math.Sign(flInt[5]));

            #region Load Rendering...
            // Check if there are enough space for feeding vertices and, if not, release VB and recapture it
            recaptureVBIfNotEnoughSpace(force, momentX, momentY, momentZ, ref triangPack, ref numTriangVerticesInVB, ref linePack, ref numLineVerticesInVB);
            
            // Draw loads and moments
            if (force != Vector3.Empty)
                drawSingleArrow(force, jointPos, isForceLoad, false, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), ref triangPack, ref linePack);

            if (momentX != Vector3.Empty)
                drawSingleArrow(momentX, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);
            
            if (momentY != Vector3.Empty)
                drawSingleArrow(momentY, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);
            
            if (momentZ != Vector3.Empty)
                drawSingleArrow(momentZ, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);
            #endregion

            #region Load text rendering
            // When requested load labels, do other kind of rendering
            if ((options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
            { 
                // Enable unit conversion system
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                #region Force Load texts
                // Get values for string display
                forceValues[0] = flInt[0];
                forceValues[1] = flInt[1];
                forceValues[2] = flInt[2];
                forceValue = (float)Math.Sqrt(flInt[0] * flInt[0] + flInt[1] * flInt[1] + flInt[2] * flInt[2]);

                if (forceValue != 0.0f)
                {
                    // Convert values to the current system
                    forceValues[0] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[0], Canguro.Model.UnitSystem.Units.Force);
                    forceValues[1] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[1], Canguro.Model.UnitSystem.Units.Force);
                    forceValues[2] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[2], Canguro.Model.UnitSystem.Units.Force);

                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Force);

                    // Build label
                    jointLabel = "(" + string.Format("{0:F3}", forceValues[0]) + ", " +
                                       string.Format("{0:F3}", forceValues[1]) + ", " +
                                       string.Format("{0:F3}", forceValues[2]) + ")" + " [" + unit + "]";

                    // Display label
                    DrawItemText(jointLabel, jointPos + (jointPos - force) * Canguro.Properties.Settings.Default.ForceLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }
                #endregion

                #region Moment texts

                unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in X axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(flInt[3]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos + (jointPos - momentX) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in Y axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(flInt[4]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos + (jointPos - momentY) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in Z axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(flInt[5]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos + (jointPos - momentZ) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }

                #endregion

                // Disable unit conversion system
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

            }
            #endregion

            return 0;
        }

        private int drawGroundDisplacementLoad(Vector3 jointPos, float[] displacements, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            bool isForceLoad = false;
            string unit, jointLabel;
            float[] forceValues = new float[4];
            float forceValue;

            // Compute vector from load magnitudes for displaying
            Vector3 force = new Vector3(-displacements[0], -displacements[1], -displacements[2]);
            float scale = (float)Math.Sqrt(force.Length());
            force.Normalize();
            force.Scale(scale);

            float sFac = 50.0f;

            // Do the same with moments
            Vector3 momentX = new Vector3(1.0f, 0.0f, 0.0f);
            momentX.Scale((float)Math.Sqrt(Math.Abs(displacements[3])) * Math.Sign(-displacements[3]) * sFac);

            Vector3 momentY = new Vector3(0.0f, 1.0f, 0.0f);
            momentY.Scale((float)Math.Sqrt(Math.Abs(displacements[4])) * Math.Sign(-displacements[4]) * sFac);

            Vector3 momentZ = new Vector3(0.0f, 0.0f, 1.0f);
            momentZ.Scale((float)Math.Sqrt(Math.Abs(displacements[5])) * Math.Sign(-displacements[5]) * sFac);

            #region Load Rendering...
            // Check if there are enough space for feeding vertices and, if not, release VB and recapture it
            recaptureVBIfNotEnoughSpace(force, momentX, momentY, momentZ, ref triangPack, ref numTriangVerticesInVB, ref linePack, ref numLineVerticesInVB);

            // Draw joint load
            if (force != Vector3.Empty)
                drawSingleArrow(force, jointPos, isForceLoad, false, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), ref triangPack, ref linePack);

            if (momentX != Vector3.Empty)
                drawSingleArrow(momentX, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);

            if (momentY != Vector3.Empty)
                drawSingleArrow(momentY, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);

            if (momentZ != Vector3.Empty)
                drawSingleArrow(momentZ, jointPos, isForceLoad, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);
            #endregion

            #region Load text rendering
            // When labels wanted, rendering is special
            if ((options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
            {
                // Enable unit conversion system
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                #region Force Load texts
                // Get force values to build label
                forceValues[0] = displacements[0];
                forceValues[1] = displacements[1];
                forceValues[2] = displacements[2];
                forceValue = (float)Math.Sqrt(displacements[0] * displacements[0] + displacements[1] * displacements[1] + displacements[2] * displacements[2]);

                if (forceValue != 0.0f)
                {
                    // Convert values to the current system
                    forceValues[0] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[0], Canguro.Model.UnitSystem.Units.Distance);
                    forceValues[1] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[1], Canguro.Model.UnitSystem.Units.Distance);
                    forceValues[2] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[2], Canguro.Model.UnitSystem.Units.Distance);

                    // Get current unit
                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

                    // Build information label
                    jointLabel = "(" + string.Format("{0:F3}", forceValues[0]) + ", " +
                                       string.Format("{0:F3}", forceValues[1]) + ", " +
                                       string.Format("{0:F3}", forceValues[2]) + ")" + " [" + unit + "]";

                    // Draw label
                    DrawItemText(jointLabel, jointPos - force, Canguro.Properties.Settings.Default.TextColor);
                }
                #endregion

                #region Moment texts

                unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Angle);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in X axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(displacements[3]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos - momentX * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in Y axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(displacements[4]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos - momentY * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Moment in Z axis
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                forceValue = (float)Math.Abs(displacements[5]);
                if (forceValue != 0.0f)
                {
                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                    DrawItemText(jointLabel, jointPos - momentZ * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                }

                #endregion

                // Disable unit conversion system
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
            }
            #endregion

            return 0;
        }
        #endregion

        #region LineLoad Renderer...

        /// <summary>
        /// Render any load over lines
        /// </summary>
        /// <param name="lines"> Line list </param>
        /// <param name="options"> Rendering options </param>
        protected void renderLineLoads(List<Item> itemsInView, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            LineElement element;

            // Sweep line list
            foreach ( Item item in itemsInView)
            {
                if (item is LineElement)
                {
                    element = (LineElement)item;

                    if (element != null && element.IsSelected == true)
                    {
                        // Get assigned loads fot the current selected line
                        AssignedLineLoads lineLoads = (AssignedLineLoads)element.Loads;

                        ItemList<Load> loadList = lineLoads[Canguro.Model.Model.Instance.ActiveLoadCase];

                        if (loadList != null)
                        {
                            string tempLoads = string.Empty;

                            foreach (LineLoad load in loadList)
                            {
                                // When load is concentrated...
                                if (load is ConcentratedSpanLoad)
                                {
                                    ConcentratedSpanLoad csl = (ConcentratedSpanLoad)load;

                                    drawConcentratedSpanLoad(csl, element, options, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
                                }
                                if (load is DistributedSpanLoad)
                                {
                                    // Distributed Loads
                                    DistributedSpanLoad dsl = (DistributedSpanLoad)load;

                                    drawDistributedSpanLoad(dsl, element, options, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
                                }
                                if (load is TemperatureLineLoad)
                                {
                                    // When text rendering is enabled, enable unit convesion system
                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                                    // Get magnitude and current unit
                                    TemperatureLineLoad tl = (TemperatureLineLoad)load;
                                    float temp = tl.Temperature;
                                    string unit;

                                    if (tl is TemperatureGradientLineLoad)
                                    {
                                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.TemperatureGradient);
                                        tempLoads += string.Format("{0:G3} [{1}]\n", temp, unit);
                                    }
                                    else
                                    {
                                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Temperature);
                                        tempLoads += string.Format("{0:G3} [{1}]\n", temp, unit);
                                    }

                                    // Disable text rendering
                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                                }
                            }

                            // Draw text
                            if (tempLoads != string.Empty)
                                DrawItemText(tempLoads, element.I.Position + 0.5f * (element.J.Position - element.I.Position), Canguro.Properties.Settings.Default.TextColor);
                        }
                    }
                }
            }
        }

        private int drawConcentratedSpanLoad(ConcentratedSpanLoad csl, LineElement element, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            float forceValue;
            string unit;

            int requiredLineVertices = 0;
            int requiredTriangVertices = 0;

            Vector3 labelPos;

            // Get load as vector
            Vector3 concentratedLoadPos = element.I.Position + csl.D * (element.J.Position - element.I.Position);

            // Compute the vector for display
            Vector3 force = (float)Math.Sqrt(Math.Abs(csl.LoadInt)) * Math.Sign(csl.LoadInt) * getLoadDirection(csl.Direction, element);

            if (force != Vector3.Empty)
            {
                #region Load Rendering

                #region Check if VB can manage the next buffers to feed
                if (csl.Type == LineLoad.LoadType.Force)
                {
                    requiredLineVertices = 2;
                    requiredTriangVertices = 6;
                }
                else
                {
                    requiredLineVertices = 4;
                    requiredTriangVertices = 12;
                }

                checkIfEnoughSpaceInVB(requiredLineVertices, ref numLineVerticesInVB, ref linePack);
                checkIfEnoughSpaceInVB(requiredTriangVertices, ref numTriangVerticesInVB, ref triangPack);
                #endregion

                if (csl.Type == LineLoad.LoadType.Force)
                {
                    // Draw Force
                    drawSingleArrow(force, concentratedLoadPos, true, false, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), ref triangPack, ref linePack);

                    #region Load text drawing
                    if ((options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
                    {
                        // When text rendering is enabled, enable unit convesion system
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                        // Get magnitude and current unit
                        forceValue = Math.Abs(csl.L);
                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Load0D);

                        // Draw text
                        labelPos = concentratedLoadPos + (concentratedLoadPos - force) * Canguro.Properties.Settings.Default.ForceLoadLineScale;
                        DrawItemText(string.Format("{0:F3} {1}", forceValue, "[" + unit + "]"), labelPos, Canguro.Properties.Settings.Default.TextColor);

                        // Disable text rendering
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                    }
                    #endregion
                }
                else
                {
                    // Draw moment
                    drawSingleArrow(force, concentratedLoadPos, true, true, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), ref triangPack, ref linePack);

                    #region Load text drawing
                    if ((options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
                    {
                        // Enable unit system conversion
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                        // Get magnitude and current unit
                        forceValue = Math.Abs(csl.L);
                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

                        // Draw texts
                        labelPos = concentratedLoadPos + (concentratedLoadPos - force) * Canguro.Properties.Settings.Default.MomentLoadLineScale;
                        DrawItemText(string.Format("{0:F3} {1}", forceValue, "[" + unit + "]"), concentratedLoadPos - force, Canguro.Properties.Settings.Default.TextColor);

                        // Disable unit system conversion
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                    }
                    #endregion
                }
                #endregion
            }

            return 0;
        }

        private int drawDistributedSpanLoad(DistributedSpanLoad dsl, LineElement element, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            string unit;
            Vector3 loadDir = element.J.Position - element.I.Position;
            Vector3 beamDir = element.J.Position - element.I.Position;
            Vector3 labelPos;

            // Do not do anything when loads are zero
            if (dsl.LoadAInt != 0.0f || dsl.LoadBInt != 0.0f)
            {
                // Configure some control points with distances and loads at each endpoint
                float[,] ctrlPts = new float[,] { 
                                        { dsl.Da, (float)Math.Sqrt(0.5f*Math.Abs(dsl.LoadAInt))*Math.Sign(dsl.LoadAInt) * Settings.Default.ForceLoadLineScale },
                                        { dsl.Db, (float)Math.Sqrt(0.5f*Math.Abs(dsl.LoadBInt))*Math.Sign(dsl.LoadBInt) * Settings.Default.ForceLoadLineScale }
                };

                #region Label rendering
                // When label magnitude displaying is enabled, draw them
                if ((options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
                {
                    // Enable unit system conversion
                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                    // Check which type of load are we rendering for having the correct current unit
                    if (dsl.Type == LineLoad.LoadType.Force)
                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Load1D);
                    else
                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

                    // Get load values at initial point (A) and configures label
                    loadDir = (float)Math.Sqrt(0.5f * Math.Abs(dsl.LoadAInt)) * Math.Sign(dsl.LoadAInt) * getLoadDirection(dsl.Direction, element);
                    // Get Position Check
                    labelPos = element.I.Position + dsl.Da * beamDir - loadDir * Canguro.Properties.Settings.Default.ForceLoadLineScale;
                    // doJointLoad(loadDir, element.I.Position + dsl.Da * (element.J.Position - element.I.Position), true);
                    // Draw text in the correct position
                    DrawItemText(dsl.La.ToString("f3") + " [" + unit + "]", labelPos, Canguro.Properties.Settings.Default.TextColor);

                    // Get load values at ending point (B) and configures label
                    loadDir = (float)Math.Sqrt(0.5f * Math.Abs(dsl.LoadBInt)) * Math.Sign(dsl.LoadBInt) * getLoadDirection(dsl.Direction, element);
                    // Get position CHECK
                    labelPos = element.I.Position + dsl.Db * beamDir - loadDir * Canguro.Properties.Settings.Default.ForceLoadLineScale;
                    //doJointLoad(loadDir, element.I.Position + dsl.Db * (element.J.Position - element.I.Position), true);
                    // Draw text in the correct position
                    DrawItemText(dsl.Lb.ToString("f3") + " [" + unit + "]", labelPos, Canguro.Properties.Settings.Default.TextColor);

                    // Disable unit system conversion
                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                }
                #endregion

                // Get color and set number of arrows according to kind of load
                int color = (dsl.Type == LineLoad.LoadType.Force) ? Settings.Default.ForceLoadForceDefaultColor.ToArgb() : Settings.Default.ForceLoadMomentDefaultColor.ToArgb();
                int numArrows = (dsl.Type == LineLoad.LoadType.Moment) ? 2 : 1;

                // Draw load
                drawPolygonalLoad(ctrlPts, element, getLoadDirection(dsl.Direction, element), color, numArrows, ref triangPack, ref linePack, ref numTriangVerticesInVB, ref numLineVerticesInVB);
            }
            return 0;
        }
        #endregion

        #region AreaLoad Renderer related...

        /// <summary>
        /// Renders any load over an area
        /// </summary>
        /// <param name="areas"> Area list </param>
        /// <param name="options"> Rendering options </param>
        protected void renderAreaLoads(List<Item> itemsInView, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
        }

        #endregion

        #region Reactions renderer...
        public void RenderReactions(Device device, Model.Model model, RenderOptions options, List<Item> itemsInView)
        {
            if (itemsInView == null) return;

            if (itemsInView.Count <= 0)
                GetItemsInView(itemsInView);

            if (itemsInView.Count > 0)
            {
                // Get resource cache instance
                ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
                int numTriangVerticesDrawn = 0;
                int numLineVerticesDrawn = 0;

                // Turn off lighting for color rendering
                device.RenderState.Lighting = false;
                device.RenderState.CullMode = Cull.None;

                // Vertex Buffer capture depends on current "renderer" (joints, lines, areas) and must be updated by them according to the feeded vertices
                // For simplicity, any "renderer" takes as parameter the last captured VertexBuffer and the number of vertices drawn
                PositionColoredPackage linePack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
                PositionColoredPackage triangPack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, false, true);

                // First, render loads over joints
                drawReactions(model, model.JointList, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);

                // Flush remaining vertices
                rc.ReleaseBuffer(numLineVerticesDrawn, 0, ResourceStreamType.Lines);
                rc.Flush(ResourceStreamType.Lines);
                rc.ReleaseBuffer(numTriangVerticesDrawn, 0, ResourceStreamType.TriangleListPositionColored);
                rc.Flush(ResourceStreamType.TriangleListPositionColored);

                //Turn on lighting
                device.RenderState.Lighting = true;
            }
        }

        private void drawReactions(Model.Model model, IEnumerable<Joint> joints, RenderOptions options, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            Vector3 jointPos;

            if (!model.HasResults || model.Results.JointReactions == null) return;

            // Sweep joint list
            foreach (Joint element in joints)
            {
                // Loads are displayed only if joint is selected
                if (element != null)
                {
                    // Get joint reactions
                    Vector3 force = new Vector3(model.Results.JointReactions[element.Id, 0],
                                                model.Results.JointReactions[element.Id, 1],
                                                model.Results.JointReactions[element.Id, 2]);

                    float[] moments = new float[]{model.Results.JointReactions[element.Id, 3],
                                                  model.Results.JointReactions[element.Id, 4],
                                                  model.Results.JointReactions[element.Id, 5]};

                    jointPos = new Vector3(element.X, element.Y, element.Z);


                    string unit, jointLabel;

                    float[] flInt = new float[] { force.X, force.Y, force.Z };
                    float[] forceValues = new float[4];
                    float forceValue = 0.0f;

                    bool isForceLoad = true;

                    // Compute a vector representing the force and scale it for having a best display
                    float scale = (float)Math.Sqrt(force.Length());
                    force.Normalize();
                    force.Scale(scale);

                    // Do the same with moments
                    Vector3 momentX = new Vector3(1.0f, 0.0f, 0.0f);
                    momentX.Scale((float)Math.Sqrt(Math.Abs(moments[0])) * Math.Sign(moments[0]));

                    Vector3 momentY = new Vector3(0.0f, 1.0f, 0.0f);
                    momentY.Scale((float)Math.Sqrt(Math.Abs(moments[1])) * Math.Sign(moments[1]));

                    Vector3 momentZ = new Vector3(0.0f, 0.0f, 1.0f);
                    momentZ.Scale((float)Math.Sqrt(Math.Abs(moments[2])) * Math.Sign(moments[2]));

                    #region Load Rendering...
                    // Check if there are enough space for feeding vertices and, if not, release VB and recapture it
                    recaptureVBIfNotEnoughSpace(force, momentX, momentY, momentZ, ref triangPack, ref numTriangVerticesInVB, ref linePack, ref numLineVerticesInVB);

                    // Draw loads and moments
                    if (force != Vector3.Empty)
                        drawSingleArrow(force, jointPos, isForceLoad, false, Canguro.Properties.Settings.Default.ForceReactionColor.ToArgb(), ref triangPack, ref linePack);

                    if (momentX != Vector3.Empty)
                        drawSingleArrow(momentX, jointPos, isForceLoad, true, Canguro.Properties.Settings.Default.MomentReactionColor.ToArgb(), ref triangPack, ref linePack);

                    if (momentY != Vector3.Empty)
                        drawSingleArrow(momentY, jointPos, isForceLoad, true, Canguro.Properties.Settings.Default.MomentReactionColor.ToArgb(), ref triangPack, ref linePack);

                    if (momentZ != Vector3.Empty)
                        drawSingleArrow(momentZ, jointPos, isForceLoad, true, Canguro.Properties.Settings.Default.MomentReactionColor.ToArgb(), ref triangPack, ref linePack);
                    #endregion

                    #region Load text rendering
                    // When requested load labels, do other kind of rendering
                    if ((options.OptionsShown & RenderOptions.ShowOptions.ReactionLoads) != 0)
                    {
                        // Enable unit conversion system
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                        #region Force Load texts
                        // Get values for string display
                        forceValues[0] = flInt[0];
                        forceValues[1] = flInt[1];
                        forceValues[2] = flInt[2];
                        forceValue = (float)Math.Sqrt(flInt[0] * flInt[0] + flInt[1] * flInt[1] + flInt[2] * flInt[2]);

                        if (forceValue != 0.0f)
                        {
                            // Convert values to the current system
                            forceValues[0] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[0], Canguro.Model.UnitSystem.Units.Force);
                            forceValues[1] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[1], Canguro.Model.UnitSystem.Units.Force);
                            forceValues[2] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[2], Canguro.Model.UnitSystem.Units.Force);

                            unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Force);

                            // Build label
                            jointLabel = "(" + string.Format("{0:F3}", forceValues[0]) + ", " +
                                               string.Format("{0:F3}", forceValues[1]) + ", " +
                                               string.Format("{0:F3}", forceValues[2]) + ")" + " [" + unit + "]";

                            // Display label
                            DrawItemText(jointLabel, jointPos + (jointPos - force) * Canguro.Properties.Settings.Default.ForceLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                        }
                        #endregion

                        #region Moment texts

                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// Moment in X axis
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        forceValue = (float)Math.Abs(moments[0]);
                        if (forceValue != 0.0f)
                        {
                            forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                            jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                            DrawItemText(jointLabel, jointPos + (jointPos - momentX) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// Moment in Y axis
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        forceValue = (float)Math.Abs(moments[1]);
                        if (forceValue != 0.0f)
                        {
                            forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                            jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                            DrawItemText(jointLabel, jointPos + (jointPos - momentY) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// Moment in Z axis
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        forceValue = (float)Math.Abs(moments[2]);
                        if (forceValue != 0.0f)
                        {
                            forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
                            jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
                            DrawItemText(jointLabel, jointPos + (jointPos - momentZ) * Canguro.Properties.Settings.Default.MomentLoadLineScale, Canguro.Properties.Settings.Default.TextColor);
                        }

                        #endregion

                        // Disable unit conversion system
                        Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                    }
                    #endregion

                }
            }
        }
        #endregion

        #region General rendering methods
        private int drawSingleArrow(Vector3 load, Vector3 position, bool isForceLoad, bool isMoment, int color, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack)
        {
            int usedVertices = 0;

            // Get endpoints
            Vector3 origin = position - load;
            Vector3 end = position;
            Vector3 dir;

            float lineScale, arrowScale;

            #region Set drawing parameters whether or not a force or a moment is being drawn
            if (isMoment)
            {
                lineScale = Settings.Default.MomentLoadLineScale;
                arrowScale = Settings.Default.MomentLoadArrowScale;
            }
            else
            {
                lineScale = Settings.Default.ForceLoadLineScale;
                arrowScale = Settings.Default.ForceLoadArrowScale;
            }
            #endregion


            // When a ground displacement load is active, directions are inverted
            if (!isForceLoad)
            {
                // Swap endpoints
                end = origin;
                origin = position;
                dir = (end - origin);
                Vector3 v4 = dir; // *Settings.Default.ForceLoadLineScale;

                if (isMoment)
                    v4 *= Settings.Default.ForceLoadLineScale;
                else
                    lineScale = 1.0f;

                end = origin + v4;
                origin = origin - dir + v4;
            }

            usedVertices = makeSingleArrow(origin, end, arrowScale, lineScale, color, ref triangPack, ref linePack);

            #region If drawing a moment, a second arrow is required
            if (isMoment)
            {
                //Draw second arrow
                dir = end - origin;
                dir.Normalize();
                end = end - 0.05f * dir;
                origin = origin + 0.1f * (end - origin);
                usedVertices += makeSingleArrow(origin, end, Settings.Default.MomentLoadArrowScale, Settings.Default.MomentLoadLineScale, color, ref triangPack, ref linePack);
            }
            #endregion

            return usedVertices;
        }

        /// <summary>
        /// Draws a distributed load from a set of control points and the current line
        /// </summary>
        /// <param name="ctrlPts"> Defines some control points for building load symbol </param>
        /// <param name="line"> The current analyzed line </param>
        /// <param name="direction"> Load direction </param>
        /// <param name="color"> Drawn color </param>
        /// <param name="numArrows"> How many arrows are needed, can be 1 or 2 </param>
        /// <param name="triangPack"> Render package for feeding triangle vertices </param>
        /// <param name="linePack"> Render packager for feeding line vertices </param>
        /// <param name="numTriangVerticesInVB"> How many triangle vertices are there in triangles VB? </param>
        /// <param name="numLineVerticesInVB"> How many line vertices are there in lines VB? </param>
        protected void drawPolygonalLoad(float[,] ctrlPts, LineElement line, Vector3 direction, int color, int numArrows, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack, ref int numTriangVerticesInVB, ref int numLineVerticesInVB)
        {
            Vector3[] singleArrow = new Vector3[3];
            float length, beamLength;
            Vector3 posI, posJ;
            Vector3 pTemp, pTempU;
            Vector3 dir;

            int numPts = ctrlPts.GetLength(0);

            if (numPts < 2)
                return;

            length = line.LengthInt;
            beamLength = length;

            Joint j1 = line.I;
            Joint j2 = line.J;

            // Lets consider a global system...
            posI = j1.Position;
            posJ = j2.Position;

            pTemp = posJ - posI;

            // Lets consider a global system...
            pTempU = Vector3.Normalize(pTemp);

            // Now, we treat direction as a unit vector in opposite direction...
            dir = -1.0f * Vector3.Normalize(direction);

            // We know the control points and force direction, so lets draw the contour...
            Vector3 segmenti, dirTemp, last;

            segmenti = ctrlPts[0, 0] * pTemp;
            segmenti += posI;
            dirTemp = ctrlPts[0, 1] * dir;
            segmenti += dirTemp;

            last = segmenti;

            // Line feeder

            // Check if VB have enough space to render next vertices
            int requiredLineVertices = 0;

            requiredLineVertices = (numPts - 1) * 2;

            checkIfEnoughSpaceInVB(requiredLineVertices, ref numLineVerticesInVB, ref linePack);

            unsafe
            {
                for (int i = 1; i < numPts; ++i)
                {
                    segmenti = ctrlPts[i, 0] * pTemp;
                    segmenti += posI;
                    dirTemp = ctrlPts[i, 1] * dir;
                    segmenti += dirTemp;

                    // First vertex in line
                    linePack.VBPointer->Position = last;
                    linePack.VBPointer->Color = color;
                    linePack.VBPointer++;

                    // Second vertex in line
                    linePack.VBPointer->Position = segmenti;
                    linePack.VBPointer->Color = color;
                    linePack.VBPointer++;

                    last = segmenti;
                }
            }

            // Draw load arrows...
            float deltaL = 0.3375f;
            float deltaT = Settings.Default.MomentLoadArrowScale;
            length = Math.Abs(length * (ctrlPts[numPts - 1, 0] - ctrlPts[0, 0]));

            // How many lines are we drawing?
            int nLines = (int)(length / deltaL + 0.5f);

            if (nLines != 0)
                deltaL = length / nLines;

            float pLine = 0.0f;
            float deltaXP, slope;
            Vector3 upTemp2, pTemp2, pTemp3;
            int ctrlPtsIndex = 0;

            // Get the slope for the first rect segment...
            slope = (nLines != 0) ? (ctrlPts[ctrlPtsIndex + 1, 1] - ctrlPts[ctrlPtsIndex, 1]) /
                                    (ctrlPts[ctrlPtsIndex + 1, 0] - ctrlPts[ctrlPtsIndex, 0])
                                    : 0.0f;

            pTempU *= deltaT;
            deltaT *= 2;

            // Check if there are enough space for adding vertices
            requiredLineVertices = (nLines + 1) * 2;
            int requiredTriangVertices = (nLines + 1) * numArrows * 3;

            checkIfEnoughSpaceInVB(requiredLineVertices, ref numLineVerticesInVB, ref linePack);
            checkIfEnoughSpaceInVB(requiredTriangVertices, ref numTriangVerticesInVB, ref triangPack);

            unsafe
            {
                for (int i = 0; i <= nLines; i++)
                {
                    // Get the position (0 -1) in the bar
                    pLine = (nLines != 0) ? ((float)i) / nLines * length / beamLength + ctrlPts[0, 0] : ctrlPts[0, 0];

                    // Find segment...
                    while (ctrlPtsIndex < (numPts - 1) && ctrlPts[ctrlPtsIndex + 1, 0] < pLine)
                    {
                        slope = (ctrlPts[ctrlPtsIndex + 1, 1] - ctrlPts[ctrlPtsIndex, 1]) /
                                (ctrlPts[ctrlPtsIndex + 1, 0] - ctrlPts[ctrlPtsIndex, 0]);
                        ++ctrlPtsIndex;
                    }

                    // Get intersection points...
                    deltaXP = (pLine - ctrlPts[ctrlPtsIndex, 0]) * slope + ctrlPts[ctrlPtsIndex, 1];

                    pTemp2 = pLine * pTemp + posI;

                    dirTemp = deltaXP * dir;

                    upTemp2 = pTemp2 + dirTemp;

                    // Draw arrow line...
                    linePack.VBPointer->Position = upTemp2;
                    linePack.VBPointer->Color = color;
                    linePack.VBPointer++;

                    linePack.VBPointer->Position = pTemp2;
                    linePack.VBPointer->Color = color;
                    linePack.VBPointer++;

                    // Check if arrow fits in the area
                    Vector3 ptlen;
                    float tempLen;

                    ptlen = upTemp2 - pTemp2;
                    tempLen = ptlen.Length();

                    Vector3 newdir = Vector3.Normalize(upTemp2 - pTemp2);

                    if (numArrows > 0)// && tempLen > deltaT)
                    {
                        // Get triangle points...
                        pTemp3 = deltaT * newdir + pTemp2;

                        singleArrow[0] = pTemp2;
                        singleArrow[1] = pTemp3 - pTempU;
                        singleArrow[2] = pTemp3 + pTempU;

                        // Feed arrow
                        triangPack.VBPointer->Position = singleArrow[0]; triangPack.VBPointer->Color = color;
                        triangPack.VBPointer++;

                        triangPack.VBPointer->Position = singleArrow[1]; triangPack.VBPointer->Color = color;
                        triangPack.VBPointer++;

                        triangPack.VBPointer->Position = singleArrow[2]; triangPack.VBPointer->Color = color;
                        triangPack.VBPointer++;

                        for (int arrow = 1; arrow < numArrows; arrow++)
                        {
                            singleArrow[0] = singleArrow[0] + 0.8f * deltaT * newdir;
                            singleArrow[1] = singleArrow[1] + 0.8f * deltaT * newdir;
                            singleArrow[2] = singleArrow[2] + 0.8f * deltaT * newdir;

                            // Feed arrow
                            triangPack.VBPointer->Position = singleArrow[0]; triangPack.VBPointer->Color = color;
                            triangPack.VBPointer++;

                            triangPack.VBPointer->Position = singleArrow[1]; triangPack.VBPointer->Color = color;
                            triangPack.VBPointer++;

                            triangPack.VBPointer->Position = singleArrow[2]; triangPack.VBPointer->Color = color;
                            triangPack.VBPointer++;
                        }
                    }
                }
            }
        }

        /// <summary> Gets a vector indicating the load direction from the property in the current line element </summary>
        /// <param name="loadDirection"> Current load direction</param>
        /// <param name="beam"> The line element </param>
        /// <returns> Line direction in a Vector3 structure </returns>
        protected Vector3 getLoadDirection(LineLoad.LoadDirection loadDirection, LineElement beam)
        {
            Vector3 loadDir = Vector3.Empty;

            // Check property and get direction
            switch (loadDirection)
            {
                case LineLoad.LoadDirection.Gravity:
                    loadDir = new Vector3(0.0f, 0.0f, -1.0f);
                    break;
                case LineLoad.LoadDirection.GlobalZ:
                    loadDir = new Vector3(0.0f, 0.0f, 1.0f);
                    break;
                case LineLoad.LoadDirection.GlobalX:
                    loadDir = new Vector3(1.0f, 0.0f, 0.0f);
                    break;
                case LineLoad.LoadDirection.GlobalY:
                    loadDir = new Vector3(0.0f, 1.0f, 0.0f);
                    break;
                case LineLoad.LoadDirection.Local1:
                    loadDir = beam.LocalAxes[0];
                    break;
                case LineLoad.LoadDirection.Local2:
                    loadDir = beam.LocalAxes[1];
                    break;
                case LineLoad.LoadDirection.Local3:
                    loadDir = beam.LocalAxes[2];
                    break;
            }

            return loadDir;
        }

        private int makeSingleArrow(Vector3 origin, Vector3 end, float arrowScale, float lineScale, int color, ref PositionColoredPackage triangPack, ref PositionColoredPackage linePack)
        {
            Vector3 v1, v2, v3, v4;
            float deltaLine, deltaTriangle;

            const int usedVertices = 7;

            // Here we compute the directional vector between origin and end, starting at end
            Vector3 dir = end - origin;
            // Invert the direction
            deltaLine = -lineScale;
            // Get the scaling factor for arrows
            deltaTriangle = arrowScale;

            // Here we have a scaled line pointing from origin to end at end position
            v4 = dir * deltaLine;
            v4 += end;

            // Here we have the direction of the line as unit vector
            v2 = dir;
            v2.Normalize();

            // Compute a vector that forms a 45[deg] angle with line dir and normalize it
            if (dir.Z != 0)
            {
                v3.X = 1.0f; v3.Y = 1.0f;
                v3.Z = -(v2.X + v2.Y) / v2.Z;
            }
            else if (dir.Y != 0)
            {
                v3.X = 1.0f; v3.Z = 1.0f;
                v3.Y = -v2.X / v2.Y;
            }
            else
            {
                v3.X = 0.0f; v3.Y = 0.0f;
                v3.Z = 1.0f;
            }

            v3.Normalize();

            // Get a normal to v3 that also forms a 45 [deg] angle with v2
            v1 = Vector3.Cross(v2, v3);

            // Scale arrow tip and place at end
            v2 = v2 * (-2.0f * deltaTriangle);
            v2 += end;

            // Scale the other two vertices for building the triangle
            v3 = v3 * deltaTriangle;
            v1 = v1 * deltaTriangle;

            // Transform vertex array
            unsafe
            {
                // First triangle
                triangPack.VBPointer->Position = end; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                triangPack.VBPointer->Position = v2 - v1; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                triangPack.VBPointer->Position = v2 + v1; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                // Second triangle
                triangPack.VBPointer->Position = end; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                triangPack.VBPointer->Position = v2 - v3; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                triangPack.VBPointer->Position = v2 + v3; triangPack.VBPointer->Color = color;
                triangPack.VBPointer++;

                // Line
                linePack.VBPointer->Position = end; linePack.VBPointer->Color = color;
                linePack.VBPointer++;

                linePack.VBPointer->Position = v4; linePack.VBPointer->Color = color;
                linePack.VBPointer++;
            }

            return usedVertices;
        }

        private void recaptureVBIfNotEnoughSpace(Vector3 force, Vector3 momentX, Vector3 momentY, Vector3 momentZ, ref PositionColoredPackage triangPack, ref int numTriangVerticesInVB, ref PositionColoredPackage linePack, ref int numLineVerticesInVB)
        {
            int requiredLineVertices = 0;
            int requiredTriangVertices = 0;

            if (force != Vector3.Empty)
            {
                requiredLineVertices += 2;
                requiredTriangVertices += 6;
            }
            if (momentX != Vector3.Empty)
            {
                requiredLineVertices += 4;
                requiredTriangVertices += 12;
            }
            if (momentY != Vector3.Empty)
            {
                requiredLineVertices += 4;
                requiredTriangVertices += 12;
            }
            if (momentZ != Vector3.Empty)
            {
                requiredLineVertices += 4;
                requiredTriangVertices += 12;
            }

            checkIfEnoughSpaceInVB(requiredLineVertices, ref numLineVerticesInVB, ref linePack);
            checkIfEnoughSpaceInVB(requiredTriangVertices, ref numTriangVerticesInVB, ref triangPack);
        }

        private void checkIfEnoughSpaceInVB(int numRequiredVertices, ref int numVerticesInVB, ref PositionColoredPackage package)
        {
            if (numVerticesInVB + numRequiredVertices >= package.NumVertices)
            {
                GraphicViewManager.Instance.ResourceManager.ReleaseBuffer(numVerticesInVB, 0, package.Stream);
                package = (PositionColoredPackage)GraphicViewManager.Instance.ResourceManager.CaptureBuffer(package.Stream, 0, numRequiredVertices, true);
                numVerticesInVB = 0;
            }
            numVerticesInVB += numRequiredVertices;
        }
        #endregion

        /// <summary>
        /// Main rendering method
        /// </summary>
        /// <param name="device"> Rendering device </param>
        /// <param name="model"> The model </param>
        /// <param name="options"> Rendering options </param>
        public virtual void Render(Device device, Model.Model model, RenderOptions options, List<Item> itemsInView)
        {
            if (itemsInView == null) return;

            if (itemsInView.Count <= 0)
                GetItemsInView(itemsInView);

            if (itemsInView.Count > 0)
            {
                // Get resource cache instance
                ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
                int numTriangVerticesDrawn = 0;
                int numLineVerticesDrawn = 0;

                // Turn off lighting for color rendering
                device.RenderState.Lighting = false;
                device.RenderState.CullMode = Cull.None;

                // Vertex Buffer capture depends on current "renderer" (joints, lines, areas) and must be updated by them according to the feeded vertices
                // For simplicity, any "renderer" takes as parameter the last captured VertexBuffer and the number of vertices drawn
                PositionColoredPackage linePack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
                PositionColoredPackage triangPack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, false, true);

                // First, render loads over joints
                renderJointLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                // Second, render loads over lines
                renderLineLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                // Third, render loads over areas
                renderAreaLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);

                // Flush remaining vertices
                rc.ReleaseBuffer(numLineVerticesDrawn, 0, ResourceStreamType.Lines);
                rc.Flush(ResourceStreamType.Lines);
                rc.ReleaseBuffer(numTriangVerticesDrawn, 0, ResourceStreamType.TriangleListPositionColored);
                rc.Flush(ResourceStreamType.TriangleListPositionColored);

                //Turn on lighting
                device.RenderState.Lighting = true;
            }
        }
    }

    #region LoadRenderer Legacy
    ///// <summary>
    ///// Load renderer. Base class for load rendering. According to the different kinds of loads, this class intends to 
    ///// set up nice symbols for displaying them accordingly to the magnitude they represent
    ///// </summary>
    //public abstract class LoadRenderer : ItemRenderer
    //{
    //    /// <summary>
    //    /// Abstrace rendering method
    //    /// </summary>
    //    /// <param name="device"> Rendering device </param>
    //    /// <param name="model"> The model to be rendered </param>
    //    /// <param name="options"> Rendering options </param>
    //    protected abstract void render(Device device, Model.Model model, RenderOptions options);

    //    /// <summary> Every Load Renderer, has its own 3D arrow model /// </summary>
    //    protected Load3DModel arrowModel = new Load3DModel();

    //    #region Private fields
    //    /// <summary> The rendering device </summary>
    //    private Device theDev = null;
    //    /// <summary> The vertex buffer used for displaying arrows </summary>
    //    private VertexBuffer arrowVB = null;
    //    /// <summary> The vertex buffer usde for displaying lines </summary>
    //    private VertexBuffer lineVB = null;

    //    /// <summary> A data stream for locking and update any vertices in arrow vertex buffer </summary>
    //    private GraphicsStream vbArrowData = null;
    //    /// <summary> A data stream for locking and update any vertices in line vertex buffer </summary>
    //    private GraphicsStream vbLineData = null;

    //    /// <summary> Base pointer in arrow vertex buffer </summary>
    //    private int vbArrowBase = 0;
    //    /// <summary> Base pointer in line vertex buffer </summary>
    //    private int vbLineBase = 0;

    //    /// <summary> Flush size for arrow vertex buffer </summary>
    //    private int vbArrowFlush = 0;
    //    /// <summary> Flush size for line vertex buffer </summary>
    //    private int vbLineFlush = 0;

    //    /// <summary> Arrow vertex buffer size </summary>
    //    private int vbArrowSize = 0;
    //    /// <summary> Line vertex buffer size </summary>
    //    private int vbLineSize = 0;

    //    /// <summary> How much bytes do triangles use? (CustomVertex size) </summary>
    //    private int vArrowSize = 0;
    //    /// <summary> How much bytes do line use? (CustomVertex size) </summary>
    //    private int vLineSize = 0;

    //    /// <summary> How many arrows have been feed to vertex data? </summary>
    //    private int vertsArrowSize = 0;
    //    /// <summary> How many lines have been feed to vertex data? </summary>
    //    private int vertsLineSize = 0;

    //    /// <summary> Pointer to arrow vertex buffer data </summary>
    //    private unsafe CustomVertex.PositionColored* vbArrowArray = null;
    //    /// <summary> Pointer to line vertex buffer data </summary>
    //    private unsafe CustomVertex.PositionColored* vbLineArray = null;

    //    /// <summary> Most of loads have a starting position </summary>
    //    private Vector3 origin = Vector3.Empty;
    //    /// <summary> Most of loads have an ending position </summary>
    //    private Vector3 end = Vector3.Empty;
    //    /// <summary> Three vertices define an arrow </summary>
    //    private Vector3[] singleArrow = new Vector3[3];

    //    /// <summary> Because forces and momments are represented as arrows, we need to know wether they are forces or momments </summary>
    //    private bool isForceLoad = false;
    //    #endregion

    //    #region 3D Model for Loads...
    //    /// <summary>
    //    /// Helper for rendering arrows as loads.
    //    /// One instance for ANY load, vertex just need to get transformated according to the position
    //    /// where they must appear in the structure
    //    /// </summary>
    //    protected class Load3DModel
    //    {
    //        /// <summary> A Vector3 array containing the requested arrow(s) tip(s) </summary>
    //        private Vector3[] arrow = null;
    //        /// <summary> A Vector3 array containgin the requested arrow line </summary>
    //        private Vector3[] line = null;
    //        /// <summary> Line direction </summary>
    //        private Vector3 dir;
    //        /// <summary> A scaling factor for line rendering issues </summary>
    //        private float scale;
    //        /// <summary> Another scaling factor for arrow rendering issues </summary>
    //        private float deltaT;

    //        // During calculation, we need to do some transformation, so we also need some variables for storing them
    //        /// <summary> First transformation </summary>
    //        private Vector3 v1 = Vector3.Empty;
    //        /// <summary> Second transformation </summary>
    //        private Vector3 v2 = Vector3.Empty;
    //        /// <summary> Third transformation </summary>
    //        private Vector3 v3 = Vector3.Empty;
    //        /// <summary> Fourth transformation </summary>
    //        private Vector3 v4 = Vector3.Empty;

    //        /// <summary>
    //        /// Gets the computed arrow, it can be the default or the transformed. 
    //        /// It is assumed we want the transformed arrow between two endpoints
    //        /// but, when there has been no transformation, the default is returned
    //        /// </summary>
    //        public Vector3[] Arrow
    //        {
    //            get { return arrow; }
    //        }

    //        /// <summary> Get the computed arrow line </summary>
    //        public Vector3[] Line
    //        {
    //            get { return line; }
    //        }

    //        /// <summary>
    //        /// Builds an arrow between two points in World space, so
    //        /// line direction is set by the line defined by these two endpoints
    //        /// </summary>
    //        /// <param name="origin"> Arrow endpoint position </param>
    //        /// <param name="end"> Arrow tip position </param>
    //        /// <param name="arrowScale"> Arrow scaling factor </param>
    //        /// <param name="lineScale"> Line scaling factor </param>
    //        /// <returns></returns>
    //        public Vector3[] CreateArrowFromEndpoints(Vector3 origin, Vector3 end, float arrowScale, float lineScale)
    //        {
    //            // Here we compute the directional vector between origin and end, starting at end
    //            dir = end - origin;
    //            // Invert the direction
    //            scale = -lineScale;
    //            // Get the scaling factor for arrows
    //            deltaT = arrowScale;

    //            // Her we have a scaled line pointing from origin to end at end position
    //            v4 = dir * scale;
    //            v4 += end;

    //            // Here we have the direction of the line as unit vector
    //            v2 = dir;
    //            v2.Normalize();

    //            // Compute a vector that forms a 45[deg] angle with line dir and normalize it
    //            if (dir.Z != 0)
    //            {
    //                v3.X = 1.0f; v3.Y = 1.0f;
    //                v3.Z = -(v2.X + v2.Y) / v2.Z;
    //            }
    //            else if (dir.Y != 0)
    //            {
    //                v3.X = 1.0f; v3.Z = 1.0f;
    //                v3.Y = -v2.X / v2.Y;
    //            }
    //            else
    //            {
    //                v3.X = 0.0f; v3.Y = 0.0f;
    //                v3.Z = 1.0f;
    //            }

    //            v3.Normalize();

    //            // Get a normal to v3 that also forms a 45 [deg] angle with v2
    //            v1 = Vector3.Cross(v2, v3);

    //            // Scale arrow tip and place at end
    //            v2 = v2 * (-2.0f * deltaT);
    //            v2 += end;

    //            // Scale the other two vertices for building the triangle
    //            v3 = v3 * deltaT;
    //            v1 = v1 * deltaT;

    //            // Transform vertex array
    //            // First triangle
    //            arrow[0] = end; arrow[1] = v2 - v1; arrow[2] = v2 + v1;

    //            // Second triangle
    //            arrow[3] = end; arrow[4] = v2 - v3; arrow[5] = v2 + v3;

    //            // Line
    //            line[0] = end; line[1] = v4;

    //            return arrow;
    //        }
    //        /// <summary>
    //        /// Creates an arrow model with tip at the origin
    //        /// </summary>
    //        public Load3DModel()
    //        {
    //            arrow = new Vector3[6];
    //            line = new Vector3[2];

    //            for (int i = 0; i < arrow.Length; ++i)
    //                arrow[i] = Vector3.Empty;

    //            line[0] = Vector3.Empty;
    //            line[1] = Vector3.Empty;
    //        }
    //    }
    //    #endregion

    //    #region JointLoad Renderer related...

    //    /// <summary>
    //    /// Render any load over joints
    //    /// </summary>
    //    /// <param name="joints"> Collection of joints </param>
    //    /// <param name="options"> Rendering options </param>
    //    protected void renderJointLoads(IEnumerable<Joint> joints, RenderOptions options)
    //    {
    //        // Joints, as any item, has properties. A string is needed for displayin some of them->load info
    //        string jointLabel = string.Empty;
    //        // The current unit system enabled
    //        string unit = string.Empty;

    //        // Force/Momment values used for getting their value as string
    //        float[] forceValues = new float[4];
    //        float forceValue;

    //        // Sweeo joint list
    //        foreach (Joint element in joints)
    //        {
    //            // Loads are displayed only if joint is selected
    //            if (element != null && element.IsSelected == true)
    //            {
    //                // Get load list for the current joint
    //                AssignedJointLoads jointLoads = (AssignedJointLoads)element.Loads;

    //                ItemList<Load> loadList = jointLoads[Canguro.Model.Model.Instance.ActiveLoadCase];

    //                // Get joint position
    //                Vector3 joint = new Vector3(element.X, element.Y, element.Z);
    //                Vector3 force, momentX, momentY, momentZ;
    //                float scale;

    //                if (loadList != null)
    //                {
    //                    foreach (JointLoad load in loadList)
    //                    {
    //                        // When force load is detected...
    //                        if (load is ForceLoad)
    //                        {
    //                            // Get load as array
    //                            ForceLoad fl = (ForceLoad)load;
    //                            float[] flInt = fl.Force;

    //                            isForceLoad = true;

    //                            // Compute a vector representing the force and scale it for having a best display
    //                            force = new Vector3(flInt[0], flInt[1], flInt[2]);
    //                            scale = (float)Math.Sqrt(force.Length());
    //                            force.Normalize();
    //                            force.Scale(scale);

    //                            // Do the same with moments
    //                            momentX = new Vector3(1.0f, 0.0f, 0.0f);
    //                            momentX.Scale((float)Math.Sqrt(Math.Abs(flInt[3])) * Math.Sign(flInt[3]));

    //                            momentY = new Vector3(0.0f, 1.0f, 0.0f);
    //                            momentY.Scale((float)Math.Sqrt(Math.Abs(flInt[4])) * Math.Sign(flInt[4]));

    //                            momentZ = new Vector3(0.0f, 0.0f, 1.0f);
    //                            momentZ.Scale((float)Math.Sqrt(Math.Abs(flInt[5])) * Math.Sign(flInt[5]));
                                
    //                            #region Load Rendering...
    //                            // When requested load labels, do other kind of rendering
    //                            if (element.IsSelected && (options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
    //                            {
    //                                #region Load text rendering
                                    
    //                                // Enable unit conversion system
    //                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

    //                                #region Force Load texts
    //                                // Draws the load
    //                                drawJointLoad(force, joint, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), isForceLoad);

    //                                // Get values for string display
    //                                forceValues[0] = flInt[0];
    //                                forceValues[1] = flInt[1];
    //                                forceValues[2] = flInt[2];
    //                                forceValue = (float)Math.Sqrt(flInt[0] * flInt[0] + flInt[1] * flInt[1] + flInt[2] * flInt[2]);

    //                                if (forceValue != 0.0f)
    //                                {
    //                                    // Convert values to the current system
    //                                    forceValues[0] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[0], Canguro.Model.UnitSystem.Units.Force);
    //                                    forceValues[1] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[1], Canguro.Model.UnitSystem.Units.Force);
    //                                    forceValues[2] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[2], Canguro.Model.UnitSystem.Units.Force);

    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Force);

    //                                    // Build label
    //                                    jointLabel = "(" + string.Format("{0:F3}", forceValues[0]) + ", " + 
    //                                                       string.Format("{0:F3}", forceValues[1]) + ", " + 
    //                                                       string.Format("{0:F3}", forceValues[2]) + ")" + " [" + unit + "]";

    //                                    // Display label
    //                                    DrawItemText(jointLabel, arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);
    //                                }
    //                                #endregion

    //                                #region Moment texts

    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in X axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentX, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
                                    
    //                                forceValue = (float)Math.Abs(flInt[3]);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);
    //                                }
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in Y axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentY, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);

    //                                forceValue = (float)Math.Abs(flInt[4]);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);
    //                                }

    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in Z axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentZ, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = (float)Math.Abs(flInt[5]);
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Moment);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);
    //                                }

    //                                #endregion

    //                                // Disable unit conversion system
    //                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

    //                                #endregion
    //                            }
    //                            else
    //                            {
    //                                // Just draw loads
    //                                #region Load rendering
    //                                drawJointLoad(force, joint, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentX, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentY, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentZ, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                #endregion
    //                            }
    //                            #endregion
    //                        }
    //                        // When ground displacement load is detected...
    //                        if (load is GroundDisplacementLoad)
    //                        {
    //                            // Get load
    //                            float[] gdl = ((GroundDisplacementLoad)load).Displacements;

    //                            isForceLoad = false;

    //                            // Compute vector from load magnitudes for displaying
    //                            force = new Vector3(-gdl[0], -gdl[1], -gdl[2]);
    //                            scale = (float)Math.Sqrt(force.Length());
    //                            force.Normalize();
    //                            force.Scale(scale);

    //                            float sFac = 50.0f;

    //                            // Do the same with moments
    //                            momentX = new Vector3(1.0f, 0.0f, 0.0f);
    //                            momentX.Scale((float)Math.Sqrt(Math.Abs(gdl[3])) * Math.Sign(-gdl[3]) * sFac);

    //                            momentY = new Vector3(0.0f, 1.0f, 0.0f);
    //                            momentY.Scale((float)Math.Sqrt(Math.Abs(gdl[4])) * Math.Sign(-gdl[4]) * sFac);

    //                            momentZ = new Vector3(0.0f, 0.0f, 1.0f);
    //                            momentZ.Scale((float)Math.Sqrt(Math.Abs(gdl[5])) * Math.Sign(-gdl[5]) * sFac);

    //                            #region Load Rendering...
    //                            // When labels wanted, rendering is special
    //                            if (element.IsSelected && (options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
    //                            {
    //                                #region Load text rendering

    //                                // Enable unit conversion system
    //                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

    //                                #region Force Load texts
    //                                // Draw joint load
    //                                drawJointLoad(force, joint, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), isForceLoad);

    //                                // Get force values to build label
    //                                forceValues[0] = gdl[0];
    //                                forceValues[1] = gdl[1];
    //                                forceValues[2] = gdl[2];
    //                                forceValue = (float)Math.Sqrt(gdl[0] * gdl[0] + gdl[1] * gdl[1] + gdl[2] * gdl[2]);

    //                                if (forceValue != 0.0f)
    //                                {
    //                                    // Convert values to the current system
    //                                    forceValues[0] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[0], Canguro.Model.UnitSystem.Units.Distance);
    //                                    forceValues[1] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[1], Canguro.Model.UnitSystem.Units.Distance);
    //                                    forceValues[2] = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValues[2], Canguro.Model.UnitSystem.Units.Distance);

    //                                    // Get current unit
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

    //                                    // Build information label
    //                                    jointLabel = "(" + string.Format("{0:F3}", forceValues[0]) + ", " +
    //                                                       string.Format("{0:F3}", forceValues[1]) + ", " +
    //                                                       string.Format("{0:F3}", forceValues[2]) + ")" + " [" + unit + "]";

    //                                    // Draw label
    //                                    DrawItemText(jointLabel, arrowModel.Line[0], Canguro.Properties.Settings.Default.TextColor);
    //                                }
    //                                #endregion

    //                                #region Moment texts

    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in X axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentX, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);

    //                                forceValue = (float)Math.Abs(gdl[3]);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Angle);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[0], Canguro.Properties.Settings.Default.TextColor);
    //                                }
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in Y axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentY, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);

    //                                forceValue = (float)Math.Abs(gdl[4]);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Angle);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[0], Canguro.Properties.Settings.Default.TextColor);
    //                                }

    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                /// Moment in Z axis
    //                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                drawJointMoment(momentZ, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                if (forceValue != 0.0f)
    //                                {
    //                                    forceValue = (float)Math.Abs(gdl[5]);
    //                                    forceValue = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(forceValue, Canguro.Model.UnitSystem.Units.Angle);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Angle);
    //                                    jointLabel = string.Format("{0:F3} {1}", forceValue, "[" + unit + "]");
    //                                    DrawItemText(jointLabel, arrowModel.Line[0], Canguro.Properties.Settings.Default.TextColor);
    //                                }

    //                                #endregion

    //                                // Disable unit conversion system
    //                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

    //                                #endregion
    //                            }
    //                            else
    //                            {
    //                                #region Load rendering
    //                                drawJointLoad(force, joint, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentX, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentY, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                drawJointMoment(momentZ, joint, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), isForceLoad);
    //                                #endregion
    //                            }
    //                            #endregion
    //                        }
    //                    }                            
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Draws a "moment" in a joint
    //    /// </summary>
    //    /// <param name="moment"> Vector representing the moment </param>
    //    /// <param name="joint"> Action joint </param>
    //    /// <param name="color"> Which colour is used to render this items </param>
    //    /// <param name="isForceLoad"> Is this a force load? </param>
    //    protected void drawJointMoment(Vector3 moment, Vector3 joint, int color, bool isForceLoad)
    //    {
    //        Vector3 dir;

    //        // When moment is zero, no rendering is done
    //        if (moment != Vector3.Empty)
    //        {
    //            // Get endpoints
    //            origin = joint - moment;
    //            end = joint;

    //            //When we have a ground displacement, arrows point outwards the joint
    //            if (!isForceLoad)
    //            {
    //                swapVectors(ref origin, ref end);
    //                dir = (end - origin);
    //                Vector3 v4 = dir * Settings.Default.ForceLoadLineScale;

    //                end = origin + v4;
    //                origin = origin - dir + v4;
    //            }

    //            // Start arrow creation
    //            arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.MomentLoadArrowScale, Settings.Default.MomentLoadLineScale);

    //            // Draw moment line
    //            appendLine(arrowModel.Line[0], arrowModel.Line[1], color);
    //            // Draw first arrow
    //            appendArrows(arrowModel.Arrow, color);

    //            //Draw second arrow
    //            dir = end - origin;
    //            dir.Normalize();
    //            end = end - 0.05f * dir;
    //            arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.MomentLoadArrowScale, Settings.Default.MomentLoadLineScale);
    //            appendArrows(arrowModel.Arrow, color);
    //        }
    //    }

    //    /// <summary>
    //    /// Draws a load in a joint
    //    /// </summary>
    //    /// <param name="force"> Vector representing the force </param>
    //    /// <param name="joint"> Action joint </param>
    //    /// <param name="color"> Colour for rendering loads </param>
    //    /// <param name="isForceLoad"> Is this a force load? </param>
    //    protected void drawJointLoad(Vector3 force, Vector3 joint, int color, bool isForceLoad)
    //    {
    //        if (force != Vector3.Empty)
    //        {
    //            #region Legacy...
    //            //origin = joint - force;
    //            //end = joint;

    //            //if (!isForceLoad)
    //            //{
    //            //    swapVectors(ref origin, ref end);
    //            //    Vector3 dir = (end - origin);
    //            //    Vector3 v4 = dir; // *Settings.Default.ForceLoadLineScale;

    //            //    end = origin + v4;
    //            //    origin = origin - dir + v4;
    //            //    arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.ForceLoadArrowScale, 1);
    //            //}
    //            //else
    //            //    arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.ForceLoadArrowScale, Settings.Default.ForceLoadLineScale);
    //            #endregion

    //            // Create the load representation
    //            doJointLoad(force, joint, isForceLoad);

    //            // Draw forces
    //            appendLine(arrowModel.Line[0], arrowModel.Line[1], color);
    //            appendArrows(arrowModel.Arrow, color);
    //        }
    //    }

    //    /// <summary>
    //    /// Creates the needed vertices displaying the force
    //    /// </summary>
    //    /// <param name="force"> Vector representing the force </param>
    //    /// <param name="joint"> Action joint </param>
    //    /// <param name="isForceLoad"> Is this a force load? </param>
    //    protected void doJointLoad(Vector3 force, Vector3 joint, bool isForceLoad)
    //    {
    //        // Get endpoints
    //        origin = joint - force;
    //        end = joint;

    //        // Build arrows
    //        if (!isForceLoad)
    //        {
    //            swapVectors(ref origin, ref end);
    //            Vector3 dir = (end - origin);
    //            Vector3 v4 = dir; // *Settings.Default.ForceLoadLineScale;

    //            end = origin + v4;
    //            origin = origin - dir + v4;
    //            arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.ForceLoadArrowScale, 1);
    //        }
    //        else
    //            arrowModel.CreateArrowFromEndpoints(origin, end, Settings.Default.ForceLoadArrowScale, Settings.Default.ForceLoadLineScale);
    //    }

    //    #endregion

    //    #region LineLoad Renderer...

    //    /// <summary>
    //    /// Render any load over lines
    //    /// </summary>
    //    /// <param name="lines"> Line list </param>
    //    /// <param name="options"> Rendering options </param>
    //    protected unsafe void renderLineLoads(IEnumerable<LineElement> lines, RenderOptions options)
    //    {
    //        float forceValue = 0.0f;
    //        Vector3 loadDir = Vector3.Empty;
    //        // Display text
    //        string cslText = string.Empty;
    //        string dslText = string.Empty;
    //        string unit = string.Empty;

    //        // Sweep line list
    //        foreach (LineElement element in lines)
    //        {
    //            if (element != null && element.IsSelected == true)
    //            {
    //                // Get line direction
    //                Vector3 lineDir = element.J.Position - element.I.Position;

    //                // Get assigned loads fot the current selected line
    //                AssignedLineLoads lineLoads = (AssignedLineLoads)element.Loads;

    //                ItemList<Load> loadList = lineLoads[Canguro.Model.Model.Instance.ActiveLoadCase];

    //                if (loadList != null)
    //                {
    //                    foreach (LineLoad load in loadList)
    //                    {
    //                        // When load is concentrated...
    //                        if (load is ConcentratedSpanLoad)
    //                        {
    //                            ConcentratedSpanLoad csl = (ConcentratedSpanLoad)load;

    //                            // Get load as vector
    //                            Vector3 concentratedLoadPos = Vector3.Empty;
    //                            concentratedLoadPos = element.I.Position + csl.D * lineDir;

    //                            Vector3 force = Vector3.Empty;

    //                            // Compute the vector for display
    //                            force = (float)Math.Sqrt(Math.Abs(csl.LoadInt)) * Math.Sign(csl.LoadInt) * getLoadDirection(load, element);

    //                            #region Load Rendering

    //                            if (csl.Type == LineLoad.LoadType.Force)
    //                            {
    //                                // Compute arrow vertices 
    //                                drawJointLoad(force, concentratedLoadPos, Settings.Default.ForceLoadForceDefaultColor.ToArgb(), true);
                                    
    //                                #region Load text drawing
    //                                if (element.IsSelected && (options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
    //                                {
    //                                    // When text rendering is enabled, enable unit convesion system
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

    //                                    // Get force magnitude and current unit
    //                                    forceValue = Math.Abs(csl.L);
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Load0D);

    //                                    // Draw text
    //                                    DrawItemText(string.Format("{0:F3} {1}", forceValue, "[" + unit + "]"), arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);

    //                                    // Disable text rendering
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

    //                                    cslText += "\n";
    //                                }
    //                                #endregion
    //                            }
    //                            else
    //                            {
    //                                // Compute moments
    //                                drawJointMoment(force, concentratedLoadPos, Settings.Default.ForceLoadMomentDefaultColor.ToArgb(), true);

    //                                #region Load text drawing
    //                                if (element.IsSelected && (options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
    //                                {
    //                                    // Enable unit system conversion
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

    //                                    // Get magnitude
    //                                    forceValue = Math.Abs(csl.L);
    //                                    // Get current unit
    //                                    unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

    //                                    // Draw texts
    //                                    DrawItemText(string.Format("{0:F3} {1}", forceValue, "[" + unit + "]"), arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);

    //                                    // Disable unit system conversion
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

    //                                    cslText += "\n";
    //                                }
    //                                #endregion
    //                            }
    //                            #endregion
    //                        }
    //                        if (load is DistributedSpanLoad)
    //                        {
    //                            // Distributed Loads
    //                            DistributedSpanLoad dsl = (DistributedSpanLoad)load;

    //                            // Do not do anything when loads are zero
    //                            if (dsl.LoadAInt != 0.0f || dsl.LoadBInt != 0.0f)
    //                            {
    //                                // Configure some control points with distances and loads at each endpoint
    //                                float[,] ctrlPts = new float[,] { 
    //                                                   { dsl.Da, (float)Math.Sqrt(0.5f*Math.Abs(dsl.LoadAInt))*Math.Sign(dsl.LoadAInt) * Settings.Default.ForceLoadLineScale },
    //                                                   { dsl.Db, (float)Math.Sqrt(0.5f*Math.Abs(dsl.LoadBInt))*Math.Sign(dsl.LoadBInt) * Settings.Default.ForceLoadLineScale }
    //                                };

    //                                // When label magnitude displaying is enabled, draw them
    //                                if (element.IsSelected && (options.OptionsShown & RenderOptions.ShowOptions.LoadMagnitudes) != 0)
    //                                {
    //                                    // Enable unit system conversion
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

    //                                    // Check which type of load are we rendering for having the correct current unit
    //                                    if (load.Type == LineLoad.LoadType.Force)
    //                                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Load1D);
    //                                    else
    //                                        unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Moment);

    //                                    // Get load values at initial point (A) and configures label
    //                                    loadDir = (float)Math.Sqrt(0.5f * Math.Abs(dsl.LoadAInt)) * Math.Sign(dsl.LoadAInt) * getLoadDirection(load, element);
    //                                    // Create Joint
    //                                    doJointLoad(loadDir, element.I.Position + dsl.Da * (element.J.Position - element.I.Position), true);
    //                                    // Draw text in the correct position
    //                                    DrawItemText(dsl.La.ToString("f3") + " [" + unit + "]", arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);

    //                                    // Get load values at initial point (A) and configures label
    //                                    loadDir = (float)Math.Sqrt(0.5f * Math.Abs(dsl.LoadBInt)) * Math.Sign(dsl.LoadBInt) * getLoadDirection(load, element);
    //                                    // Create Joint
    //                                    doJointLoad(loadDir, element.I.Position + dsl.Db * (element.J.Position - element.I.Position), true);
    //                                    // Draw text in the correct position
    //                                    DrawItemText(dsl.Lb.ToString("f3") + " [" + unit + "]", arrowModel.Line[1], Canguro.Properties.Settings.Default.TextColor);
                                        
    //                                    // Disable unit system conversion
    //                                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
    //                                }

    //                                // Get color and set number of arrows according to kind of load
    //                                int color = (load.Type == LineLoad.LoadType.Force) ? Settings.Default.ForceLoadForceDefaultColor.ToArgb() : Settings.Default.ForceLoadMomentDefaultColor.ToArgb();
    //                                int numArrows = (load.Type == LineLoad.LoadType.Moment) ? 2 : 1;

    //                                // Draw load
    //                                drawPolygonalLoad(ctrlPts, element, getLoadDirection(load, element), color, numArrows);
    //                            }

    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Draws a distributed load from a set of control points and the current line
    //    /// </summary>
    //    /// <param name="ctrlPts"> Defines some control points for building load symbol </param>
    //    /// <param name="line"> The current analyzed line </param>
    //    /// <param name="direction"> Load direction </param>
    //    /// <param name="color"> Drawn color </param>
    //    /// <param name="numArrows"> How many arrows are needed, can be 1 or 2 </param>
    //    protected void drawPolygonalLoad(float[,] ctrlPts, LineElement line, Vector3 direction, int color, int numArrows)
    //    {
    //        float length, beamLength;
    //        Vector3 posI, posJ;
    //        Vector3 pTemp, pTempU;
    //        Vector3 dir;

    //        int numPts = ctrlPts.GetLength(0);

    //        if (numPts < 2)
    //            return;

    //        length = line.LengthInt;
    //        beamLength = length;

    //        Joint j1 = line.I;
    //        Joint j2 = line.J;

    //        // Lets consider a global system...
    //        posI = j1.Position;
    //        posJ = j2.Position;

    //        pTemp = posJ - posI;

    //        // Lets consider a global system...
    //        pTempU = Vector3.Normalize(pTemp);

    //        // Now, we treat direction as a unit vector in opposite direction...
    //        dir = -1.0f * Vector3.Normalize(direction);

    //        // We know the control points and force direction, so lets draw the contour...
    //        Vector3 segmenti, dirTemp, last;

    //        segmenti = ctrlPts[0, 0] * pTemp;
    //        segmenti += posI;
    //        dirTemp = ctrlPts[0, 1] * dir;
    //        segmenti += dirTemp;

    //        last = segmenti;            

    //        for (int i = 1; i < numPts; ++i)
    //        {
    //            segmenti = ctrlPts[i, 0] * pTemp;
    //            segmenti += posI;
    //            dirTemp = ctrlPts[i, 1] * dir;
    //            segmenti += dirTemp;

    //            appendLine(last, segmenti, color);
    //            last = segmenti;
    //        }

    //        // Draw load arrows...
    //        float deltaL = 0.3375f;
    //        float deltaT = Settings.Default.MomentLoadArrowScale;
    //        length = Math.Abs(length * (ctrlPts[numPts - 1, 0] - ctrlPts[0, 0]));

    //        // How many lines are we drawing?
    //        int nLines = (int)(length / deltaL + 0.5f);

    //        if (nLines != 0)
    //            deltaL = length / nLines;

    //        float pLine = 0.0f;
    //        float deltaXP, slope;
    //        Vector3 upTemp2, pTemp2, pTemp3;
    //        int ctrlPtsIndex = 0;

    //        // Get the slope for the first rect segment...
    //        slope = (nLines != 0) ? (ctrlPts[ctrlPtsIndex + 1, 1] - ctrlPts[ctrlPtsIndex, 1]) /
    //                                (ctrlPts[ctrlPtsIndex + 1, 0] - ctrlPts[ctrlPtsIndex, 0])
    //                                : 0.0f;

    //        pTempU *= deltaT;
    //        deltaT *= 2;

    //        for (int i = 0; i <= nLines; i++)
    //        {
    //            // Get the position (0 -1) in the bar
    //            pLine = (nLines != 0) ? ((float)i) / nLines * length / beamLength + ctrlPts[0, 0] : ctrlPts[0, 0];

    //            // Find segment...
    //            while (ctrlPtsIndex < (numPts - 1) && ctrlPts[ctrlPtsIndex + 1, 0] < pLine)
    //            {
    //                slope = (ctrlPts[ctrlPtsIndex + 1, 1] - ctrlPts[ctrlPtsIndex, 1]) /
    //                        (ctrlPts[ctrlPtsIndex + 1, 0] - ctrlPts[ctrlPtsIndex, 0]);
    //                ++ctrlPtsIndex;
    //            }

    //            // Get intersection points...
    //            deltaXP = (pLine - ctrlPts[ctrlPtsIndex, 0]) * slope + ctrlPts[ctrlPtsIndex, 1];

    //            pTemp2 = pLine * pTemp + posI;

    //            dirTemp = deltaXP * dir;

    //            upTemp2 = pTemp2 + dirTemp;

    //            // Draw arrow line...
    //            appendLine(upTemp2, pTemp2, color);

    //            // Check if arrow fits in the area
    //            Vector3 ptlen;
    //            float tempLen;

    //            ptlen = upTemp2 - pTemp2;
    //            tempLen = ptlen.Length();

    //            Vector3 newdir = Vector3.Normalize(upTemp2 - pTemp2);

    //            if (numArrows > 0 && tempLen > deltaT)
    //            {
    //                // Get triangle points...
    //                pTemp3 = deltaT * newdir + pTemp2;

    //                singleArrow[0] = pTemp2;
    //                singleArrow[1] = pTemp3 - pTempU;
    //                singleArrow[2] = pTemp3 + pTempU;

    //                appendArrows(singleArrow, color);

    //                for (int arrow = 1; arrow < numArrows; arrow++)
    //                {
    //                    singleArrow[0] = singleArrow[0] + 0.8f * deltaT * newdir;
    //                    singleArrow[1] = singleArrow[1] + 0.8f * deltaT * newdir;
    //                    singleArrow[2] = singleArrow[2] + 0.8f * deltaT * newdir;

    //                    appendArrows(singleArrow, color);
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Gets a vector indicating the load direction from the property in the current line element
    //    /// </summary>
    //    /// <param name="load"> Current load </param>
    //    /// <param name="beam"> The line element </param>
    //    /// <returns> Line direction in a Vector3 structure </returns>
    //    protected Vector3 getLoadDirection(LineLoad load, LineElement beam)
    //    {
    //        Vector3 loadDir = Vector3.Empty;

    //        // Check property and get direction
    //        switch (load.Direction)
    //        {
    //            case LineLoad.LoadDirection.Gravity:
    //                loadDir = new Vector3(0.0f, 0.0f, -1.0f);
    //                break;
    //            case LineLoad.LoadDirection.GlobalZ:
    //                loadDir = new Vector3(0.0f, 0.0f, 1.0f);
    //                break;
    //            case LineLoad.LoadDirection.GlobalX:
    //                loadDir = new Vector3(1.0f, 0.0f, 0.0f);
    //                break;
    //            case LineLoad.LoadDirection.GlobalY:
    //                loadDir = new Vector3(0.0f, 1.0f, 0.0f);
    //                break;
    //            case LineLoad.LoadDirection.Local1:
    //                loadDir = beam.LocalAxes[0];
    //                break;
    //            case LineLoad.LoadDirection.Local2:
    //                loadDir = beam.LocalAxes[1];
    //                break;
    //            case LineLoad.LoadDirection.Local3:
    //                loadDir = beam.LocalAxes[2];
    //                break;
    //        }

    //        return loadDir;
    //    }

    //    #endregion

    //    #region AreaLoad Renderer related...

    //    /// <summary>
    //    /// Renders any load over an area
    //    /// </summary>
    //    /// <param name="areas"> Area list </param>
    //    /// <param name="options"> Rendering options </param>
    //    protected unsafe void renderAreaLoads(IEnumerable<AreaElement> areas, RenderOptions options)
    //    {
    //    }

    //    #endregion

    //    #region General rendering, buffer and stream management...

    //    /// <summary>
    //    /// Make vertices in vertex buffer to be flushed (drawn) and relocks it via its graphic stream
    //    /// </summary>
    //    /// <param name="gs"> The graphic stream that locked the vertex buffer </param>
    //    private unsafe void flushAndReallocateStream(GraphicsStream gs)
    //    {
    //        // Flush stream (send vertices to T&L stage)
    //        flushStream(gs);

    //        // Because we are using tow buffers, we must check which is being flushed
    //        if (vbArrowData == gs)
    //        {
    //            // Update base pointer
    //            vbArrowBase += vertsArrowSize;
    //            // Check if it has space befor reaching the end. If not, make base start at zero
    //            vbArrowBase = (vbArrowBase >= vbArrowSize - 3 * vbArrowFlush) ? 0 : vbArrowBase;
    //            // Relock VB
    //            vbArrowData = arrowVB.Lock(vbArrowBase * vArrowSize, vbArrowFlush * vArrowSize, (vbArrowBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
    //            // Get the pointer to the locked vertex buffer
    //            vbArrowArray = (CustomVertex.PositionColored*)vbArrowData.InternalDataPointer;
    //            // No vertices to display
    //            vertsArrowSize = 0;
    //        }
    //        else
    //        {
    //            // Update base pointer
    //            vbLineBase += vertsLineSize;
    //            // Check if it has space befor reaching the end. If not, make base start at zero
    //            vbLineBase = (vbLineBase >= vbLineSize - 3 * vbLineFlush) ? 0 : vbLineBase;
    //            // Relock VB
    //            vbLineData = lineVB.Lock(vbLineBase * vLineSize, vbLineFlush * vLineSize, (vbLineBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
    //            // Get the pointer to the locked vertex buffer
    //            vbLineArray = (CustomVertex.PositionColored*)vbLineData.InternalDataPointer;
    //            // No vertices to display
    //            vertsLineSize = 0;
    //        }
    //    }

    //    /// <summary>
    //    /// Send vertices to T&L stage (render them)
    //    /// </summary>
    //    /// <param name="gs"> The active graphic stream </param>
    //    private unsafe void flushStream(GraphicsStream gs)
    //    {
    //        PrimitiveType pt;
    //        VertexBuffer theVB;
    //        int dataVertices;
    //        int vbBase;
    //        int primCount;

    //        // If we are mapping triangles...
    //        if (vbArrowData == gs)
    //        {
    //            theVB = arrowVB;
    //            dataVertices = vertsArrowSize;
    //            pt = PrimitiveType.TriangleList;
    //            vbBase = vbArrowBase;
    //            primCount = vertsArrowSize / 3;
    //        }
    //        else    // We have some lines
    //        {
    //            theVB = lineVB;
    //            dataVertices = vertsLineSize;
    //            pt = PrimitiveType.LineList;
    //            vbBase = vbLineBase;
    //            primCount = vertsLineSize / 2;
    //        }

    //        /// Unlock the active stream before flushing vertices
    //        theVB.Unlock();
    //        if (dataVertices != 0)
    //        {
    //            theDev.SetStreamSource(0, theVB, 0);
    //            theDev.DrawPrimitives(pt, vbBase, primCount);
    //        }
    //    }

    //    /// <summary>
    //    /// Swap tow vectors
    //    /// </summary>
    //    /// <param name="origin"> First vector </param>
    //    /// <param name="end"> Second vector </param>
    //    private void swapVectors(ref Vector3 origin, ref Vector3 end)
    //    {
    //        Vector3 aux = end;

    //        end = origin;
    //        origin = aux;
    //    }

    //    /// <summary>
    //    /// Appends a Line to the line vertex buffer
    //    /// </summary>
    //    /// <param name="origin"> Line's origin </param>
    //    /// <param name="end"> Line's end</param>
    //    /// <param name="color"> Line color </param>
    //    private unsafe void appendLine(Vector3 origin, Vector3 end, int color)
    //    {
    //        // Draw arrow line...
    //        // Assign vertex properties
    //        vbLineArray->Position = origin;
    //        vbLineArray->Color = color;
    //        // Set the next vertex in vertices aray
    //        ++vbLineArray;
    //        // We have one more vertex in the stream, update it
    //        ++vertsLineSize;

    //        // Assign vertex properties
    //        vbLineArray->Position = end;
    //        vbLineArray->Color = color;
    //        // Set the next vertex in vertices aray
    //        ++vbLineArray;
    //        // We have one more vertex in the stream, update it
    //        ++vertsLineSize;

    //        // Flush vertices to allow the GPU to start processing
    //        if (vertsLineSize >= vbLineFlush - 1)
    //            flushAndReallocateStream(vbLineData);
    //    }

    //    /// <summary>
    //    /// Append arrows to the arrow vertex buffer
    //    /// </summary>
    //    /// <param name="arrow"> An array of arrows (triangles) </param>
    //    /// <param name="color"> Triangle color </param>
    //    private unsafe void appendArrows(Vector3[] arrow, int color)
    //    {
    //        // Draw arrow triangles
    //        for (int i = 0; i < arrow.Length; i += 3)
    //        {
    //            // Assign vertex properties
    //            vbArrowArray->Position = arrow[i];
    //            vbArrowArray->Color = color;
    //            // Set the next vertex in vertices aray
    //            ++vbArrowArray;
    //            // We have one more vertex in the stream, update it
    //            ++vertsArrowSize;

    //            // Assign vertex properties
    //            vbArrowArray->Position = arrow[i + 1];
    //            vbArrowArray->Color = color;
    //            // Set the next vertex in vertices aray
    //            ++vbArrowArray;
    //            // We have one more vertex in the stream, update it
    //            ++vertsArrowSize;

    //            // Assign vertex properties
    //            vbArrowArray->Position = arrow[i + 2];
    //            vbArrowArray->Color = color;
    //            // Set the next vertex in vertices aray
    //            ++vbArrowArray;
    //            // We have one more vertex in the stream, update it
    //            ++vertsArrowSize;

    //            // Flush vertices to allow the GPU to start processing
    //            if (vertsArrowSize >= vbArrowFlush - 1)
    //                flushAndReallocateStream(vbArrowData);
    //        }
    //    }

    //    #endregion

    //    /// <summary>
    //    /// Main rendering method
    //    /// </summary>
    //    /// <param name="device"> Rendering device </param>
    //    /// <param name="model"> The model </param>
    //    /// <param name="options"> Rendering options </param>
    //    public void Render(Device device, Model.Model model, RenderOptions options)
    //    {
    //        // Get resource cache instance
    //        ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

    //        // Get the device
    //        theDev = device;

    //        // Get vertex buffers
    //        //arrowVB = rc.ArrowsVB;
    //        //lineVB = rc.LinesVB;

    //        // Check if both buffers have valid data
    //        if (arrowVB == null || lineVB == null) return;

    //        // Get arrows vertex buffer properties
    //        //vbArrowBase = rc.ArrowsVbBase;
    //        //vbArrowFlush = rc.ArrowsVbFlush;
    //        //vbArrowSize = rc.ArrowsVbSize;

    //        // Get lines vertex buffer properties
    //        //vbLineBase = rc.LinesVbBase;
    //        //vbLineFlush = rc.LinesVbFlush;
    //        //vbLineSize = rc.LinesVbSize;

    //        // Set device parameters
    //        device.VertexFormat = CustomVertex.PositionColored.Format;

    //        // Lock arrows vertex buffer and get internal data
    //        vArrowSize = CustomVertex.PositionColored.StrideSize;
    //        vbArrowData = arrowVB.Lock(vbArrowBase * vArrowSize, vbArrowFlush * vArrowSize, (vbArrowBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
    //        vertsArrowSize = 0;

    //        // Lock lines vertex buffer and get internal data
    //        vLineSize = CustomVertex.PositionColored.StrideSize;
    //        vbLineData = lineVB.Lock(vbLineBase * vLineSize, vbLineFlush * vLineSize, (vbLineBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
    //        vertsLineSize = 0;

    //        // Turn off lighting for color rendering
    //        device.RenderState.Lighting = false;
    //        device.RenderState.CullMode = Cull.None;

    //        // Inicia código NO SEGURO
    //        unsafe
    //        {
    //            vbArrowArray = (CustomVertex.PositionColored*)vbArrowData.InternalDataPointer;
    //            vbLineArray = (CustomVertex.PositionColored*)vbLineData.InternalDataPointer;

    //            render(device, model, options);
    //        }
    //        // Termina código NO SEGURO

    //        // Flush remaining vertices
    //        device.SetStreamSource(0, arrowVB, 0);
    //        flushStream(vbArrowData);
    //        if (vertsArrowSize != 0)
    //        {
    //            vbArrowBase += vertsArrowSize;
    //            vbArrowBase = (vbArrowBase >= vbArrowSize - 3 * vbArrowFlush) ? 0 : vbArrowBase;

    //            vertsArrowSize = 0;
    //        }
    //        //rc.ArrowsVbBase = vbArrowBase;

    //        device.SetStreamSource(0, lineVB, 0);
    //        flushStream(vbLineData);
    //        if (vertsLineSize != 0)
    //        {
    //            vbLineBase += vertsLineSize;
    //            vbLineBase = (vbLineBase >= vbLineSize - 3 * vbLineFlush) ? 0 : vbLineBase;

    //            vertsLineSize = 0;
    //        }
    //        //rc.LinesVbBase = vbLineBase;

    //        //Turn on lighting
    //        device.RenderState.Lighting = true;
    //    }
    //}
    #endregion


    #region Joint Load Rendering Legacy
    //private int drawSingleMoment(Vector3 moment, Vector3 position, int color, bool isForceLoad, PositionColoredPackage triangPack, PositionColoredPackage linePack)
    //{
    //    int usedVertices = 0;

    //    // Get endpoints
    //    Vector3 origin = position - moment;
    //    Vector3 end = position;
    //    Vector3 dir;

    //    //When we have a ground displacement, arrows point outwards the joint
    //    if (!isForceLoad)
    //    {
    //        end = origin;
    //        origin = position;
    //        dir = (end - origin);
    //        Vector3 v4 = dir * Settings.Default.ForceLoadLineScale;

    //        end = origin + v4;
    //        origin = origin - dir + v4;
    //    }

    //    // Start arrow creation
    //    usedVertices = makeSingleArrow(origin, end, Settings.Default.MomentLoadArrowScale, Settings.Default.MomentLoadLineScale, color, triangPack, linePack);

    //    //Draw second arrow
    //    dir = end - origin;
    //    dir.Normalize();
    //    end = end - 0.05f * dir;
    //    origin = origin + 0.1f * (end - origin);
    //    usedVertices += makeSingleArrow(origin, end, Settings.Default.MomentLoadArrowScale, Settings.Default.MomentLoadLineScale, color, triangPack, linePack);

    //    return usedVertices;
    //}
    #endregion
}
