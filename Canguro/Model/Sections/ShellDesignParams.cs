using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class ShellDesignParams
    {
        float topBar1, topBar2;
        float bottomBar1, bottomBar2;
        ShellRebarLayouts rebarLayout;
        Material.Material rebarMaterial;

        public ShellDesignParams()
        {
            rebarLayout = ShellRebarLayouts.Default;
            topBar1 = topBar2 = bottomBar1 = bottomBar2 = 0;
            rebarMaterial = Material.MaterialManager.Instance.DefaultRebar;
        }

        public float TopBar1
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(topBar1, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, topBar1, GetType().GetProperty("TopBar1"));
                topBar1 = Model.Instance.UnitSystem.ToInternational(Math.Max(0, value), Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        public float TopBar2
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(topBar2, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, topBar2, GetType().GetProperty("TopBar2"));
                topBar2 = Model.Instance.UnitSystem.ToInternational(Math.Max(0, value), Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        public float BottomBar1
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(bottomBar1, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, bottomBar1, GetType().GetProperty("BottomBar1"));
                bottomBar1 = Model.Instance.UnitSystem.ToInternational(Math.Max(0, value), Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        public float BottomBar2
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(bottomBar2, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, bottomBar2, GetType().GetProperty("BottomBar2"));
                bottomBar2 = Model.Instance.UnitSystem.ToInternational(Math.Max(0, value), Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        public ShellRebarLayouts RebarLayout
        {
            get
            {
                return rebarLayout;
            }
            set
            {
                Model.Instance.Undo.Change(this, rebarLayout, GetType().GetProperty("RebarLayout"));
                rebarLayout = value;
            }
        }

        public Material.Material RebarMaterial
        {
            get
            {
                return rebarMaterial;
            }
            set
            {
                Model.Instance.Undo.Change(this, rebarMaterial, GetType().GetProperty("RebarMaterial"));
                rebarMaterial = value;
            }
        }
    }
}
