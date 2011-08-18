using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.UnitSystem
{
    /// <summary>
    /// Clase Singleton para administrar los sistemas de unidades.
    /// Guarda una lista de UnitSystem, uno de cada clase.
    /// Además mantiene uno como activo.
    /// </summary>
    sealed public class UnitSystemsManager
    {
        /// <summary>
        /// Constructora que agrega a la lista un UnitSystem de cada clase.
        /// </summary>
        private UnitSystemsManager()
        {
            currentSystem = lastSystem = MetricSystem.Instance;
            unitSystems.Add(InternationalSystem.Instance);
            unitSystems.Add(MetricSystem.Instance);
            unitSystems.Add(EnglishSystem.Instance);
        }

        public static readonly UnitSystemsManager Instance = new UnitSystemsManager();
        private List<UnitSystem> unitSystems = new List<UnitSystem>();
        private UnitSystem currentSystem;
        private UnitSystem lastSystem;
        private bool enabled = true;

        /// <summary>
        /// Regresa la lista de sistemas de unidades en modo de sólo lectura.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<UnitSystem> UnitSystems
        {
            get
            {
                return unitSystems.AsReadOnly();
            }
        }

        public UnitSystem CurrentSystem
        {
            get
            {
                return currentSystem;
            }
            set
            {
                currentSystem = value;
                Properties.Settings.Default.UnitSystem = value.GetType().Name;
                Properties.Settings.Default.Save();
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    if (value)
                        currentSystem = lastSystem;
                    else
                    {
                        lastSystem = currentSystem;
                        currentSystem = InternationalSystem.Instance;
                    }
                    enabled = value;
                }
            }
        }
    }
}
