using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que define las propiedades de un caso de análisis modal.
    /// No se toman en cuenta todas las propiedades. Para el caso de eigenvectores, se asumen los defaults de SAP:
    /// Frecuency Shift = 0
    /// Cutoff Frequency = 0
    /// Convergence Tolerance = 1.0E-9
    /// Allow Automatic Frequency Shifting = false
    /// Loads Applied = void
    /// Para el caso de vectore de Ritz se aplican siempre las cargas de aceleración en los 3 ejes:
    /// ACCEL : UX, UY, UZ 
    /// Max Cycles = 0 
    /// Target Dynamic Participation Ratios (%) = 99
    /// 
    /// En futuras versiones se agregarán los campos necesarios.
    /// </summary>
    [Serializable]
    public class ModalCaseProps : AnalysisCaseProps
    {
        private uint maxModes = 12; // Valor default en SAP.
        private uint minModes = 1; // En general no es necesario cambiar este parámetro.
        private ModesMethod modesType = ModesMethod.RitzVectors; // Se recomienda el uso de vectores de Ritz

        private List<ModalCaseFactor> loads = new List<ModalCaseFactor>();

        /// <summary>
        /// Constructor. Initializes the loads with the recommended values: UX+UY+UZ.
        /// </summary>
        public ModalCaseProps()
        {
            loads.Add(new ModalCaseFactor(new AccelLoad(AccelLoad.AccelLoadValues.UX)));
            loads.Add(new ModalCaseFactor(new AccelLoad(AccelLoad.AccelLoadValues.UY)));
            loads.Add(new ModalCaseFactor(new AccelLoad(AccelLoad.AccelLoadValues.UZ)));
        }

        /// <summary>
        /// LoadCase or Accel list with factors. It's copied when it's read or set.
        /// Applies only when using Ritz vectors
        /// </summary>
        public List<ModalCaseFactor> Loads
        {
            get
            {
                return new List<ModalCaseFactor>(loads);
            }
            set
            {
                Model.Instance.Undo.Change(this, loads, this.GetType().GetProperty("Loads"));
                loads = new List<ModalCaseFactor>();
                foreach (ModalCaseFactor f in value)
                {
                    if (f.AppliedLoad is LoadCase || f.AppliedLoad is AccelLoad) // || l is Link (ver 2)
                        loads.Add(f);
                }
            }
        }

        public enum ModesMethod : byte
        {
            EigenVectors,
            RitzVectors,
        }

        /// <summary>
        /// Propiedad de solo lectura que regresa el nombre del tipo de caso en el idioma local.
        /// </summary>
        public string Name
        {
            get
            {
                return Culture.Get("modalCaseName");
            }
        }

        public AnalysisCase DependsOn
        {
            get { return null; }
        }

        /// <summary>
        /// Máximo número de modos de vibrar a buscar.
        /// </summary>
        public uint MaxModes
        {
            get
            {
                return maxModes;
            }
            set
            {
                if (maxModes != value && value >= minModes && value <= 1000)
                {
                    Model.Instance.Undo.Change(this, maxModes, GetType().GetProperty("MaxModes"));
                    maxModes = value;
                }
            }

        }

        /// <summary>
        /// Mínimo número de modos de vibrar a buscar.
        /// </summary>
        public uint MinModes
        {
            get
            {
                return minModes;
            }
            set
            {
                if (minModes != value && value > 0 && value < maxModes)
                {
                    Model.Instance.Undo.Change(this, minModes, GetType().GetProperty("MinModes"));
                    minModes = value;
                }
            }

        }

        /// <summary>
        /// Propiedad que define el método de buscar modos de vibrar: eigenvectores (Subspace Iteration),
        /// o vectores de Ritz (Load Dependent Ritz Vectors).
        /// </summary>
        public ModesMethod ModesType
        {
            get
            {
                return modesType;
            }
            set
            {
                if (modesType != value)
                {
                    Model.Instance.Undo.Change(this, modesType, GetType().GetProperty("ModesType"));
                    modesType = value;
                }
            }

        }
    }
}
