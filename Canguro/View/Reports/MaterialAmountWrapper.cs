using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Section;
using Canguro.Model.Material;

namespace Canguro.View.Reports
{
    class MaterialAmountWrapper : ReportData
    {
        string section;
        string material;
        float amount;

        protected MaterialAmountWrapper(string section, string material, float amount)
        {
            this.section = section;
            this.material = material;
            this.amount = amount;
        }

        protected MaterialAmountWrapper(Section section, float amount)
        {
            this.section = section.Name;
            this.material = section.Material.Name;
            this.amount = amount;
        }

        public static List<ReportData> GetAmountsPerSection(Model.Model model)
        {
            List<ReportData> amounts = new List<ReportData>();
            try
            {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                Dictionary<string, float> sectionMass = new Dictionary<string, float>();
                Dictionary<string, float> materialMass = new Dictionary<string, float>();
                foreach (LineElement line in model.LineList)
                {
                    if (line != null && line.Properties is StraightFrameProps)
                    {
                        FrameSection section = ((StraightFrameProps)line.Properties).Section;
                        Material material = section.Material;
                        float mass = section.Area * material.Density * line.Length;
                        if (sectionMass.ContainsKey(section.Name))
                            sectionMass[section.Name] += mass;
                        else
                            sectionMass.Add(section.Name, mass);

                        if (materialMass.ContainsKey(material.Name))
                            materialMass[material.Name] += mass;
                        else
                            materialMass.Add(material.Name, mass);
                    }
                }
                foreach (string sec in sectionMass.Keys)
                    amounts.Add(new MaterialAmountWrapper(model.Sections[sec], sectionMass[sec]));
                foreach (string mat in materialMass.Keys)
                    amounts.Add(new MaterialAmountWrapper(Culture.Get("total"), MaterialManager.Instance.Materials[mat].Name, materialMass[mat]));
            }
            finally
            {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
            }
            return amounts;
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 4000)]
        public string Material
        {
            get { return material; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 4000)]
        public string Section
        {
            get { return section; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 4000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string Amount
        {
            get { return Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.FromInternational(amount, Canguro.Model.UnitSystem.Units.Mass).ToString("G3"); }
            set { }
        }
    }
}
