using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class ASD01 : SteelDesignOptions
    {
        THDesignOptions tHDesign;
        FrameTypeOptions frameType;
        float patLLF;
        float sRatioLimit;
        uint maxIter;
        char seisCat;
        bool seisCode;
        bool seisLoad;
        bool plugWeld;
        bool checkDefl;

        float dLRat;
        float sDLAndLLRat;
        float lLRat;
        float totalRat;
        float netRat;

        public ASD01()
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
            if (copy is ASD01)
                CopyFrom((ASD01)copy);
        }

        public void CopyFrom(ASD01 copy)
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

            AddCombination("DL", D, new float[] { 1 });
            AddCombination("DL + LL", DL, new float[] { 1, 1 });
            AddCombination("DL + 0.75LL + 0.75WL", DLW, new float[] { 1, 0.75f, 0.75f });
            AddCombination("DL + 0.75LL - 0.75WL", DLW, new float[] { 1, 0.75f, -0.75f });
            AddCombination("DL + WL", DW, new float[] { 1, 1 });
            AddCombination("DL - WL", DW, new float[] { 1, -1 });
            AddCombination("0.6DL + WL", DW, new float[] { 0.6f, 1 });
            AddCombination("0.6DL - WL", DW, new float[] { 0.6f, -1 });
            AddCombination("DL + 0.75LL + 0.525QL", DLQ, new float[] { 1, 0.75f, 0.525f });
            AddCombination("DL + 0.75LL - 0.525QL", DLQ, new float[] { 1, 0.75f, -0.525f });
            AddCombination("DL + 0.7QL", DQ, new float[] { 1, 0.7f });
            AddCombination("DL - 0.7QL", DQ, new float[] { 1, -0.7f });
            AddCombination("0.6DL + 0.7QL", DQ, new float[] { 0.6f, 0.7f });
            AddCombination("0.6DL - 0.7QL", DQ, new float[] { 0.6f, -0.7f });

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
            return Culture.Get("AISC_ASD01");
        }
    }
}
