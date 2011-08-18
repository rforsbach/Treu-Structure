using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Serializer
{
    class ModelRepairer
    {
        public void Repair(Model model)
        {
            ItemList<Joint> jList = model.JointList;
            ItemList<LineElement> lList = model.LineList;
            ItemList<AreaElement> aList = model.AreaList;
            Dictionary<string, Load.LoadCase> lCases = model.LoadCases;

            // Repair Joints
            foreach (Joint val in jList)
            {
                if (val != null)
                {
                    val.Loads.Repair();
                    if (val.Layer != model.Layers[val.Layer.Id])
                        val.Layer = model.Layers[val.Layer.Id];
                }
            }

            // Repair Lines
            foreach (LineElement val in lList)
            {
                if (val != null && val.Id > 0)
                {
                    if (val.I != jList[val.I.Id])
                    {
                        if (jList[val.I.Id] != null)
                            val.I = jList[val.I.Id];
                        else
                            ((IList<Joint>)jList)[(int)val.I.Id] = val.I;
                    }
                    if (val.J != jList[val.J.Id] && jList[val.J.Id] != null)
                    {
                        if (jList[val.I.Id] != null)
                            val.J = jList[val.J.Id];
                        else
                            ((IList<Joint>)jList)[(int)val.J.Id] = val.J;
                    }
                    if (val.Loads != null)
                        val.Loads.Repair();
                    if (val.Properties is StraightFrameProps)
                    {
                        StraightFrameProps prop = (StraightFrameProps)val.Properties;
                        if (model.Sections[prop.Section.Name] == null)
                            model.Sections[prop.Section.Name] = prop.Section;
                        else if (prop.Section != model.Sections[prop.Section.Name])
                            prop.Section = (Section.FrameSection)model.Sections[prop.Section.Name];

                    }

                    //// Fix Releases
                    //JointDOF dofI = val.DoFI;
                    //JointDOF dofJ = val.DoFJ;
                    //dofJ.T1 = (dofI.T1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T1;
                    //dofJ.T2 = (dofI.T2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T2;
                    //dofJ.T3 = (dofI.T3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T3;
                    //dofJ.R1 = (dofI.R1 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.R1;
                    //dofJ.T2 = (dofI.R3 == JointDOF.DofType.Free && dofJ.R3 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T2;
                    //dofJ.T3 = (dofI.R2 == JointDOF.DofType.Free && dofJ.R2 == JointDOF.DofType.Free) ? JointDOF.DofType.Restrained : dofJ.T3;
                    //val.DoFJ = dofJ;
                }
            }

            Repair(Material.MaterialManager.Instance.Materials, model.Sections);

            // Repair AbstractCases
            foreach (Load.AbstractCase aCase in model.AbstractCases)
            {
                if (aCase is Load.AnalysisCase)
                    Repair(model, (Load.AnalysisCase)aCase);
                else if (aCase is Load.LoadCombination)
                    Repair(model, (Load.LoadCombination)aCase);
            }

            foreach (Design.DesignOptions opt in model.DesignOptions)
                Repair(opt);
        }

        private void Repair(Design.DesignOptions design)
        {
            List<string> combos = new List<string>(design.DesignCombinations.Count);
            foreach (Load.LoadCombination c in design.DesignCombinations)
                combos.Add(c.Name);
            design.DesignCombinations.Clear();

            foreach (Load.AbstractCase aCase in Model.Instance.AbstractCases)
                if (combos.Contains(aCase.Name) && (aCase is Load.LoadCombination))
                    design.DesignCombinations.Add((Load.LoadCombination)aCase);
        }

        private void Repair(Catalog<Material.Material> materials, Catalog<Section.Section> sections)
        {
            foreach (Section.FrameSection sec in sections)
            {
                if (sec.Material == null)
                    sec.Material = Material.MaterialManager.Instance.DefaultSteel;
                else if (materials[sec.Material.Name] == null)
                    materials[sec.Material.Name] = sec.Material;
                else if (materials[sec.Material.Name] != sec.Material)
                    sec.Material = materials[sec.Material.Name];
            }
        }

        private void Repair(Model model, Load.AnalysisCase value)
        {
            if (value.Properties is Load.StaticCaseProps)
            {
                IList<Load.StaticCaseFactor> list = ((Load.StaticCaseProps)value.Properties).Loads;
                foreach (Load.StaticCaseFactor factor in list)
                    if (factor.AppliedLoad is Load.LoadCase)
                    {
                        if (model.LoadCases.ContainsKey(((Load.LoadCase)factor.AppliedLoad).Name))
                            factor.AppliedLoad = model.LoadCases[((Load.LoadCase)factor.AppliedLoad).Name];
                        else
                            factor.AppliedLoad = model.ActiveLoadCase;
                    }
            }
        }

        private void Repair(Model model, Load.LoadCombination value)
        {
            IList<Load.AbstractCaseFactor> cases = value.Cases;
            Dictionary<string, int> ids = new Dictionary<string, int>();
            for (int i = 0; i < model.AbstractCases.Count; i++)
                if (model.AbstractCases[i] != null)
                    ids.Add(model.AbstractCases[i].Name, i);

            for (int i = 0; i < cases.Count; i++)
                cases[i] = new Canguro.Model.Load.AbstractCaseFactor(model.AbstractCases[ids[cases[i].Case.Name]], cases[i].Factor);
        }
    }
}
