using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class UBC97_Conc : ConcreteDesignOptions
    {
        THDesignOptions tHDesign;
        uint numCurves;
        uint numPoints;
        bool minEccen;
        float patLLF;
        float uFLimit;
        float phiB;
        float phiCTied;
        float phiCSpiral;
        float phiV;

        public UBC97_Conc()
        {
            SetDefaults();
        }

        public override void SetDefaults()
        {
            tHDesign = THDesignOptions.Envelopes;
            numCurves = 24;
            numPoints = 11;
            minEccen = true;
            patLLF = 0.75F;
            uFLimit = 0.95F;
            phiB = 0.9F;
            phiCTied = 0.7F;
            phiCSpiral = 0.75F;
            phiV = 0.85F;
        }

        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is UBC97_Conc)
                CopyFrom((UBC97_Conc)copy);
        }

        public override List<Canguro.Model.Load.LoadCombination> AddDefaultCombos()
        {
            designCombinations = new List<LoadCombination>();

            LoadCase.LoadCaseType[] D = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead };
            LoadCase.LoadCaseType[] DL = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live };
            LoadCase.LoadCaseType[] DLW = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live, LoadCase.LoadCaseType.Wind };
            LoadCase.LoadCaseType[] DLQ = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Live, LoadCase.LoadCaseType.Quake };
            LoadCase.LoadCaseType[] DW = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Wind };
            LoadCase.LoadCaseType[] DQ = new LoadCase.LoadCaseType[] { LoadCase.LoadCaseType.Dead, LoadCase.LoadCaseType.Quake };

            AddCombination("1.4DL", D, new float[] { 1.4f });
            AddCombination("1.4DL + 1.7LL", DL, new float[] { 1.4f, 1.7f });
            AddCombination("1.05DL + 1.275LL + 1.275WL", DLW, new float[] { 1.05f, 1.275f, 1.275f });
            AddCombination("1.05DL + 1.275LL - 1.275WL", DLW, new float[] { 1.05f, 1.275f, -1.275f });
            AddCombination("0.9DL + 1.3WL", DW, new float[] { 0.9f, 1.3f });
            AddCombination("0.9DL - 1.3WL", DW, new float[] { 0.9f, -1.3f });
            AddCombination("1.05DL + 1.275LL + 1.4025QL", DLQ, new float[] { 1.05f, 1.275f, 1.4025f });
            AddCombination("0.05DL + 1.275LL - 1.4025QL", DLQ, new float[] { 1.05f, 1.275f, -1.4025f });
            AddCombination("0.9DL + 1.43QL", DQ, new float[] { 0.9f, 1.43f });
            AddCombination("0.9DL - 1.43QL", DQ, new float[] { 0.9f, -1.43f });

            AddToModel();

            return DesignCombinations;
        }

        public void CopyFrom(UBC97_Conc copy)
        {
            tHDesign = copy.tHDesign;
            numCurves = copy.numCurves;
            numPoints = copy.numPoints;
            minEccen = copy.minEccen;
            patLLF = copy.patLLF;
            uFLimit = copy.uFLimit;
            phiB = copy.phiB;
            phiCTied = copy.phiCTied;
            phiCSpiral = copy.phiCSpiral;
            phiV = copy.phiV;
        }

        [System.ComponentModel.Browsable(false)]
        public THDesignOptions THDesign
        {
            get { return tHDesign; }
            set { tHDesign = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public uint NumCurves
        {
            get { return numCurves; }
            set { numCurves = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public uint NumPoints
        {
            get { return numPoints; }
            set { numPoints = value; }
        }

        public bool MinEccen
        {
            get { return minEccen; }
            set { minEccen = value; }
        }

        public float PatLLF
        {
            get { return patLLF; }
            set { patLLF = value; }
        }

        public float UFLimit
        {
            get { return uFLimit; }
            set { uFLimit = value; }
        }

        public float PhiB
        {
            get { return phiB; }
            set { phiB = value; }
        }

        public float PhiCTied
        {
            get { return phiCTied; }
            set { phiCTied = value; }
        }

        public float PhiCSpiral
        {
            get { return phiCSpiral; }
            set { phiCSpiral = value; }
        }

        public float PhiV
        {
            get { return phiV; }
            set { phiV = value; }
        }

        public override string ToString()
        {
            return "UBC97";
        }
    }

}
