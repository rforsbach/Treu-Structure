using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class ConcreteColumnSectionProps : ConcreteSectionProps
    {
        private float coverToRebarCenter = 0.0381F;
        private int numberOfBars3Dir = 3;
        private int numberOfBars2Dir = 3;
        private float spacingC = 0.15f;
        private string barSize = "#3";
        private ReinforcementCheckDesign checkOrDesign = ReinforcementCheckDesign.Check;
        private ReinforcementConfiguration rConfiguration = ReinforcementConfiguration.Rectangular;
        private LateralReinforcement lateralR = LateralReinforcement.Ties;

        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float CoverToRebarCenter
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(coverToRebarCenter, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != coverToRebarCenter)
                {
                    Model.Instance.Undo.Change(this, CoverToRebarCenter, GetType().GetProperty("CoverToRebarCenter"));
                    coverToRebarCenter = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        public int NumberOfBars
        {
            get
            {
                if (rConfiguration == ReinforcementConfiguration.Circular)
                    return numberOfBars2Dir;
                else
                    return 2 * numberOfBars2Dir + 2 * numberOfBars3Dir - 4;
            }
            set
            {
                if (value != numberOfBars2Dir)
                {
                    Model.Instance.Undo.Change(this, numberOfBars2Dir, GetType().GetProperty("NumberOfBars"));
                    numberOfBars2Dir = value;
                }
            }
        }

        /// <summary>
        /// Distance betweem reinforcement rebars.
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float SpacingC
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(spacingC, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != spacingC)
                {
                    Model.Instance.Undo.Change(this, SpacingC, GetType().GetProperty("SpacingC"));
                    spacingC = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        public int NumberOfBars3Dir
        {
            get
            {
                return numberOfBars3Dir;
            }
            set
            {
                if (value != numberOfBars3Dir)
                {
                    Model.Instance.Undo.Change(this, numberOfBars3Dir, GetType().GetProperty("NumberOfBars3Dir"));
                    numberOfBars3Dir = value;
                }
            }
        }

        public int NumberOfBars2Dir
        {
            get
            {
                return numberOfBars2Dir;
            }
            set
            {
                if (value != numberOfBars2Dir)
                {
                    Model.Instance.Undo.Change(this, numberOfBars2Dir, GetType().GetProperty("NumberOfBars2Dir"));
                    numberOfBars2Dir = value;
                }
            }
        }

        public string BarSize
        {
            get
            {
                return barSize;
            }
            set
            {
                if (!value.Equals(barSize))
                {
                    Model.Instance.Undo.Change(this, barSize, GetType().GetProperty("BarSize"));
                    barSize = value;
                }
            }
        }

        public ReinforcementCheckDesign CheckOrDesign
        {
            get
            {
                return checkOrDesign;
            }
            set
            {
                if (value != checkOrDesign)
                {
                    Model.Instance.Undo.Change(this, checkOrDesign, GetType().GetProperty("CheckOrDesign"));
                    checkOrDesign = value;
                }
            }
        }

        /// <summary>
        /// Reinforcement Configuration
        /// </summary>
        public ReinforcementConfiguration RConfiguration
        {
            get
            {
                return rConfiguration;
            }
            set
            {
                if (value != rConfiguration)
                {
                    Model.Instance.Undo.Change(this, rConfiguration, GetType().GetProperty("RConfiguration"));
                    rConfiguration = value;
                }
            }
        }

        /// <summary>
        /// Lateral Reinforcement
        /// </summary>
        public LateralReinforcement LateralR
        {
            get
            {
                return lateralR;
            }
            set
            {
                if (value != lateralR)
                {
                    Model.Instance.Undo.Change(this, lateralR, GetType().GetProperty("LateralR"));
                    lateralR = value;
                }
            }
        }
    
        public enum ReinforcementConfiguration : byte
        {
            Rectangular, Circular,
        }

        public enum LateralReinforcement : byte
        {
            Ties, Spiral,
        }

        public enum ReinforcementCheckDesign : byte
        {
            Check, Design,
        }
    }

    public class BarSizes
    {
        
        private Dictionary<string, float> sizes;

        public static readonly BarSizes Instance = new BarSizes();
        private BarSizes()
        {
            sizes = new Dictionary<string, float>();
            sizes.Add("#2", 0.00635f);
            sizes.Add("#3", 0.009525f);
            sizes.Add("#4", 0.0127f);
            sizes.Add("#5", 0.015875f);
            sizes.Add("#6", 0.01905f);
            sizes.Add("#7", 0.022225f);
            sizes.Add("#8", 0.0254f);
            sizes.Add("#9", 0.028651f);
            sizes.Add("#10", 0.032258f);
            sizes.Add("#11", 0.035814f);
            sizes.Add("#14", 0.043002f);
            sizes.Add("#18", 0.057328f);
            sizes.Add("10M", 0.0113f);
            sizes.Add("15M", 0.016f);
            sizes.Add("20M", 0.0195f);
            sizes.Add("25M", 0.0252f);
            sizes.Add("30M", 0.0299f);
            sizes.Add("35M", 0.0357f);
            sizes.Add("45M", 0.0437f);
            sizes.Add("55M", 0.0564f);
            sizes.Add("6d", 0.006f);
            sizes.Add("8d", 0.008f);
            sizes.Add("10d", 0.01f);
            sizes.Add("12d", 0.012f);
            sizes.Add("14d", 0.014f);
            sizes.Add("16d", 0.016f);
            sizes.Add("20d", 0.02f);
            sizes.Add("25d", 0.025f);
            sizes.Add("26d", 0.026f);
            sizes.Add("28d", 0.028f);
        }
        
        public float this[string rebarName]
        {
            get { return sizes[rebarName]; }
            set { sizes[rebarName] = value; }
        }

        public float GetArea(string rebarName)
        {
            return (float)((sizes[rebarName] * sizes[rebarName]) / 4f * Math.PI);
        }

        public Dictionary<string, float>.KeyCollection Keys
        {
            get { return sizes.Keys; }
        }

        public Dictionary<string, float>.ValueCollection Values
        {
            get { return sizes.Values; }
        }
    }
}
