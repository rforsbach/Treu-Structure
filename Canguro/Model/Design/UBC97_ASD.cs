using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class UBC97_ASD : SteelDesignOptions
    {
        THDesignOptions tHDesign;
        UBCFrameType frameType;
        float patLLF;
        float sRatioLimit;
        uint maxIter;
        SeismicZone seisZone;
        float latFactor;

        bool checkDefl;

        float dLRat;
        float sDLAndLLRat;
        float lLRat;
        float totalRat;
        float netRat;

        public UBC97_ASD()
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
            tHDesign = THDesignOptions.Envelopes;
            frameType = UBCFrameType.MomentFrame;
            patLLF = 0.75F;
            sRatioLimit = 0.95F;
            maxIter = 1;
            checkDefl = false;
            dLRat = 120;
            sDLAndLLRat = 120;
            lLRat = 360;
            totalRat = 240;
            netRat = 240;

            seisZone = SeismicZone.Zone4;
            latFactor = 1;
        }

        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is UBC97_ASD)
                CopyFrom((UBC97_ASD)copy);
        }

        public void CopyFrom(UBC97_ASD copy)
        {
            tHDesign = copy.tHDesign;
            frameType = copy.frameType;
            patLLF = copy.patLLF;
            sRatioLimit = copy.sRatioLimit;
            maxIter = copy.maxIter;
            checkDefl = copy.checkDefl;
            dLRat = copy.dLRat;
            sDLAndLLRat = copy.sDLAndLLRat;
            lLRat = copy.lLRat;
            totalRat = copy.totalRat;
            netRat = copy.netRat;
            latFactor = copy.latFactor;
            seisZone = copy.seisZone;
        }

        public override List<LoadCombination> AddDefaultCombos()
        {
            designCombinations = new List<LoadCombination>();

            LoadCase.LoadCaseType[] D = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead };
            LoadCase.LoadCaseType[] DL = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live };
            LoadCase.LoadCaseType[] DLW = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live, LoadCase.LoadCaseType.Wind };
            LoadCase.LoadCaseType[] DLQ = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live, LoadCase.LoadCaseType.Quake };
            LoadCase.LoadCaseType[] DW = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Wind };
            LoadCase.LoadCaseType[] DQ = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Quake };

            AddCombination("DL", D, new float[] { 1 });
            AddCombination("DL + LL", DL, new float[] { 1, 1 });
            AddCombination("DL + 0.75LL + 0.75WL", DLW, new float[] { 1, 0.75f, 0.75f });
            AddCombination("DL + 0.75LL - 0.75WL", DLW, new float[] { 1, 0.75f, -0.75f });
            AddCombination("DL + WL", DW, new float[] { 1, 1 });
            AddCombination("DL - WL", DW, new float[] { 1, -1 });
            AddCombination("0.6DL + WL", DW, new float[] { 0.6f, 1 });
            AddCombination("0.6DL - WL", DW, new float[] { 0.6f, -1 });
            AddCombination("DL + 0.75LL + 0.5357QL", DLQ, new float[] { 1, 0.75f, 0.5357f });
            AddCombination("DL + 0.75LL - 0.5357QL", DLQ, new float[] { 1, 0.75f, -0.5357f });
            AddCombination("DL + 0.7143QL", DQ, new float[] { 1, 0.7143f });
            AddCombination("DL - 0.7143QL", DQ, new float[] { 1, -0.7143f });
            AddCombination("0.9DL + 0.7143QL", DQ, new float[] { 0.9f, 0.7143f });
            AddCombination("0.9DL - 0.7143QL", DQ, new float[] { 0.9f, -0.7143f });

            AddToModel();

            return DesignCombinations;
        }

        [System.ComponentModel.Browsable(false)]
        public THDesignOptions TimeHistoryDesign
        {
            get { return tHDesign; }
            set { tHDesign = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public UBCFrameType FrameType
        {
            get { return frameType; }
            set { frameType = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public float PatLLF
        {
            get { return patLLF; }
            set { patLLF = value; }
        }

        public float SRatioLimit
        {
            get { return sRatioLimit; }
            set { sRatioLimit = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public uint MaxIter
        {
            get { return maxIter; }
            set { maxIter = value; }
        }

        public bool CheckDefl
        {
            get { return checkDefl; }
            set { checkDefl = value; }
        }

        public float DLRat
        {
            get { return dLRat; }
            set { dLRat = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public float SDLAndLLRat
        {
            get { return sDLAndLLRat; }
            set { sDLAndLLRat = value; }
        }

        public float LLRat
        {
            get { return lLRat; }
            set { lLRat = value; }
        }

        public float LatFactor
        {
            get { return latFactor; }
            set { latFactor = value; }
        }

        public float TotalRat
        {
            get { return totalRat; }
            set { totalRat = value; }
        }

        public float NetRat
        {
            get { return netRat; }
            set { netRat = value; }
        }

        public SeismicZone SeisZone
        {
            get { return seisZone; }
            set { seisZone = value; }
        }

        public override string ToString()
        {
            return Culture.Get("UBC97_ASD");
        }
    }
}
