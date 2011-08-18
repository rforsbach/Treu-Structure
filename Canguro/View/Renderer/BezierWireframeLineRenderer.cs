using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Renders a curved line based in a bezier curve
    /// </summary>
    public class BezierWireframeLineRenderer : LineRenderer
    {
        /// <summary> Points that builds the curve </summary>
        private Vector3[] curvedAxis = null;
        /// <summary> Displacement at node I </summary>
        private Vector3 displacedI = Vector3.Empty;
        /// <summary> Joint I rotation </summary>
        private Vector3 rotationI = Vector3.Empty;
        /// <summary> Displacement at node J </summary>
        private Vector3 displacedJ = Vector3.Empty;
        /// <summary> Joint J rotation </summary>
        private Vector3 rotationJ = Vector3.Empty;

        // Probando....
        /// <summary>
        /// Main rendering method
        /// </summary>
        /// <param name="device"> Rendering device </param>
        /// <param name="lines"> Line list </param>
        /// <param name="options"> Rendering options </param>
        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            // Get resource cache instance
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

            // Get line vertex buffer
            VertexBuffer theVB = null; // rc.LinesVB;
            int vbSize = 0; //rc.LinesVbSize;                // Vertex buffer size
            int vbBase = 0; //rc.LinesVbBase;                // Base pointer in VB
            int vbFlush = 0; //rc.LinesVbFlush;              // Flush size for this VB
            int nVertices = 0, presentVertices = 0;     // How many vertices are being drawn

            // No need of normals, just position and color for describing this vertex format
            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.SetStreamSource(0, theVB, 0);

            device.RenderState.Lighting = false;        // Disable lighting
            device.RenderState.CullMode = Cull.None;    // Disable backface culling

            // Drawing state: picking or scene
            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            // Default joint color
            int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
            // Default color for selected joints
            int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
            // Size of the vertex format
            int vSize = CustomVertex.PositionColored.StrideSize;

            int lineColor;

            GraphicsStream vbData;

            displacedI = Vector3.Empty;
            rotationI = Vector3.Empty;
            displacedJ = Vector3.Empty;
            rotationJ = Vector3.Empty;

            bool locked = false;

            try
            {
                // Inicia código NO SEGURO
                unsafe
                {
                    CustomVertex.PositionColored* vbArray;
                    // Lock the VertexBuffer and get the unsafe pointer from the new lock
                    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, LockFlags.NoOverwrite);
                    locked = true;
                    vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;

                    float deformedTransScaleFactor = 0.0f;
                    float deformedRotScaleFactor = 0.0f;
                    if (model.HasResults)
                    {
                        //deformedRotScaleFactor = 0.1f * Results.PaintScaleFactorRotation;
                        deformedTransScaleFactor = model.Results.PaintScaleFactorTranslation;
                    }

                    foreach (LineElement l in lines)
                    {
                        if (l != null && model.HasResults && model.Results.JointDisplacements != null)
                        {
                            // Set line color according to the rendering state
                            if (pickingMode)
                                lineColor = rc.GetNextPickIndex(l);
                            else
                                lineColor = (l.IsSelected) ? jointSelectedColor : jointColor;

                            // Displaced joints: first I and then J
                            displacedI.X = l.I.Position.X + model.Results.JointDisplacements[l.I.Id, 0];
                            displacedI.Y = l.I.Position.Y + model.Results.JointDisplacements[l.I.Id, 1];
                            displacedI.Z = l.I.Position.Z + model.Results.JointDisplacements[l.I.Id, 2];

                            displacedJ.X = l.J.Position.X + model.Results.JointDisplacements[l.J.Id, 0];
                            displacedJ.Y = l.J.Position.Y + model.Results.JointDisplacements[l.J.Id, 1];
                            displacedJ.Z = l.J.Position.Z + model.Results.JointDisplacements[l.J.Id, 2];

                            // 
                            float delta = (float)Math.Acos(Vector3.Dot(Vector3.Normalize(l.LocalAxes[0]), Vector3.Normalize(displacedJ - displacedI)));

                            displacedI.X = l.I.Position.X + deformedTransScaleFactor * model.Results.JointDisplacements[l.I.Id, 0];
                            displacedI.Y = l.I.Position.Y + deformedTransScaleFactor * model.Results.JointDisplacements[l.I.Id, 1];
                            displacedI.Z = l.I.Position.Z + deformedTransScaleFactor * model.Results.JointDisplacements[l.I.Id, 2];

                            displacedJ.X = l.J.Position.X + deformedTransScaleFactor * model.Results.JointDisplacements[l.J.Id, 0];
                            displacedJ.Y = l.J.Position.Y + deformedTransScaleFactor * model.Results.JointDisplacements[l.J.Id, 1];
                            displacedJ.Z = l.J.Position.Z + deformedTransScaleFactor * model.Results.JointDisplacements[l.J.Id, 2];

                            float alfa = (float)Math.Acos(Vector3.Dot(Vector3.Normalize(l.LocalAxes[0]), Vector3.Normalize(displacedJ - displacedI)));

                            float angleRatio = Math.Abs(alfa / delta);

                            deformedRotScaleFactor = angleRatio;

                            rotationI.X = deformedRotScaleFactor * model.Results.JointDisplacements[l.I.Id, 3];
                            rotationI.Y = deformedRotScaleFactor * model.Results.JointDisplacements[l.I.Id, 4];
                            rotationI.Z = deformedRotScaleFactor * model.Results.JointDisplacements[l.I.Id, 5];

                            rotationJ.X = deformedRotScaleFactor * model.Results.JointDisplacements[l.J.Id, 3];
                            rotationJ.Y = deformedRotScaleFactor * model.Results.JointDisplacements[l.J.Id, 4];
                            rotationJ.Z = deformedRotScaleFactor * model.Results.JointDisplacements[l.J.Id, 5];


                            // Build mesh
                            //curvedAxis = ExtrudedShape.Instance.MakeExtrusionAxis(displacedI, rotationI, displacedJ, rotationJ, l.LocalAxes);
                            curvedAxis = ExtrudedShape.Instance.MakeExtrusionAxis(displacedI, rotationI, displacedJ, rotationJ, l.LocalAxes[0]);

                            // How many vertices does the mesh have?
                            nVertices = curvedAxis.GetLength(0);

                            // Put elements in the vertex buffer
                            for (int i = 0; i < nVertices - 1; ++i)
                            {
                                vbArray->Position = curvedAxis[i];
                                vbArray->Color = lineColor;
                                ++vbArray;
                                ++presentVertices;

                                vbArray->Position = curvedAxis[i + 1];
                                vbArray->Color = lineColor;
                                ++vbArray;
                                ++presentVertices;
                            }
                            // Flush vertices to allow the GPU to start processing
                            if (presentVertices >= vbFlush)
                            {
                                // Unlock VB for flushing the vertex stream
                                theVB.Unlock();
                                locked = false;

                                device.DrawPrimitives(PrimitiveType.LineList, vbBase, presentVertices / 2);

                                vbBase += presentVertices; presentVertices = 0;

                                // If the space allocated in memory is over, discard buffer, else append data
                                if (vbBase >= vbSize)
                                    vbBase = 0;

                                // Re-Lock the VertexBuffer and obtain the unsafe pointer from the new lock
                                vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
                                locked = true;
                                vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;
                            }
                        }
                    }
                }   // Termina código NO SEGURO
            }
            catch (Exception)
            { }
            finally
            {
                if (locked) // Unlock VB for flushing the vertex stream
                    theVB.Unlock();
            }

            if (presentVertices != 0)
                device.DrawPrimitives(PrimitiveType.LineList, vbBase, presentVertices / 2);
        }
    }
}
