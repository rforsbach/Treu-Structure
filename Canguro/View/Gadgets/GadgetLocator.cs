using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View
{
    public struct GadgetLocator
    {
        public int Offset;
        public int Size;
    }

    public struct GadgetLODLocator
    {
        public GadgetLocator verticesLocator;
        public int[] indexOffsets;
        public int totalIndices;
    }
}
