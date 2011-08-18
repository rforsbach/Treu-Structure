using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa una combinación (Combo) de casos de análisis abstractos. 
    /// Así, esta clase forma un Composite.
    /// </summary>
    [Serializable]
    public class LoadCombination : AbstractCase
    {
        private CombinationType combinationType = CombinationType.LinearAdd;
        private List<AbstractCaseFactor> cases = new List<AbstractCaseFactor>();

        /// <summary>
        /// Constructora que asigna el nombre.
        /// </summary>
        /// <param name="name"></param>
        public LoadCombination(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Enumeración de los tipos de combinación admitidos.
        /// </summary>
        public enum CombinationType
        {
            LinearAdd, Envelope, AbsoluteAdd, SRSS,
        }

        /// <summary>
        /// Lista de AbstractCases y respectivos factores.
        /// Es de sólo lectura, por lo que no se incluye en la función Undo.
        /// </summary>
        public List<AbstractCaseFactor> Cases
        {
            get
            {
                return cases;
            }
        }

        /// <summary>
        /// Tipo de combinación. Especifica la forma en que se combinarán los estados de análisis.
        /// </summary>
        public CombinationType Type
        {
            get
            {
                return combinationType;
            }
            set
            {
                if (value != combinationType)
                {
                    Model.Instance.Undo.Change(this, combinationType, GetType().GetProperty("Type"));
                    combinationType = value;
                }
            }
        }
    }
}
