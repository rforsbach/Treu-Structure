using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Properties;
using System.IO;

namespace Canguro
{
    public partial class PreferencesDialog : Form
    {
        public PreferencesDialog()
        {
            InitializeComponent();
        }

        Preferences preferences = new Preferences();

        private void PreferencesDialog_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = preferences;
        }

        private void PreferencesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (DialogResult == DialogResult.Cancel)
                Settings.Default.Reload();
            else
            {
                Settings.Default.Save();
                if (preferences.needRestart)
                    Restart();
            }
            Canguro.Model.Model.Instance.ChangeModel();
        }

        private void Restart()
        {
            DialogResult result = MessageBox.Show(Culture.Get("needRestart"), Culture.Get("restart"), MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Application.Exit();
            }
        }

        private class Preferences : Canguro.Utility.GlobalizedObject
        {
            public enum LanguageSetting
            {
                English,
                Español
            }

            public bool needRestart = false;

            public void Save()
            {
                Settings.Default.Save();
                Settings.Default.Reload();
            }

            public void Reload()
            {
                Settings.Default.Reload();
            }

            [Category("Language")]
            [Description("You'll need to restart Treu Structure to see this setting change")]
            public LanguageSetting Language
            {
                get { return (Settings.Default.Culture.ToLower().StartsWith("en"))? LanguageSetting.English : LanguageSetting.Español; }
                set
                {
                    if (value != Language)
                    {
                        Settings.Default.Culture = (value == LanguageSetting.English) ? "en-US" : "es-MX";
                        needRestart = true;
                    }
                }
            }

            [Category("Color")]
            public Color ForceLoadForceDefaultColor
            {
                get { return Settings.Default.ForceLoadForceDefaultColor; }
                set
                {
                    Settings.Default.ForceLoadForceDefaultColor = value;
                }// color
            }

            [Category("Color")]
            public Color ForceLoadMomentDefaultColor
            {
                get { return Settings.Default.ForceLoadMomentDefaultColor; }
                set
                {
                    Settings.Default.ForceLoadMomentDefaultColor = value;
                }// color
            }

            [Category("Color")]
            public Color GrndDispLoadRotDefautlColor
            {
                get { return Settings.Default.GrndDispLoadRotDefautlColor; }
                set
                {
                    Settings.Default.GrndDispLoadRotDefautlColor = value;
                }// color
            }

            [Category("Color")]
            public Color GrndDispLoadTransDefaultColor
            {
                get { return Settings.Default.GrndDispLoadTransDefaultColor; }
                set
                {
                    Settings.Default.GrndDispLoadTransDefaultColor = value;
                }// color
            }

            [Category("Color")]
            public Color JointDefaultColor
            {
                get { return Settings.Default.JointDefaultColor; }
                set
                {
                    Settings.Default.JointDefaultColor = value;
                }// color
            }

            [Category("Color")]
            public Color JointSelectedDefaultColor
            {
                get { return Settings.Default.SelectedDefaultColor; }
                set
                {
                    Settings.Default.SelectedDefaultColor = value;
                }// color
            }

            [Category("Color")]
            public Color SmallPanelForeColor
            {
                get { return Settings.Default.SmallPanelForeColor; }
                set
                {
                    Settings.Default.SmallPanelForeColor = value;
                }// color
            }


            [Category("Color")]
            public Color BackgroundColor
            {
                get { return Settings.Default.BackColor; }
                set
                {
                    Settings.Default.BackColor = value;
                }// color
            }
        }
    }
}