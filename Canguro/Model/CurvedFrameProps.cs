using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class CurvedFrameProps : LineProps
    {
        public enum CurveType
        {
            Circular3rdPoint,
            Circular3rdJoint,
            CircularRadius,
            Parabolic3rdPoint,
            Parabolic3rdJoint
        }

        public Section.FrameSection Section
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public CurveType CurveForm
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Segments
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
