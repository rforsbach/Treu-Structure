using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public class RCDF2001 : ConcreteDesignOptions
    {
        THDesignOptions tHDesign;
        uint numCurves;
        uint numPoints;
        bool minEccen;
        float patLLF;
        float uFLimit;
        float phiB;
        float phiT;
        float phiCTied;
        float phiCSpiral;
        float phiV;

        public RCDF2001()
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
            phiT = 0.8F;
            phiCTied = 0.7F;
            phiCSpiral = 0.8F;
            phiV = 0.8F;
        }

        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is RCDF2001)
                CopyFrom((RCDF2001)copy);
        }

        public void CopyFrom(RCDF2001 copy)
        {
            tHDesign = copy.tHDesign;
            numCurves = copy.numCurves;
            numPoints = copy.numPoints;
            minEccen = copy.minEccen;
            patLLF = copy.patLLF;
            uFLimit = copy.uFLimit;
            phiB = copy.phiB;
            phiT = copy.phiT;
            phiCTied = copy.phiCTied;
            phiCSpiral = copy.phiCSpiral;
            phiV = copy.phiV;
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
            AddCombination("1.4DL + 1.4LL", DL, new float[] { 1.4f, 1.4f });
            AddCombination("1.1DL + 1.1LL + WL", DLW, new float[] { 1.1f, 1.1f, 1f });
            AddCombination("1.1DL + 1.1LL - WL", DLW, new float[] { 1.1f, 1.1f, -1f });
            AddCombination("0.9DL + WL", DW, new float[] { 0.9f, 1f });
            AddCombination("0.9DL - WL", DW, new float[] { 0.9f, -1f });
            AddCombination("1.1DL + 1.1LL + 1.1QL", DLQ, new float[] { 1.1f, 1.1f, 1.1f });
            AddCombination("1.1DL + 1.1LL - 1.1QL", DLQ, new float[] { 1.1f, 1.1f, -1.1f });
            AddCombination("0.9DL + 1.1QL", DQ, new float[] { 0.9f, 1.1f });
            AddCombination("0.9DL - 1.1QL", DQ, new float[] { 0.9f, -1.1f });

            AddToModel();

            return DesignCombinations;
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

        [System.ComponentModel.Browsable(false)]
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

        public float PhiT
        {
            get { return phiT; }
            set { phiT = value; }
        }

        public float PhiCTied
        {
            get { return phiCTied; }
            set { phiCTied = value; }
        }

        [System.ComponentModel.Browsable(false)]
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
            return "RCDF";
        }
    }
}
