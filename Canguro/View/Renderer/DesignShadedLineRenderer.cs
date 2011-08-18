using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Section;

using Canguro.Utility;

namespace Canguro.View.Renderer
{
    public class DesignShadedLineRenderer : ShadedLineRenderer
    {
        Material designMaterial = new Material();

        public DesignShadedLineRenderer(WireframeLineRenderer wlr):base(wlr)
        {
        }

        #region Legacy
        // Last Tested
        //public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        //{
            //ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            //Catalog<Section> sections = Canguro.Model.Model.Instance.Sections;

            //VertexBuffer theVB = rc.SectionsVB;
            //int vbBase = rc.SectionsVbBase;
            //int vbFlush = rc.SectionsVbFlush;

            //device.VertexFormat = CustomVertex.PositionNormal.Format;
            //device.SetStreamSource(0, theVB, 0);

            //System.Drawing.Color ambientColor = device.RenderState.Ambient;
            //Material pickMtrl = new Material();
            //Material designMaterial = new Material();

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
            //                        vbArray->Normal = -l.LocalAxes[0];
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
            //                        vbArray->Normal = l.LocalAxes[0];
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
            //                    if (lastMaterial != 1) device.Material = sectionMaterials[1];
            //                    if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
            //                    {
            //                        // Steel Design
            //                        float ratio;
            //                        if (string.IsNullOrEmpty(model.Results.DesignSteelSummary[l.Id].ErrMsg))
            //                            ratio = model.Results.DesignSteelSummary[l.Id].Ratio;
            //                        else
            //                            ratio = 1f;

            //                        designMaterial.Diffuse = System.Drawing.Color.FromArgb(getColorFromDesignRatio(ratio));
            //                        device.Material = designMaterial;
            //                    }
            //                    else
            //                    {
            //                        // Concrete Design
            //                        float ratio = 1f;
            //                        StraightFrameProps props = l.Properties as StraightFrameProps;
            //                        if (props != null)
            //                        {
            //                            if (props.Section.ConcreteProperties is ConcreteBeamSectionProps)
            //                            {
            //                                if (string.IsNullOrEmpty(model.Results.DesignConcreteBeam[l.Id].ErrMsg))
            //                                    ratio = 0.5f;
            //                                else
            //                                    ratio = 1f;
            //                            }
            //                            else if (props.Section.ConcreteProperties is ConcreteColumnSectionProps)
            //                            {
            //                                if (string.IsNullOrEmpty(model.Results.DesignConcreteColumn[l.Id].ErrMsg))
            //                                {
            //                                    float rebarArea = 0f;
            //                                    ConcreteColumnSectionProps cProps = (ConcreteColumnSectionProps)props.Section.ConcreteProperties;
            //                                    rebarArea = cProps.NumberOfBars * BarSizes.Instance.GetArea(cProps.BarSize);
            //                                    ratio = 1.5f - rebarArea / model.Results.DesignConcreteColumn[l.Id].PMMArea;
            //                                    if (ratio > 1f) ratio = 0.99f;
            //                                }
            //                                else
            //                                    ratio = 1f;
            //                            }
            //                        }
            //                        designMaterial.Diffuse = System.Drawing.Color.FromArgb(getColorFromDesignRatio(ratio));
            //                        device.Material = designMaterial;
            //                    }
            //                }

            //                device.DrawPrimitives(PrimitiveType.TriangleList, sectionDictionary[sec].offset, sectionDictionary[sec].size / 3);
            //            }
            //        }
            //    }
            //}   // Termina código NO SEGURO

            //device.Transform.World = Matrix.Identity;

            //device.RenderState.ShadeMode = ShadeMode.Flat;

            //if (pickingMode)
            //{
            //    device.RenderState.Ambient = ambientColor;
            //    device.Lights[0].Enabled = true;
            //}
        //}
        #endregion

        protected override void setLineColor(Device device, ResourceManager rc, LineElement l, FrameSection sec, bool pickingMode)
        {
            // Select material
            if (pickingMode)
            {
                pickMaterial.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                pickMaterial.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                device.Material = pickMaterial;
            }
            else
            {
                if (lastMaterial != 1) device.Material = sectionMaterials[1];
                if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                {
                    // Steel Design
                    float ratio;
                    if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].ErrMsg))
                        ratio = Canguro.Model.Model.Instance.Results.DesignSteelSummary[l.Id].Ratio;
                    else
                        ratio = 1f;

                    designMaterial.Diffuse = System.Drawing.Color.FromArgb(ColorUtils.GetColorFromDesignRatio(ratio));
                    device.Material = designMaterial;
                }
                else
                {
                    // Concrete Design
                    float ratio = 1f;
                    StraightFrameProps props = l.Properties as StraightFrameProps;
                    if (props != null)
                    {
                        if (props.Section.ConcreteProperties is ConcreteBeamSectionProps)
                        {
                            if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignConcreteBeam[l.Id].ErrMsg))
                                ratio = 0.5f;
                            else
                                ratio = 1f;
                        }
                        else if (props.Section.ConcreteProperties is ConcreteColumnSectionProps)
                        {
                            if (string.IsNullOrEmpty(Canguro.Model.Model.Instance.Results.DesignConcreteColumn[l.Id].ErrMsg))
                            {
                                float rebarArea = 0f;
                                ConcreteColumnSectionProps cProps = (ConcreteColumnSectionProps)props.Section.ConcreteProperties;
                                rebarArea = cProps.NumberOfBars * BarSizes.Instance.GetArea(cProps.BarSize);
                                ratio = 1.5f - rebarArea / Canguro.Model.Model.Instance.Results.DesignConcreteColumn[l.Id].PMMArea;
                                if (ratio > 1f) ratio = 0.99f;
                            }
                            else
                                ratio = 1f;
                        }
                    }
                    designMaterial.Diffuse = System.Drawing.Color.FromArgb(ColorUtils.GetColorFromDesignRatio(ratio));
                    device.Material = designMaterial;
                }
            }
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
