using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa los casos de Espectro de Respuesta.
    /// Como no se usará el modelo de diafragma rígido, el valor de Diaphragm Eccentricity Ratio = 0
    /// </summary>
    [Serializable]
    public class ResponseSpectrumCaseProps : AnalysisCaseProps
    {
        private ModalCombinationType modalCombinationType = ModalCombinationType.CQC;
        private DirectionalCombinationType directionalCombinationType = DirectionalCombinationType.SRSS;
        private AnalysisCase modalAnalysisCase = null;
        private ResponseSpectrum responseSpectrumFunction = (ResponseSpectrum)Model.Instance.DefaultResponseSpectrum.Clone();
        private float scaleFactor = 1;
        private float modalDamping = 0.05f;

        private List<ResponseSpectrumCaseFactor> loads = new List<ResponseSpectrumCaseFactor>();

        public ResponseSpectrumCaseProps()
        {
        }

        public ResponseSpectrumCaseProps(AccelLoad.AccelLoadValues direction)
        {
            loads.Add(new ResponseSpectrumCaseFactor(new AccelLoad(direction)));
        }

        /// <summary>
        /// Tipo de combinación modal.
        /// Representa la forma en que se combinan los diferentes modos en cada dirección.
        /// </summary>
        public enum ModalCombinationType : byte
        {
            CQC, SRSS, ABS, GMC, Pct10, DblSum,
        }

        /// <summary>
        /// Tipo de combinación direccional.
        /// Representa la forma en que se combinan las direcciones de los modos.
        /// Se aplica después de la combinación modal para obtener un resultado único.
        /// </summary>
        public enum DirectionalCombinationType : byte
        {
            SRSS, ABS,
        }

        /// <summary>
        /// El nombre del tipo de análisis en el idioma local.
        /// </summary>
        public string Name
        {
            get
            {
                return Culture.Get("responseSpectrumName");
            }
        }

        public AnalysisCase DependsOn
        {
            get { return modalAnalysisCase; }
        }

        /// <summary>
        /// Tipo de combinación modal. Es la forma en que se combinarán los modos de vibrar en una sola dirección.
        /// </summary>
        public ModalCombinationType ModalCombination
        {
            get
            {
                return modalCombinationType;
            }
            set
            {
                if (value != modalCombinationType)
                {
                    Model.Instance.Undo.Change(this, modalCombinationType, GetType().GetProperty("ModalCombination"));
                    modalCombinationType = value;
                }
            }
        }

        /// <summary>
        /// Método de combinación direccional de modos. Es la forma en que se combinarán los resultados en cada 
        /// dirección modal para obtener un desultado único.
        /// </summary>
        public DirectionalCombinationType DirectionalCombination
        {
            get
            {
                return directionalCombinationType;
            }
            set
            {
                if (value != directionalCombinationType)
                {
                    Model.Instance.Undo.Change(this, directionalCombinationType, GetType().GetProperty("DirectionalCombination"));
                    directionalCombinationType = value;
                }
            }
        }

        /// <summary>
        /// Caso de análisis modal que se usará como base para el espectro de respuesta.
        /// Es necesario que se use AnalysisCase con ModalCaseProps, ya que los resultados de este AnalysisCase
        /// se obtienen de combinar los resultados de un análisis modal.
        /// </summary>
        public AnalysisCase ModalAnalysisCase
        {
            get
            {
                return modalAnalysisCase;
            }
            set
            {
                if (value != null && value != modalAnalysisCase && value.Properties is ModalCaseProps)
                {
                    Model.Instance.Undo.Change(this, modalAnalysisCase, GetType().GetProperty("ModalAnalysisCase"));
                    modalAnalysisCase = value;
                }
            }
        }

        /// <summary>
        /// Función de espectro de respuesta. Actualmente se representa con string. Se espera que la función exista.
        /// </summary>
        public ResponseSpectrum ResponseSpectrumFunction
        {
            get
            {
                return responseSpectrumFunction;
            }
            set
            {
                if (responseSpectrumFunction != value)
                {
                    Model.Instance.Undo.Change(this, responseSpectrumFunction, GetType().GetProperty("ResponseSpectrumFunction"));
                    responseSpectrumFunction = (ResponseSpectrum)value.Clone();
                }
            }
        }

        /// <summary>
        /// Factor de escala para la función de espectro de respuesta. Tiene que ser un valor positivo.
        /// </summary>
        public float ScaleFactor
        {
            get
            {
                return scaleFactor;
            }
            set
            {
                if (value > 0 && value != scaleFactor)
                {
                    Model.Instance.Undo.Change(this, scaleFactor, GetType().GetProperty("ScaleFactor"));
                    scaleFactor = value;
                }
            }
        }

        /// <summary>
        /// Amortiguamiento modal. Se utiliza en CQC y GMC para combinar los modos.
        /// Este valor es independiente del amortiguamiento de la curva de espectro de respuesta,
        /// aunque en general se usa el mismo valor.
        /// El valor asignado se ajusta para estar en el rango [0,1]
        /// </summary>
        public float ModalDamping
        {
            get
            {
                return modalDamping;
            }
            set
            {
                if (value != modalDamping)
                {
                    value = (value < 0) ? 0 : (value > 1) ? 1 : value;
                    Model.Instance.Undo.Change(this, modalDamping, GetType().GetProperty("ModalDamping"));
                    modalDamping = value;
                }
            }
        }


        /// <summary>
        /// LoadCase list. It's copied when it's read or set.
        /// AccelLoad's
        /// </summary>
        public List<ResponseSpectrumCaseFactor> Loads
        {
            get
            {
                return new List<ResponseSpectrumCaseFactor>(loads);
            }
            set
            {
                Model.Instance.Undo.Change(this, loads, this.GetType().GetProperty("Loads"));
                loads = new List<ResponseSpectrumCaseFactor>();
                foreach (ResponseSpectrumCaseFactor f in value)
                {
                    if (f.Accel is AccelLoad)
                        loads.Add(f);
                }
            }
        }

    }
}
