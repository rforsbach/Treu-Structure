using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.View.Gadgets;

namespace Canguro.View.Renderer
{
    public class WireframeJointRenderer : JointRenderer
    {
        Vector3 displacedJoint = Vector3.Empty;

        public override void Render(Device device, Model.Model model, Canguro.Model.Constraint constraint, RenderOptions options)
        {
            throw new System.NotImplementedException();
        }

        #region Legacy
        //public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<Canguro.Model.Joint> joints, RenderOptions options)
        //{
        //    ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
        //    VertexBuffer theVB = rc.MainVB;

        //    VertexBuffer linesVB = rc.LinesVB;

        //    if (theVB == null || linesVB == null) return;

        //    int vbBase = rc.MainVBBase;
        //    int vbFlush = rc.VbFlush;
        //    int vbSize = rc.VbSize;

        //    // Added for symbol rendering
        //    #region Restraints
        //    int linesVBBase = rc.LinesVbBase;
        //    int linesVBFlush = rc.LinesVbFlush;
        //    int linesVBSize = rc.LinesVbSize;
        //    int lineVertex = 0;
        //    #endregion

        //    device.VertexFormat = CustomVertex.PositionColored.Format;
        //    device.SetStreamSource(0, theVB, 0);
        //    device.RenderState.Lighting = false;

        //    bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
        //    int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
        //    int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
        //    int vSize = CustomVertex.PositionColored.StrideSize;
        //    GraphicsStream vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
        //    int vertsSize = 0;

        //    // Added for symbol rendering
        //    #region Restraints
        //    device.SetStreamSource(0, linesVB, 0);
        //    GraphicsStream linesVBData = linesVB.Lock(linesVBBase * vSize, linesVBFlush * vSize, (linesVBBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
        //    #endregion

        //    device.RenderState.PointSize = 5;

        //    Matrix rot = Matrix.Identity;


        //    // Inicia código NO SEGURO
        //    unsafe
        //    {
        //        device.SetStreamSource(0, theVB, 0);
        //        CustomVertex.PositionColored* vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;

        //        // Added for symbol rendering
        //        #region Restraints
        //        device.SetStreamSource(0, linesVB, 0);
        //        CustomVertex.PositionColored* vbSymbolArray = (CustomVertex.PositionColored*)linesVBData.InternalDataPointer;
        //        #endregion

        //        Vector3 currPos = Vector3.Empty;

        //        float deformedScaleFactor = 0f;
        //        if (model.HasResults)
        //            deformedScaleFactor = model.Results.PaintScaleFactorTranslation;

        //        foreach (Model.Joint j in joints)
        //        {
        //            if (j != null && j.IsVisible)
        //            {
        //                #region Joint rendering and joint text drawing
        //                device.SetStreamSource(0, theVB, 0);
        //                if (options.ShowDeformed == true && model.Results.JointDisplacements != null)
        //                {
        //                    displacedJoint.X = j.Position.X + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 0];
        //                    displacedJoint.Y = j.Position.Y + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 1];
        //                    displacedJoint.Z = j.Position.Z + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 2];

        //                    vbArray->Position = displacedJoint;
        //                }
        //                else
        //                    vbArray->Position = j.Position;

        //                // Text rendering -> Joint Strains
        //                if (((options.OptionsShown & RenderOptions.ShowOptions.Strains) != 0) && j.IsSelected)
        //                {
        //                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

        //                    float d1 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 0], Canguro.Model.UnitSystem.Units.Distance);
        //                    float d2 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 1], Canguro.Model.UnitSystem.Units.Distance);
        //                    float d3 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 2], Canguro.Model.UnitSystem.Units.Distance);
        //                    string unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

        //                    DrawItemText("( " + d1.ToString("G5") + ", " + d2.ToString("G5") + ", " + d3.ToString("G5") + ")" + "[" + unit + "]", j.Position, Canguro.Properties.Settings.Default.TextColor);

        //                    Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
        //                }

        //                // Text rendering -> Joint IDs
        //                if (((options.OptionsShown & RenderOptions.ShowOptions.JointIDs) != 0) && j.IsSelected)
        //                {
        //                    DrawItemText(j.Id.ToString(), j.Position, Canguro.Properties.Settings.Default.TextColor);
        //                }

        //                if (pickingMode)
        //                    vbArray->Color = rc.GetNextPickIndex(j);
        //                else
        //                    vbArray->Color = (j.IsSelected) ? jointSelectedColor : jointColor;
        //                vbArray++;

        //                vertsSize++;
        //                // Flush vertices to allow the GPU to start processing
        //                if (vertsSize >= vbFlush)
        //                {
        //                    device.SetStreamSource(0, theVB, 0);
        //                    theVB.Unlock();
        //                    device.DrawPrimitives(PrimitiveType.PointList, vbBase, vertsSize);
        //                    vbBase += vertsSize;

        //                    // If the space allocated in memory is over, discard buffer, else append data
        //                    if (vbBase >= vbSize)
        //                        vbBase = 0;

        //                    // Re-lock the VertexBuffer and obtain the unsafe pointer from the new lock
        //                    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
        //                    vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;
        //                    vertsSize = 0;
        //                }
        //                #endregion

        //                #region Joint DOF Rendering
        //                if ((options.OptionsShown & RenderOptions.ShowOptions.JointDOFs) != 0)
        //                {
        //                    // Added for symbol drawing
        //                    bool[] orientations = null;
        //                    int[][] constraintIndices = getConstraints(j, ref orientations);
        //                    int rows, cols;

        //                    rows = constraintIndices.GetLength(0);

        //                    if (constraintIndices != null && orientations != null)
        //                    {
        //                        device.SetStreamSource(0, linesVB, 0);
        //                        for (int i = 0; i < rows; ++i)
        //                        {
        //                            if (constraintIndices[i] != null && constraintIndices[i].Length != 0)
        //                            {
        //                                cols = constraintIndices[i].Length;
        //                                for (int k = 0; k < cols; ++k)
        //                                {
        //                                    if (orientations[i])
        //                                    {
        //                                        if (i == 0) // Plane XY... X = Y, Y = -X
        //                                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].Y, -SymVertices[constraintIndices[i][k]].X, SymVertices[constraintIndices[i][k]].Z);
        //                                        else if (i == 1)
        //                                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].Z, SymVertices[constraintIndices[i][k]].Y, -SymVertices[constraintIndices[i][k]].X);
        //                                        else if (i == 2)
        //                                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].X, -SymVertices[constraintIndices[i][k]].Z, SymVertices[constraintIndices[i][k]].Y);
        //                                        else
        //                                            currPos = SymVertices[constraintIndices[i][k]];
        //                                    }
        //                                    else
        //                                        currPos = SymVertices[constraintIndices[i][k]];

        //                                    vbSymbolArray->Position = j.Position + currPos;
        //                                    vbSymbolArray->Color = (j.IsSelected) ? jointSelectedColor : System.Drawing.Color.MediumAquamarine.ToArgb(); //jointSelectedColor;

        //                                    ++vbSymbolArray;
        //                                    ++lineVertex;
        //                                }
        //                            }
        //                        }

        //                        // Flush vertices to allow the GPU to start processing
        //                        if (lineVertex >= linesVBFlush)
        //                        {
        //                            device.SetStreamSource(0, linesVB, 0);
        //                            linesVB.Unlock();
        //                            device.DrawPrimitives(PrimitiveType.LineList, linesVBBase, lineVertex / 2);
        //                            linesVBBase += lineVertex;

        //                            // If the space allocated in memory is over, discard buffer, else append data
        //                            if (linesVBBase >= linesVBSize - 3 * linesVBFlush)
        //                                linesVBBase = 0;

        //                            // Re-lock the VertexBuffer and obtain the unsafe pointer from the new lock
        //                            linesVBData = linesVB.Lock(linesVBBase * vSize, linesVBFlush * vSize, (linesVBBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
        //                            vbSymbolArray = (CustomVertex.PositionColored*)linesVBData.InternalDataPointer;
        //                            lineVertex = 0;
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }
        //        }
        //    }   // Termina código NO SEGURO

        //    // Flush remaining vertices for joints
        //    device.SetStreamSource(0, theVB, 0);
        //    theVB.Unlock();
        //    if (vertsSize != 0)
        //    {
        //        device.DrawPrimitives(PrimitiveType.PointList, vbBase, vertsSize);

        //        vbBase += vertsSize;

        //        // If the space allocated in memory is over, discard buffer, else append data
        //        if (vbBase >= vbSize)
        //            vbBase = 0;
        //    }

        //    // Flush remaining vertices for lines (joint DOF's)
        //    #region Restraints
        //    device.SetStreamSource(0, linesVB, 0);
        //    linesVB.Unlock();
        //    if (lineVertex != 0)
        //    {
        //        device.DrawPrimitives(PrimitiveType.LineList, linesVBBase, lineVertex / 2);

        //        linesVBBase += lineVertex;

        //        // If the space allocated in memory is over, discard buffer, else append data
        //        if (linesVBBase >= linesVBSize - 3 * linesVBFlush)
        //            linesVBBase = 0;
        //    }
        //    #endregion

        //    rc.VbBase = vbBase;

        //    #region Restraints
        //    rc.LinesVbBase = linesVBBase;
        //    #endregion
        //}
        #endregion

        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<Canguro.Model.Joint> joints, RenderOptions options, bool dontDrawInvisibles, bool drawHidden)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

            #region Restraints
            // Added for symbol rendering
            //VertexBuffer linesVB = rc.LinesVB;

            //int linesVBBase = rc.LinesVbBase;
            //int linesVBFlush = rc.LinesVbFlush;
            //int linesVBSize = rc.LinesVbSize;
            //int lineVertex = 0;
            #endregion

            rc.ActiveStream = ResourceStreamType.Points;
            
            device.RenderState.Lighting = false;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();
            int vertsSize = 0;

            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Points, false, true);

            #region Restraints
            // Added for symbol rendering
            //device.SetStreamSource(0, linesVB, 0);
            //GraphicsStream linesVBData = linesVB.Lock(linesVBBase * vSize, linesVBFlush * vSize, (linesVBBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
            #endregion

            device.RenderState.PointSize = 5;
            Matrix rot = Matrix.Identity;

            #region Restraints
            // Added for symbol rendering
            //device.SetStreamSource(0, linesVB, 0);
            //CustomVertex.PositionColored* vbSymbolArray = (CustomVertex.PositionColored*)linesVBData.InternalDataPointer;
            #endregion

            int verts = 0;

            Vector3 currPos = Vector3.Empty;

            float deformedScaleFactor = 0f;
            if (model.HasResults)
                deformedScaleFactor = model.Results.PaintScaleFactorTranslation;

            foreach (Model.Joint j in joints)
            {
                if (j != null && (j.IsVisible || !dontDrawInvisibles))
                {
                    if ((options.OptionsShown & RenderOptions.ShowOptions.ShowJoints) > 0 || drawHidden)
                    {
                        #region Joint rendering
                        // Inicia código NO SEGURO
                        unsafe
                        {
                            if (options.ShowDeformed == true && model.Results.JointDisplacements != null)
                            {
                                displacedJoint.X = j.Position.X + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 0];
                                displacedJoint.Y = j.Position.Y + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 1];
                                displacedJoint.Z = j.Position.Z + options.DeformationScale * deformedScaleFactor * model.Results.JointDisplacements[j.Id, 2];

                                package.VBPointer->Position = displacedJoint;
                            }
                            else
                                package.VBPointer->Position = j.Position;

                            if (pickingMode)
                                package.VBPointer->Color = rc.GetNextPickIndex(j);
                            else if ((j.HassMass || j.DoF.AllDOF != 0) && !j.IsSelected)
                                package.VBPointer->Color = Canguro.Properties.Settings.Default.jointMassColour.ToArgb();
                            else
                                package.VBPointer->Color = (j.IsSelected) ? jointSelectedColor : jointColor;

                            package.VBPointer++;
                        } // Termina código NO SEGURO
                        vertsSize++;
                        ++verts;

                        // Flush vertices to allow the GPU to start processing
                        if (vertsSize >= package.NumVertices)
                        {
                            rc.ReleaseBuffer(vertsSize, 0, ResourceStreamType.Points);
                            package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Points, false, true);
                            vertsSize = 0;
                        }
                        #endregion

                        #region Joint text drawing
                        string jointText = string.Empty;

                        // Text rendering -> Joint IDs
                        if (((options.OptionsShown & RenderOptions.ShowOptions.JointIDs) != 0) && j.IsSelected)
                        {
                            jointText += "Id: " + j.Id.ToString() + "\n";
                            //DrawItemText(j.Id.ToString(), j.Position, Canguro.Properties.Settings.Default.TextColor);
                        }

                        // Text rendering -> Joint Coordinates
                        if ((options.OptionsShown & RenderOptions.ShowOptions.JointCoordinates) != 0 && j.IsSelected)
                        {
                            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                            float d1 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(j.Position.X, Canguro.Model.UnitSystem.Units.Distance);
                            float d2 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(j.Position.Y, Canguro.Model.UnitSystem.Units.Distance);
                            float d3 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(j.Position.Z, Canguro.Model.UnitSystem.Units.Distance);
                            string unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

                            jointText += "Coord: ( " + d1.ToString("G3") + ", " + d2.ToString("G3") + ", " + d3.ToString("G3") + ")" + "[" + unit + "]";

                            //DrawItemText("( " + d1.ToString("G3") + ", " + d2.ToString("G3") + ", " + d3.ToString("G3") + ")" + "[" + unit + "]", j.Position, Canguro.Properties.Settings.Default.TextColor);

                            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                        }

                        // Text rendering -> Joint Strains
                        if (((options.OptionsShown & RenderOptions.ShowOptions.Strains) != 0) && j.IsSelected)
                        {
                            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;

                            float d1 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 0], Canguro.Model.UnitSystem.Units.Distance);
                            float d2 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 1], Canguro.Model.UnitSystem.Units.Distance);
                            float d3 = Canguro.Model.Model.Instance.UnitSystem.FromInternational(model.Results.JointDisplacements[j.Id, 2], Canguro.Model.UnitSystem.Units.Distance);
                            string unit = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.UnitName(Canguro.Model.UnitSystem.Units.Distance);

                            jointText += "( " + d1.ToString("G5") + ", " + d2.ToString("G5") + ", " + d3.ToString("G5") + ")" + "[" + unit + "]";

                            //DrawItemText("( " + d1.ToString("G5") + ", " + d2.ToString("G5") + ", " + d3.ToString("G5") + ")" + "[" + unit + "]", j.Position, Canguro.Properties.Settings.Default.TextColor);

                            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                        }

                        if (jointText != string.Empty)
                            DrawItemText(jointText, j.Position, Canguro.Properties.Settings.Default.TextColor);
                        #endregion

                        #region Joint DOF Rendering
                        //if ((options.OptionsShown & RenderOptions.ShowOptions.JointDOFs) != 0)
                        //{
                        //    // Added for symbol drawing
                        //    bool[] orientations = null;
                        //    int[][] constraintIndices = getConstraints(j, ref orientations);
                        //    int rows, cols;

                        //    rows = constraintIndices.GetLength(0);

                        //    if (constraintIndices != null && orientations != null)
                        //    {
                        //        device.SetStreamSource(0, linesVB, 0);
                        //        for (int i = 0; i < rows; ++i)
                        //        {
                        //            if (constraintIndices[i] != null && constraintIndices[i].Length != 0)
                        //            {
                        //                cols = constraintIndices[i].Length;
                        //                for (int k = 0; k < cols; ++k)
                        //                {
                        //                    if (orientations[i])
                        //                    {
                        //                        if (i == 0) // Plane XY... X = Y, Y = -X
                        //                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].Y, -SymVertices[constraintIndices[i][k]].X, SymVertices[constraintIndices[i][k]].Z);
                        //                        else if (i == 1)
                        //                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].Z, SymVertices[constraintIndices[i][k]].Y, -SymVertices[constraintIndices[i][k]].X);
                        //                        else if (i == 2)
                        //                            currPos = new Vector3(SymVertices[constraintIndices[i][k]].X, -SymVertices[constraintIndices[i][k]].Z, SymVertices[constraintIndices[i][k]].Y);
                        //                        else
                        //                            currPos = SymVertices[constraintIndices[i][k]];
                        //                    }
                        //                    else
                        //                        currPos = SymVertices[constraintIndices[i][k]];

                        //                    vbSymbolArray->Position = j.Position + currPos;
                        //                    vbSymbolArray->Color = (j.IsSelected) ? jointSelectedColor : System.Drawing.Color.MediumAquamarine.ToArgb(); //jointSelectedColor;

                        //                    ++vbSymbolArray;
                        //                    ++lineVertex;
                        //                }
                        //            }
                        //        }

                        //        // Flush vertices to allow the GPU to start processing
                        //        if (lineVertex >= linesVBFlush)
                        //        {
                        //            device.SetStreamSource(0, linesVB, 0);
                        //            linesVB.Unlock();
                        //            device.DrawPrimitives(PrimitiveType.LineList, linesVBBase, lineVertex / 2);
                        //            linesVBBase += lineVertex;

                        //            // If the space allocated in memory is over, discard buffer, else append data
                        //            if (linesVBBase >= linesVBSize - 3 * linesVBFlush)
                        //                linesVBBase = 0;

                        //            // Re-lock the VertexBuffer and obtain the unsafe pointer from the new lock
                        //            linesVBData = linesVB.Lock(linesVBBase * vSize, linesVBFlush * vSize, (linesVBBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                        //            vbSymbolArray = (CustomVertex.PositionColored*)linesVBData.InternalDataPointer;
                        //            lineVertex = 0;
                        //        }
                        //    }
                        //}
                        #endregion
                    }

                    if ((options.OptionsShown & RenderOptions.ShowOptions.JointDOFs) != 0)
                        rc.GadgetManager.GadgetList.AddLast(new Gadget(j, GadgetType.Restraint));
                }
            }

            rc.ReleaseBuffer(vertsSize, 0, ResourceStreamType.Points);
            rc.Flush(ResourceStreamType.Points);

            // Flush remaining vertices for lines (joint DOF's)
            #region Restraints
            //device.SetStreamSource(0, linesVB, 0);
            //linesVB.Unlock();
            //if (lineVertex != 0)
            //{
            //    device.DrawPrimitives(PrimitiveType.LineList, linesVBBase, lineVertex / 2);

            //    linesVBBase += lineVertex;

            //    // If the space allocated in memory is over, discard buffer, else append data
            //    if (linesVBBase >= linesVBSize - 3 * linesVBFlush)
            //        linesVBBase = 0;
            //}

            //rc.LinesVbBase = linesVBBase;
            #endregion
        }
    }
}
