
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Model;
using Canguro.Model.Section;


namespace Canguro.View.Renderer
{
    class BezierShadedLineRenderer : LineRenderer
    {
        private Vector3[] curvedAxis = null;
        private Vector3[,] extrudedMesh;
        private int[] extrudedIndices = null;

        private int lastRenderedVertices = 0;

        private Material[] sectionMaterials = new Material[4];

        public BezierShadedLineRenderer()
        {
            // Steel
            sectionMaterials[0].Diffuse = System.Drawing.Color.SteelBlue;
            sectionMaterials[0].Ambient = System.Drawing.Color.SteelBlue;

            // Steel selected
            sectionMaterials[1].Diffuse = System.Drawing.Color.SteelBlue;
            sectionMaterials[1].Ambient = System.Drawing.Color.SteelBlue;
            sectionMaterials[1].Emissive = System.Drawing.Color.Gray;

            // Other materials
            sectionMaterials[2].Diffuse = System.Drawing.Color.Gold;
            sectionMaterials[2].Ambient = System.Drawing.Color.Gold;

            // Other materials selected
            sectionMaterials[3].Diffuse = System.Drawing.Color.Gold;
            sectionMaterials[3].Ambient = System.Drawing.Color.Gold;
            sectionMaterials[3].Emissive = System.Drawing.Color.Gray;
        }

        // Probando....
        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            Catalog<Section> sections = Canguro.Model.Model.Instance.Sections;

            VertexBuffer theVB = null; // rc.ExtrusionVB;
            int vbSize = 0; // rc.ExtrusionVbSize;
            int vbBase = 0; // rc.ExtrusionVbBase;
            int vbFlush = 0; // rc.ExtrusionVbFlush;
            int nVertices = 0, presentVertices = 0;

            device.VertexFormat = CustomVertex.PositionNormal.Format;
            device.SetStreamSource(0, theVB, 0);

            System.Drawing.Color ambientColor = device.RenderState.Ambient;
            Material pickMtrl = new Material();

            if (!device.RenderState.Lighting) device.RenderState.Lighting = true;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            int vSize = CustomVertex.PositionNormal.StrideSize;

            GraphicsStream vbData;

            int lastMaterial = -1;

            if (pickingMode)
            {
                device.RenderState.Ambient = System.Drawing.Color.White;
                device.Lights[0].Enabled = false;
            }

            device.RenderState.CullMode = Cull.None;

            // Inicia código NO SEGURO
            unsafe
            {
                CustomVertex.PositionNormal* vbArray;
                // Lock the VertexBuffer and get the unsafe pointer from the new lock
                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
                vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;

                foreach (LineElement l in lines)
                {
                    if (l != null)
                    {
                        if (l.Properties is StraightFrameProps)
                        {
                            FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                            if (rc.ExtrusionEnded == false)
                            {

                                // Build mesh
                                curvedAxis = ExtrudedShape.Instance.MakeExtrusionAxis(l.I.Position, new Vector3(-1.0f, -1.0f, -1.0f), l.J.Position, new Vector3(1.0f, 1.0f, 1.0f), l.LocalAxes);

                                ExtrudedShape.Instance.BuildMesh(sec.Contour, curvedAxis, ref extrudedMesh, ref extrudedIndices, true, null, null);

                                // How many vertices does the mesh have?
                                nVertices = extrudedMesh.GetLength(1);
                                presentVertices += nVertices;

                                // Put elements in the vertex buffer
                                for (int i = 0; i < nVertices; ++i)
                                {
                                    vbArray->Position = extrudedMesh[0, i];
                                    vbArray->Normal = extrudedMesh[1, i];
                                    //vbArray->Color = System.Drawing.Color.Red.ToArgb();
                                    ++vbArray;
                                    ++lastRenderedVertices;
                                }

                                #region Picking mode rendering (material and colours)
                                // Select material
                                if (pickingMode)
                                {
                                    pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                                    pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                                    device.Material = pickMtrl;
                                }
                                else
                                {
                                    if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                                    {
                                        if (l.IsSelected)
                                        {
                                            if (lastMaterial != 1) device.Material = sectionMaterials[1];
                                            lastMaterial = 1;
                                        }
                                        else
                                        {
                                            if (lastMaterial != 0) device.Material = sectionMaterials[0];
                                            lastMaterial = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (l.IsSelected)
                                        {
                                            if (lastMaterial != 3) device.Material = sectionMaterials[3];
                                            lastMaterial = 3;
                                        }
                                        else
                                        {
                                            if (lastMaterial != 2) device.Material = sectionMaterials[2];
                                            lastMaterial = 2;
                                        }
                                    }
                                }
                                #endregion

                                // Flush vertices to allow the GPU to start processing
                                //if (presentVertices >= vbFlush)
                                //{
                                //    // Unlock VB for flushing the vertex stream
                                //    theVB.Unlock();

                                //    device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, presentVertices / 3);

                                //    vbBase += presentVertices; presentVertices = 0;

                                //    // If the space allocated in memory is over, discard buffer, else append data
                                //    if (vbBase >= vbSize)
                                //        vbBase = 0;

                                //    // Re-Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                                //    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                                //    vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                                //}
                            }
                            else
                            {
                                presentVertices = lastRenderedVertices;
                                rc.ExtrusionEnded = true;
                            }
                        }
                    }
                }
            }   // Termina código NO SEGURO

            rc.ExtrusionEnded = true;

            // Unlock VB for flushing the vertex stream
            theVB.Unlock();

            if (presentVertices != 0)
                device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, presentVertices / 3);

            if (pickingMode)
            {
                device.RenderState.Ambient = ambientColor;
                device.Lights[0].Enabled = true;
            }
        }
        #region Commented last good rendering method...
/*      // Ok... solo vertices, repeticion y lentitud pero jala...
        public override void Render(Device device, System.Collections.Generic.IEnumerable<LineElement> lines)
        {
            ResourceCache rc = GraphicViewManager.Instance.ResourceCache;
            Catalog<Section> sections = Model.Model.Instance.Sections;

            VertexBuffer theVB = rc.SectionsVB;
            int vbSize = rc.SectionsVbSize;
            int vbBase = rc.SectionsVbBase;
            int vbFlush = rc.SectionsVbFlush;
            int nVertices = 0, presentVertices = 0;

            device.VertexFormat = CustomVertex.PositionNormal.Format;
            device.SetStreamSource(0, theVB, 0);

            System.Drawing.Color ambientColor = device.RenderState.Ambient;
            Material pickMtrl = new Material();

            if (!device.RenderState.Lighting) device.RenderState.Lighting = true;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            int vSize = CustomVertex.PositionNormal.StrideSize;

            GraphicsStream vbData;

            int lastMaterial = -1;

            if (pickingMode)
            {
                device.RenderState.Ambient = System.Drawing.Color.White;
                device.Lights[0].Enabled = false;
            }

            Dictionary<Section, SectionLocator> sectionDictionary = new Dictionary<Section, SectionLocator>();

            device.RenderState.CullMode = Cull.None;

            // Inicia código NO SEGURO
            unsafe
            {
                CustomVertex.PositionNormal* vbArray;
                // Lock the VertexBuffer and get the unsafe pointer from the new lock
                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
                vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;

                foreach (LineElement l in lines)
                {
                    if (l != null)
                    {
                        if (l.Properties is StraightFrameProps)
                        {
                            FrameSection sec = ((StraightFrameProps)l.Properties).Section;

                            if (!sectionDictionary.ContainsKey(sec))
                            {
                                // Build mesh
                                curvedAxis = ExtrudedShape.Instance.MakeExtrusionAxis(l.I.Position, new Vector3(-1.0f, -1.0f, -1.0f), l.J.Position, new Vector3(1.0f, 1.0f, 1.0f), l.LocalAxes);
                                ExtrudedShape.Instance.BuildMesh(sec.Contour, curvedAxis, ref extrudedMesh);

                                // How many vertices does the mesh have?
                                nVertices = extrudedMesh.GetLength(1);
                                presentVertices += nVertices;

                                // Put elements in the vertex buffer
                                for (int i = 0; i < nVertices; ++i)
                                {
                                    vbArray->Position = extrudedMesh[0, i];
                                    vbArray->Normal = extrudedMesh[1, i];
                                    ++vbArray;
                                }
                            }

                            #region Picking mode rendering (material and colours)
                            // Select material
                            if (pickingMode)
                            {
                                pickMtrl.Diffuse = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                                pickMtrl.Ambient = System.Drawing.Color.FromArgb(rc.GetNextPickIndex(l));
                                device.Material = pickMtrl;
                            }
                            else
                            {
                                if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
                                {
                                    if (l.IsSelected)
                                    {
                                        if (lastMaterial != 1) device.Material = sectionMaterials[1];
                                        lastMaterial = 1;
                                    }
                                    else
                                    {
                                        if (lastMaterial != 0) device.Material = sectionMaterials[0];
                                        lastMaterial = 0;
                                    }
                                }
                                else
                                {
                                    if (l.IsSelected)
                                    {
                                        if (lastMaterial != 3) device.Material = sectionMaterials[3];
                                        lastMaterial = 3;
                                    }
                                    else
                                    {
                                        if (lastMaterial != 2) device.Material = sectionMaterials[2];
                                        lastMaterial = 2;
                                    }
                                }
                            }
                            #endregion

                            // Flush vertices to allow the GPU to start processing
                            if (presentVertices >= vbFlush)
                            {
                                // Unlock VB for flushing the vertex stream
                                theVB.Unlock();

                                device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, presentVertices / 3);

                                vbBase += presentVertices;
                                presentVertices = 0;

                                // If the space allocated in memory is over, discard buffer, else append data
                                if (vbBase >= vbSize)
                                    vbBase = 0;

                                // Re-Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                                vbArray = (CustomVertex.PositionNormal*)vbData.InternalDataPointer;
                            }
                        }
                    }
                }
            }   // Termina código NO SEGURO

            // Unlock VB for flushing the vertex stream
            theVB.Unlock();

            if (presentVertices != 0)
                device.DrawPrimitives(PrimitiveType.TriangleList, vbBase, presentVertices / 3);

            if (pickingMode)
            {
                device.RenderState.Ambient = ambientColor;
                device.Lights[0].Enabled = true;
            }
        }
*/
    #endregion 
    }
}

#region OBJ Writter code for testing purposes
/*
 * OBJ Writer for calling after extrusion
    System.IO.StreamWriter stream = new System.IO.StreamWriter("objModel.obj");

    for (int i = 0; i < nVertices; ++i)
        stream.WriteLine("v " + extrudedMesh[0, i].X.ToString() + " " + extrudedMesh[0, i].Y.ToString() + " " + extrudedMesh[0, i].Z.ToString());

    for (int i = 0; i < nVertices; ++i)
        stream.WriteLine("vn " + extrudedMesh[1, i].X.ToString() + " " + extrudedMesh[1, i].Y.ToString() + " " + extrudedMesh[1, i].Z.ToString());

    stream.WriteLine("g");

    for (int i = 0; i < nIndices; i += 3)
        stream.WriteLine("f " + meshIB[i].ToString()   + "//" + meshIB[i].ToString()   + " " +
                                meshIB[i+1].ToString() + "//" + meshIB[i+1].ToString() + " " +
                                meshIB[i+2].ToString() + "//" + meshIB[i+2].ToString());

    stream.Close();
*/

//System.IO.StreamWriter stream = new System.IO.StreamWriter("objModel.obj");

//for (int i = 0; i < nVertices; ++i)
//    stream.WriteLine("v " + extrudedMesh[0, i].X.ToString() + " " + extrudedMesh[0, i].Y.ToString() + " " + extrudedMesh[0, i].Z.ToString());

//for (int i = 0; i < nVertices; ++i)
//    stream.WriteLine("vn " + extrudedMesh[1, i].X.ToString() + " " + extrudedMesh[1, i].Y.ToString() + " " + extrudedMesh[1, i].Z.ToString());

//stream.WriteLine("g");

//for (int i = 0; i < nVertices; i += 3)
//{
//    int v1, v2, v3;
//    v1 = i+1; v2 = i + 2; v3 = i + 3;
//    stream.WriteLine("f " + v1.ToString() + "//" + v1.ToString() + " " +
//                            v2.ToString() + "//" + v2.ToString() + " " +
//                            v3.ToString() + "//" + v3.ToString());
//}
#endregion