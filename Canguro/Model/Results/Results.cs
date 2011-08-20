using System;
using System.Collections.Generic;

using Canguro.Analysis;

namespace Canguro.Model.Results
{
    [Serializable]
    public class Results
    {
        #region General Fields
        /// <summary>
        /// Analysis ID that relates to Results calculated in server
        /// </summary>
        private int analysisID;
        private ResultsCase activeCase = null;
        private ResultsCasesList resultsCases;
        private bool finished = false;
        private StressHelper stressHelper = new StressHelper();
        #endregion

        #region ResultsFields        
        private float[,] assembledJointMasses = null;
        private float[][,] jointDisplacements = null;
        private float[][,] jointVelocities = null;
        private float[][,] jointAccelerations = null;
        private float[][,] jointReactions = null;
        private float[][] baseReactions = null;
        private float[] paintScaleFactorTranslation = null;
        [NonSerialized]
        private float[][] paintScaleFactorElementForces = null;
        
        /// <summary>
        /// Las fuerzas derivadas de la contribución de los LineElements en los nodos. Indizado por LineElement y AbstractCase, esto es, 6I + 6J = 12 fuerzas por barra.
        /// </summary>
        private float[][,,] elementJointForces = null;
        
        /// <summary>
        /// Modal Load Participation Ratios
        /// </summary>
        private Dictionary<string, List<ModalLPRRow>> modalLPR = null;

        /// <summary>
        /// Modal Participating Mass Ratios
        /// </summary>
        private float[][] modalPMR = null;

        /// <summary>
        /// Modal Participation Factors
        /// </summary>
        private float[][] modalPF = null;

        /// <summary>
        /// Modal Periods
        /// </summary>
        private float[][] modalPeriods = null;

        /// <summary>
        /// Response Spectrum Modal Information
        /// </summary>
        private float[,] responseSpectrumMI = null;

        private SteelDesignSummary[] designSteelSummary = null;
        private SteelDesignPMMDetails[] designSteelPMMDetails = null;
        private SteelDesignShearDetails[] designSteelShearDetails = null;
        private Dictionary<Canguro.Model.LineElement, AluminiumDesign1> designAluminium = null;
        private ConcreteColumnDesign[] designConcreteColumn = null;
        private ConcreteBeamDesign[] designConcreteBeam = null;
        private Dictionary<int, string> messages = new Dictionary<int,string>();
        #endregion

        public Results(int analysisID)
        {            
            this.analysisID = analysisID;
            resultsCases = new ResultsCasesList();
        }

        /// <summary>
        /// Method to initialize all the results arrays. Should be called after adding all results cases
        /// </summary>
        public void Init()
        {
            int i;
            int numJoints = Model.Instance.JointList.Count;
            int numLines = Model.Instance.LineList.Count;
            int numRC = resultsCases.Count;

            paintScaleFactorTranslation = new float[numRC];
            
            assembledJointMasses = new float[numJoints, 6];
            
            modalLPR = new Dictionary<string, List<ModalLPRRow>>();
            modalPMR = new float[numRC][];
            modalPF = new float[numRC][];
            modalPeriods = new float[numRC][];
            
            responseSpectrumMI = null;

            jointDisplacements = new float[numRC][,];
            for (i = 0; i < numRC; i++)
                jointDisplacements[i] = new float[numJoints, 6];

            jointReactions = new float[numRC][,];
            for (i = 0; i < numRC; i++)
                jointReactions[i] = new float[numJoints, 6];
            
            jointAccelerations = new float[numRC][,];
            for (i = 0; i < numRC; i++)
                jointAccelerations[i] = new float[numJoints, 6];

            jointVelocities = new float[numRC][,];
            for (i = 0; i < numRC; i++)
                jointVelocities[i] = new float[numJoints, 6];
            
            elementJointForces = new float[numRC][,,];
            for (i = 0; i < numRC; i++)
                elementJointForces[i] = new float[numLines, 2, 6];

            baseReactions = new float[numRC][];
            for (i = 0; i < numRC; i++)
                baseReactions[i] = new float[18];

            //// Design data
            //designAluminium;
            //designColdFormed
            
            designSteelSummary = new SteelDesignSummary[numLines];
            for (i = 0; i < numLines; i++)
                designSteelSummary[i] = new SteelDesignSummary();
            designSteelPMMDetails = new SteelDesignPMMDetails[numLines];
            for (i = 0; i < numLines; i++)
                designSteelPMMDetails[i] = new SteelDesignPMMDetails();
            designSteelShearDetails = new SteelDesignShearDetails[numLines];
            for (i = 0; i < numLines; i++)
                designSteelShearDetails[i] = new SteelDesignShearDetails();

            designConcreteColumn = new ConcreteColumnDesign[numLines];
            for (i = 0; i < numLines; i++)
                designConcreteColumn[i] = new ConcreteColumnDesign();
            designConcreteBeam = new ConcreteBeamDesign[numLines];
            for (i = 0; i < numLines; i++)
                designConcreteBeam[i] = new ConcreteBeamDesign();
        }

        /// <summary>
        /// Gets the Analysis ID that relates to Results calculated in server
        /// </summary>
        public int AnalysisID
        {
            get { return analysisID; }
        }

        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        internal StressHelper StressHelper
        {
            get { return stressHelper; }
            set { stressHelper = value; }
        }

        public float[,] AssembledJointMasses
        {
            get { return assembledJointMasses; }
        }

        public ResultsCase ActiveCase
        {
            get
            {
                return activeCase;
            }
            set
            {
                if (value != null)
                {
                    activeCase = value;
                    stressHelper.IsDirty = true;
                }
            }
        }

        public ResultsCasesList ResultsCases
        {
            get { return resultsCases; }
        }

        public float PaintScaleFactorTranslation
        {
            get 
            {
                if (activeCase == null || !activeCase.IsLoaded) return 1f;
                int id = activeCase.Id;

                if (paintScaleFactorTranslation[id] == 0f && JointDisplacements != null)
                {
                    // Get maximum Joint displacement
                    for (int i = 1; i < JointDisplacements.GetLength(0); i++)
                    {
                        if (Math.Abs(JointDisplacements[i, 0]) > paintScaleFactorTranslation[id])
                            paintScaleFactorTranslation[id] = Math.Abs(JointDisplacements[i, 0]);
                        if (Math.Abs(JointDisplacements[i, 1]) > paintScaleFactorTranslation[id])
                            paintScaleFactorTranslation[id] = Math.Abs(JointDisplacements[i, 1]);
                        if (Math.Abs(JointDisplacements[i, 2]) > paintScaleFactorTranslation[id])
                            paintScaleFactorTranslation[id] = Math.Abs(JointDisplacements[i, 2]);
                    }

                    // Get maximum Line displacement
                    Canguro.Analysis.LineDeformationCalculator calc = new Canguro.Analysis.LineDeformationCalculator();

                    float[,] diagram;
                    int numPoints = 3;
                    Load.AbstractCase ac = activeCase.AbstractCase;

                    foreach (Canguro.Model.LineElement line in Canguro.Model.Model.Instance.LineList)
                    {
                        if (line != null)
                        {
                            diagram = calc.GetCurvedAxis(line, ac, Canguro.Analysis.LineDeformationCalculator.DeformationAxis.Local2, numPoints);
                            paintScaleFactorTranslation[id] = Math.Max(paintScaleFactorTranslation[id], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                            diagram = calc.GetCurvedAxis(line, ac, Canguro.Analysis.LineDeformationCalculator.DeformationAxis.Local3, numPoints);
                            paintScaleFactorTranslation[id] = Math.Max(paintScaleFactorTranslation[id], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                        }
                    }

                    // Get scale as the inverse of the maximum overall displacement
                    if (paintScaleFactorTranslation[id] < 1e-20f)
                        paintScaleFactorTranslation[id] = 1f;

                    ItemList<Joint> jList = Canguro.Model.Model.Instance.JointList;
                    Microsoft.DirectX.Vector3[] box = Model.Instance.Summary.BoundingBox;
                    float length = Microsoft.DirectX.Vector3.Length(box[1] - box[0]);
                    if (Math.Abs(length) < 0.00001)
                        length = 1f;

                    paintScaleFactorTranslation[id] = 0.05f * length / paintScaleFactorTranslation[id];
                }

                return paintScaleFactorTranslation[activeCase.Id]; 
            }
            set 
            {
                if (activeCase == null) return;

                paintScaleFactorTranslation[activeCase.Id] = value; 
            }
        }

        static float[] emptyScaleFactorElementForces = { 0f, 0f };
        public float[] PaintScaleFactorElementForces
        {
            get
            {
                if (activeCase == null) return emptyScaleFactorElementForces;
                int id = activeCase.Id;

                if ((paintScaleFactorElementForces == null || paintScaleFactorElementForces[id][0] == 0f) && ElementJointForces != null)
                {
                    float[,] diagram;
                    int numPoints = 3;
                    
                    if (paintScaleFactorElementForces == null)
                    {
                        paintScaleFactorElementForces = new float[ResultsCases.Count][];
                        for (int i = 0; i < paintScaleFactorElementForces.Length; i++)
                            paintScaleFactorElementForces[i] = new float[2];
                    }
                    
                    paintScaleFactorElementForces[id][0] = paintScaleFactorElementForces[id][1] = 0f;
                    Analysis.LineStressCalculator calc = new Analysis.LineStressCalculator();
                    Load.AbstractCase ac = activeCase.AbstractCase;

                    foreach (LineElement line in Canguro.Model.Model.Instance.LineList)
                    {
                        if (line != null)
                        {
                            //Shears
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Axial, numPoints);
                            paintScaleFactorElementForces[id][0] = Math.Max(paintScaleFactorElementForces[id][0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Shear22, numPoints);
                            paintScaleFactorElementForces[id][0] = Math.Max(paintScaleFactorElementForces[id][0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Shear33, numPoints);
                            paintScaleFactorElementForces[id][0] = Math.Max(paintScaleFactorElementForces[id][0], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));

                            //Moments
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Torsion, numPoints);
                            paintScaleFactorElementForces[id][1] = Math.Max(paintScaleFactorElementForces[id][1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment22, numPoints);
                            paintScaleFactorElementForces[id][1] = Math.Max(paintScaleFactorElementForces[id][1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                            diagram = calc.GetForcesDiagram(ac, line, Canguro.Model.Load.LineForceComponent.Moment33, numPoints);
                            paintScaleFactorElementForces[id][1] = Math.Max(paintScaleFactorElementForces[id][1], Math.Max(Math.Max(Math.Abs(diagram[0, 1]), Math.Abs(diagram[1, 1])), Math.Abs(diagram[2, 1])));
                        }
                    }

                    for (int i = 0; i < paintScaleFactorElementForces[id].Length; i++)
                        paintScaleFactorElementForces[id][i] = 1f / paintScaleFactorElementForces[id][i];


                    //ItemList<Joint> jList = Canguro.Model.Model.Instance.JointList;
                    //Microsoft.DirectX.Vector3 min = new Microsoft.DirectX.Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                    //Microsoft.DirectX.Vector3 max = new Microsoft.DirectX.Vector3(float.MinValue, float.MinValue, float.MinValue);
                    //foreach (Joint j in jList)
                    //{
                    //    if (j != null)
                    //    {
                    //        Microsoft.DirectX.Vector3 pos = j.Position;
                    //        min.X = (min.X > pos.X) ? pos.X : min.X;
                    //        min.Y = (min.Y > pos.Y) ? pos.Y : min.Y;
                    //        min.Z = (min.Z > pos.Z) ? pos.Z : min.Z;
                    //        max.X = (max.X < pos.X) ? pos.X : max.X;
                    //        max.Y = (max.Y < pos.Y) ? pos.Y : max.Y;
                    //        max.Z = (max.Z < pos.Z) ? pos.Z : max.Z;
                    //    }
                    //}

                    //float length = Microsoft.DirectX.Vector3.Length(max - min);
                    //if (Math.Abs(length) < 0.00001)
                    //    length = 1f;

                    //paintScaleFactorTranslation[id] /= length;
                }

                return paintScaleFactorElementForces[activeCase.Id];
            }
            set
            {
                if (activeCase == null) return;

                paintScaleFactorElementForces[activeCase.Id] = value;
            }
        }

        public float[] BaseReactions
        {
            get
            {
                if (activeCase == null)
                    return null;

                return baseReactions[activeCase.Id];
            }
        }

        /// <summary>
        /// Modal Load Participation Ratios
        /// </summary>
        ///   
         public Dictionary<string, List<ModalLPRRow>> ModalLPR
        {
            get
            {
                //string path = ResultsPath.FirstPart(activeCase.FullPath);

                //if (modalLPR.ContainsKey(path))
                //    return modalLPR[path];

                //return null;

                return modalLPR;
            }
            //set
            //{
            //    string path = ResultsPath.FirstPart(activeCase.FullPath);

            //    if (modalLPR.ContainsKey(path))
            //        modalLPR[path] = value;
            //    else
            //        modalLPR.Add(path, value);
            //}
        }

        /// <summary>
        /// Modal Periods
        /// </summary>
        public float[] ModalPeriods
        {
            get
            {
                if (activeCase == null)
                    return null;

                return modalPeriods[activeCase.Id];
            }
            set
            {
                modalPeriods[activeCase.Id] = value;
            }
        }

        internal float[] GetModalPeriods(ResultsCase rcase)
        {
            if (rcase == null || modalPeriods == null)
                return null;

            return modalPeriods[rcase.Id];
        }

        /// <summary>
        /// Modal Participation Factors
        /// </summary>
        public float[] ModalPF
        {
            get
            {
                if (activeCase == null)
                    return null;

                return modalPF[activeCase.Id];
            }
            set
            {
                modalPF[activeCase.Id] = value;
            }
        }

        /// <summary>
        /// Modal Participating Mass Ratios
        /// </summary>
        public float[] ModalPMR
        {
            get
            {
                if (activeCase == null)
                    return null;

                return modalPMR[activeCase.Id];
            }
            set
            {
                modalPMR[activeCase.Id] = value;
            }
        }

        /// <summary>
        /// Response Spectrum Modal Information
        /// </summary>
        public float[,] ResponseSpectrumMI
        {
            get
            {
                if (activeCase == null)
                    return null;

                return responseSpectrumMI;
            }
            set
            {
                responseSpectrumMI = value;
            }
        }

        public float[,,] ElementJointForces
        {
            get
            {
                if (activeCase == null)
                    return null;

                return elementJointForces[activeCase.Id];
            }
        }

        public float[,] JointDisplacements
        {
            get
            {
                if (activeCase == null)
                    return null;

                return jointDisplacements[activeCase.Id];
            }
        }

        public float[,] JointVelocities
        {
            get
            {
                if (activeCase == null)
                    return null;

                return jointVelocities[activeCase.Id];
            }
        }

        public float[,] JointAccelerations
        {
            get
            {
                if (activeCase == null)
                    return null;

                return jointAccelerations[activeCase.Id];
            }
        }

        public float[,] JointReactions
        {
            get
            {
                if (activeCase == null)
                    return null;

                return jointReactions[activeCase.Id];
            }
            //set
            //{
            //    jointReactions[activeCase.Id] = value;
            //}
        }

        #region DesignResults
        public int DesignAluminium
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ConcreteColumnDesign[] DesignConcreteColumn
        {
            get
            {
                return designConcreteColumn;
            }
            set
            {
                designConcreteColumn = value;
            }
        }

        public ConcreteBeamDesign[] DesignConcreteBeam
        {
            get
            {
                return designConcreteBeam;
            }
            set
            {
                designConcreteBeam = value;
            }
        }

        public SteelDesignSummary[] DesignSteelSummary
        {
            get 
            {
                return designSteelSummary; 
            }
            set 
            {
                designSteelSummary = value; 
            }
        }

        public SteelDesignPMMDetails[] DesignSteelPMMDetails
        {
            get 
            {
                return designSteelPMMDetails; 
            }
            set 
            {
                designSteelPMMDetails = value; 
            }
        }

        public SteelDesignShearDetails[] DesignSteelShearDetails
        {
            get 
            {
                return designSteelShearDetails; 
            }
            set 
            {
                designSteelShearDetails = value; 
            }
        }
        #endregion

        public Dictionary<int, string> Messages
        {
            get
            {
                return messages;
            }
        }
    }
}
