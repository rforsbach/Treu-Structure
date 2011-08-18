using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Model;
using Canguro.Model.Section;

using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public class DeformedLineShadedRenderer : LineRenderer
    {
        private float[,] jointDisplacements;
        private float paintScaleFactorTranslation;
        private Canguro.Model.Load.AbstractCase abstractCase;
        private LineDeformationCalculator calc = null;

        private Vector3[,] extrudedMesh = null;
        private Vector3[] curvedAxis = null;

        private Material pickMtrl;

        private short[] meshIB = null;

        List<LineElement> gouraudLines;
        List<LineElement> wireframeLines;

        private DeformedLineWireframeRenderer deformedLineWireframeRenderer = null;

        public DeformedLineShadedRenderer(DeformedLineWireframeRenderer dlwr)
        {
            deformedLineWireframeRenderer = dlwr;

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

        protected virtual int GetColor(Canguro.Model.Results.ResultsCase rc, LineElement line, int index, LODLevels lineLODLevel, StressHelper srh)
        {
            return 0;
        }

        private void drawDeformedShadedLine(ResourceManager rc, Device device, Model.Model model, LineElement l, LODLevels lineLODLevel, FrameSection sec, RenderOptions options, bool pickingMode,
            ref PositionNormalColoredPackage package, ref int verticesInVB, ref int indicesInIB, ref int startOffset)
        {
            short[] activeContourIndices = sec.ContourIndices[(int)lineLODLevel.LODContour];
            short[] cover = null;

            int i;
            int presentColor;

            int numPoints = (int)lineLODLevel.LODSegments + 1;
            int requiredMeshVertices = 0, requiredMeshIndices = 0;
            int requiredCoverVertices = 0, requiredCoverIndices = 0;

            #region Get joint deformations and curve vectors
            // Get joint defomations
            Vector3 vI = new Vector3(jointDisplacements[l.I.Id, 0], jointDisplacements[l.I.Id, 1], jointDisplacements[l.I.Id, 2]);
            Vector3 vJ = new Vector3(jointDisplacements[l.J.Id, 0], jointDisplacements[l.J.Id, 1], jointDisplacements[l.J.Id, 2]);

            Vector3 lineDir = l.J.Position - l.I.Position;
            lineDir.Normalize();

            vI = options.DeformationScale * paintScaleFactorTranslation * vI + l.I.Position + l.EndOffsets.EndIInternational * lineDir;
            vJ = options.DeformationScale * paintScaleFactorTranslation * vJ + l.J.Position - l.EndOffsets.EndJInternational * lineDir;
            #endregion

            #region Get deformed curve vector
            // Get deformed line 'direction'
            Vector3 newDir = vJ - vI;

            float[,] local2Values = calc.GetCurvedAxis(l, abstractCase, LineDeformationCalculator.DeformationAxis.Local2, numPoints);
            float[,] local3Values = calc.GetCurvedAxis(l, abstractCase, LineDeformationCalculator.DeformationAxis.Local3, numPoints);

            requiredMeshVertices = local2Values.GetLength(0);

            curvedAxis = new Vector3[requiredMeshVertices];

            // Get axis
            for (i = 0; i < requiredMeshVertices; ++i)
            {
                curvedAxis[i] = vI + local2Values[i, 0] * newDir +
                                     local2Values[i, 1] * options.DeformationScale * paintScaleFactorTranslation * l.LocalAxes[1] +
                                     local3Values[i, 1] * options.DeformationScale * paintScaleFactorTranslation * l.LocalAxes[2];

            }
            #endregion

            Vector2 cardinalPointOffset = Vector2.Empty;
            sec.GetOffsetForCardinalPoint(l.CardinalPoint, ref cardinalPointOffset);

            // Build mesh
            ExtrudedShape.Instance.BuildMesh(sec.Contour, activeContourIndices, cardinalPointOffset, curvedAxis, ref extrudedMesh, ref meshIB, l.LocalAxes);

            // How many vertices does the mesh have?
            requiredMeshVertices = extrudedMesh.GetLength(1);
            requiredMeshIndices = meshIB.GetLength(0);

            // Are we going to draw covers? If that's the case, how many indices are to be used?
            if (lineLODLevel.LODContour == LODContour.High || lineLODLevel.LODContour == LODContour.HighStress)
            {
                cover = (lineLODLevel.LODContour == LODContour.High) ? sec.CoverHigh : sec.CoverHighstress;
                requiredCoverIndices = 2 * cover.Length;
                requiredCoverVertices = 2 * activeContourIndices.Length;
            }
            else
            {
                requiredCoverIndices = requiredCoverVertices = 0;
            }

            if (verticesInVB + requiredMeshVertices + requiredCoverVertices >= package.NumVertices || indicesInIB + requiredMeshIndices + requiredCoverIndices >= package.NumIndices)
            {
                rc.ReleaseBuffer(verticesInVB, indicesInIB, ResourceStreamType.TriangleListPositionNormalColored);
                package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, requiredMeshIndices + requiredCoverIndices, requiredMeshVertices + requiredCoverVertices, true);
                verticesInVB = 0;
                indicesInIB = 0;

                startOffset = package.StartVBFlushOffset;
            }

            unsafe
            {
                #region Vertex buffer feeder
                // Put elements in the vertex buffer
                if (!vertexColoringEnabled || pickingMode)
                {
                    #region Picking mode rendering (material and colours)
                    // Select material
                    if (pickingMode)
                    {
                        //device.RenderState.Lighting = false;
                        pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                        presentColor = pickMtrl.Diffuse.ToArgb();
                    }
                    else
                    {
                        //device.RenderState.Lighting = true;
                        //if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                        //{
                        //    if (l.IsSelected)
                        //        presentColor = System.Drawing.Color.White.ToArgb();
                        //    else
                        //        presentColor = sectionMaterials[0].Diffuse.ToArgb();
                        //}
                        //else
                        //{
                        //    if (l.IsSelected)
                        //        presentColor = System.Drawing.Color.White.ToArgb();
                        //    else
                        //        presentColor = sectionMaterials[2].Diffuse.ToArgb();
                        //}

                        presentColor = getLineColor(rc, l, pickingMode, options.LineColoredBy);
                    }
                    #endregion

                    #region Mesh vertices
                    for (i = 0; i < requiredMeshVertices; ++i)
                    {
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = extrudedMesh[1, i];
                        package.VBPointer->Color = presentColor;
                        ++package.VBPointer;
                    }
                    #endregion

                    #region "Front" cover vertices
                    for (i = 0; i < requiredCoverVertices / 2; ++i)
                    {
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = -Vector3.Normalize(curvedAxis[1] - curvedAxis[0]);
                        package.VBPointer->Color = presentColor;
                        ++package.VBPointer;
                    }
                    #endregion

                    #region "Back" cover vertices
                    for (i = requiredMeshVertices - requiredCoverVertices / 2; i < requiredMeshVertices; ++i)
                    {
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = Vector3.Normalize(curvedAxis[numPoints - 1] - curvedAxis[numPoints - 2]);
                        package.VBPointer->Color = presentColor;
                        ++package.VBPointer;
                    }
                    #endregion
                }
                else
                {
                    #region Mesh Vertices
                    int j = -1;
                    StressHelper srh = model.Results.StressHelper;
                    for (i = 0; i < requiredMeshVertices; ++i)
                    {
                        //j += (i % (sec.ContourIndices[(int)lineLODLevel.LODContour].Length - 1) == 0) ? 1 : 0;
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = extrudedMesh[1, i];
                        package.VBPointer->Color = GetColor(Canguro.Model.Model.Instance.Results.ActiveCase, l, i, lineLODLevel, srh);
                        ++package.VBPointer;
                    }
                    #endregion

                    #region "Front" cover vertices
                    for (i = 0; i < requiredCoverVertices / 2; ++i)
                    {
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = -Vector3.Normalize(curvedAxis[1] - curvedAxis[0]);
                        package.VBPointer->Color = GetColor(Canguro.Model.Model.Instance.Results.ActiveCase, l, activeContourIndices[i % (activeContourIndices.Length - 1)], lineLODLevel, srh);
                        ++package.VBPointer;
                    }
                    #endregion

                    #region "Back" cover vertices
                    for (i = requiredMeshVertices - requiredCoverVertices / 2; i < requiredMeshVertices; ++i)
                    {
                        package.VBPointer->Position = extrudedMesh[0, i];
                        package.VBPointer->Normal = Vector3.Normalize(curvedAxis[numPoints - 1] - curvedAxis[numPoints - 2]);
                        package.VBPointer->Color = GetColor(Canguro.Model.Model.Instance.Results.ActiveCase, l, activeContourIndices[i % (activeContourIndices.Length - 1)], lineLODLevel, srh);
                        ++package.VBPointer;
                    }
                    #endregion

                    GetColor(null, null, 0, lineLODLevel, srh);
                }
                #endregion

                #region Index buffer feeder

                #region Mesh indices
                for (i = 0; i < requiredMeshIndices; ++i)
                {
                    *package.IBPointer = (short)(meshIB[i] + verticesInVB + package.Offset - startOffset);
                    ++package.IBPointer;
                }
                #endregion

                #region "Front" cover indices
                for (i = 0; i < requiredCoverIndices / 2; ++i)
                {
                    *package.IBPointer = (short)(cover[i] + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
                    ++package.IBPointer;
                }
                #endregion

                #region "Back" cover indices
                for (i = 0; i < requiredCoverIndices / 2; i+=3)
                {
                    *package.IBPointer = (short)(cover[i+2] + (requiredCoverVertices / 2) + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
                    ++package.IBPointer;
                    *package.IBPointer = (short)(cover[i+1] + (requiredCoverVertices / 2) + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
                    ++package.IBPointer;
                    *package.IBPointer = (short)(cover[i] + (requiredCoverVertices / 2) + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
                    ++package.IBPointer;
                }
                #endregion

                #endregion
            }

            verticesInVB += (requiredMeshVertices + requiredCoverVertices);
            indicesInIB += (requiredMeshIndices + requiredCoverIndices);
        }

        private void renderDeformedLine(ResourceManager rc, Device device, Model.Model model, LineElement l, RenderOptions options, bool pickingMode, ref PositionNormalColoredPackage package, ref int verticesInVB, ref int indicesInIB, ref int startOffset)
        {
            if (l != null && l.IsVisible)
            {
                drawReleaseIfNeeded(rc, l, options);

                LODLevels lineLODLevel = options.LOD.GetLOD(l);

                if (lineLODLevel.LODContour == LODContour.Wireframe)
                    wireframeLines.Add(l);
                else
                {
                    FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                    if (sec.FaceNormals)
                        drawDeformedShadedLine(rc, device, model, l, lineLODLevel, sec, options, pickingMode, ref package, ref verticesInVB, ref indicesInIB, ref startOffset);
                    else
                        gouraudLines.Add(l);
                }
            }
        }

        /// <summary> </summary>
        /// <param name="device"></param>
        /// <param name="model"></param>
        /// <param name="lines"></param>
        /// <param name="options"></param>
        /// <param name="itemsInView"></param>
        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            if (!model.HasResults || model.Results.JointDisplacements == null) return;

            int verticesInVB = 0, indicesInIB = 0, startOffset = 0;
            lastMaterialColor = 0;

            // Renderer and device environment variables
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            Catalog<Section> sections = Canguro.Model.Model.Instance.Sections;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int numPoints = Properties.Settings.Default.ElementForcesSegments;

            #region Device RenderStates, Materials and lights. RenderStates Checking
            System.Drawing.Color ambientColor = device.RenderState.Ambient;

            device.RenderState.ColorVertex = true;

            if (!vertexColoringEnabled && !device.RenderState.Lighting)
            {
                if (!device.RenderState.Lighting)
                    device.RenderState.Lighting = true;
                if (device.RenderState.ShadeMode != ShadeMode.Flat)
                    device.RenderState.ShadeMode = ShadeMode.Flat;
            }
            else
            {
                if(device.RenderState.Lighting)
                    device.RenderState.Lighting = false;
                if (device.RenderState.ShadeMode != ShadeMode.Gouraud)
                    device.RenderState.ShadeMode = ShadeMode.Gouraud;
            }

            if (pickingMode)
            {
                device.RenderState.Ambient = System.Drawing.Color.White;
                device.Lights[0].Enabled = false;
                device.RenderState.Lighting = false;
            }

            if (device.RenderState.CullMode != Cull.Clockwise)
                device.RenderState.CullMode = Cull.Clockwise;

            pickMtrl = new Material();
            Material nonEmissiveMat = new Material();

            device.Material = nonEmissiveMat;
            #endregion

            // Get a calcultor for getting deformations on the line
            calc = new LineDeformationCalculator();

            paintScaleFactorTranslation = model.Results.PaintScaleFactorTranslation;
            jointDisplacements = model.Results.JointDisplacements;
            abstractCase = Canguro.Model.Model.Instance.Results.ActiveCase.AbstractCase;

            rc.ActiveStream = ResourceStreamType.TriangleListPositionNormalColored;

            PositionNormalColoredPackage package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, true, true);
            startOffset = package.StartVBFlushOffset;

            gouraudLines.Clear();
            wireframeLines.Clear();

            if (lines != null && ((IList<LineElement>)lines).Count > 0)
            {
                foreach (LineElement l in lines)
                    renderDeformedLine(rc, device, model, l, options, pickingMode, ref package, ref verticesInVB, ref indicesInIB, ref startOffset);
            }
            else if (itemsInView != null)
            {
                // Get list of items in view (Bounding Box)
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    LineElement l;
                    foreach (Item item in itemsInView)
                    {
                        l = item as LineElement;
                        renderDeformedLine(rc, device, model, l, options, pickingMode, ref package, ref verticesInVB, ref indicesInIB, ref startOffset);
                    }
                }
            }

            // Flush remaining vertices
            rc.ReleaseBuffer(verticesInVB, indicesInIB, ResourceStreamType.TriangleListPositionNormalColored);
            rc.Flush(ResourceStreamType.TriangleListPositionNormalColored);

            verticesInVB = indicesInIB = 0;

            if (gouraudLines.Count > 0)
            {
                // Activate Gourarud Shading
                device.RenderState.ShadeMode = ShadeMode.Gouraud;

                package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, true, true);
                startOffset = package.StartVBFlushOffset;

                foreach (LineElement gl in gouraudLines)
                {
                    LODLevels lineLODLevel = options.LOD.GetLOD(gl);
                    FrameSection sec = ((StraightFrameProps)gl.Properties).Section;

                    drawDeformedShadedLine(rc, device, model, gl, lineLODLevel, sec, options, pickingMode, ref package, ref verticesInVB, ref indicesInIB, ref startOffset);
                }

                // Flush remaining vertices
                rc.ReleaseBuffer(verticesInVB, indicesInIB, ResourceStreamType.TriangleListPositionNormalColored);
                rc.Flush(ResourceStreamType.TriangleListPositionNormalColored);
            }

            if (wireframeLines.Count > 0)
            {
                deformedLineWireframeRenderer.SetVertexColoring(true);
                deformedLineWireframeRenderer.Render(device, model, wireframeLines, options, itemsInView);
                deformedLineWireframeRenderer.SetVertexColoring(false);
            }

            if (pickingMode)
            {
                device.RenderState.Ambient = ambientColor;
                device.Lights[0].Enabled = true;
            }
        }
    }
}

#region Last stable version used this code block before integrating it in renderDeformedLine. If want it back, replace for if-else block
//foreach (Item item in itemsInView)
//{
//    l = item as LineElement;
//    #region Code moved to renderDeformedLine
//    //if (l != null && l.IsVisible)
//    //{
//    //    LODLevels lineLODLevel = options.LOD.GetLOD(l);

//    //    if (lineLODLevel.LODContour == LODContour.Wireframe)
//    //        wireframeLines.Add(l);
//    //    else
//    //    {
//    //        FrameSection sec = ((StraightFrameProps)l.Properties).Section;

//    //        if (sec.FaceNormals)
//    //            drawDeformedShadedLine(rc, device, l, lineLODLevel, sec, options, pickingMode, ref package, ref verticesInVB, ref indicesInIB, ref startOffset);
//    //        else
//    //            gouraudLines.Add(l);

//    //        #region Code moved to method void drawDeformedShadedLine(...)
//    //        //FrameSection sec = ((StraightFrameProps)l.Properties).Section;

//    //        //short[] activeContourIndices = sec.ContourIndices[(int)lineLODLevel.LODContour];

//    //        //numPoints = (int)options.LOD.GetLOD(l).LODSegments + 1;

//    //        //#region Get joint deformations and curve vectors
//    //        //// Get joint defomations
//    //        //vI = new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
//    //        //                 model.Results.JointDisplacements[l.I.Id, 1],
//    //        //                 model.Results.JointDisplacements[l.I.Id, 2]);
//    //        //vJ = new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
//    //        //                 model.Results.JointDisplacements[l.J.Id, 1],
//    //        //                 model.Results.JointDisplacements[l.J.Id, 2]);

//    //        //vI = options.DeformationScale * model.Results.PaintScaleFactorTranslation * vI + l.I.Position;
//    //        //vJ = options.DeformationScale * model.Results.PaintScaleFactorTranslation * vJ + l.J.Position;
//    //        //#endregion

//    //        //#region Get deformed curve vector
//    //        //// Get deformed line 'direction'
//    //        //newDir = vJ - vI;

//    //        //float[,] local2Values = calc.GetCurvedAxis(l, abstractCase, LineDeformationCalculator.DeformationAxis.Local2, numPoints);
//    //        //float[,] local3Values = calc.GetCurvedAxis(l, abstractCase, LineDeformationCalculator.DeformationAxis.Local3, numPoints);

//    //        //requiredMeshVertices = local2Values.GetLength(0);

//    //        //curvedAxis = new Vector3[requiredMeshVertices];

//    //        //// Get axis
//    //        //for (int i = 0; i < requiredMeshVertices; ++i)
//    //        //{
//    //        //    curvedAxis[i] = vI + local2Values[i, 0] * newDir +
//    //        //                         local2Values[i, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[1] +
//    //        //                         local3Values[i, 1] * options.DeformationScale * model.Results.PaintScaleFactorTranslation * l.LocalAxes[2];

//    //        //}
//    //        //#endregion

//    //        //// Build mesh
//    //        //ExtrudedShape.Instance.BuildMesh(sec.Contour, activeContourIndices, curvedAxis, ref extrudedMesh, ref meshIB, l.LocalAxes);

//    //        //// How many vertices does the mesh have?
//    //        //requiredMeshVertices = extrudedMesh.GetLength(1);
//    //        //requiredMeshIndices = meshIB.GetLength(0);

//    //        //// Are we going to draw covers? If that's the case, how many indices are to be used?
//    //        //if (lineLODLevel.LODContour == LODContour.High || lineLODLevel.LODContour == LODContour.HighStress)
//    //        //{
//    //        //    cover = (lineLODLevel.LODContour == LODContour.High) ? sec.CoverHigh : sec.CoverHighstress;
//    //        //    requiredCoverIndices = 2 * cover.Length;
//    //        //    requiredCoverVertices = 2 * activeContourIndices.Length;
//    //        //}
//    //        //else
//    //        //{
//    //        //    requiredCoverIndices = requiredCoverVertices = 0;
//    //        //}

//    //        //if (verticesInVB + requiredMeshVertices + requiredCoverVertices >= package.NumVertices || indicesInIB + requiredMeshIndices + requiredCoverIndices >= package.NumIndices)
//    //        //{
//    //        //    rc.ReleaseBuffer(verticesInVB, indicesInIB, ResourceStreamType.TriangleListPositionNormalColored);
//    //        //    package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, requiredMeshIndices, requiredMeshVertices, true);
//    //        //    verticesInVB = 0;
//    //        //    indicesInIB = 0;

//    //        //    startOffset = package.StartVBFlushOffset;
//    //        //}

//    //        //unsafe
//    //        //{
//    //        //    #region Vertex buffer feeder
//    //        //    // Put elements in the vertex buffer
//    //        //    if (!vertexColoringEnabled || pickingMode)
//    //        //    {
//    //        //        #region Picking mode rendering (material and colours)
//    //        //        presentColor = System.Drawing.Color.Red.ToArgb();
//    //        //        // Select material
//    //        //        if (pickingMode)
//    //        //        {
//    //        //            device.RenderState.Lighting = false;
//    //        //            pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
//    //        //            //pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
//    //        //            //device.Material = pickMtrl;
//    //        //            presentColor = pickMtrl.Diffuse.ToArgb();
//    //        //        }
//    //        //        else
//    //        //        {
//    //        //            device.RenderState.Lighting = true;
//    //        //            if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
//    //        //            {
//    //        //                if (l.IsSelected)
//    //        //                {
//    //        //                    //if (lastMaterial != 1)
//    //        //                    //device.Material = sectionMaterials[1];
//    //        //                    presentColor = System.Drawing.Color.White.ToArgb();
//    //        //                }
//    //        //                else
//    //        //                {
//    //        //                    //if (lastMaterial != 0) 
//    //        //                    //device.Material = sectionMaterials[0];
//    //        //                    presentColor = sectionMaterials[0].Diffuse.ToArgb();
//    //        //                }
//    //        //            }
//    //        //            else
//    //        //            {
//    //        //                if (l.IsSelected)
//    //        //                {
//    //        //                    //if (lastMaterial != 3) 
//    //        //                    //device.Material = sectionMaterials[3];
//    //        //                    presentColor = System.Drawing.Color.White.ToArgb();
//    //        //                }
//    //        //                else
//    //        //                {
//    //        //                    //if (lastMaterial != 2) 
//    //        //                    //device.Material = sectionMaterials[2];
//    //        //                    presentColor = sectionMaterials[2].Diffuse.ToArgb();
//    //        //                }
//    //        //            }
//    //        //        }
//    //        //        #endregion

//    //        //        #region Mesh vertices
//    //        //        for (int i = 0; i < requiredMeshVertices; ++i)
//    //        //        {
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = extrudedMesh[1, i];
//    //        //            package.VBPointer->Color = presentColor;
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion

//    //        //        #region "Front" cover vertices
//    //        //        for (int i = 0; i < requiredCoverVertices / 2; ++i)
//    //        //        {
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = -Vector3.Normalize(curvedAxis[1] - curvedAxis[0]);
//    //        //            package.VBPointer->Color = presentColor;
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion

//    //        //        #region "Back" cover vertices
//    //        //        for (int i = requiredMeshVertices - requiredCoverVertices / 2; i < requiredMeshVertices; ++i)
//    //        //        {
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = Vector3.Normalize(curvedAxis[numPoints - 1] - curvedAxis[numPoints - 2]);
//    //        //            package.VBPointer->Color = presentColor;
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion
//    //        //    }
//    //        //    else
//    //        //    {
//    //        //        #region Mesh Vertices
//    //        //        int j = -1;
//    //        //        for (int i = 0; i < requiredMeshVertices; ++i)
//    //        //        {
//    //        //            j += (i % sec.ContourIndices[(int)lineLODLevel.LODContour].Length == 0) ? 1 : 0;
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = extrudedMesh[1, i];
//    //        //            package.VBPointer->Color = GetColor(resultsCase, l, activeContourIndices[i % activeContourIndices.Length] + j * activeContourIndices.Length, lineLODLevel);
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion

//    //        //        #region "Front" cover vertices
//    //        //        for (int i = 0; i < requiredCoverVertices / 2; ++i)
//    //        //        {
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = -Vector3.Normalize(curvedAxis[1] - curvedAxis[0]);
//    //        //            package.VBPointer->Color = GetColor(resultsCase, l, activeContourIndices[i % activeContourIndices.Length], lineLODLevel);
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion

//    //        //        #region "Back" cover vertices
//    //        //        for (int i = requiredMeshVertices - requiredCoverVertices / 2; i < requiredMeshVertices; ++i)
//    //        //        {
//    //        //            package.VBPointer->Position = extrudedMesh[0, i];
//    //        //            package.VBPointer->Normal = Vector3.Normalize(curvedAxis[numPoints - 1] - curvedAxis[numPoints - 2]);
//    //        //            package.VBPointer->Color = GetColor(resultsCase, l, activeContourIndices[i % activeContourIndices.Length], lineLODLevel);
//    //        //            ++package.VBPointer;
//    //        //        }
//    //        //        #endregion

//    //        //        GetColor(null, null, 0, lineLODLevel);
//    //        //    }
//    //        //    #endregion

//    //        //    #region Index buffer feeder

//    //        //    #region Mesh indices
//    //        //    for (int i = 0; i < requiredMeshIndices; ++i)
//    //        //    {
//    //        //        *package.IBPointer = (short)(meshIB[i] + verticesInVB + package.Offset - startOffset);
//    //        //        ++package.IBPointer;
//    //        //    }
//    //        //    #endregion

//    //        //    #region "Front" cover indices
//    //        //    for (int i = 0; i < requiredCoverIndices / 2; ++i)
//    //        //    {
//    //        //        *package.IBPointer = (short)(cover[i] + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
//    //        //        ++package.IBPointer;
//    //        //    }
//    //        //    #endregion

//    //        //    #region "Back" cover indices
//    //        //    for (int i = 0; i < requiredCoverIndices / 2; ++i)
//    //        //    {
//    //        //        *package.IBPointer = (short)(cover[i] + (requiredCoverVertices / 2) + requiredMeshVertices + verticesInVB + package.Offset - startOffset);
//    //        //        ++package.IBPointer;
//    //        //    }
//    //        //    #endregion

//    //        //    #endregion
//    //        //}

//    //        //verticesInVB += (requiredMeshVertices + requiredCoverVertices);
//    //        //indicesInIB += (requiredMeshIndices + requiredCoverIndices);
//    //        #endregion
//    //    }
//    //}
//    #endregion
//}
#endregion

#region Legacy, tried to optimize using a Direct3D Mesh
//public class ModelMesh
//{
//    private Mesh[] subMesh;
//    private Material[] subMeshMaterial;

//    public Mesh GetSubmesh(int index)
//    {
//        if (index >= 0 && index < subMesh.GetLength(0))
//            return subMesh[index];
//        else
//            return null;
//    }

//    public void SetSubmesh(Mesh submesh, int index)
//    {
//        if (subMesh != null && index >= 0 && index < subMesh.GetLength(0))
//            subMesh[index] = submesh;
//    }

//    public void SetSubmesh(int index, int nFaces, int nVertices, VertexFormats vertexFormat, Device device, object vertexData, object indicesData, Material submeshMat)
//    {
//        if (index >= 0 && index < subMesh.GetLength(0))
//        {
//            subMesh[index] = new Mesh(nFaces, nVertices, 0, vertexFormat, device);

//            subMesh[index].SetIndexBufferData(indicesData, LockFlags.None);
//            subMesh[index].SetVertexBufferData(vertexData, LockFlags.None);
//            subMeshMaterial[index] = submeshMat;

//            int[] adjacency = new int[subMesh[index].NumberFaces * 3];
//            subMesh[index].GenerateAdjacency(0.0f, adjacency);
//            subMesh[index].OptimizeInPlace(MeshFlags.OptimizeVertexCache, adjacency);
//        }
//    }

//    public void Render(Device device)
//    {
//        int i, j, subMeshes;

//        subMeshes = subMesh.GetLength(0);

//        for (i = 0; i < subMeshes; ++i)
//        {
//            if (subMeshMaterial[i] != null && subMesh[i] != null)
//            {
//                device.Material = subMeshMaterial[i];

//                //numSubsets = subMesh[i].GetAttributeTable().GetLength(0);

//                for (j = 0; j < subMesh[i].NumberAttributes; ++j)
//                    subMesh[i].DrawSubset(j);
//            }
//        }
//    }

//    public ModelMesh(int nSubMeshes)
//    {
//        subMesh = new Mesh[nSubMeshes];
//        subMeshMaterial = new Material[nSubMeshes];
//    }
//}
#endregion

#region Mesh Testing... Pruebas para ver como funciona con una mesh
//if (analyzedMesh == null)
//{
//    foreach (LineElement l in lines)
//        if (l != null && l.IsVisible)
//            ++nSubMeshes;

//    analyzedMesh = new ModelMesh(nSubMeshes);

//    int meshIndex = 0;
//    short[] indices = null;
//    int[] meshIndices = null;

//    foreach (LineElement l in lines)
//    {
//        if (l != null && l.IsVisible)
//        {
//            if (l.Properties is StraightFrameProps)
//            {
//                FrameSection sec = ((StraightFrameProps)l.Properties).Section;

//                #region Get joint deformations and curve vectors
//                // Get joint defomations
//                vI = new Vector3(model.Results.JointDisplacements[l.I.Id, 0],
//                                 model.Results.JointDisplacements[l.I.Id, 1],
//                                 model.Results.JointDisplacements[l.I.Id, 2]);
//                vJ = new Vector3(model.Results.JointDisplacements[l.J.Id, 0],
//                                 model.Results.JointDisplacements[l.J.Id, 1],
//                                 model.Results.JointDisplacements[l.J.Id, 2]);

//                vI = model.Results.PaintScaleFactorTranslation * vI + l.I.Position;
//                vJ = model.Results.PaintScaleFactorTranslation * vJ + l.J.Position;

//                // Get deformed line 'direction'
//                newDir = vJ - vI;

//                float[,] local2Values = calc.GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, LineDeformationCalculator.DeformationAxis.Local2, numPoints);
//                float[,] local3Values = calc.GetCurvedAxis(l, model.Results.ActiveCase.AbstractCase, LineDeformationCalculator.DeformationAxis.Local3, numPoints);

//                nVertices = local2Values.GetLength(0);

//                curvedAxis = new Vector3[nVertices];

//                // Get axis
//                for (int i = 0; i < nVertices; ++i)
//                {
//                    curvedAxis[i] = vI + local2Values[i, 0] * newDir +
//                                         local2Values[i, 1] * model.Results.PaintScaleFactorTranslation * l.LocalAxes[1] +
//                                         local3Values[i, 1] * model.Results.PaintScaleFactorTranslation * l.LocalAxes[2];

//                }
//                #endregion

//                // Build mesh
//                ExtrudedShape.Instance.BuildMesh(sec.Contour, curvedAxis, curveDerivative, ref extrudedMesh, ref meshIndices);

//                // How many vertices does the mesh have?
//                nVertices = extrudedMesh.GetLength(1);
//                presentVertices += nVertices;

//                #region Picking mode rendering (material and colours)
//                presentColor = System.Drawing.Color.Red.ToArgb();
//                // Select material
//                if (pickingMode)
//                {
//                    pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
//                    pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
//                    device.Material = pickMtrl;
//                    presentColor = pickMtrl.Diffuse.ToArgb();
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
//                #endregion

//                indices = new short[meshIndices.GetLength(0)];

//                for (int i = 0; i < meshIndices.GetLength(0); ++i)
//                    indices[i] = (short)meshIndices[i];

//                CustomVertex.PositionNormalColored[] vertexData = new CustomVertex.PositionNormalColored[nVertices];

//                // Put elements in the vertex buffer
//                for (int i = 0; i < nVertices; ++i)
//                {
//                    vertexData[i].Position = extrudedMesh[0, i];
//                    vertexData[i].Normal = extrudedMesh[1, i];
//                    vertexData[i].Color = presentColor;
//                }

//                analyzedMesh.SetSubmesh(meshIndex++, indices.GetLength(0) / 3, nVertices, CustomVertex.PositionNormalColored.Format, device, vertexData, indices, pickMtrl); 
//            }
//        }
//    }
//}

//if (analyzedMesh != null)
//    analyzedMesh.Render(device);
#endregion