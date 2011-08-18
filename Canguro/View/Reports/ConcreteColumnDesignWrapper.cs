using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ConcreteColumnDesignWrapper : ReportData
    {
        uint id;
        string error;
        string warning;
        string pMMArea;
        string pMMRatio;
        string vMajRebar;
        string vMinRebar;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public ConcreteColumnDesignWrapper(Canguro.Model.LineElement frame, Canguro.Model.Results.ConcreteColumnDesign design)
        {
            id = frame.Id;
            error = design.ErrMsg;
            warning = design.WarnMsg;
            pMMArea = us.FromInternational(design.PMMArea, Canguro.Model.UnitSystem.Units.Area).ToString("E3");
            pMMRatio = design.PMMRatio;  
            vMajRebar = design.VMajRebar.ToString();  
            vMinRebar = design.VMinRebar.ToString();
        }

        private static List<System.ComponentModel.PropertyDescriptor> myProps = null;
        [System.ComponentModel.Browsable(false)]
        public override List<System.ComponentModel.PropertyDescriptor> Properties
        {
            get
            {
                if (myProps == null)
                    myProps = InitProps();
                return myProps;
            }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 800)]
        public string Frame
        {
            get { return id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 3000)]
        public string Error
        {
            get { return error; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1200)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Area)]
        public string PMMArea
        {
            get { return pMMArea; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1200)]
        public string PMMRatio
        {
            get { return (pMMRatio == null) ? "0.000E+000" : pMMRatio; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1200)]
        public string VMajRebar
        {
            get { return vMajRebar; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1200)]
        public string VMinRebar
        {
            get { return vMinRebar; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 2000)]
        public string Warning
        {
            get { return warning; }
            set { }
        }
    }
}