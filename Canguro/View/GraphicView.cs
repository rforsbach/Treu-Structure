using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.View.Renderer;

namespace Canguro.View
{
    /// <summary>
    /// Class describing how does a viewport behaves, its events, tasks, and how it is controlled
    /// </summary>
    public class GraphicView
    {
        /// <summary> The id of this view, related to the index in the views array in GraphicViewManager </summary>
        private int id;
        /// <summary> The viewport contained </summary>
        private Viewport viewport;
        /// <summary> Instance of the graphic view manager </summary>
        private GraphicViewManager manager;

        /// <summary> Projection Matrix for this viewport </summary>
        private Matrix projectionMatrix;
        /// <summary> View Matrix for this viewport </summary>
        private Matrix viewMatrix;
        /// <summary> World Matrix for this viewport. Read-only and set to Identity: No ops over this matrix, just over the other two (projection and view)</summary>
        private static readonly Matrix worldMatrix = Matrix.Identity;
        /// <summary> To whom is attached this viewport? </summary>
        //private Device device;
        /// <summary> A ModelRenderer instance: What is showing this viewport </summary>
        private ModelRenderer modelRenderer;
        /// <summary> Do we have any animation in this viewport? </summary>
        private bool isAnimated;
        /// <summary> A scale for configuring the current projection </summary>
        public const float ViewportProjectionScale = .05f;
        /// <summary> How far do we see in the projection? </summary>
        public const float ViewportProjectionZPlane = 5000.0f;
        /// <summary> A constant for determining the size of reference axes (screen coordinates)</summary>
        private const float axisSize = 30.0f;
        /// <summary> How much is displaced the reference axes from viewport contour </summary>
        private const float axisMargin = 5.0f;

        /// <summary> An arcball controller for interacton tasks (pan, rotate, and zoom) </summary>
        private ArcBall arcBallCtrl;
        private Stack<ArcBall> arcBallStack;

        public int ArcBallsInStack
        {
            get { return arcBallStack.Count; }
        }

        public void PushArcBall(ArcBall arcBall)
        {
            arcBallStack.Push(arcBall);
        }

        public ArcBall PopArcBall()
        {
            if (arcBallStack.Count > 0)
                return arcBallStack.Pop();
            else
                throw new StackOverflowException("No elements to pop");
            return null;
        }

        /// <summary> Public property for accessing the arcball controller </summary>
        public ArcBall ArcBallCtrl
        {
            get 
            { 
                if(arcBallStack.Count > 0)
                    return arcBallStack.Peek();
                else
                    throw new InvalidCallException("Stack has no items");
            }

            //get { return arcBallCtrl; }
        }

        public int Id
        {
            get { return id; }
        }

        /// <summary> Public property for setting/getting the animation state in the current viewport </summary>
        public bool IsAnimated
        {
            get { return isAnimated; }
            set { isAnimated = value; }
        }

        /// <summary>
        ///  Class constructor
        /// </summary>
        public GraphicView(int id)
        {
            this.id = id;

            // Make device reference pòint to null
            //this.device = null;

            // Create the model renderer for this view.
            this.modelRenderer = new SimpleModelRenderer(new WireframeJointRenderer(), new WireframeLineRenderer(), new WireframeAreaRenderer(), new SimpleLoadRenderer(), new ElementForcesRenderer(), new GadgetRenderer());
            //this.modelRenderer = new SimpleModelRenderer(Canguro.Model.Model.Instance);
            //this.modelRenderer = new SimpleModelRenderer(Canguro.Model.Model.Instance, new WireframeJointRenderer(), new BezierWireframeLineRenderer(), new ShadedAreaRenderer(), new SimpleLoadRenderer());
            //this.modelRenderer = new SimpleModelRenderer(Canguro.Model.Model.Instance, new WireframeJointRenderer(), new BezierShadedLineRenderer(), new ShadedAreaRenderer(), new SimpleLoadRenderer());
            //this.modelRenderer = new SimpleModelRenderer(Canguro.Model.Model.Instance, new WireframeJointRenderer(), new WireframeLineRenderer(), new ShadedAreaRenderer(), new SimpleLoadRenderer());

            // No update needed
            updateModelNeeded = false;
            updateResourcesNeeded = false;

            // Set default projection and view matrices
            projectionMatrix = Matrix.Identity;
            viewMatrix = Matrix.LookAtRH(new Vector3(5, 5, 0), new Vector3(5, 5, -1), new Vector3(0, 1, 0));

            // Get the graphic view manager instance
            manager = GraphicViewManager.Instance;

            // Build a new arcball controller for this view
            arcBallStack = new Stack<ArcBall>();
            arcBallStack.Push(new ArcBall(this));
            //arcBallCtrl = new ArcBall(this);

            // Ser animation flag to false
            isAnimated = false;
        }

        /// <summary>
        /// Method for updating the view (resources, clearing the device, set transformations, lighting, and rendering callers)
        /// </summary>
        internal void Update(Device device)
        {
            // When there's no device, do nothing
            if (device == null)
                return;

            // Check if resources need to be updated
            if (updateResourcesNeeded)
                updateResources();

            // Check if model needs an update
            if (updateModelNeeded)
                updateModel();
            
            // Make Current viewport
            device.Viewport = viewport;

            // There are two rendering stages:
            // 1.- Picking surface rendering
            // 2.- Scene rendering
            // When drawing the picking surface, its needed to paint with a black background so check what is going to be rendered
            if (GraphicViewManager.Instance.DrawingPickingSurface)
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0, 1.0f, 0);
            else if(GraphicViewManager.Instance.PrintingHiResImage)
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Properties.Settings.Default.DefaultPrintinBackgroundColor, 1.0f, 0);
            else
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Properties.Settings.Default.BackColor, 1.0f, 0);

            // Set matrices:
            // 1.- Projection matrix
            // 2.- View matrix
            device.Transform.Projection = projectionMatrix;
            device.Transform.View = viewMatrix;
            #region Legacy: Matrices
            //device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 10,
            //     viewport.Width / viewport.Height, 1.0f, 1000.0f);
            //device.Transform.Projection = Matrix.OrthoLH(20, 20, 0, 20);
            #endregion

            // Set lighting state
            device.RenderState.Lighting = false;
            
            // Calc light direction
            Vector3 zero = Vector3.Empty, lightDir = Canguro.Utility.CommonAxes.GlobalAxes[2];
            Unproject(ref zero);
            Unproject(ref lightDir);

            lightDir = lightDir - zero;
            float lightDirLen = lightDir.LengthSq();
            if (!float.IsInfinity(lightDirLen) && !float.IsNaN(lightDirLen) && lightDirLen > 0.0000001)
            {
                device.Lights[0].Direction = Vector3.Scale(lightDir, (float)(1f/Math.Sqrt(lightDirLen)));
                device.Lights[0].Update();
            }

            // Draw grid and axes if required When picking surface is not being drawn

            Vector3[] bbCorners = Canguro.Model.Model.Instance.Summary.BoundingBox;
            Vector3 llCorner = bbCorners[0];
            Vector3 urCorner = Vector3.Maximize(bbCorners[1], new Vector3(10, 10, 10));
            float width = 0f, height = 0f;

            /// 1.- Viewport contour
            /// 2.- Grid (if option is set)
            /// 3.- World axes (if option is set)
            /// 4.- Window or reference axes
            if (!manager.DrawingPickingSurface)
            {
                if (!manager.PrintingHiResImage)
                    DrawViewportContour(device);

                if ((modelRenderer.RenderOptions.OptionsShown & RenderOptions.ShowOptions.GridFloor) > 0)
                {
                    bbCorners = Canguro.Model.Model.Instance.Summary.BoundingBox;
                    llCorner = bbCorners[0];
                    urCorner = Vector3.Maximize(bbCorners[1], new Vector3(10, 10, 10));

                    if (llCorner == Vector3.Empty)
                        llCorner = new Vector3(-0.001f, -0.001f, -0.001f);
                    else
                        llCorner.Z = -.001f;

                    width = Math.Abs(urCorner.X - bbCorners[0].X);
                    if (width <= 0.001f)
                        width = 10f;
                    width += 1f;

                    height = Math.Abs(urCorner.Y - bbCorners[0].Y);
                    if (height <= 0.001f)
                        height = 10f;
                    height += 1f;

                    drawGrid(device, 1f, width, height, llCorner);
                }

                if ((modelRenderer.RenderOptions.OptionsShown & RenderOptions.ShowOptions.GlobalAxes) > 0)
                    drawWorldAxes(device);

                if (manager.DrawWorldAxes)
                    drawWindowAxes(device);
            }

            // Clear buffer Z
            if (GraphicViewManager.Instance.DrawingPickingSurface)
                device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
            else if (GraphicViewManager.Instance.PrintingHiResImage)
                device.Clear(ClearFlags.ZBuffer, Properties.Settings.Default.DefaultPrintinBackgroundColor, 1.0f, 0);
            else
                device.Clear(ClearFlags.ZBuffer, Properties.Settings.Default.BackColor, 1.0f, 0);

            // Call ModelRenderer
            if (modelRenderer != null)
                modelRenderer.Render(device);

            // Then, draw floor if required
            if (!manager.DrawingPickingSurface)
            {
                if (!manager.PrintingHiResImage)
                    DrawViewportContour(device);

                if ((modelRenderer.RenderOptions.OptionsShown & RenderOptions.ShowOptions.GridFloor) > 0)
                    drawFloor(device, 1f, width, height, llCorner);
            }
        }

        /// <summary>
        /// Draws the contour of the current viewport
        /// </summary>
        public void DrawViewportContour(Device device)
        {
            // Build a Direct3D line object
            Line contour = new Line(device);

            // We need 4 lines for drawing the contour so 4 vertices are also needed
            Vector2[] lineVertices = new Vector2[4];

            // We set the default color for the viewport contour's to gray
            System.Drawing.Color color = System.Drawing.Color.Gray;

            // If this instance is the active view, then, change color to white
            if (this == GraphicViewManager.Instance.ActiveView)
                color = System.Drawing.Color.White;

            // Set vertices values
            lineVertices[0] = new Vector2(          0.0f,            0.0f);
            lineVertices[1] = new Vector2(Viewport.Width,            0.0f);
            lineVertices[2] = new Vector2(Viewport.Width, Viewport.Height);
            lineVertices[3] = new Vector2(          0.0f, Viewport.Height);

            // We want a bolder contour
            contour.Width = 3.0f;            

            // Draw the lines
            lock (device)
            {
                device.BeginScene();
                try
                {
                    contour.Begin();
                    try
                    {
                        contour.Draw(new Vector2[] { lineVertices[0], lineVertices[1] }, color);
                        contour.Draw(new Vector2[] { lineVertices[1], lineVertices[2] }, color);
                        contour.Draw(new Vector2[] { lineVertices[2], lineVertices[3] }, color);
                        contour.Draw(new Vector2[] { lineVertices[3], lineVertices[0] }, color);
                    }
                    finally
                    {
                        contour.End();
                    }
                }
                finally
                {
                    device.EndScene();
                }
            }

            // Because we need no more the contour, dispose, so GC can collect it
            contour.Dispose();
        }

        /// <summary>
        /// Given a Vector3 data, unprojects it according to the matrices (world, projection and view of this view)
        /// </summary>
        /// <param name="vector"> The vector/point to unproject to world coordinates </param>
        public void Unproject(ref Vector3 vector)
        {
            vector.Unproject(viewport, projectionMatrix, viewMatrix, worldMatrix); 
        }

        /// <summary>
        /// Projects a Vector3 data into screen coordinates according to the matrices used by this view
        /// </summary>
        /// <param name="vector"> The vector to project to screen coordinates </param>
        public void Project(ref Vector3 vector)
        {
            vector.Project(viewport, projectionMatrix, viewMatrix, worldMatrix);
        }

        /// <summary>
        /// Renders the world axes in the current view
        /// </summary>
        private void drawWorldAxes(Device device)
        {
            lock (device)
            {
                // Start drawing to the screen
                device.BeginScene();
                try
                {
                    // We need no lighting, drawing just lines which need no illumination
                    device.RenderState.Lighting = false;

                    // Six vertices needed for building axes
                    CustomVertex.PositionColored[] globalAxis = new CustomVertex.PositionColored[6];
                    globalAxis[0].Position = new Vector3(-0.001f, 0, -0.001f); globalAxis[0].Color = System.Drawing.Color.Red.ToArgb();
                    globalAxis[1].Position = new Vector3(5, 0, -0.001f); globalAxis[1].Color = System.Drawing.Color.Red.ToArgb();
                    globalAxis[2].Position = new Vector3(-0.001f, 0, -0.001f); globalAxis[2].Color = System.Drawing.Color.Lime.ToArgb();
                    globalAxis[3].Position = new Vector3(-0.001f, 5, -0.001f); globalAxis[3].Color = System.Drawing.Color.Lime.ToArgb();
                    globalAxis[4].Position = new Vector3(-0.001f, 0, -0.001f); globalAxis[4].Color = System.Drawing.Color.Blue.ToArgb();
                    globalAxis[5].Position = new Vector3(-0.001f, 0, 5); globalAxis[5].Color = System.Drawing.Color.Blue.ToArgb();

                    // Set vertex format: lines need position and color
                    device.VertexFormat = CustomVertex.PositionColored.Format;
                    // Draw primitives
                    device.DrawUserPrimitives(PrimitiveType.LineList, 3, globalAxis);
                }
                finally
                {
                    device.EndScene();
                }
            }
        }

        /// <summary>
        /// Draw reference aces in screen coordinates
        /// </summary>
        private void drawWindowAxes(Device device)
        {
            // Get three vectors: i, j, k and origin
            Vector3 zero = new Vector3(0,0,0), x = new Vector3(1, 0, 0), y = new Vector3(0, 1, 0), z = new Vector3(0, 0, 1);
            // Project them to screen coordinates
            Project(ref zero);
            Project(ref x);
            Project(ref y);
            Project(ref z);

            // Get distances for one unit in every direction
            x = x - zero;
            y = y - zero;
            z = z - zero;

            // Get a scaling factor, we need the bigger segment
            float scale = axisSize / Math.Max(x.Length(), Math.Max(y.Length(), z.Length()));

            // Get a Direct3D Line object
            Line line = new Line(device);

            // This is the position where we want to put the reference axes: down rigth corner
            Vector2 axisP = new Vector2(viewport.X + axisSize + axisMargin, viewport.Y + viewport.Height - axisSize - axisMargin);
            
            // We need three extra vertices
            Vector2[] verts = new Vector2[3];

            verts[0] = new Vector2(axisP.X + x.X * scale, axisP.Y + x.Y * scale);
            verts[1] = new Vector2(axisP.X + y.X * scale, axisP.Y + y.Y * scale);
            verts[2] = new Vector2(axisP.X + z.X * scale, axisP.Y + z.Y * scale);

            // Set a bolder line
            line.Width = 3;

            lock (device)
            {
                device.BeginScene();
                try
                {
                    // Draw axes
                    line.Begin();
                    try
                    {
                        line.Draw(new Vector2[] { axisP, verts[0] }, System.Drawing.Color.Red);
                        line.Draw(new Vector2[] { axisP, verts[1] }, System.Drawing.Color.Lime);
                        line.Draw(new Vector2[] { axisP, verts[2] }, System.Drawing.Color.Blue);
                    }
                    finally
                    {
                        line.End();
                    }

                    // If it is possible, paint a label aside each axis
                    if (GraphicViewManager.Instance.ResourceManager.LabelFont != null && !GraphicViewManager.Instance.ResourceManager.LabelFont.Disposed)
                    {
                        GraphicViewManager.Instance.ResourceManager.LabelFont.DrawText(null, "x", new System.Drawing.Point((int)verts[0].X, (int)verts[0].Y),
                                                                                     GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : System.Drawing.Color.White);
                        GraphicViewManager.Instance.ResourceManager.LabelFont.DrawText(null, "y", new System.Drawing.Point((int)verts[1].X, (int)verts[1].Y - 15),
                                                                                     GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : System.Drawing.Color.White);
                        GraphicViewManager.Instance.ResourceManager.LabelFont.DrawText(null, "z", new System.Drawing.Point((int)verts[2].X, (int)verts[2].Y),
                                                                                     GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : System.Drawing.Color.White);
                    }
                }
                finally
                {
                    device.EndScene();
                }
            }
            
            // Dispose the line, its no longer needed
            line.Dispose();
        }

        /// <summary>
        /// Draws a grid of the told dimensions
        /// </summary>
        /// <param name="d"> Distance between each line in the grid (separation) </param>
        /// <param name="w"> Width </param>
        /// <param name="h"> Height </param>
        /// <param name="origin"> Origin (Lower right corner of the grid) </param>
        private void drawGrid(Device device, float d, float w, float h, Vector3 origin)
        {
            // Directions needed 
            Vector3 i = new Vector3(1, 0, 0);
            Vector3 j = new Vector3(0, 1, 0);
            Vector3 k = new Vector3(0, 0, 1);
            int lines = 0, counter = 0;
            
            // How many lines are we going to have?
            int verticalSpans, horizontalSpans;

            verticalSpans = (int)(w / d + 1);
            horizontalSpans = (int)(h / d + 1);

            // We need some vertices for grid contour, important data are position and color because we are just using colours
            CustomVertex.PositionColored[] lineVertices = new CustomVertex.PositionColored[5];
            // Build axes lines
            lineVertices[0].Position = origin;
            lineVertices[1].Position = origin + w * d * i;
            lineVertices[2].Position = lineVertices[1].Position + h * d * j;
            lineVertices[3].Position = origin + h * d * j;
            lineVertices[4].Position = origin;

            lineVertices[0].Color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();
            lineVertices[1].Color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();
            lineVertices[2].Color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();
            lineVertices[3].Color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();
            lineVertices[4].Color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();

            // Build grid lines
            CustomVertex.PositionColored[] grid = new CustomVertex.PositionColored[verticalSpans * 2 + horizontalSpans * 2];
            // Vertical Lines
            for (counter = 0, lines = 0; counter < verticalSpans*2; ++counter, ++lines)
            {
                grid[counter].Position = origin + d * lines * i;                grid[counter].Color = System.Drawing.Color.FromArgb(90, 90, 90, 90).ToArgb();
                ++counter;
                grid[counter].Position = grid[counter - 1].Position + h * j;    grid[counter].Color = System.Drawing.Color.FromArgb(90, 90, 90, 90).ToArgb();
            }

            // Horizontal Lines
            for (counter = verticalSpans*2, lines = 0; counter < verticalSpans*2 + horizontalSpans*2; ++counter, ++lines)
            {
                grid[counter].Position = origin + d * lines * j;                grid[counter].Color = System.Drawing.Color.FromArgb(90, 90, 90, 90).ToArgb();
                ++counter;
                grid[counter].Position = grid[counter - 1].Position + w * i;    grid[counter].Color = System.Drawing.Color.FromArgb(90, 90, 90, 90).ToArgb();
            }

            lock (device)
            {
                device.BeginScene();
                try
                {
                    // Set vertex format and draw elements
                    device.VertexFormat = CustomVertex.PositionColored.Format;

                    device.DrawUserPrimitives(PrimitiveType.LineList, verticalSpans + horizontalSpans, grid);

                    device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, lineVertices);
                }
                finally
                {
                    device.EndScene();
                }
            }
        }

        private void drawFloor(Device device, float d, float w, float h, Vector3 origin)
        {
            // Directions needed 
            Vector3 i = new Vector3(1, 0, 0);
            Vector3 j = new Vector3(0, 1, 0);
            Vector3 k = new Vector3(0, 0, 1);

            /// We need to enable culling and alpha blending because its intended to have a transparent grid whe viewing from above and nothing when from bellow
            /// So, store previous values for resetting them in the end
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            // Build grid plane
            int color = System.Drawing.Color.FromArgb(90, 150, 150, 150).ToArgb();
            CustomVertex.PositionNormalColored[] rectangle = new CustomVertex.PositionNormalColored[6];
            rectangle[0].Position = origin;                             rectangle[0].Normal = k; rectangle[0].Color = color;
            rectangle[1].Position = origin + w * d * i;                 rectangle[1].Normal = k; rectangle[1].Color = color;
            rectangle[2].Position = origin + w * d * i + h * d * j;     rectangle[2].Normal = k; rectangle[2].Color = color;
            rectangle[3].Position = origin;                             rectangle[3].Normal = k; rectangle[3].Color = color;
            rectangle[4].Position = origin + w * d * i + h * d * j;     rectangle[4].Normal = k; rectangle[4].Color = color;
            rectangle[5].Position = origin + h * d * j;                 rectangle[5].Normal = k; rectangle[5].Color = color;

            lock (device)
            {
                device.BeginScene();
                try
                {
                    // For having the grid plane we need:
                    device.RenderState.CullMode = Cull.Clockwise;                   // Enable Face Culling
                    device.RenderState.Lighting = false;                            // Disable lighting
                    device.RenderState.AlphaBlendEnable = true;                     // Enable alpha blending
                    device.RenderState.SourceBlend = Blend.BothSourceAlpha;
                    device.RenderState.DestinationBlend = Blend.DestinationColor;

                    // Set vertex format and draw grid plane
                    device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, 2, rectangle);

                    // For having contour and gird lines drawn:
                    device.RenderState.AlphaBlendEnable = alphaEnable;              // Disable alpha blending
                    device.RenderState.CullMode = cull;                             // Disable face culling
                }
                finally
                {
                    device.EndScene();
                }
            }
        }

        /// <summary>
        /// Property: Gets/Sets a model renderer for this view
        /// </summary>
        public Canguro.View.Renderer.ModelRenderer ModelRenderer
        {
            get
            {
                return modelRenderer;
            }
            set
            {
                modelRenderer = value;
            }
        }

        /// <summary>
        /// Property: Gets/Sets a viewport for this view. When setting, it also adjust the projection matrix.
        /// </summary>
        public Viewport Viewport
        {
            get
            {
                return viewport;
            }
            set
            {
                viewport = value;
                
                // Recalcular matrices de proyección
                projectionMatrix = Matrix.OrthoRH(viewport.Width * ViewportProjectionScale,
                    viewport.Height * ViewportProjectionScale, -ViewportProjectionZPlane, ViewportProjectionZPlane);
                //projectionMatrix = Matrix.OrthoOffCenterLH(0, viewport.Width * ViewportProjectionScale, -viewport.Height * ViewportProjectionScale, 0, -ViewportProjectionZPlane, ViewportProjectionZPlane);
            }
        }

        /// <summary>
        /// Sirve para modificar la perspetiva
        /// </summary>
        public float FieldOfViewY
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Aplica la transformación métrica
        /// </summary>
        public Microsoft.DirectX.Matrix ViewMatrix
        {
            get
            {
                return viewMatrix;
            }
            set
            {
                viewMatrix = value;
                //viewMatrix = Matrix.LookAtRH(new Vector3(5, 5, 0), new Vector3(5, 5, -1), new Vector3(0, 1, 0)) * value;
            }
        }

        /// <summary>
        /// Sets a flag to enqueue a needed update of the model at the modelRenderer
        /// </summary>
        public void SetUpdateModelFlag()
        {
            updateModelNeeded = true;
        }

        /// <summary>
        /// Sets a flag to enqueue a needed update of all the DirectX Resources, including Vertex Buffers
        /// </summary>
        public void SetUpdateResourcesFlag()
        {
            updateResourcesNeeded = true;
        }

        private bool updateModelNeeded;
        private bool updateResourcesNeeded;

        /// <summary>
        /// Updates model info on the modelRenderer
        /// </summary>
        private void updateModel()
        {
            // Update Model and Model contents
            if (modelRenderer != null)
                modelRenderer.UpdateModel();
            
            updateModelNeeded = false;
        }

        /// <summary>
        /// Updates all DirectX resources in ModelRenderer or its Item Renderers,
        /// including Vertex Buffers.
        /// </summary>
        private void updateResources()
        {
            if (modelRenderer != null)
                modelRenderer.UpdateResources();

            updateResourcesNeeded = false;
        }

        ///// <summary>
        ///// A private property which stores the view matrices used till now
        ///// </summary>
        //private MatrixStack viewHistory;
    }
}


#region Code for making objects transparent
/// We need to enable culling and alpha blending because its intended to have a transparent grid whe viewing from above and nothing when from bellow
///// So, store previous values for resetting them in the end
//Cull cull = device.RenderState.CullMode;
//bool alphaEnable = device.RenderState.AlphaBlendEnable;

//// For having the grid plane we need:
//device.RenderState.CullMode = Cull.Clockwise;                   // Enable Face Culling
//device.RenderState.Lighting = false;                            // Disable lighting
//device.RenderState.AlphaBlendEnable = true;                     // Enable alpha blending
//device.RenderState.SourceBlend = Blend.DestinationAlpha;
//device.RenderState.DestinationBlend = Blend.SourceColor;

//// Call ModelRenderer
//if (modelRenderer != null)
//    modelRenderer.Render(device);

//// For having contour and gird lines drawn:
//device.RenderState.AlphaBlendEnable = alphaEnable;              // Disable alpha blending
//device.RenderState.CullMode = cull;                             // Disable face culling
#endregion