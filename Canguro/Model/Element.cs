using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Clase que abstrae el concepto de elemento. En la primera versión heredan Joint y LineElement
    /// </summary>
    [Serializable]
    public abstract class Element : Item
    {
        protected Load.AssignedLoads loads = null;

        public Element() { }

        internal Element(Element src) : base(src) { }

        /// <summary>
        /// Lo único que tienen en común todos los elementos es que permiten tener cargas. 
        /// El objeto AssignedLoads no se crea hasta que es necesario, de manera que los elementos se crean
        /// más rápido y el programa consume menos memoria antes de que se necesiten las cargas.
        /// </summary>
        [ModelAttributes.GridPosition(20, 80)]
        [System.ComponentModel.DisplayName("elementLoadsProp")]
        public Canguro.Model.Load.AssignedLoads Loads
        {
            get
            {
                if (loads == null)
                    loads = Canguro.Model.Load.AssignedLoads.FactorNewFromType(this);
                    //loads = new Canguro.Model.Load.AssignedLoads(this);
                return loads;
            }
            set
            {
                if (value == loads) return;
                if (loads == null)
                    loads = Canguro.Model.Load.AssignedLoads.FactorNewFromType(this);

                ItemList<Load.Load> copy = value[Canguro.Model.Model.Instance.ActiveLoadCase];
                ItemList<Load.Load> list = loads[Canguro.Model.Model.Instance.ActiveLoadCase];
                if (list != null && list.Count > 0)
                    for (int i = list.Count; i > 0; i--)
                        list.RemoveAt(i-1);
                if (copy != null)
                    foreach (Load.Load l in copy)
                        if (l != null)
                        {
                            Load.Load nl = (Load.Load)l.Clone();
                            nl.Id = 0;
                            loads.Add(nl);
                        }
            }
        }
    }
}
