using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    public class DownloadProps : ICloneable
    {
        private uint percentage;
        private bool finished;
        private string fileName;
        private string caseName;

        public DownloadProps() { }
        public DownloadProps(CanguroServer.ManifestItem item)
        {
            LoadManifestItem(item);
        }

        public string CaseName
        {
            get { return caseName; }
            set { caseName = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public uint Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }

        public bool Finished
        {
            get { return finished; }
            set
            {
                finished = value;
                if (finished)
                    Canguro.Model.Model.Instance.NewResults();
            }
        }

        public void LoadManifestItem(CanguroServer.ManifestItem item)
        {
            fileName = item.FilePath;
            caseName = ResultsPath.Format(Utility.AnalysisUtils.DecodeStream(item.Description));
        }

        #region ICloneable Members

        /// <summary>
        /// Método heredado de IClonable
        /// </summary>
        /// <returns>Regresa una copia superficial (completa en este caso) de sí mismo</returns>
        public object Clone()
        {
            DownloadProps dp = new DownloadProps();
            dp.caseName = caseName;
            dp.fileName = fileName;
            dp.finished = finished;
            dp.percentage = percentage;

            return dp;
        }

        #endregion
    }
}
