using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que abstrae los conceptos de AnalysisCase y LoadCombination. 
    /// Cada AbstractCase representa un caso a analizar.
    /// El concepto de AbstractCase se refiere más a los resultados que al modelo.
    /// </summary>
    [Serializable]
    public abstract class AbstractCase : Canguro.Utility.GlobalizedObject, INamed
    {
        private string name = "";
        private bool isActive = true;

        /// <summary>
        /// Constructor that assigns a name
        /// </summary>
        /// <param name="name">The name for the Abstract case. Has to be unique.</param>
        public AbstractCase(string name)
        {
            Name = name;
        }


        /// <summary>
        /// This property defines if the AbstractCase should be analyzed.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive != value)
                {
                    Model.Instance.Undo.Change(this, isActive, GetType().GetProperty("IsActive"));
                    isActive = value;

                    if (isActive && this is AnalysisCase)
                    {
                        AnalysisCase ac = ((AnalysisCase)this).Properties.DependsOn;
                        if (ac != null)
                            ac.IsActive = true;
                    }
                    else if (isActive && this is LoadCombination)
                    {
                        foreach (AbstractCaseFactor acf in ((LoadCombination)this).Cases)
                        {
                            if (acf.Case != null)
                                acf.Case.IsActive = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Propiedad en común que tienen los AbstractCases es el nombre.
        /// Éste debe ser único, ya que es el identificador del caso.
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
                Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                IList<AbstractCase> list = Model.Instance.AbstractCases;
                bool valid = false;
                string tmp = value;
                int i = 0;
                while (!valid)
                {
                    valid = true;
                    
                    // The '~' character is reserved as the breadCrymb delimiter in ResultsCases
                    if (value.Contains("~"))
                        value.Replace('~', '-');

                    foreach (AbstractCase ac in list)
                        if (string.Compare(ac.name, tmp, true) == 0 && ac != this)
                            valid = false;
                    if (!valid)
                        tmp = value + "(" + i++ + ")";
                }
                if (valid)
                    name = tmp;
            }
        }

        /// <summary>
        /// Returns the name of the Case
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
