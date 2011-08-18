using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    [Serializable]
    public class TemperatureLineLoad : LineLoad
    {
        protected float temperature = 0;

        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Temperature)]
        public virtual float Temperature
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(temperature, Canguro.Model.UnitSystem.Units.Temperature);
            }
            set
            {
                float tmp = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Temperature);
                if (tmp != temperature)
                {
                    Model.Instance.Undo.Change(this, Temperature, GetType().GetProperty("Temperature"));
                    temperature = tmp;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("TL ({0:F})", Temperature);
        }

        public override object Clone()
        {
            TemperatureLineLoad load = new TemperatureLineLoad();
            load.temperature = temperature;
            return load;
        }
    }

    [Serializable]
    public class TemperatureGradientLineLoad : TemperatureLineLoad
    {
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.TemperatureGradient)]
        public override float Temperature
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(temperature, Canguro.Model.UnitSystem.Units.TemperatureGradient);
            }
            set
            {
                float tmp = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.TemperatureGradient);
                if (tmp != temperature)
                {
                    Model.Instance.Undo.Change(this, Temperature, GetType().GetProperty("Temperature"));
                    temperature = tmp;
                }
            }
        }

        private GradientDirection type = GradientDirection.G22;

        public GradientDirection LoadType
        {
            get { return type; }
            set
            {
                type = value;
            }
        }

        public enum GradientDirection
        {
            G22,
            G33,
        }

        public override string ToString()
        {
            return string.Format("TGL ({0:F} {1})", Temperature, this.LoadType);
        }

        public override object Clone()
        {
            TemperatureGradientLineLoad load = new TemperatureGradientLineLoad();
            load.temperature = temperature;
            load.LoadType = this.LoadType;
            return load;
        }
    }
}
