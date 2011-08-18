using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    public struct VertexBufferData
    {
        /// <summary> Size of vertex buffer</summary>
        public int Size;
        
        /// <summary> The active offset for this vertex buffer </summary>
        public int Offset;
        
        /// <summary> Flush size for vertex buffer </summary>
        public int Flush;

        /// <summary> The number of vertices currently locked </summary>
        public int LockSize;

        /// <summary> The Vertex Buffer </summary>
        public VertexBuffer VertexBuffer;

        /// <summary> The size of the vertex </summary>
        public int StrideSize;

        /// <summary> Determines whether the buffer is currently locked </summary>
        public bool IsLocked;

        /// <summary> The offset to start drawing primitives when autoFlushPrimitives is used </summary>
        public int StartFlushOffset;

        /// <summary> Determines whether the buffer is currently linked to an index buffer </summary>
        public bool UsingIndices;
    }
}
