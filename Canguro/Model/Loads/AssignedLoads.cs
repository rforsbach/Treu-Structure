using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa la relación entre Items, Loads y LoadCases
    /// </summary>
    [Serializable]
    public class AssignedLoads
    {
        protected Item item;
        private Dictionary<LoadCase, ItemList<Load>> loads;

        /// <summary>
        /// Constructora que asigna el Item, que no se puede cambiar.
        /// El objeto loads no se crea hasta que sea necesario.
        /// </summary>
        /// <param name="item"></param>
        public AssignedLoads(Item item)
        {
            this.item = item;
            loads = null;
        }

        /// <summary>
        /// Wrapper de Dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ItemList<Load> this[LoadCase key]
        {
            get
            {
                if (loads != null && key != null && loads.ContainsKey(key))
                    return loads[key];
                return null;
            }
        }

        /// <summary>
        /// Agrega una carga a un LoadCase específico
        /// </summary>
        /// <param name="load"></param>
        /// <param name="loadCase"></param>
        public void Add(Load load, LoadCase loadCase)
        {
            if (loads == null)
                loads = new Dictionary<LoadCase, ItemList<Load>>();

            ItemList<Load> list = this[loadCase];
            if (list == null)
            {
                list = new ItemList<Load>();
                Model.Instance.Undo.Add(loadCase, list, loads);
                loads.Add(loadCase, list);
            }
            Model.Instance.Undo.Add(load, list);
            list.Add(load);
        }

        /// <summary>
        /// Agrega una carga al LoadCase activo
        /// </summary>
        /// <param name="load"></param>
        public void Add(Load load)
        {
            Add(load, Model.Instance.ActiveLoadCase);
        }

        /// <summary>
        /// Quita una carga del LoadCase activo
        /// </summary>
        /// <param name="load"></param>
        public void Remove(Load load)
        {
            Remove(load, Model.Instance.ActiveLoadCase);
        }

        /// <summary>
        /// Quita una carga de un LoadCase específico
        /// Si la carga no está registrada, se lanza una InvalidIndexException
        /// </summary>
        /// <param name="load"></param>
        /// <param name="loadCase"></param>
        public void Remove(Load load, LoadCase loadCase)
        {
            if (loads == null || !loads.ContainsKey(loadCase))
                throw new InvalidIndexException();
            ItemList<Load> list = loads[loadCase];

            if (!list.Contains(load))
                throw new InvalidIndexException();
            Model.Instance.Undo.Remove(load, list);
            list.Remove(load);

            if (list.Count == 0)
            {
                Model.Instance.Undo.Remove(loadCase, list, loads);
                loads.Remove(loadCase);
            }

        }

        /// <summary>
        /// Elimina un LoadCase,
        /// Si no está registrado, se ignora
        /// </summary>
        /// <param name="loadCase"></param>
        public void Remove(LoadCase loadCase)
        {
            if (loads == null || !loads.ContainsKey(loadCase))
                return;
            ItemList<Load> list = loads[loadCase];

            Model.Instance.Undo.Remove(loadCase, list, loads);
            loads.Remove(loadCase);
        }

        public static AssignedLoads FactorNewFromType(Element element)
        {
            if (element != null)
            {
                Type type = element.GetType();

                if (type == typeof(Joint))
                    return new AssignedJointLoads(element);
                else if (type == typeof(LineElement))
                    return new AssignedLineLoads(element);
                else if (type == typeof(AreaElement))
                    return new AssignedAreaLoads(element);
            }
            
            return null;
        }

        public override string ToString()
        {
            int num = 0;
            if ((loads != null) && (loads.ContainsKey(Model.Instance.ActiveLoadCase)))
                foreach (Load l in loads[Model.Instance.ActiveLoadCase])
                    if (l != null) 
                        num++;
            return num.ToString() + " " + Culture.Get("loads");
        }

        /// <summary>
        /// Sets the object to use only LoadCases in the current Model object.
        /// </summary>
        internal void Repair()
        {
            if (loads != null && loads.Count > 0)
            {
                Dictionary<string, LoadCase> lCases = Model.Instance.LoadCases;
                List<LoadCase> list = new List<LoadCase>(loads.Keys);
                foreach (LoadCase lc in list)
                {
                    if (lc != null && lCases.ContainsKey(lc.Name) && lc != lCases[lc.Name])
                    {
                        loads.Add(lCases[lc.Name], loads[lc]);
                        loads.Remove(lc);
                    }
                }
            }
        }
    }
}
