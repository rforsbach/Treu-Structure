using System;
using System.Collections.Generic;

namespace Canguro.Model.Results
{
    public class ResultsCase
    {
        private int id;
        private string name;
        private string fullPath;
        private bool isLoaded;
        Load.AbstractCase abstractCase;

        public ResultsCase(int id, string fullPathOrBreadCrumb)
        {
            // Set as root
            if (string.IsNullOrEmpty(fullPathOrBreadCrumb))
                throw new ArgumentException("The fullpath isn't valid");

            this.id = id;
            FullPath = fullPathOrBreadCrumb;
            this.abstractCase = null;
            isLoaded = false;
        }

        public int Id
        {
            get { return id; }
        }

        public Load.AbstractCase AbstractCase
        {
            get 
            {
                if (abstractCase == null)
                {
                    // Find the abstract case
                    string root = ResultsPath.FirstPart(fullPath);

                    foreach (Canguro.Model.Load.AbstractCase ac in Canguro.Model.Model.Instance.AbstractCases)
                        if (ac.Name.Equals(root, StringComparison.OrdinalIgnoreCase))
                        {
                            abstractCase = ac;
                            break;
                        }
                }

                return abstractCase; 
            }
            set { abstractCase = value; }
        }

        /// <summary>
        /// Gets or sets whether this ResultsCase has been completely downloaded and thus, its
        /// related data is valid.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }          
       
        public string FullPath
        {
            get { return fullPath; }
            private set
            {
                fullPath = value.Replace('~', '/');
                name = null;
            }
        }
        
        /// <summary>
        /// Gets the name of the ResultsCase.
        /// For example: "Modal (Mode 1: 0.54s)"
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = string.Empty;
                    string[] path = fullPath.Split('/');

                    name = path[0];
                    if (path.Length > 1)
                    {
                        name += " - ";

                        for (int i = 1; i < path.Length; i++)
                            name += path[i] + ' ';

                        name = name.Substring(0, name.Length - 1);
                    }
                }

                return name;
            }
        }

        public override string ToString()
        {
            Canguro.Model.Load.AnalysisCase ac = AbstractCase as Canguro.Model.Load.AnalysisCase;
            if (ac != null && ac.Properties != null && ac.Properties is Canguro.Model.Load.ModalCaseProps)
            {
                // Fetch Period
                Results results = Canguro.Model.Model.Instance.Results;
                if (results != null)
                {
                    float[] period = results.GetModalPeriods(this);

                    if (period != null)
                        return Name + " (" + period[0].ToString("G5") + Canguro.Model.Model.Instance.UnitSystem.UnitName(Canguro.Model.UnitSystem.Units.Time) + ")";
                }
            }


            return Name;
        }
    }
}
