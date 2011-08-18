using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    public class ResultsCasesList : List<ResultsCase>
    {
        Dictionary<string, ResultsCase> resultsCaseDictionary;

        public ResultsCasesList()
        {
            resultsCaseDictionary = new Dictionary<string, ResultsCase>();
        }

        public new void Add(ResultsCase rc)
        {
            while (rc.Id > Count - 1)
            {
                base.Add(null);
            }
            this[rc.Id] = rc;
            if (!resultsCaseDictionary.ContainsKey(rc.FullPath))
                resultsCaseDictionary.Add(rc.FullPath, rc);
            else
                resultsCaseDictionary[rc.FullPath] = rc;
        }

        public void Add(string resultsPath)
        {
            ResultsCase rc = new ResultsCase(Count, resultsPath);
            base.Add(rc);
            resultsCaseDictionary.Add(resultsPath, rc);
        }

        public ResultsCase GetResultsCase(string resultsPath)
        {
            if (resultsCaseDictionary.ContainsKey(resultsPath))
                return resultsCaseDictionary[resultsPath];

            return null;
        }

        public ResultsCase GetResultsCase(int id)
        {
            return base[id];
        }

        public ResultsCase FindPath(string resultsPath)
        {
            foreach (ResultsCase rc in this)
            {
                if (ResultsPath.Contains(rc.FullPath, resultsPath))
                    return rc;
            }

            return null;
        }

        public ResultsCase this[string resultsPath]
        {
            get { return GetResultsCase(resultsPath); }
        }

        public ResultsCase this[int id]
        {
            get { return GetResultsCase(id); }
            internal set
            {
                if (id == value.Id)
                    base[id] = value;
            }
        }
    }
}
