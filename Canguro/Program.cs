using System; 
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;

namespace Canguro
{
    static class Program
    {
        /// <summary>
        /// This static constructor executes before Main
        /// Sets the Culture to the value defined in Settings (before Canguro.Culture initializes)
        /// </summary>
        static Program()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new
                System.Globalization.CultureInfo(Properties.Settings.Default.Culture);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Windows Vista Virtualization Problem FIX
            if (fixVistaVirtualizationError())
                return;
            // End of Windows Vista FIX

            Utility.Updater updater = new Utility.Updater();
            if (updater.Update())
                return;

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            Splash.Show(3000);
            Model.Model model = Model.Model.Instance;
            Controller.Controller controller = Controller.Controller.Instance;
            View.GraphicViewManager view = View.GraphicViewManager.Instance;
            model.Reset();
            //TestModel(model);

            using (MainFrm frm = new MainFrm())
            {
                frm.Show();

                try
                {
                    view.InitializeGraphics(frm.ScenePanel, frm);

                    controller.MainFrm = frm;

                    if (args.Length > 0)
                        controller.LoadModel(args[0]);

                    //////// TESTS /////////////
                    //TestView(view);
                    //model.AbstractCases.Add(new Canguro.Model.Load.AnalysisCase("aCase1"));
                    //Model.Load.AnalysisCase anc = model.AbstractCases[0] as Model.Load.AnalysisCase; 
                    //if (anc != null)
                    //{
                    //    Model.Load.StaticCaseProps props = anc.Properties as Model.Load.StaticCaseProps;
                    //    if (props != null)
                    //    {
                    //        List<Model.Load.StaticCaseFactor> list = props.Loads;
                    //        list.Add(new Canguro.Model.Load.StaticCaseFactor(model.ActiveLoadCase));
                    //        props.Loads = list;
                    //    }
                    //}
                    ////////// END TESTS /////////////
                    Application.Run(frm);
                    //              Model.Model.Instance.Save();
                    controller.Execute("cancel");
                    updater.CancelDownload();
                    updater.InstallUpdate();
                }
                    
#if DEBUG
                catch (View.NoDirectXSupportException ex)
                {
                    MessageBox.Show(Culture.Get("strDirectXFatalError") + "\n" + ex.ToString(), Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Microsoft.DirectX.DirectXException ex)
                {
                    MessageBox.Show(Culture.Get("strDirectXFatalError") + "\n" + ex.ToString(), Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (NotSupportedException ex)
                {
                    MessageBox.Show(ex.Message, Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
#else
                catch (View.NoDirectXSupportException ex)
                {
                    MessageBox.Show(Culture.Get("strDirectXFatalError"), Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Microsoft.DirectX.DirectXException ex)
                {
                    MessageBox.Show(Culture.Get("strDirectXFatalError"), Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (NotSupportedException ex)
                {
                    MessageBox.Show(ex.Message, Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Culture.Get("strFatalError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
#endif

            }
        }

        private static void TestModel(Canguro.Model.Model model)
        {
            model.JointList.Add(new Canguro.Model.Joint(-2, 5, 0));
            model.JointList.Add(new Canguro.Model.Joint(0, 0, 0));
            model.JointList.Add(new Canguro.Model.Joint(2, 5, 0));
            model.JointList.Add(new Canguro.Model.Joint(0, 1, 0));
            Model.AreaProps props = new Model.AreaProps();
            Model.AreaElement area = new Canguro.Model.AreaElement(props, model.JointList[1], model.JointList[2], model.JointList[3], model.JointList[4]);
            model.AreaList.Add(area);
            Canguro.Model.Load.Load l = null;
            area.Loads.Add(l);
        }

        private static bool fixVistaVirtualizationError()
        {            
            try
            {
                bool fixedVirtualization = Properties.Settings.Default.VistaVirtualizationFixed;
                return false;
            }
            catch (NullReferenceException)
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Treu Software\Treu Structure\TreuStructure.exe.config");
                System.IO.File.Move(path, path + ".old");
                MessageBox.Show(Culture.Get("vistaVirtualizationFixed"));
                Application.Restart();
                Application.Exit();
                return true;
            }
        }
    }
}
