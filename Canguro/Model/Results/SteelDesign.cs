using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results {
    [Serializable]
    public class SteelDesignSummary {
        private string status;
        private float ratio;
        private string errMsg;
        private string warnMsg;
        private string[] designData;

        public string[] DesignData {
            get { return designData; }
            set { designData = value; }
        }

        public string Status {
            get { return (status == null) ? "" : status; }
            set 
            {
                status = value.Replace("See ", "").Replace("ErrMsg", Culture.Get("error")).Replace("WarnMsg", Culture.Get("warning")).Replace("Overstressed", Culture.Get("Overstressed"));
            }
        }        

        public float Ratio {
            get { return ratio; }
            set { ratio = value; }
        }

        public string ErrMsg {
            get { return (errMsg == null) ? "" : errMsg.Replace("; Internal error", " "); }
            set { errMsg = value.Replace("No Messages", ""); }
        }

        public string WarnMsg {
            get { return (warnMsg == null) ? "" : warnMsg; }
            set { warnMsg = value.Replace("No Messages", ""); }
        }

        public string DesignType
        {
            get { return designData[1]; }
            set { designData[1] = value; }
        }

        public string RatioType
        {
            get { return designData[2]; }
            set { designData[2] = value; }
        }

        public string Combo
        {
            get { return designData[3]; }
            set { designData[3] = value; }
        }

        public string Location
        {
            get { return designData[4]; }
            set { designData[4] = value; }
        }
    }

    [Serializable]
    public class SteelDesignPMMDetails {
        private float totalRatio;
        private float pRatio;
        private float mMajRatio;
        private float mMinRatio;
        private string errMsg;
        private string warnMsg;
        private string status;

        private string[] designData;

        public string[] DesignData {
            get { return designData; }
            set { designData = value; }
        }

        public string Status {
            get { return (status == null) ? "" : status; }
            set
            {
                status = value.Replace("See ", "").Replace("ErrMsg", Culture.Get("error")).Replace("WarnMsg", Culture.Get("warning")).Replace("Overstressed", Culture.Get("Overstressed"));
            }
        }        

        public float TotalRatio {
            get { return totalRatio; }
            set { totalRatio = value; }
        }

        public float PRatio {
            get { return pRatio; }
            set { pRatio = value; }
        }

        public float MMajRatio {
            get { return mMajRatio; }
            set { mMajRatio = value; }
        }

        public float MMinRatio {
            get { return mMinRatio; }
            set { mMinRatio = value; }
        }

        public string ErrMsg {
            get { return (errMsg == null) ? "" : errMsg.Replace("; Internal error", " "); }
            set { errMsg = value.Replace("No Messages", ""); }
        }

        public string WarnMsg {
            get { return (warnMsg == null) ? "" : warnMsg; }
            set { warnMsg = value.Replace("No Messages", ""); }
        }

        public string Combo
        {
            get { return designData[2]; }
            set { designData[2] = value; }
        }

        public string Location
        {
            get { return designData[3]; }
            set { designData[3] = value; }
        }
    }

    [Serializable]
    public class SteelDesignShearDetails {
        private float vMajorRatio;
        private float vMinorRatio;
        private string errMsg;
        private string warnMsg;
        private string status;
        private string[] designData;

        public string[] DesignData {
            get { return designData; }
            set { designData = value; }
        }

        public string Status {
            get { return (status == null) ? "" : status; }
            set
            {
                status = value.Replace("See ", "").Replace("ErrMsg", Culture.Get("error")).Replace("WarnMsg", Culture.Get("warning")).Replace("Overstressed", Culture.Get("Overstressed"));
            }
        }        

        public float VMajorRatio {
            get { return vMajorRatio; }
            set { vMajorRatio = value; }
        }

        public float VMinorRatio {
            get { return vMinorRatio; }
            set { vMinorRatio = value; }
        }

        public string ErrMsg {
            get { return (errMsg == null) ? "" : errMsg.Replace("; Internal error", " "); }
            set { errMsg = value.Replace("No Messages", ""); }
        }

        public string WarnMsg {
            get { return (warnMsg == null) ? "" : warnMsg; }
            set { warnMsg = value.Replace("No Messages", ""); }
        }

        public string VMajorCombo
        {
            get { return designData[2]; }
            set { designData[2] = value; }
        }

        public string VMinorCombo
        {
            get { return designData[7]; }
            set { designData[7] = value; }
        }

        public string VMajorLocation
        {
            get { return designData[3]; }
            set { designData[3] = value; }
        }

        public string VMinorLocation
        {
            get { return designData[8]; }
            set { designData[8] = value; }
        }
    }
}
