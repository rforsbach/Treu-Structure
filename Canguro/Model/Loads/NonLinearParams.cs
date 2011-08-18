using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    public class NonLinearParams : Canguro.Utility.GlobalizedObject
    {
        string unloading = "Unload Entire";
        string geoNonLin = "P - Delta";
        string resultsSave = "Final State";
        int minNumState = 10;
        int maxNumState = 100;
        int maxTotal = 200;
        int maxNull = 50;
        int maxIterCS = 10;
        int maxIterNR = 40;
        float itConvTol = 0.0001f;
        bool useEvStep = true;
        float evLumpTol = 0.01f;
        int lSPerIter = 20;
        float lSTol = 0.1f;
        float lSStepFact = 1.618f;
        bool frameTC = true;
        bool frameHinge = true;
        bool cableTC = true;
        bool linkTC = true;
        bool linkOther = true;
        int tFMaxIter = 10;
        float tFTol = 0.01f;
        float tFAccelFact = 1;
        bool tFNoStop = false;

        /// <summary>
        /// Hinge Unloading Method
        /// Unload Entire / Local Redist / Restart Secant
        /// </summary>
        public string Unloading
        {
            get { return unloading; }
            set { unloading = value; }
        }

        /// <summary>
        /// Geometric Nonlinearity (P - Delta / Large Displ)
        /// </summary>
        public string GeoNonLin
        {
            get { return geoNonLin; }
            set { geoNonLin = value; }
        }

        /// <summary>
        /// Results Saved (Final State / Multiple States)
        /// </summary>
        public string ResultsSave
        {
            get { return resultsSave; }
            set { resultsSave = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Total steps per Stage
        /// </summary>
        public int MinNumState
        {
            get { return minNumState; }
            set { minNumState = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Total steps per Stage
        /// </summary>
        public int MaxNumState
        {
            get { return maxNumState; }
            set { maxNumState = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Total steps per Stage
        /// </summary>
        public int MaxTotal
        {
            get { return maxTotal; }
            set { maxTotal = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Null (Zero) Steps per Stage
        /// </summary>
        public int MaxNull
        {
            get { return maxNull; }
            set { maxNull = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Constant-Stiff Iterations per Step
        /// </summary>
        public int MaxIterCS
        {
            get { return maxIterCS; }
            set { maxIterCS = value; }
        }

        /// <summary>
        /// Control
        /// Maximum Newton-Raphson Iterations per Step
        /// </summary>
        public int MaxIterNR
        {
            get { return maxIterNR; }
            set { maxIterNR = value; }
        }

        /// <summary>
        /// Control
        /// Iteration Convergence Tolerance (Relative)
        /// </summary>
        public float ItConvTol
        {
            get { return itConvTol; }
            set { itConvTol = value; }
        }

        /// <summary>
        /// Control
        /// Use Event-to-event Stepping
        /// </summary>
        public bool UseEvStep
        {
            get { return useEvStep; }
            set { useEvStep = value; }
        }

        /// <summary>
        /// Control
        /// Event Lumpiong Tolerance (Relative)
        /// </summary>
        public float EvLumpTol
        {
            get { return evLumpTol; }
            set { evLumpTol = value; }
        }

        /// <summary>
        /// Control
        /// Max Line searches per Iteration
        /// </summary>
        public int LSPerIter
        {
            get { return lSPerIter; }
            set { lSPerIter = value; }
        }

        /// <summary>
        /// Control
        /// Line-search Acceptance Tolerance (Relative)
        /// </summary>
        public float LSTol
        {
            get { return lSTol; }
            set { lSTol = value; }
        }

        /// <summary>
        /// Control
        /// Line-search Step Factor
        /// </summary>
        public float LSStepFact
        {
            get { return lSStepFact; }
            set { lSStepFact = value; }
        }

        /// <summary>
        /// Material
        /// Frame Element Tension/Compression Only
        /// </summary>
        public bool FrameTC
        {
            get { return frameTC; }
            set { frameTC = value; }
        }

        /// <summary>
        /// Material
        /// Frame Element Hinge
        /// </summary>
        public bool FrameHinge
        {
            get { return frameHinge; }
            set { frameHinge = value; }
        }

        /// <summary>
        /// Material
        /// Cable Tension Only
        /// </summary>
        public bool CableTC
        {
            get { return cableTC; }
            set { cableTC = value; }
        }

        /// <summary>
        /// Material
        /// Link Gap/Hook/Spring Nonlinear Properties
        /// </summary>
        public bool LinkTC
        {
            get { return linkTC; }
            set { linkTC = value; }
        }

        /// <summary>
        /// Material
        /// Link Other Nonlinear Properties
        /// </summary>
        public bool LinkOther
        {
            get { return linkOther; }
            set { linkOther = value; }
        }

        /// <summary>
        /// Target Force Iteration
        /// Maximum Iterations per Stage
        /// </summary>
        public int TFMaxIter
        {
            get { return tFMaxIter; }
            set { tFMaxIter = value; }
        }

        /// <summary>
        /// Target Force Iteration
        /// Convergence Tolerance (Relative)
        /// </summary>
        public float TFTol
        {
            get { return tFTol; }
            set { tFTol = value; }
        }

        /// <summary>
        /// Target Force Iteration
        /// Acceleration Factor
        /// </summary>
        public float TFAccelFact
        {
            get { return tFAccelFact; }
            set { tFAccelFact = value; }
        }

        /// <summary>
        /// Target Force Iteration
        /// Continue Analysis If No convergence
        /// </summary>
        public bool TFNoStop
        {
            get { return tFNoStop; }
            set { tFNoStop = value; }
        }
    }
}
