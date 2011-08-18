using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Utility;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa un estado de carga.
    /// Sólo guarda el id, nombre y factor de peso propio.
    /// La relación con cargas e Items la genera AssignedLoads.
    /// </summary>
    [Serializable]
    public class LoadCase : GlobalizedObject, AnalysisCaseAppliedLoad, INamed
    {
        /// <summary>
        /// Tipo de estado de carga. Se usa principalmente para el diseño y para la forma de combinarlos.
        /// </summary>
        public enum LoadCaseType : byte
        {
            Dead, /*SuperDead,*/ Live, ReduceLive, Quake, Wind, Snow, Wave, Other,
        }

        private float selfWeight = 0;
        private string name;
        private LoadCaseType caseType;

        /// <summary>
        /// Constructora que asigna el nombre al LoadCase.
        /// Asegura que el nombre no esté en uso.
        /// </summary>
        /// <param name="name"></param>
        public LoadCase(string name, LoadCaseType caseType)
        {
            Name = name;
            this.caseType = caseType;
        }

        /// <summary>
        /// Tipo de estado de carga. Se usa para el análisis y para hacer combinaciones.
        /// </summary>
        public LoadCaseType CaseType
        {
            get
            {
                return caseType;
            }
            set
            {
                if (caseType != value)
                {
                    Model.Instance.Undo.Change(this, caseType, GetType().GetProperty("CaseType"));
                    caseType = value;
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string AutoLoad
        {
            get
            {
                switch (caseType)
                {
                    case LoadCaseType.Quake:
                    case LoadCaseType.Wave:
                    case LoadCaseType.Wind:
                        return "None";
                    default:
                        return string.Empty;
                }
            }
            set
            {
            }
        }

        public override string ToString()
        {
            return Name + " (" + Culture.Get(CaseType.ToString()) + ")";
        }

        /// <summary>
        /// Factor de peso propio del LoadCase.
        /// </summary>
        public float SelfWeight
        {
            get
            {
                return selfWeight;
            }
            set
            {
                Model.Instance.Undo.Change(this, selfWeight, GetType().GetProperty("SelfWeight"));
                selfWeight = value;
            }
        }

        /// <summary>
        /// Nombre del LoadCase. No se puede repetir.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                value = value.Trim().Replace("\"", "''");
                value = (value.Length > 0) ? value : Culture.Get("Case");
                string aux = value;
                bool ok = false;
                int i = 0;
                Dictionary<string, LoadCase> dic = Model.Instance.LoadCases;
                while (!ok)
                {
                    ok = true;
                    foreach (string lc in dic.Keys)
                        if (lc.Equals(aux))
                            ok = false;
                    if (!ok)
                        aux = value + "(" + ++i + ")";
                }
                Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                if (name != null && dic.ContainsKey(name) && dic[name] == this)
                {
                    dic.Remove(name);
                    dic.Add(aux, this);
                }
                name = aux;
            }
        }
    }
}
