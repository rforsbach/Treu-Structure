using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class UBC97_LRFD : SteelDesignOptions
    {
        THDesignOptions tHDesign;
        UBCFrameType frameType;
        float patLLF;
        float sRatioLimit;
        uint maxIter;
        float phiB;
        float phiC;
        float phiT;
        float phiV;
        float phiCA;
        bool checkDefl;
        float dLRat;
        float sDLAndLLRat;
        float lLRat;
        float totalRat;
        float netRat;

        SeismicZone seisZone;
        float impFactor;

        public UBC97_LRFD()
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
            phiB = 0.9F;
            phiC = 0.85F;
            phiT = 0.9F;
            phiV = 0.9F;
            phiCA = 0.9F;
            checkDefl = false;
            dLRat = 120;
            sDLAndLLRat = 120;
            lLRat = 360;
            totalRat = 240;
            netRat = 240;

            seisZone = SeismicZone.Zone4;
            impFactor = 1;
        }

        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is UBC97_LRFD)
                CopyFrom((UBC97_LRFD)copy);
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

            AddCombination("1.4DL", D, new float[] { 1.4f });
            AddCombination("1.2DL + 1.6LL", DL, new float[] { 1.2f, 1.6f });
            AddCombination("1.2DL + 0.5LL + 1.3WL", DLW, new float[] { 1.2f, 0.5f, 1.3f });
            AddCombination("1.2DL + 075LL - 1.3WL", DLW, new float[] { 1.2f, 0.5f, -1.3f });
            AddCombination("1.2DL + 0.8WL", DW, new float[] { 1.2f, 0.8f });
            AddCombination("1.2DL - 0.8WL", DW, new float[] { 1.2f, -0.8f });
            AddCombination("0.9DL + 1.3WL", DW, new float[] { 0.9f, 1.3f });
            AddCombination("0.9DL - 1.3WL", DW, new float[] { 0.9f, -1.3f });
            AddCombination("1.2DL + 0.5LL + QL", DLQ, new float[] { 1.2f, 0.5f, 1f });
            AddCombination("1.2DL + 0.5LL - QL", DLQ, new float[] { 1.2f, 0.5f, -1f });
            AddCombination("0.9DL + QL", DQ, new float[] { 0.9f, 1f });
            AddCombination("0.9DL - QL", DQ, new float[] { 0.9f, -1f });

            AddToModel();

            return DesignCombinations;
        }

        public void CopyFrom(UBC97_LRFD copy)
        {
            tHDesign = copy.tHDesign;
            frameType = copy.frameType;
            patLLF = copy.patLLF;
            sRatioLimit = copy.sRatioLimit;
            maxIter = copy.maxIter;
            phiB = copy.phiB;
            phiC = copy.phiC;
            phiT = copy.phiT;
            phiV = copy.phiV;
            checkDefl = copy.checkDefl;
            dLRat = copy.dLRat;
            sDLAndLLRat = copy.sDLAndLLRat;
            lLRat = copy.lLRat;
            totalRat = copy.totalRat;
            netRat = copy.netRat;

            seisZone = copy.seisZone;
            impFactor = copy.impFactor;
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

        public float PhiB
        {
            get { return phiB; }
            set { phiB = value; }
        }

        public float PhiC
        {
            get { return phiC; }
            set { phiC = value; }
        }

        public float PhiT
        {
            get { return phiT; }
            set { phiT = value; }
        }

        public float PhiV
        {
            get { return phiV; }
            set { phiV = value; }
        }

        public float PhiCA
        {
            get { return phiCA; }
            set { phiCA = value; }
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

        public float ImpFactor
        {
            get { return impFactor; }
            set { impFactor = value; }
        }

        public override string ToString()
        {
            return Culture.Get("UBC97_LRFD");
        }
    }
}
