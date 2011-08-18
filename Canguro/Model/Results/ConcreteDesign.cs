using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    [Serializable]
    public class ConcreteColumnDesign
    {
        //public Section.Section designSect;
        //public DesignType designType;
        //public DesignOpt designOpt;
        //public DesignStatus status;
        //public float location;
        //public float pmmArea;
        //public float pmmRatio;
        //public int vMajCombo;
        //public int vMajRebar;
        //public int vMinCombo;
        //public int vMinRebar;
        //public int error;
        //public int warning;
        //private Canguro.Model.Load.LoadCombination pmmCombo;


        private string status;
        private float pMMArea;
        private string pMMRatio;
        private float vMajRebar;
        private float vMinRebar;
        private string errMsg;
        private string warnMsg;
        private string[] designData;

        public string Status {
            get { return (status == null) ? "" : status; }
            set
            {
                status = value.Replace("See ", "").Replace("ErrMsg", Culture.Get("error")).Replace("WarnMsg", Culture.Get("warning")).Replace("Overstressed", Culture.Get("Overstressed"));
            }
        }

        public float PMMArea
        {
            get { return pMMArea; }
            set { pMMArea = value; }
        }

        public string PMMRatio
        {
            get { return (pMMRatio == null) ? "" : pMMRatio; }
            set { pMMRatio = value; }
        }

        public float VMajRebar
        {
            get { return vMajRebar; }
            set { vMajRebar = value; }
        }

        public float VMinRebar
        {
            get { return vMinRebar; }
            set { vMinRebar = value; }
        }

        public string ErrMsg {
            get { return (errMsg == null) ? "" : errMsg.Replace("; Internal error", " "); }
            set { errMsg = value.Replace("No Messages", ""); }
        }

        public string WarnMsg {
            get { return (warnMsg == null) ? "" : warnMsg; }
            set { warnMsg = value.Replace("No Messages", ""); }
        }

        public string[] DesignData
        {
            get { return designData; }
            set { designData = value; }
        }

        public string VMajCombo
        {
            get { return designData[4]; }
            set { designData[4] = value; }
        }

        public string VMinCombo
        {
            get { return designData[5]; }
            set { designData[5] = value; }
        }
    }

    [Serializable]
    public class ConcreteBeamDesign
    {
        private string status;
        private float fTopArea;
        private float fBotArea;
        private float vRebar;
        private string errMsg;
        private string warnMsg;
        private string[] designData;

        public string Status
        {
            get { return (status == null) ? "" : status; }
            set
            {
                status = value.Replace("See ", "").Replace("ErrMsg", Culture.Get("error")).Replace("WarnMsg", Culture.Get("warning")).Replace("Overstressed", Culture.Get("Overstressed"));
            }
        }

        public float FTopArea
        {
            get { return fTopArea; }
            set { fTopArea = value; }
        }

        public float FBotArea
        {
            get { return fBotArea; }
            set { fBotArea = value; }
        }

        public float VRebar
        {
            get { return vRebar; }
            set { vRebar = value; }
        }
        
        public string[] DesignData
        {
            get { return designData; }
            set { designData = value; }
        }

        public string ErrMsg
        {
            get { return (errMsg == null) ? "" : errMsg.Replace("; Internal error", " "); }
            set { errMsg = value.Replace("No Messages", ""); }
        }

        public string WarnMsg
        {
            get { return (warnMsg == null) ? "" : warnMsg; }
            set { warnMsg = value.Replace("No Messages", ""); }
        }

        public string FTopCombo
        {
            get { return designData[2]; }
            set { designData[2] = value; }
        }

        public string FBotCombo
        {
            get { return designData[3]; }
            set { designData[3] = value; }
        }

        public string VCombo
        {
            get { return designData[4]; }
            set { designData[4] = value; }
        }

        //public Section.Section designSect;
        //public DesignType designType;
        //public DesignStatus status;
        //public float location;
        //public int error;
        //public int warning;
        //private Canguro.Model.Load.LoadCombination fTopCombo;
        //private float fTopArea;
        //private Canguro.Model.Load.LoadCombination fBotCombo;
        //private float fBotArea;
        //private Canguro.Model.Load.LoadCombination vCombo;
        //private float vRebar;
        //private Canguro.Model.Load.LoadCombination tLngCombo;
        //private float tLngArea;
        //private Canguro.Model.Load.LoadCombination tTrnCombo;
        //private float tTrnRebar;
    }
}
