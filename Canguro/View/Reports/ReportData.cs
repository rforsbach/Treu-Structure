using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Canguro.View.Reports
{
    class ReportData : Utility.GlobalizedObject
    {
        protected List<PropertyDescriptor> props;
        public ReportData()
        {
        }

        protected List<PropertyDescriptor> InitProps()
        {
            props = new List<PropertyDescriptor>();
            PropertyDescriptorCollection tmp = GetProperties();
//            tmp.Sort(new Canguro.Model.ModelAttributes.GridPositionComparer());
            foreach (PropertyDescriptor p in tmp)
                if (p.IsBrowsable)
                    props.Add(p);
            props.Sort(new Canguro.Model.ModelAttributes.GridPositionComparerGeneric());
            return props;
        }

        private string GetPropertyValue(int index)
        {
            if (props == null)
                props = Properties;
            if (props.Count > index && !props[index].IsReadOnly)
                return props[index].GetValue(this).ToString();
            return "";
        }

        [Browsable(false)]
        public virtual List<PropertyDescriptor> Properties
        {
            get 
            {
                if (props == null)
                    props = InitProps();
                return props; 
            }
        }

        [Browsable(false)]
        public string Data1
        {
            get { return GetPropertyValue(0); }
            set { }
        }
        [Browsable(false)]
        public string Data2
        {
            get { return GetPropertyValue(1); }
            set { }
        }
        [Browsable(false)]
        public string Data3
        {
            get { return GetPropertyValue(2); }
            set { }
        }
        [Browsable(false)]
        public string Data4
        {
            get { return GetPropertyValue(3); }
            set { }
        }
        [Browsable(false)]
        public string Data5
        {
            get { return GetPropertyValue(4); }
            set { }
        }
        [Browsable(false)]
        public string Data6
        {
            get { return GetPropertyValue(5); }
            set { }
        }
        [Browsable(false)]
        public string Data7
        {
            get { return GetPropertyValue(6); }
            set { }
        }
        [Browsable(false)]
        public string Data8
        {
            get { return GetPropertyValue(7); }
            set { }
        }
        [Browsable(false)]
        public string Data9
        {
            get { return GetPropertyValue(8); }
            set { }
        }
        [Browsable(false)]
        public string Data10
        {
            get { return GetPropertyValue(9); }
            set { }
        }
    }
}