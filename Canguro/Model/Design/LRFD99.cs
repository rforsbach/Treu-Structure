using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class LRFD99 : SteelDesignOptions
    {
        THDesignOptions tHDesign;
        FrameTypeOptions frameType;
        float patLLF;
        float sRatioLimit;
        uint maxIter;
        float phiB;
        float phiC;
        float phiTY;
        float phiV;
        float phiTF;
        float phiVT;
        float phiCA;
        bool checkDefl;
        float dLRat;
        float sDLAndLLRat;
        float lLRat;
        float totalRat;
        float netRat;

        char seisCat;
        bool seisCode;
        bool seisLoad;
        bool plugWeld;

        public LRFD99()
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
            tHDesign = THDesignOptions.Envelopes;
            frameType = FrameTypeOptions.MomentFrame;
            patLLF = 0.75F;
            sRatioLimit = 0.95F;
            maxIter = 1;
            phiB = 0.9F;
            phiC = 0.85F;
            phiTY = 0.9F;
            phiV = 0.9F;
            phiTF = 0.75F;
            phiVT = 0.75F;
            phiCA = 0.9F;
            checkDefl = false;
            dLRat = 120;
            sDLAndLLRat = 120;
            lLRat = 360;
            totalRat = 240;
            netRat = 240;

            seisCat = 'D';
            seisCode = false;
            seisLoad = false;
            plugWeld = true;
        }

        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is LRFD99)
                CopyFrom((LRFD99)copy);
        }

        public void CopyFrom(LRFD99 copy)
        {
            tHDesign = copy.tHDesign;
            frameType = copy.frameType;
            patLLF = copy.patLLF;
            sRatioLimit = copy.sRatioLimit;
            maxIter = copy.maxIter;
            phiB = copy.phiB;
            phiC = copy.phiC;
            phiTY = copy.phiTY;
            phiV = copy.phiV;
            phiTF = copy.phiTF;
            phiVT = copy.phiVT;
            phiCA = copy.phiCA;
            checkDefl = copy.checkDefl;
            dLRat = copy.dLRat;
            sDLAndLLRat = copy.sDLAndLLRat;
            lLRat = copy.lLRat;
            totalRat = copy.totalRat;
            netRat = copy.netRat;

            seisCat = copy.seisCat;
            seisCode = copy.seisCode;
            seisLoad = copy.seisLoad;
            plugWeld = copy.plugWeld;
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
            AddCombination("1.2DL + LL + 1.6WL", DLW, new float[] { 1.2f, 1f, 1.6f });
            AddCombination("1.2DL + LL - 1.6WL", DLW, new float[] { 1.2f, 1f, -1.6f });
            AddCombination("1.2DL + 0.8WL", DW, new float[] { 1.2f, 0.8f });
            AddCombination("1.2DL - 0.8WL", DW, new float[] { 1.2f, -0.8f });
            AddCombination("0.9DL + 1.6WL", DW, new float[] { 0.9f, 1.6f });
            AddCombination("0.9DL - 1.6WL", DW, new float[] { 0.9f, -1.6f });
            AddCombination("1.2DL + LL + QL", DLQ, new float[] { 1.2f, 1f, 1f });
            AddCombination("1.2DL + LL - QL", DLQ, new float[] { 1.2f, 1f, -1f });
            AddCombination("0.9DL + QL", DQ, new float[] { 0.9f, 1f });
            AddCombination("0.9DL - QL", DQ, new float[] { 0.9f, -1f });

            AddToModel();

            return designCombinations;
        }

        [System.ComponentModel.Browsable(false)]
        public THDesignOptions TimeHistoryDesign
        {
            get { return tHDesign; }
            set { tHDesign = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public FrameTypeOptions FrameType
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

        public float PhiTY
        {
            get { return phiTY; }
            set { phiTY = value; }
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

        public float PhiTF
        {
            get { return phiTF; }
            set { phiTF = value; }
        }

        public float PhiVT
        {
            get { return phiVT; }
            set { phiVT = value; }
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

        public char SeisCat
        {
            get { return seisCat; }
            set 
            { 
                value = char.ToUpper(value);
                seisCat = (value >= 'A' && value <= 'F') ? value : seisCat;
            }
        }

        public bool SeisCode
        {
            get { return seisCode; }
            set { seisCode = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public bool SeisLoad
        {
            get { return seisLoad; }
            set { seisLoad = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public bool PlugWeld
        {
            get { return plugWeld; }
            set { plugWeld = value; }
        }

        public override string ToString()
        {
            return Culture.Get("AISC_LRFD99");
        }
    }
}
