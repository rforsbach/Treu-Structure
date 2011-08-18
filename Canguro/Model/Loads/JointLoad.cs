using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Generalización de Cargas en nodos.
    /// Guarda las 6 componentes de la carga (6 DoFs)
    /// </summary>
    [Serializable]
    public class JointLoad : Load
    {
        protected float[] loadComponents;

        /// <summary>
        /// Constructora que inicializa el arreglo loadComponents con valores de 0.
        /// </summary>
        public JointLoad()
        {
            loadComponents = new float[6];
            loadComponents[0] = 0;
            loadComponents[1] = 0;
            loadComponents[2] = 0;
            loadComponents[3] = 0;
            loadComponents[4] = 0;
            loadComponents[5] = 0;
        }

        public override object Clone()
        {
            object o = base.Clone();
            ((JointLoad)o).loadComponents = (float[])loadComponents.Clone();

            return o;
        }
    }
}
