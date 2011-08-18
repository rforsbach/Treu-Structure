using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Canguro.Model;
using Canguro.Model.Load;
using Canguro.Utility;

namespace Canguro.Analysis
{
    internal abstract class ModelCalculator
    {
        protected Canguro.Model.Model model
        {
            get { return Canguro.Model.Model.Instance; }
        }

        protected Vector3 toLocal(LineElement line, Vector3 v)
        {
            Matrix r;

            line.RotationMatrix(out r);
            return Vector3.TransformCoordinate(v, Matrix.TransposeMatrix(r));
        }

        // Get Load direction in Local Coordinate frame
        protected Vector3 getLocalDir(LineElement line, LineLoad.LoadDirection direction)
        {
            Vector3 dir = Vector3.Empty;
            switch (direction)
            {
                case LineLoad.LoadDirection.GlobalX:
                    dir = toLocal(line, CommonAxes.GlobalAxes[0]);
                    break;
                case LineLoad.LoadDirection.GlobalY:
                    dir = toLocal(line, CommonAxes.GlobalAxes[1]);
                    break;
                case LineLoad.LoadDirection.GlobalZ:
                    dir = toLocal(line, CommonAxes.GlobalAxes[2]);
                    break;
                case LineLoad.LoadDirection.Gravity:
                    dir = Vector3.Scale(toLocal(line, CommonAxes.GlobalAxes[2]), -1);
                    break;
                case LineLoad.LoadDirection.Local1:
                    dir = CommonAxes.GlobalAxes[0];
                    break;
                case LineLoad.LoadDirection.Local2:
                    dir = CommonAxes.GlobalAxes[1];
                    break;
                case LineLoad.LoadDirection.Local3:
                    dir = CommonAxes.GlobalAxes[2];
                    break;
                default:
                    break;
            }

            return dir;
        }
    }
}
