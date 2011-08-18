using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.View.Renderer
{
    public class ShadedLineRenderer : LineRenderer
    {
        private WireframeLineRenderer wireframeLineRenderer;

        List<LineElement> gouraudLines;
        List<LineElement> wireframeLines;

        public ShadedLineRenderer(WireframeLineRenderer wlr)
        {
            wireframeLineRenderer = wlr;

            gouraudLines = new List<LineElement>();
            wireframeLines = new List<LineElement>();

            #region Legacy
            //// Steel
            //sectionMaterials[0].Diffuse = System.Drawing.Color.SteelBlue;
            //sectionMaterials[0].Ambient = System.Drawing.Color.SteelBlue;

            //// Steel selected
            //sectionMaterials[1].Diffuse = System.Drawing.Color.SteelBlue;
            //sectionMaterials[1].Ambient = System.Drawing.Color.SteelBlue;
            //sectionMaterials[1].Emissive = System.Drawing.Color.Gray;

            //// Other materials
            //sectionMaterials[2].Diffuse = System.Drawing.Color.Gold;
            //sectionMaterials[2].Ambient = System.Drawing.Color.Gold;

            //// Other materials selected
            //sectionMaterials[3].Diffuse = System.Drawing.Color.Gold;
            //sectionMaterials[3].Ambient = System.Drawing.Color.Gold;
            //sectionMaterials[3].Emissive = System.Drawing.Color.Gray;
            #endregion
        }

        private void renderLine(Device device, ResourceManager rc, LineElement l, bool pickingMode, RenderOptions options)
        {
            if (l != null && l.IsVisible)
            {
                drawReleaseIfNeeded(rc, l, options);

                if (l.Properties is StraightFrameProps)
                {
                    FrameSection sec = ((StraightFrameProps)l.Properties).Section;
                    if (options.LOD.GetLOD(l).LODContour == LODContour.Wireframe && !GraphicViewManager.Instance.PrintingHiResImage)
                        wireframeLines.Add(l);
                    else
                    {
                        if (sec.FaceNormals)
                            drawLine(device, rc, l, sec, pickingMode, options);
                        else
                            gouraudLines.Add(l);     // If using vertex normals, enqueue it for later processing
                    }
                }
            }
        }

        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            Catalog<Section> sections = Canguro.Model.Model.Instance.Sections;
            lastMaterialColor = 0;

            gouraudLines.Clear();
            wireframeLines.Clear();

            rc.ActiveStream = ResourceStreamType.TriangleListPositionNormalColored;

            System.Drawing.Color ambientColor = device.RenderState.Ambient;

            if (!device.RenderState.Lighting)
                device.RenderState.Lighting = true;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();

            if (pickingMode)
            {
                if(device.RenderState.Ambient != System.Drawing.Color.White)
                    device.RenderState.Ambient = System.Drawing.Color.White;
                if(device.Lights[0].Enabled == true)
                    device.Lights[0].Enabled = false;
            }

            if(device.RenderState.CullMode != Cull.Clockwise)
                device.RenderState.CullMode = Cull.Clockwise;

            if(device.RenderState.ShadeMode != ShadeMode.Flat)
                device.RenderState.ShadeMode = ShadeMode.Flat;

            if(device.RenderState.ColorVertex == true)
                device.RenderState.ColorVertex = false;

            if (lines != null && ((IList<LineElement>)lines).Count > 0)
            {
                foreach (LineElement l in lines)
                    renderLine(device, rc, l, pickingMode, options);
            }
            else if (itemsInView != null)
            {
                // Get list of items in view (Bounding Box)
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    // Draw lines using Face Normals
                    LineElement l;
                    foreach (Item item in itemsInView)
                    {
                        l = item as LineElement;
                        renderLine(device, rc, l, pickingMode, options);
                    }
                }
            }

            if (gouraudLines.Count > 0)
            {
                // Draw lines using vertex normals
                device.RenderState.ShadeMode = ShadeMode.Gouraud;

                foreach (LineElement lg in gouraudLines)
                {
                    FrameSection sec = ((StraightFrameProps)lg.Properties).Section;
                    drawLine(device, rc, lg, sec, pickingMode, options);
                }
            }

            #region Legacy Code
            //VertexBuffer theVB = rc.SectionsVB;
            //int vbBase = rc.SectionsVbBase;
            //int vbFlush = rc.SectionsVbFlush;

            //device.VertexFormat = CustomVertex.PositionNormal.Format;
            //device.SetStreamSource(0, theVB, 0);

            //System.Drawing.Color ambientColor = device.RenderState.Ambient;
            //Material pickMtrl = new Material();

            //if (!device.RenderState.Lighting) device.RenderState.Lighting = true;

            //bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            //int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            //int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            //int vSize = CustomVertex.PositionNormal.StrideSize;
            //GraphicsStream vbData;
            //int vertsSize = 0;
            //Matrix rotMatrix;
            //int lastMaterial = -1;

            //if (pickingMode)
            //{
            //    device.RenderState.Ambient = System.Drawing.Color.White;
            //    device.Lights[0].Enabled = false;
            //}

            //Dictionary<Section, SectionLocator> sectionDictionary = new Dictionary<Section, SectionLocator>();

            //device.RenderState.CullMode = Cull.Clockwise;
            //device.RenderState.ShadeMode = ShadeMode.Gouraud;

            //// Inicia código NO SEGURO
            //unsafe
            //{
            //    CustomVertex.PositionNormal* vbArray;

            //    foreach (LineElement l in lines)
            //    {
            //        if (l != null && l.IsVisible)
            //        {
            //            if (l.Properties is StraightFrameProps)
            //            {
            //                FrameSection sec = ((StraightFrameProps)l.Properties).Section;

            //                if (!sectionDictionary.ContainsKey(sec))
            //                {
            //                    // Lock the VertexBuffer and obtain the unsafe pointer from the new lock
            //                    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
            //                    vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
            //                    vertsSize = 0;

            //                    // Get triangulated section indices
            //                    int[] sectionIndices = sec.TriangSectionIndices;
            //                    int nIndices = sectionIndices.Length;

            //                    #region Draw initial triangulated section
            //                    for (int i = 0; i < nIndices; ++i)
            //                    {
            //                        vbArray->Position = new Vector3(0, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
            //                        vbArray->Normal = new Vector3(-1, 0, 0);
            //                        vbArray++;
            //                        vertsSize++;
            //                    }
            //                    #endregion

            //                    #region Draw beam
            //                    int contourLen = sec.Contour[0].Length;
            //                    for (int i = 0; i < contourLen; i++)
            //                    {
            //                        // First triangle
            //                        vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
            //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                        vbArray++;
            //                        vertsSize++;

            //                        vbArray->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
            //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                        vbArray++;
            //                        vertsSize++;

            //                        if (i + 1 < contourLen)
            //                        {
            //                            vbArray->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
            //                        }
            //                        else
            //                        {
            //                            vbArray->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
            //                        }
            //                        vbArray++;
            //                        vertsSize++;

            //                        // Second triangle
            //                        vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
            //                        vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                        vbArray++;
            //                        vertsSize++;

            //                        if (i + 1 < contourLen)
            //                        {
            //                            vbArray->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
            //                            vbArray++;
            //                            vertsSize++;

            //                            vbArray->Position = new Vector3(0, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][i + 1].Y, sec.Contour[1][i + 1].X);
            //                            vbArray++;
            //                            vertsSize++;
            //                        }
            //                        else
            //                        {
            //                            vbArray->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
            //                            vbArray++;
            //                            vertsSize++;

            //                            vbArray->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
            //                            if (sec.FaceNormals)
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
            //                            else
            //                                vbArray->Normal = new Vector3(0, sec.Contour[1][0].Y, sec.Contour[1][0].X);
            //                            vbArray++;
            //                            vertsSize++;
            //                        }
            //                    }
            //                    #endregion

            //                    #region Draw end triangulated section
            //                    for (int i = nIndices - 1; i >= 0; --i)
            //                    {
            //                        vbArray->Position = new Vector3(1, sec.Contour[0][sectionIndices[i]].Y, sec.Contour[0][sectionIndices[i]].X);
            //                        vbArray->Normal = new Vector3(1, 0, 0);
            //                        vbArray++;
            //                        vertsSize++;
            //                    }
            //                    #endregion

            //                    theVB.Unlock();

            //                    SectionLocator locator;
            //                    locator.offset = vbBase;
            //                    locator.size = vertsSize;
            //                    sectionDictionary.Add(sec, locator);

            //                    vbBase += vertsSize;
            //                }

            //                l.RotationMatrix(out rotMatrix);

            //                // rotMatrix * Matrix.Translation(l.I.Position);
            //                rotMatrix.M41 = l.I.Position.X;
            //                rotMatrix.M42 = l.I.Position.Y;
            //                rotMatrix.M43 = l.I.Position.Z;

            //                // Matrix.Scaling(len, 1, 1) * rotMatrix;
            //                float len = l.LengthInt;
            //                rotMatrix.M11 *= len;
            //                rotMatrix.M12 *= len;
            //                rotMatrix.M13 *= len;

            //                device.Transform.World = rotMatrix;

            //                // Select material
            //                if (pickingMode)
            //                {
            //                    pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
            //                    pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
            //                    device.Material = pickMtrl;
            //                }
            //                else
            //                {
            //                    if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
            //                    {
            //                        if (l.IsSelected)
            //                        {
            //                            if (lastMaterial != 1) device.Material = sectionMaterials[1];
            //                            lastMaterial = 1;
            //                        }
            //                        else
            //                        {
            //                            if (lastMaterial != 0) device.Material = sectionMaterials[0];
            //                            lastMaterial = 0;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (l.IsSelected)
            //                        {
            //                            if (lastMaterial != 3) device.Material = sectionMaterials[3];
            //                            lastMaterial = 3;
            //                        }
            //                        else
            //                        {
            //                            if (lastMaterial != 2) device.Material = sectionMaterials[2];
            //                            lastMaterial = 2;
            //                        }
            //                    }
            //                }

            //                device.DrawPrimitives(PrimitiveType.TriangleList, sectionDictionary[sec].offset, sectionDictionary[sec].size / 3);
            //            }
            //        }
            //    }
            //}   // Termina código NO SEGURO
            #endregion

            device.Transform.World = Matrix.Identity;

            // Draw wireframe Lines
            if (wireframeLines != null && wireframeLines.Count > 0)
            {
                // Set lighting state
                device.RenderState.Lighting = false;
                wireframeLineRenderer.Render(device, model, wireframeLines, options, itemsInView);
            }

            if (pickingMode)
            {
                device.RenderState.Ambient = ambientColor;
                device.Lights[0].Enabled = true;
            }
        }

        private void drawLine(Device device, ResourceManager rc, LineElement l, FrameSection sec, bool pickingMode, RenderOptions options)
        {
            #region Set Rotation Matrix
            Matrix rotMatrix;
            Vector3 actualPos;
            Vector3[] localAxes;
            Vector2 offsetForCardinalPoint = Vector2.Empty;

            sec.GetOffsetForCardinalPoint(l.CardinalPoint, ref offsetForCardinalPoint);

            l.RotationMatrix(out rotMatrix);
            localAxes = l.LocalAxes;

            // rotMatrix * Matrix.Translation(l.I.Position);
            actualPos = l.I.Position + l.EndOffsets.EndIInternational * localAxes[0] - offsetForCardinalPoint.Y * localAxes[1] - offsetForCardinalPoint.X * localAxes[2];

            rotMatrix.M41 = actualPos.X;
            rotMatrix.M42 = actualPos.Y;
            rotMatrix.M43 = actualPos.Z;

            // Matrix.Scaling(len, 1, 1) * rotMatrix;
            float len = l.LengthInt - l.EndOffsets.EndIInternational - l.EndOffsets.EndJInternational;
            rotMatrix.M11 *= len;
            rotMatrix.M12 *= len;
            rotMatrix.M13 *= len;

            device.Transform.World = rotMatrix;
            #endregion

            setLineColor(device, rc, l, sec, pickingMode, options.LineColoredBy);

            rc.GadgetManager.LineGadgets.DrawCanonicalSection(device, sec, options.LOD.GetLOD(l).LODContour);
        }

        /*
        public override void Render(Device device, System.Collections.Generic.IEnumerable<LineElement> lines)
        {
            ResourceCache rc = GraphicViewManager.Instance.ResourceCache;
            Catalog<Section> sections = Model.Model.Instance.Sections;

            int vbBase = 0;
            int vbFlush = 120;
            int vbSize = 60000;

            int vbIndex = rc.AddVB(typeof(CustomVertex.PositionNormal), vbSize + vbFlush,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormal.Format, Pool.Default);

            VertexBuffer theVB = rc[vbIndex];

            if (theVB == null) return;

            device.VertexFormat = CustomVertex.PositionNormal.Format;
            device.SetStreamSource(0, theVB, 0);
            Material mtrl = new Material();
            System.Drawing.Color col = System.Drawing.Color.White;
            mtrl.Diffuse = col;
            mtrl.Ambient = col;
            device.Material = mtrl;
            GraphicViewManager.Instance.DeviceLighting = true;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            int vSize = CustomVertex.PositionNormal.StrideSize;
            GraphicsStream vbData;
            int vertsSize = 0;
            int lineColor;
            Matrix rotMatrix;
            //int sectionMaxVertices = 120;

            Dictionary<Section, intint> sectionDictionary = new Dictionary<Section, intint>();

            // Inicia código NO SEGURO
            unsafe
            {
                CustomVertex.PositionNormal* vbArray;

                foreach (LineElement l in lines)
                {
                    if (l != null && l.IsVisible)
                    {
                        if (l.Properties is StraightFrameProps)
                        {
                            FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                            //if (pickingMode)
                            //    lineColor = rc.GetNextPickIndex(l);
                            //else
                            //    lineColor = (l.IsSelected) ? jointSelectedColor : jointColor;
                            
                            if (!sectionDictionary.ContainsKey(sec))
                            {
                                // Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
                                vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                                vertsSize = 0;
                                Vector3 currentNormal;

                                int contourLen = sec.Contour[0].Length;
                                for (int i = 0; i < contourLen-1; i++)
                                {
                                    currentNormal = new Vector3(0, sec.Contour[1][i].Y, sec.Contour[1][i].X);
                                    
                                    vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                                    //vbArray->Color = lineColor;                                   
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;

                                    vbArray->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                                    //vbArray->Color = lineColor;
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;

                                    vbArray->Position = new Vector3(0, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
                                    //vbArray->Color = lineColor;
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;

                                    vbArray->Position = new Vector3(0, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
                                    //vbArray->Color = lineColor;
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;

                                    vbArray->Position = new Vector3(1, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                                    //vbArray->Color = lineColor;
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;

                                    vbArray->Position = new Vector3(1, sec.Contour[0][i + 1].Y, sec.Contour[0][i + 1].X);
                                    //vbArray->Color = lineColor;
                                    vbArray->Normal = currentNormal;
                                    vbArray++;
                                    vertsSize++;
                                }

                                currentNormal = new Vector3(0, sec.Contour[1][contourLen - 1].Y, sec.Contour[1][contourLen - 1].X);
                                
                                vbArray->Position = new Vector3(0, sec.Contour[0][contourLen - 1].Y, sec.Contour[0][contourLen - 1].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = new Vector3(1, sec.Contour[0][contourLen - 1].Y, sec.Contour[0][contourLen - 1].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = new Vector3(1, sec.Contour[0][contourLen - 1].Y, sec.Contour[0][contourLen - 1].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = new Vector3(1, sec.Contour[0][0].Y, sec.Contour[0][0].X);
                                //vbArray->Color = lineColor;
                                vbArray->Normal = currentNormal;
                                vbArray++;
                                vertsSize++;

                                theVB.Unlock();

                                intint secintint;
                                secintint.offset = vbBase;
                                secintint.size = vertsSize - 2;
                                sectionDictionary.Add(sec, secintint);

                                vbBase += vertsSize;
                            }

                            l.RotationMatrix(out rotMatrix);

                            // rotMatrix * Matrix.Translation(l.I.Position);
                            rotMatrix.M41 = l.I.Position.X;
                            rotMatrix.M42 = l.I.Position.Y;
                            rotMatrix.M43 = l.I.Position.Z;

                            // Matrix.Scaling(len, len, len) * rotMatrix;
                            float len = l.Length;
                            rotMatrix.M11 *= len;
                            rotMatrix.M12 *= len;
                            rotMatrix.M13 *= len;
                            rotMatrix.M21 *= len;
                            rotMatrix.M22 *= len;
                            rotMatrix.M23 *= len;
                            rotMatrix.M31 *= len;
                            rotMatrix.M32 *= len;
                            rotMatrix.M33 *= len;

                            device.Transform.World = rotMatrix;

                            device.DrawPrimitives(PrimitiveType.TriangleList, sectionDictionary[sec].offset, sectionDictionary[sec].size);
                        }
                    }
                }
            }   // Termina código NO SEGURO

            rc.DelVB(vbIndex);
            device.Transform.World = Matrix.Identity;
        }
        */

        /*
        public override void Render(Device device, System.Collections.Generic.IEnumerable<LineElement> lines)
        {
            ResourceCache rc = GraphicViewManager.Instance.ResourceCache;
            Catalog<Section> sections = Model.Model.Instance.Sections;

            int vbBase = 0;
            int vbFlush = 50;
            //int mainVbFlush = 100;
            int vbSize = 20000;

            int vbIndex = rc.AddVB(typeof(CustomVertex.PositionNormal), vbSize + vbFlush,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormal.Format, Pool.Default);

            VertexBuffer theVB = rc[vbIndex];
            //VertexBuffer theVB = rc.MainVB;

            if (theVB == null) return;

            device.VertexFormat = CustomVertex.PositionNormal.Format;
            device.SetStreamSource(0, theVB, 0);

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            int vSize = CustomVertex.PositionNormal.StrideSize;
            //GraphicsStream vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
            GraphicsStream vbData;
            int vertsSize = 0;
            int lineColor;
            Matrix rotMatrix;
            int sectionMaxVertices = 120;

            // Inicia código NO SEGURO
            unsafe
            {
                //CustomVertex.PositionNormal* vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                CustomVertex.PositionNormal* vbArray;

                foreach (LineElement l in lines)
                {
                    if (l != null && l.IsVisible)
                    {
                        if (l.Properties is StraightFrameProps)
                        {
                            FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                            l.RotationMatrix(out rotMatrix);
                            rotMatrix.M41 = l.I.Position.X;
                            rotMatrix.M42 = l.I.Position.Y;
                            rotMatrix.M43 = l.I.Position.Z;
                            device.Transform.World = rotMatrix;

                            // Re-Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                            vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                            vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                            vertsSize = 0;

                            Vector3[] localAxes = l.LocalAxes;
                            
                            float len = l.Length;

                            if (pickingMode)
                                lineColor = rc.GetNextPickIndex(l);
                            else
                                lineColor = (l.IsSelected) ? jointSelectedColor : jointColor;

                            int contourLen = sec.Contour[0].Length;
                            for (int i = 0; i < contourLen; i++)
                            {
                                //vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][i].Y + localAxes[2] * sec.Contour[0][i].X;
                                vbArray->Position = new Vector3(0, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i].Y + localAxes[2] * sec.Contour[1][i].X;
                                vbArray++;
                                vertsSize++;

                                //vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][i].Y + localAxes[2] * sec.Contour[0][i].X;
                                vbArray->Position = new Vector3(len, sec.Contour[0][i].Y, sec.Contour[0][i].X);
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i].Y + localAxes[2] * sec.Contour[1][i].X;
                                vbArray++;
                                vertsSize++;
                            }

                            //vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][0].Y + localAxes[2] * sec.Contour[0][0].X;
                            vbArray->Position = new Vector3(0, sec.Contour[0][0].Y, sec.Contour[0][0].X);
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][0].Y + localAxes[2] * sec.Contour[1][0].X;
                            vbArray++;
                            vertsSize++;

                            //vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][0].Y + localAxes[2] * sec.Contour[0][0].X;
                            vbArray->Position = new Vector3(len, sec.Contour[0][0].Y, sec.Contour[0][0].X);
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][0].Y + localAxes[2] * sec.Contour[1][0].X;
                            vbArray++;
                            vertsSize++;

                            // Flush vertices to allow the GPU to start processing
                            //if (vertsSize >= vbFlush - sectionMaxVertices)
                            {
                                theVB.Unlock();
                                device.DrawPrimitives(PrimitiveType.TriangleStrip, vbBase, vertsSize-2);
                                //mainVbBase += mainVbFlush;
                                vbBase += vertsSize;

                                // If the space allocated in memory is over, discard buffer, else append data
                                if (vbBase >= vbSize)
                                    vbBase = 0;
                            }
                        }
                    }
                }
            }   // Termina código NO SEGURO

            // Flush remaining vertices
            //theVB.Unlock();
            if (vertsSize != 0)
                device.DrawPrimitives(PrimitiveType.TriangleStrip, vbBase, vertsSize - 2);

            rc.DelVB(vbIndex);
            //rc.VbBase = vbBase;
            //device.Transform.World = Matrix.Identity;
        }
        */

        /*
        public override void Render(Device device, System.Collections.Generic.IEnumerable<LineElement> lines)
        {
            ResourceCache rc = GraphicViewManager.Instance.ResourceCache;
            Catalog<Section> sections = Model.Model.Instance.Sections;

            int vbBase = 0;
            int vbFlush = 5000;
            //int mainVbFlush = 100;
            int vbSize = 20000;

            int vbIndex = rc.AddVB(typeof(CustomVertex.PositionNormal), vbSize + vbFlush,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormal.Format, Pool.Default);

            VertexBuffer theVB = rc[vbIndex];
            //VertexBuffer theVB = rc.MainVB;

            if (theVB == null) return;

            device.VertexFormat = CustomVertex.PositionNormal.Format;
            device.SetStreamSource(0, theVB, 0);

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            int vSize = CustomVertex.PositionNormal.StrideSize;
            GraphicsStream vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
            //GraphicsStream vbData;
            int vertsSize = 0;
            int lineColor;
            //Matrix rotMatrix;
            int sectionMaxVertices = 120;

            // Inicia código NO SEGURO
            unsafe
            {
                CustomVertex.PositionNormal* vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                //CustomVertex.PositionColored* vbArray;

                foreach (LineElement l in lines)
                {
                    if (l != null && l.IsVisible)
                    {
                        if (l.Properties is StraightFrameProps)
                        {
                            FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                            Vector3[] localAxes = l.LocalAxes;

                            float len = l.Length;

                            if (pickingMode)
                                lineColor = rc.GetNextPickIndex(l);
                            else
                                lineColor = (l.IsSelected) ? jointSelectedColor : jointColor;

                            int contourLen = sec.Contour[0].Length;
                            for (int i = 0; i < contourLen - 1; i++)
                            {
                                vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][i].Y + localAxes[2] * sec.Contour[0][i].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i].Y + localAxes[2] * sec.Contour[1][i].X;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][i].Y + localAxes[2] * sec.Contour[0][i].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i].Y + localAxes[2] * sec.Contour[1][i].X;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][i + 1].Y + localAxes[2] * sec.Contour[0][i + 1].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i + 1].Y + localAxes[2] * sec.Contour[1][i + 1].X;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][i + 1].Y + localAxes[2] * sec.Contour[0][i + 1].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i + 1].Y + localAxes[2] * sec.Contour[1][i + 1].X;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][i].Y + localAxes[2] * sec.Contour[0][i].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i].Y + localAxes[2] * sec.Contour[1][i].X;
                                vbArray++;
                                vertsSize++;

                                vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][i + 1].Y + localAxes[2] * sec.Contour[0][i + 1].X;
                                vbArray->Color = lineColor;
                                vbArray->Normal = localAxes[1] * sec.Contour[1][i + 1].Y + localAxes[2] * sec.Contour[1][i + 1].X;
                                vbArray++;
                                vertsSize++;
                            }

                            vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][contourLen - 1].Y + localAxes[2] * sec.Contour[0][contourLen - 1].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][contourLen - 1].Y + localAxes[2] * sec.Contour[1][contourLen - 1].X;
                            vbArray++;
                            vertsSize++;

                            vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][contourLen - 1].Y + localAxes[2] * sec.Contour[0][contourLen - 1].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][contourLen - 1].Y + localAxes[2] * sec.Contour[1][contourLen - 1].X;
                            vbArray++;
                            vertsSize++;

                            vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][0].Y + localAxes[2] * sec.Contour[0][0].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][0].Y + localAxes[2] * sec.Contour[1][0].X;
                            vbArray++;
                            vertsSize++;

                            vbArray->Position = l.I.Position + localAxes[1] * sec.Contour[0][0].Y + localAxes[2] * sec.Contour[0][0].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][0].Y + localAxes[2] * sec.Contour[1][0].X;
                            vbArray++;
                            vertsSize++;

                            vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][contourLen - 1].Y + localAxes[2] * sec.Contour[0][contourLen - 1].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][contourLen - 1].Y + localAxes[2] * sec.Contour[1][contourLen - 1].X;
                            vbArray++;
                            vertsSize++;

                            vbArray->Position = l.J.Position + localAxes[1] * sec.Contour[0][0].Y + localAxes[2] * sec.Contour[0][0].X;
                            vbArray->Color = lineColor;
                            vbArray->Normal = localAxes[1] * sec.Contour[1][0].Y + localAxes[2] * sec.Contour[1][0].X;
                            vbArray++;
                            vertsSize++;

                            // Flush vertices to allow the GPU to start processing
                            if (vertsSize >= vbFlush - sectionMaxVertices)
                            {
                                theVB.Unlock();
                                device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, vertsSize / 3);
                                //mainVbBase += mainVbFlush;
                                vbBase += vertsSize;

                                // If the space allocated in memory is over, discard buffer, else append data
                                if (vbBase >= vbSize)
                                    vbBase = 0;

                                // Re-Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                                vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                                vertsSize = 0;
                            }
                        }
                    }
                }
            }   // Termina código NO SEGURO

            // Flush remaining vertices
            theVB.Unlock();
            if (vertsSize != 0)
                device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, vertsSize / 3);

            rc.DelVB(vbIndex);
            //rc.VbBase = vbBase;
            //device.Transform.World = Matrix.Identity;
        }
        */
    }
}
