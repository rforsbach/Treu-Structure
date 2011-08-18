using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Model.Design
{
    [Serializable]
    public abstract class DesignOptions : Canguro.Utility.GlobalizedObject, ICloneable
    {
        private bool isActive = true;
        protected List<Load.LoadCombination> designCombinations = new List<Canguro.Model.Load.LoadCombination>();

        [System.ComponentModel.Browsable(false)]
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        [System.ComponentModel.Browsable(false)]
        public List<Load.LoadCombination> DesignCombinations
        {
            get { return designCombinations; }
        }

        public abstract void SetDefaults();
        public abstract List<Load.LoadCombination> AddDefaultCombos();

        /// <summary>
        /// Método heredado de IClonable
        /// </summary>
        /// <returns>Regresa una copia superficial (completa en este caso) de sí mismo</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetSeismicZoneName(SeismicZone zone)
        {
            switch (zone)
            {
                case SeismicZone.Zone0: return "Zone 0";
                case SeismicZone.Zone1: return "Zone 1";
                case SeismicZone.Zone2: return "Zone 2";
                case SeismicZone.Zone3: return "Zone 3";
                default: return "Zone 4";
            }
        }

        public string GetFrameTypeName(UBCFrameType frameType)
        {
            switch (frameType)
            {
                case UBCFrameType.BracedFrame: return "Braced Frame";
                default: return "Moment Frame";
            }
        }

        public string GetTHDesignName(THDesignOptions obj)
        {
            return (obj == THDesignOptions.Envelopes) ? "Envelopes" : "Step-by-Step";
        }

        public abstract void CopyFrom(DesignOptions copy);

        protected void AddCombination(string name, LoadCase.LoadCaseType[] types, float[] factors)
        {
            foreach (AbstractCase ac in Model.Instance.AbstractCases)
                if (ac.Name.Equals(name))
                {
                    if (ac is LoadCombination)
                        designCombinations.Add((LoadCombination)ac);
                    return;
                }

            LoadCombination combo = new LoadCombination(name);
            for (int i = 0; i < types.Length; i++)
            {
                float factor = 1;
                if (factors.Length > i)
                    factor = factors[i];
                List<AbstractCaseFactor> loads = GetLoads(types[i], factors[i]);
                if (loads.Count > 0)
                    combo.Cases.AddRange(loads);
                else
                    return;
            }
            if (combo.Cases.Count > 0) designCombinations.Add(combo);
        }

        protected void AddToModel()
        {
            bool[] add = new bool[designCombinations.Count];
            for (int i = 0; i < designCombinations.Count; i++)
                add[i] = true;

            for (int i = 0; i < designCombinations.Count; i++)
                foreach (AbstractCase ac in Model.Instance.AbstractCases)
                    if (ac.Name.Equals(designCombinations[i].Name))
                        add[i] = false;

            for (int i = 0; i < designCombinations.Count; i++)
                if (add[i])
                    Model.Instance.AbstractCases.Add(designCombinations[i]);
        }

        protected List<AbstractCaseFactor> GetLoads(LoadCase.LoadCaseType type, float factor)
        {
            List<AbstractCaseFactor> list = new List<Canguro.Model.Load.AbstractCaseFactor>();
            foreach(AbstractCase ac in Model.Instance.AbstractCases)
            {
                 if (ac is AnalysisCase && ((AnalysisCase)ac).Properties is StaticCaseProps)
                 {
                     StaticCaseProps props = (StaticCaseProps)((AnalysisCase)ac).Properties;
                     if (!(props is PDeltaCaseProps))
                     {
                         bool use = false;
                         foreach (StaticCaseFactor fact in props.Loads)
                         {
                             if (fact.AppliedLoad is LoadCase && ((LoadCase)fact.AppliedLoad).CaseType == type)
                             {
                                 use = true;
                             }
                             else
                             {
                                 use = false;
                                 break;
                             }
                         }
                         if (use)
                             list.Add(new AbstractCaseFactor(ac, factor));
                     }
                 }
                 else if (type == LoadCase.LoadCaseType.Quake && ac is AnalysisCase && ((AnalysisCase)ac).Properties is ResponseSpectrumCaseProps)
                     list.Add(new AbstractCaseFactor(ac, factor));
            }
            return list;
        }
    }

    public enum THDesignOptions
    {
        Envelopes,
        Step_by_Step,
    }

    public enum FrameTypeOptions
    {
        MomentFrame,
        BracedFrame,
    }

    public enum SeismicZone
    {
        Zone0,
        Zone1,
        Zone2,
        Zone3,
        Zone4,
    }

    public enum UBCFrameType
    {
        MomentFrame,
        BracedFrame,
    }
}
