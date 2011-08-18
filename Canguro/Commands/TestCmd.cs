using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Tracking;
using System.Reflection;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to make authomated tests to the system. Should not be availabe in Release mode.
    /// </summary>
    public class TestCmd : Canguro.Commands.ModelCommand
    {
        private TestCmd() { }
        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly TestCmd Instance = new TestCmd();

        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "TestCmd"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("TestCmd");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Runs a test.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Type t in Assembly.GetEntryAssembly().GetTypes())
            {
                if ("Canguro.View.Reports".Equals(t.Namespace))
                {
                    Console.WriteLine("{0}{1}", t.Namespace.Replace(".",""), t.Name);
                    //foreach (PropertyInfo prop in t.GetProperties())
                    //{
                    //    Console.WriteLine("{0}\t{1}", prop.ToString().Replace(".", ""), prop.Name);
                    //}
                }
            }
        }
    }
}
