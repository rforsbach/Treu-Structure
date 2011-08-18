using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    public abstract class ResourcePackage
    {
        // Vertex Buffer data
        public int Offset;
        public int NumVertices;
        public int NumIndices;
        //public IndexBuffer IndexBuffer;
        public ResourceStreamType Stream;

        public int StartVBFlushOffset;

        // Index Buffer data
        public unsafe short* IBPointer;
        public int IBOffset;

        public int NumPrimitives
        {
            get
            {
                unsafe
                {
                    if (IBPointer == (int*)null)
                        return NumPrimitivesVB;
                    else
                        return NumPrimitivesIB;
                }
            }
        }
        
        public int NumPrimitivesVB
        {
            get
            {
                switch (Stream)
                {
                    case ResourceStreamType.Points:
                        return NumVertices;
                    case ResourceStreamType.Lines:
                        return NumVertices / 2;
                    default:
                    //case ResourceStream.TriangleListPositionColored:
                    //case ResourceStream.TriangleListPositionNormalColored:
                        return NumVertices / 3;
                }
            }
        }

        public int NumPrimitivesIB
        {
            get
            {
                switch (Stream)
                {
                    case ResourceStreamType.Points:
                        return NumIndices;
                    case ResourceStreamType.Lines:
                        return NumIndices / 2;
                    default:
                        //case ResourceStream.TriangleListPositionColored:
                        //case ResourceStream.TriangleListPositionNormalColored:
                        return NumIndices / 3;
                }
            }
        }
    }
}
