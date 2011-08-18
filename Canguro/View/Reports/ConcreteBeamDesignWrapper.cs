using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Results;

namespace Canguro.View.Reports
{
    class ConcreteBeamDesignWrapper : ReportData
    {
        private uint frameID;
        private string topRebarArea;
        private string bottomRebarArea;
        private string vRebar;
        private string error;
        private string warning;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public ConcreteBeamDesignWrapper(LineElement frame, ConcreteBeamDesign design)
        {
            frameID = frame.Id;
            bottomRebarArea = us.FromInternational(design.FBotArea, Canguro.Model.UnitSystem.Units.Area).ToString("E3"); 
            topRebarArea = us.FromInternational(design.FTopArea, Canguro.Model.UnitSystem.Units.Area).ToString("E3"); 
            vRebar = us.FromInternational(design.VRebar, Canguro.Model.UnitSystem.Units.Area).ToString("E3"); 

            error = design.ErrMsg;
            warning = design.WarnMsg;
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
            get { return frameID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1200)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Area)]
        public string TopRebarArea
        {
            get { return topRebarArea; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1200)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Area)]
        public string BottomRebarArea
        {
            get { return bottomRebarArea; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1200)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Area)]
        public string VRebar
        {
            get { return vRebar; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 2000)]
        public string Error
        {
            get { return error; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 2000)]
        public string Warning
        {
            get { return warning; }
            set { }
        }
    }
}