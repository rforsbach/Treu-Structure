using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que se usa para relacionar un Caso de análisis (o combinación) con un factor de escala.
    /// Se usa en LoadCombination para guardar una lista de estas relaciones.
    /// </summary>
    [Serializable]
    public struct AbstractCaseFactor
    {
        private AbstractCase aCase;
        private float factor;

        /// <summary>
        /// Constructora que asigna el par caso-factor.
        /// Se llama a las propiedades para asegurar consistencia.
        /// </summary>
        /// <param name="abstractCase"></param>
        /// <param name="factor"></param>
        public AbstractCaseFactor(AbstractCase abstractCase, float factor)
        {
            aCase = abstractCase;
            this.factor = factor;
            Case = abstractCase;
            Factor = factor;
        }

        /// <summary>
        /// Constructora que asigna el valor default 1 como factor.
        /// </summary>
        /// <param name="abstractCase"></param>
        public AbstractCaseFactor(AbstractCase abstractCase)
            : this(abstractCase, 1)
        {
        }

        /// <summary>
        /// Propiedad que representa el estado de carga. 
        /// Si el estado que se quiere asignar es null, se ignora la llamada.
        /// Si el estado no pertenece al modelo, se lanza una InvalidCallException.
        /// </summary>
        public AbstractCase Case
        {
            get
            {
                return aCase;
            }
            set
            {
                if (value != null && value != aCase)
                {
                    Model.Instance.Undo.Change(this, aCase, GetType().GetProperty("Case"));
                    if (!Model.Instance.AbstractCases.Contains(value))
                        throw new InvalidCallException(Culture.Get("EML0001"));
                    aCase = value;
                }
            }
        }

        /// <summary>
        /// El factor de escala para el AbstractCase.
        /// </summary>
        public float Factor
        {
            get
            {
                return factor;
            }
            set
            {
                if (value != factor)
                {
                    Model.Instance.Undo.Change(this, factor, GetType().GetProperty("Factor"));
                    factor = value;
                }
            }
        }
    }
}
