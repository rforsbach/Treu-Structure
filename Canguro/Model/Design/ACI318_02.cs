using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    /// <summary>
    /// Design options for the ACI 318-2002 Design code
    /// </summary>
    [Serializable]
    public class ACI318_02 : ConcreteDesignOptions
    {
        THDesignOptions tHDesign;
        uint numCurves;
        uint numPoints;
        bool minEccen;
        float patLLF;
        float uFLimit;
        float phiT;
        float phiCTied;
        float phiCSpiral;
        float phiV;
        float phiVSeismic;
        float phiVJoint;
        char seisCat;

        /// <summary>
        /// Default constructor. Calls SetDefaults()
        /// </summary>
        public ACI318_02()
        {
            SetDefaults();
        }

        /// <summary>
        /// Sets default values for all properties
        /// </summary>
        public override void SetDefaults()
        {
            tHDesign = THDesignOptions.Envelopes;
            numCurves = 24;
            numPoints = 11;
            minEccen = true;
            patLLF = 0.75F;
            uFLimit = 0.95F;
            seisCat = 'D';
            phiT = 0.9F;
            phiCTied = 0.65F;
            phiCSpiral = 0.7F;
            phiV = 0.75F;
            phiVSeismic = 0.6F;
            phiVJoint = 0.85F;
        }

        /// <summary>
        /// Copies all properties from another ACI318_02 object
        /// </summary>
        /// <param name="copy">ACI318_02 object</param>
        public override void CopyFrom(DesignOptions copy)
        {
            if (copy is ACI318_02)
                CopyFrom((ACI318_02)copy);
        }

        /// <summary>
        /// Copies all properties from another ACI318_02 object
        /// </summary>
        /// <param name="copy">ACI318_02 object</param>
        public void CopyFrom(ACI318_02 copy)
        {
            tHDesign = copy.tHDesign;
            numCurves = copy.numCurves;
            numPoints = copy.numPoints;
            minEccen = copy.minEccen;
            patLLF = copy.patLLF;
            uFLimit = copy.uFLimit;
            seisCat = copy.seisCat;
            phiT = copy.phiT;
            phiCTied = copy.phiCTied;
            phiCSpiral = copy.phiCSpiral;
            phiV = copy.phiV;
            phiVSeismic = copy.phiVSeismic;
            phiVJoint = copy.phiVJoint;
        }

        /// <summary>
        /// Adds the default combination for the ACI318_02 to a List
        /// </summary>
        /// <returns>The List with the default combinations</returns>
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

            return DesignCombinations;
        }

        /// <summary>
        /// Gets or sets the Time History design options
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public THDesignOptions THDesign
        {
            get { return tHDesign; }
            set { tHDesign = value; }
        }

        /// <summary>
        /// Gets or sets the Number of curves
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public uint NumCurves
        {
            get { return numCurves; }
            set { numCurves = value; }
        }

        /// <summary>
        /// Gets or sets the Number of points for design
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public uint NumPoints
        {
            get { return numPoints; }
            set { numPoints = value; }
        }

        /// <summary>
        /// Gets or sets the Minimum design eccentricity
        /// </summary>
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

        public float PhiVSeismic
        {
            get { return phiVSeismic; }
            set { phiVSeismic = value; }
        }

        public float PhiVJoint
        {
            get { return phiVJoint; }
            set { phiVJoint = value; }
        }

        /// <summary>
        /// Gets or sets the Seismic category
        /// </summary>
        public char SeisCat
        {
            get { return seisCat; }
            set { seisCat = value; }
        }

        /// <summary>
        /// Gets a string describing the design options
        /// </summary>
        /// <returns>"ACI 318-02"</returns>
        public override string ToString()
        {
            return "ACI 318-02";
        }
    }
}
