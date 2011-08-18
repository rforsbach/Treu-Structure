using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    /// <summary>
    /// Stores any DirectX resource used for rendering purposes
    /// </summary>
    public class ResourceManager
    {
        #region Fields
        /// <summary> List containing all vertex buffers used by the application </summary>
        private VertexBufferData[] vertexBuffers;
        /// <summary> List containing all index buffers used by the application </summary>
        private IndexBufferData[] indexBuffers;

        /// <summary> The rendering device instantiated by the application </summary>
        private Device device = null;
        /// <summary> A surface used for picking calculations </summary>
        private Surface pickSurface = null;
        /// <summary> A surface used for painting the current tracking selection and snap actions </summary>
        private Surface trackingSurface = null;
        /// <summary> Array of surfaces used to cache the animation frames and display them faster </summary>
        private Surface[] animationCache = null;
        /// <summary> An array of lines which works as helpers for snap actions </summary>
        private Line[] snapLines = null;
        /// <summary> List of picked items </summary>
        private List<Canguro.Model.Item> pickItems = null;

        bool lastAutoFlush = false;
        private ResourceStreamType activeStream;

        // Extrusion buffer admin variables
        /// <summary> Tells if the process to extrude a shape has finished and system can present results </summary>
        private bool extrusionEnded = false;
        /// <summary> Tells if the process to extrude a shape has finished and system can present results </summary>
        public bool ExtrusionEnded
        {
            get { return extrusionEnded; }
            set { extrusionEnded = value; }
        }

        /// <summary> Vertex Buffer recommended size </summary>
        public const int RecommendedVbSize = 40000;
        /// <summary> Flush recommended size </summary>
        public const int RecommendedFlushSize = 5000;

        /// <summary> Font used for displaying any text on screen </summary>
        private Font labelFont = null;
        /// <summary> Public property: Gets the font resource </summary>
        public Font LabelFont
        {
            get { return labelFont; }
        }
        #endregion

        private const int minimumIndices = 120;
        private const int minimumVertices = 120;
        internal const int AnimationFrames = 17;

        #region Creation and Disposing
        /// <summary>
        /// For almost any resource, its needed the rendering device for attaching to it
        /// </summary>
        /// <param name="device"> Rendering device </param>
        public void SetDevice(Device device)
        {
            // Whenever it s needed to set a device, resources have to be disposed
            disposeResources();

            // Set device
            this.device = device;
            if (device != null)
            {   
                // Create vertex and index buffer lists
                vertexBuffers = new VertexBufferData[4];
                indexBuffers = new IndexBufferData[4];

                int maxPrimCount;

                if(device.DeviceCaps.MaxPrimitiveCount > 0)
                    maxPrimCount = device.DeviceCaps.MaxPrimitiveCount;
                else
                    maxPrimCount = 40000;

                // Get a recommended value size base in device capabilities
                int maxRecommendedSize = Math.Min(maxPrimCount / 4, RecommendedVbSize);
                int maxRecommendedFlush = Math.Min(maxRecommendedSize / 4, RecommendedFlushSize);

                // Make Buffers
                vertexBuffers[(int)ResourceStreamType.Points] = genVB(maxRecommendedSize, maxRecommendedFlush, ResourceStreamType.Points);
                vertexBuffers[(int)ResourceStreamType.Lines]  = genVB(maxRecommendedSize, maxRecommendedFlush, ResourceStreamType.Lines);
                vertexBuffers[(int)ResourceStreamType.TriangleListPositionColored] = genVB(maxRecommendedSize, maxRecommendedFlush, ResourceStreamType.TriangleListPositionColored);
                vertexBuffers[(int)ResourceStreamType.TriangleListPositionNormalColored] = genVB(maxRecommendedSize, maxRecommendedFlush, ResourceStreamType.TriangleListPositionNormalColored);

                indexBuffers[(int)ResourceStreamType.Points] = genIB(maxRecommendedSize, maxRecommendedFlush);
                indexBuffers[(int)ResourceStreamType.Lines] = genIB(maxRecommendedSize, maxRecommendedFlush);
                indexBuffers[(int)ResourceStreamType.TriangleListPositionColored] = genIB(maxRecommendedSize, maxRecommendedFlush);
                indexBuffers[(int)ResourceStreamType.TriangleListPositionNormalColored] = genIB(maxRecommendedSize, maxRecommendedFlush);                

                // Create the font: Arial 8
                System.Drawing.Font systemFont = new System.Drawing.Font("Arial", 8.0f, System.Drawing.FontStyle.Regular);
                labelFont = new Font(device, systemFont);
                labelFont.PreloadCharacters(0, 255);
            }
        }

        /// <summary> Dispose every resource created by the resource cache </summary>
        private void disposeResources()
        {
            // Check if label font is valid
            if (labelFont != null)
            {
                // Then disposte it
                labelFont.Dispose();
                // and make it point to null
                labelFont = null;
            }
            // Check if VB list have valid values
            if (vertexBuffers != null)
            {
                // Dispose each of the vertex buffers
                foreach (VertexBufferData vb in vertexBuffers)
                    if (vb.VertexBuffer != null)
                        vb.VertexBuffer.Dispose();                            
            }

            if (indexBuffers != null)
            {
                foreach (IndexBufferData ib in indexBuffers)
                    if (ib.IndexBuffer != null)
                        ib.IndexBuffer.Dispose();
            }

            // Dispose animation cache
            disposeAnimationCache();

            // Dispose picking surface
            if (pickSurface != null)
            {
                pickSurface.Dispose();
                pickSurface = null;
            }

            // Dispose tracking surface
            if (trackingSurface != null)
            {
                trackingSurface.Dispose();
                trackingSurface = null;
            }
            
            // Dispose any snap lines
            if (snapLines != null)
            {
                foreach (Line l in snapLines)
                    l.Dispose();
                snapLines = null;
            }

            // Reset indices of the picked items
            ResetPickIndices();
        }

        private void disposeAnimationCache()
        {
            if (animationCache != null)
            {
                int frameCount = animationCache.Length;
                for (int i = 0; i < frameCount; i++)
                {
                    if (animationCache[i] != null && !animationCache[i].Disposed)
                        animationCache[i].Dispose();
                    animationCache[i] = null;
                }

                animationCache = null;
            }
        }

        public void ResetAnimationCache()
        {
            disposeAnimationCache();
        }
        #endregion

        #region Device Events
        /// <summary>
        /// Subscription to device disposing event
        /// </summary>
        /// <param name="sender"> Device being disposed </param>
        /// <param name="e"> Event arguments </param>
        public void device_Disposing(object sender, EventArgs e)
        {
            // Call dispose resources method
            disposeResources();
        }

        public void device_DeviceReset(object sender, EventArgs e)
        {
            if (gadgetManager != null)
                gadgetManager.Reset();

            ResetAnimationCache();
        }
        #endregion

        /// <summary> Creates a VB </summary>
        /// <param name="vbSize"> Vertices in VertexBuffer </param>
        /// <param name="vbFlushSize"> Vertices to lock and flush when rendering or adding data </param>
        /// <param name="type"> VertexBuffer Kind, can be any of ResourceStreamType (Points, lines, triangles with/without normals) </param>
        /// <returns> The created Vertex Buffer </returns>
        private VertexBufferData genVB(int vbSize, int vbFlushSize, ResourceStreamType streamType)
        {
            Type vbType;
            VertexFormats vbFormat;
            VertexBufferData vbData = new VertexBufferData();

            // Get VB Type
            switch(streamType)
            {
                case ResourceStreamType.TriangleListPositionNormalColored:
                    vbType = typeof(CustomVertex.PositionNormalColored);
                    vbFormat = CustomVertex.PositionNormalColored.Format;
                    vbData.StrideSize = CustomVertex.PositionNormalColored.StrideSize;
                    break;
                default:
                //case ResourceStreamType.Points:
                //case ResourceStreamType.Lines:
                //case ResourceStreamType.TriangleListPositionColored:
                    vbType = typeof(CustomVertex.PositionColored);
                    vbFormat = CustomVertex.PositionColored.Format;
                    vbData.StrideSize = CustomVertex.PositionColored.StrideSize;
                    break;
            }

            try
            {
                // Set state variables for this buffer
                vbData.Size = vbSize;
                vbData.IsLocked = false;
                vbData.Flush = vbFlushSize;
                vbData.Offset = vbData.StartFlushOffset = 0;

                // All buffers are built in Default pool memory and as write only and dynamic
                vbData.VertexBuffer = new VertexBuffer(vbType, vbData.Size, device, Usage.WriteOnly | Usage.Dynamic, vbFormat, Pool.Default);
            }
            catch (OutOfMemoryException e)
            {
                // Si no hubo memoria suficiente tratar con la mitad recursivamente
                if (vbData.Size > minimumVertices)
                    vbData = genVB(vbData.Size / 2, vbData.Flush / 2, streamType);
                else
                    throw e;    // Si es muy poca la memoria el programa no puede funcionar
            }
            return vbData;
        }

        /// <summary> Creates an IndexBuffer </summary>
        /// <param name="ibSize"> IB size </param>
        /// <param name="ibFlushSize"> IB flush size </param>
        /// <returns> The created IndexBuffer </returns>
        private IndexBufferData genIB(int ibSize, int ibFlushSize)
        {
            IndexBufferData ibData = new IndexBufferData();

            try
            {
                // Set state variables for this buffer
                ibData.Size = ibSize;
                ibData.IsLocked = false;
                ibData.Flush = ibFlushSize;
                ibData.Offset = ibData.StartFlushOffset = 0;

                // All buffers are built in Default pool memory and as write only and dynamic
                ibData.IndexBuffer = new IndexBuffer(typeof(short), ibData.Size, device, Usage.WriteOnly | Usage.Dynamic, Pool.Default);
            }
            catch (OutOfMemoryException e)
            {
                // Si no hubo memoria suficiente tratar con la mitad recursivamente
                if (ibData.Size > minimumIndices)
                    ibData = genIB(ibData.Size / 2, ibData.Flush / 2);
                else
                    throw e;    // Si es muy poca la memoria el programa no puede funcionar
            }
            return ibData;
        }        

        #region Tracking Surface property
        public Surface TrackingSurface
        {
            get 
            { 
                if( trackingSurface == null || trackingSurface.Disposed)
                    trackingSurface = device.CreateRenderTarget(device.Viewport.Width, device.Viewport.Height, /*device.GetBackBuffer(0,0,BackBufferType.Mono).Description.Format*/ device.DisplayMode.Format, MultiSampleType.None, 0, false);

                return trackingSurface;
            }
        }
        #endregion

        #region Picking Surface
        public Surface PickingSurface
        {
            get
            {
                if (pickSurface == null || pickSurface.Disposed)
                {
                    
                    pickSurface = device.CreateOffscreenPlainSurface(device.CreationParameters.FocusWindow.Width, device.CreationParameters.FocusWindow.Height, device.DisplayMode.Format, Pool.SystemMemory);
                }

                return pickSurface;
            }
        }

        public int GetNextPickIndex(Canguro.Model.Item item)
        {
            if (pickItems == null)
                ResetPickIndices();
            
            pickItems.Add(item);
            return pickItems.Count;
        }

        public void ResetPickIndices()
        {
            int capacity = 0;
            if (Canguro.Model.Model.Instance.JointList != null)
                capacity += Canguro.Model.Model.Instance.JointList.Count;
            if (Canguro.Model.Model.Instance.LineList != null)
                capacity += Canguro.Model.Model.Instance.LineList.Count;
            if (Canguro.Model.Model.Instance.AreaList != null)
                capacity += Canguro.Model.Model.Instance.AreaList.Count;

            if (pickItems != null)
            {
                pickItems.Clear();

                if (pickItems.Capacity < capacity)
                    pickItems.Capacity = capacity;
            }
            else
                pickItems = new List<Canguro.Model.Item>(capacity);
        }

        public const int InverseAlphaMask = 0x00FFFFFF;
        public Canguro.Model.Item PickedItem(int index)
        {
            index &= InverseAlphaMask;
            if (index > 0 && index <= pickItems.Count)
                return pickItems[index - 1];

            return null;
        }
        #endregion                                

        #region Animation Surfaces
        public Surface CreateAnimationFrame(int frameNumber)
        {
            if (frameNumber < 0 || frameNumber >= AnimationFrames)
                throw new ArgumentOutOfRangeException("The frameNumber " + frameNumber.ToString() + " does not exist");

            Surface s = AnimationCache[frameNumber];

            if (s != null && !s.Disposed)
            {
                s.Dispose();
                //throw new InvalidCallException("Cannot create a frame that's already been created. Use ResetAnimationCache first");
            }

            animationCache[frameNumber] = s = device.CreateRenderTarget(device.Viewport.Width, device.Viewport.Height, device.GetBackBuffer(0, 0, BackBufferType.Mono).Description.Format, MultiSampleType.None, 0, false);

            return s;
        }

        public Surface[] AnimationCache
        {
            get
            {
                if (animationCache == null)
                    animationCache = new Surface[17];

                return animationCache;
            }
        }
        #endregion

        #region Snap
        public Line[] SnapLines
        {
            get
            {
                if (snapLines == null)
                {
                    snapLines = new Line[3];

                    snapLines[0] = new Line(device);
                    snapLines[0].Width = 2;
                    snapLines[1] = new Line(device);
                    snapLines[1].PatternScale = 1.0f;
                    snapLines[1].Width = 1;
                    snapLines[2] = new Line(device);
                    snapLines[2].Width = 5;
                }
                return snapLines;
            }
        }
        #endregion

        #region Gadgets
        /// <summary>
        /// The Gadget Manager. Handles the life cycle of the gadgets in video memory and its resources
        /// </summary>
        private Gadgets.GadgetManager gadgetManager = null;

        public Gadgets.GadgetManager GadgetManager
        {
            get 
            {
                if (gadgetManager == null)
                    gadgetManager = new Gadgets.GadgetManager(this);

                return gadgetManager; 
            }
        }
        #endregion

        #region Vertex Buffers Management
        public ResourcePackage CaptureBuffer(ResourceStreamType streamType, bool useIndices, bool autoFlushVertices)
        {
            return CaptureBuffer(streamType, ((useIndices) ? 1 : 0), 1, autoFlushVertices);
        }

        public ResourcePackage CaptureBuffer(ResourceStreamType streamType, int minimumIndices, int minimumVertices, bool autoFlushVertices)
        {
            if (minimumVertices < 1)
                throw new ArgumentOutOfRangeException();

            bool bufferSizeOverflow = false;
            ResourcePackage package;
            GraphicsStream vbStream, ibStream;
            VertexBufferData vbd = vertexBuffers[(int)streamType];
            IndexBufferData ibd = indexBuffers[(int)streamType];

            // If changing from indexed buffer to simple vertex buffer or vice versa, force flush
            flush(vbd, ibd, streamType, ((minimumIndices > 0) ^ vbd.UsingIndices), autoFlushVertices);

            int lockSize = Math.Min(Math.Max(minimumVertices, vbd.Flush), vbd.Size);
            int lockIndicesSize = Math.Min(Math.Max(minimumIndices, ibd.Flush), ibd.Size); ;

            #region Check if Flush is necessary

            if ((vbd.Offset + lockSize >= vbd.Size) || (minimumIndices > 0) && (ibd.Offset + lockIndicesSize >= ibd.Size))
            {
                Flush(streamType);
                GadgetManager.Reset();
                bufferSizeOverflow = true;
            }

            if (bufferSizeOverflow)
            {
                vertexBuffers[(int)streamType].Offset = 0;
                vertexBuffers[(int)streamType].StartFlushOffset = 0;
                indexBuffers[(int)streamType].Offset = 0;
                indexBuffers[(int)streamType].StartFlushOffset = 0;
            }

            #endregion

            #region VertexBuffer Lock
            vbStream = vbd.VertexBuffer.Lock(vertexBuffers[(int)streamType].Offset * vbd.StrideSize, lockSize * vbd.StrideSize, (!bufferSizeOverflow) ? LockFlags.NoOverwrite : LockFlags.Discard);

            vertexBuffers[(int)streamType].IsLocked = true;
            vertexBuffers[(int)streamType].LockSize = lockSize;
            if (!autoFlushVertices)
                vertexBuffers[(int)streamType].StartFlushOffset = vertexBuffers[(int)streamType].Offset + lockSize;
           
            switch (streamType)
            {
                case ResourceStreamType.TriangleListPositionNormalColored:
                    PositionNormalColoredPackage pncp = new PositionNormalColoredPackage();
                    package = pncp;
                    unsafe
                    {
                        pncp.VBPointer = (CustomVertex.PositionNormalColored *)vbStream.InternalDataPointer;
                    }
                    break;
                default:
                    //case ResourceStreamType.Points:
                    //case ResourceStreamType.Lines:
                    //case ResourceStreamType.TriangleListPositionColored:
                    PositionColoredPackage pcp = new PositionColoredPackage();
                    package = pcp;
                    unsafe
                    {
                        pcp.VBPointer = (CustomVertex.PositionColored*)vbStream.InternalDataPointer;
                    }
                    break;
            }
            #endregion

            #region IndexBuffer Lock
            if (minimumIndices > 0)
            {
                ibStream = ibd.IndexBuffer.Lock(indexBuffers[(int)streamType].Offset * sizeof(short), lockIndicesSize * sizeof(short), (!bufferSizeOverflow) ? LockFlags.NoOverwrite : LockFlags.Discard);

                indexBuffers[(int)streamType].IsLocked = true;
                indexBuffers[(int)streamType].LockSize = lockIndicesSize;
                if (!autoFlushVertices)
                    indexBuffers[(int)streamType].StartFlushOffset = indexBuffers[(int)streamType].Offset + lockIndicesSize;

                unsafe
                {
                    package.IBPointer = (short*)ibStream.InternalDataPointer;
                }

                package.IBOffset = indexBuffers[(int)streamType].Offset;
            }
            else
            {
                package.IBOffset = 0;
                unsafe
                {
                    package.IBPointer = (short*)null;
                }
            }
            #endregion

            vertexBuffers[(int)streamType].UsingIndices = (minimumIndices > 0);
            package.Offset = vertexBuffers[(int)streamType].Offset;
            package.Stream = streamType;
            package.NumVertices = lockSize;
            package.NumIndices = lockIndicesSize;
            package.StartVBFlushOffset = vertexBuffers[(int)streamType].StartFlushOffset;

            // If not enough memory could be allocated, then throw an exception
            if (package.NumVertices < minimumVertices)
                throw new OutOfMemoryException("Cannot get a vertex buffer of the specified size, probably because it's more than the maximum allowed (" + vbd.Size + ")");
            if (package.NumIndices < minimumIndices)
                throw new OutOfMemoryException("Cannot get an index buffer of the specified size, probably because it's more than the maximum allowed (" + ibd.Size + ")");

            return package;
        }

        public void Flush(ResourceStreamType streamType)
        {
            flush(vertexBuffers[(int)streamType], indexBuffers[(int)streamType], streamType, true, false);
        }
        
        private void flush(VertexBufferData vbd, IndexBufferData ibd, ResourceStreamType streamType, bool forceFlush, bool autoFlushVertices)
        {
            int vertices = vbd.Offset - vbd.StartFlushOffset;
            int indices = ibd.Offset - ibd.StartFlushOffset;

            // Cannot get the vertex buffer if it is currently locked
            if (vbd.IsLocked)
                throw new InvalidCallException("Cannot get or flush a vertex buffer which is currently locked. Release it before getting a new one or flushing it.");
            // Cannot get the index buffer if it is currently locked
            if (vbd.UsingIndices && ibd.IsLocked)
                throw new InvalidCallException("Cannot get or flush an index buffer which is currently locked. Release it before getting a new one or flushing it.");

            // Draw pending primitives before acquiring a new lock
            if (forceFlush || (lastAutoFlush && (!autoFlushVertices || 
                ((vertices) > RecommendedFlushSize) || (vbd.UsingIndices && ((indices) > RecommendedFlushSize)))))
            {
                if (!vbd.UsingIndices)
                {
                    if (vertices > 0)
                    {
                        switch (streamType)
                        {
                            case ResourceStreamType.Points:
                                ActiveStream = ResourceStreamType.Points;
                                device.DrawPrimitives(PrimitiveType.PointList, vbd.StartFlushOffset, vertices);
                                break;
                            case ResourceStreamType.Lines:
                                ActiveStream = ResourceStreamType.Lines;
                                device.DrawPrimitives(PrimitiveType.LineList, vbd.StartFlushOffset, vertices / 2);
                                break;
                            case ResourceStreamType.TriangleListPositionColored:
                                ActiveStream = ResourceStreamType.TriangleListPositionColored;
                                device.DrawPrimitives(PrimitiveType.TriangleList, vbd.StartFlushOffset, vertices / 3);
                                break;
                            case ResourceStreamType.TriangleListPositionNormalColored:
                                ActiveStream = ResourceStreamType.TriangleListPositionNormalColored;
                                device.DrawPrimitives(PrimitiveType.TriangleList, vbd.StartFlushOffset, vertices / 3);
                                break;
                        }
                    }

                    vertexBuffers[(int)streamType].StartFlushOffset = vbd.Offset;
                }
                else
                {
                    if (vertices > 0 && indices > 0)
                    {
                        switch (streamType)
                        {
                            case ResourceStreamType.Points:
                                throw new InvalidCallException("Cannot flush points using indices.");
                            case ResourceStreamType.Lines:
                                ActiveStream = ResourceStreamType.Lines;
                                device.DrawIndexedPrimitives(PrimitiveType.LineList, vbd.StartFlushOffset, 0, vertices, ibd.StartFlushOffset, indices/2);
                                break;
                            case ResourceStreamType.TriangleListPositionColored:
                                ActiveStream = ResourceStreamType.TriangleListPositionColored;
                                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, vbd.StartFlushOffset, 0, vertices, ibd.StartFlushOffset, indices/3);
                                break;
                            case ResourceStreamType.TriangleListPositionNormalColored:
                                ActiveStream = ResourceStreamType.TriangleListPositionNormalColored;
                                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, vbd.StartFlushOffset, 0, vertices, ibd.StartFlushOffset, indices/3);
                                break;
                        }
                    }

                    vertexBuffers[(int)streamType].StartFlushOffset = vbd.Offset;
                    indexBuffers[(int)streamType].StartFlushOffset = ibd.Offset;
                }
            }

            lastAutoFlush = autoFlushVertices;
        }

        public void ReleaseBuffer(int numVerticesDrawn, int numIndicesDrawn, ResourceStreamType streamType)
        {
            VertexBufferData vbd = vertexBuffers[(int)streamType];
            IndexBufferData ibd = indexBuffers[(int)streamType];

            #region Release VB
            if (!vbd.IsLocked)
                throw new InvalidCallException("Cannot release an unlocked vertex buffer");

            vbd.VertexBuffer.Unlock();

            vertexBuffers[(int)streamType].Offset += numVerticesDrawn;
            vertexBuffers[(int)streamType].IsLocked = false;

            if (vertexBuffers[(int)streamType].Offset >= vbd.Size)
                throw new IndexOutOfRangeException("More vertices were sent than the size of the buffer: " + streamType);
            #endregion

            #region Release IndexBuffer
            if (vbd.UsingIndices)
            {
                if (!ibd.IsLocked)
                    throw new InvalidCallException("Cannot release an unlocked index buffer");

                ibd.IndexBuffer.Unlock();

                indexBuffers[(int)streamType].Offset += numIndicesDrawn;
                indexBuffers[(int)streamType].IsLocked = false;

                if (indexBuffers[(int)streamType].Offset >= ibd.Size)
                    throw new IndexOutOfRangeException("More indices were sent than the size of the buffer: " + streamType);
            }
            #endregion
        }
        #endregion

        public ResourceStreamType ActiveStream
        {
            get { return activeStream; }
            set
            {
                //if (activeStream != value)
                {
                    activeStream = value;
                    switch (activeStream)
                    {
                        case ResourceStreamType.TriangleListPositionNormalColored:
                            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                            break;
                        default:
                        //case ResourceStream.Points:
                        //case ResourceStream.Lines:
                        //case ResourceStream.TriangleListPositionColored:
                            device.VertexFormat = CustomVertex.PositionColored.Format;
                            break;
                    }
                    device.Indices = indexBuffers[(int)activeStream].IndexBuffer;
                    device.SetStreamSource(0, vertexBuffers[(int)activeStream].VertexBuffer, 0);
                }
            }
        }        
    }
}
