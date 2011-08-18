using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Resources;

namespace Canguro
{
    sealed public class Culture
    {
        private static ResourceManager manager = new ResourceManager("Canguro.Properties.resources", System.Reflection.Assembly.GetExecutingAssembly());

        static Culture() 
        {
            cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;            
        }

        public static string Get(string name)
        {
            try
            {
                string ret = manager.GetString(name, cultureInfo);
                if (ret == null)
                {
                    System.Diagnostics.Debug.WriteLine(name);
                    return name;
                }
                return ret;
            }
            catch { } return name;
        }

        public static string Name
        {
            get
            {
                return cultureInfo.Name;
            }
            set
            {
                cultureInfo = new CultureInfo(value);
            }
        }

        private static CultureInfo cultureInfo;

        public static string DisplayName
        {
            get
            {
                return cultureInfo.DisplayName;
            }
        }
    }
}
