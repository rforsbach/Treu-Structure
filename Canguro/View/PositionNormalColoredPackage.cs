using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    public class PositionNormalColoredPackage : Canguro.View.ResourcePackage
    {
        public unsafe CustomVertex.PositionNormalColored* VBPointer;
    }
}
