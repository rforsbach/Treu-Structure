using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Canguro.Utility
{
    class Updater
    {
        private char[] trimChars = "\t\n\r ".ToCharArray();
        private bool cancelDownload = false;

        public bool CheckVersion()
        {
            try
            {
                string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString().ToLower().Trim(trimChars);

                string url = Properties.Settings.Default.VersionCheckURL.Trim(trimChars);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                
                byte[] buf = new byte[128];
                
                int count = response.GetResponseStream().Read(buf, 0, buf.Length);
                string newVersion = Encoding.ASCII.GetString(buf, 0, count).ToLower().Trim(trimChars);
                string updateVersion = Properties.Settings.Default.UpdateInstallerVersion.Trim(trimChars);

                if (isHigherVersion(newVersion, version) && !version.Equals(newVersion))
                {
                    if (!newVersion.Equals(updateVersion))
                    {
                        Properties.Settings.Default.UpdateInstallerVersion = newVersion;
                        Properties.Settings.Default.Save();
                    }
                    else if (!string.IsNullOrEmpty(Properties.Settings.Default.UpdateInstallerPath.Trim()))
                        return false;

                    return true;
                }

                return false;
            }
            catch
            { }
            return false;
        }

        private bool checkInstallVersion()
        {
            try
            {
                string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString().ToLower().Trim(trimChars);
                string updateVersion = Properties.Settings.Default.UpdateInstallerVersion.Trim(trimChars);

                if (isHigherVersion(updateVersion, version) && !version.Equals(updateVersion))
                    return true;
            }
            catch { }
            
            return false;
        }

        public void CancelDownload()
        {
            cancelDownload = true;
        }

        private bool isHigherVersion(string version, string current)
        {
            foreach (char c in version.ToCharArray())
                if (!char.IsDigit(c) && c != '.')
                    return false;
            string[] arrVer = version.Split(new char[] { '.' });
            string[] arrCur = current.Split(new char[] { '.' });
            for (int i=0; i<arrVer.Length; i++)
            {
                if (int.Parse(arrVer[i]) > int.Parse(arrCur[i]))
                    return true;
                if (int.Parse(arrVer[i]) < int.Parse(arrCur[i]))
                    return false;
            }

            return false;
        }

        public bool Update()
        {
            try
            {
                if (InstallUpdate())
                    return true;

                ThreadStart start = new ThreadStart(DownloadNewVersion);
                Thread thread = new Thread(start);
                thread.Start();
            }
            catch { }
            return false;
        }

        private void DownloadNewVersion()
        {
            try
            {
                string setupFile = Properties.Settings.Default.UpdateInstallerPath;
                if (CheckVersion())
                {
                    string url = Properties.Settings.Default.UpdateInstallerURL;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    string tmpPath = Path.GetTempFileName();
                    FileStream fs = new FileStream(tmpPath, FileMode.Append);

                    byte[] buf = new byte[15000];

                    int count;
                    while ((count = response.GetResponseStream().Read(buf, 0, buf.Length)) > 0)
                    {
                        fs.Write(buf, 0, count);
                        if (cancelDownload)
                        {
                            fs.Close();
                            return;
                        }
                    }
                    fs.Close();
                    setupFile = Path.Combine(Path.GetDirectoryName(tmpPath), "tss.msi");
                    if (File.Exists(setupFile))
                        File.Delete(setupFile);

                    File.Move(tmpPath, setupFile);

                    Properties.Settings.Default.UpdateInstallerPath = setupFile;
                    Properties.Settings.Default.Save();

                    Properties.Settings.Default.Reload();
                    // .UpdateInstallerPath = setupFile;
                    //Properties.Settings.Default.Save();
                }
            }
            catch 
            {
                Properties.Settings.Default.UpdateInstallerVersion = "";
                Properties.Settings.Default.UpdateInstallerPath = "";
                Properties.Settings.Default.Save();
            }
        }

        private bool Confirm()
        {
            return MessageBox.Show(Culture.Get("confirmInstallUpdate"),
                Culture.Get("confirmInstallUpdateTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public bool InstallUpdate()
        {
            try
            {
                string setupFile = Properties.Settings.Default.UpdateInstallerPath.Trim(trimChars);

                if (!string.IsNullOrEmpty(setupFile))
                {
                    if (File.Exists(setupFile) && checkInstallVersion())
                    {
                        if (Confirm())
                        {
                            Properties.Settings.Default.UpdateInstallerPath = "";
                            Properties.Settings.Default.UpdateInstallerVersion = "";
                            Properties.Settings.Default.Save();

                            Properties.Settings.Default.Reload();

                            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(setupFile);
                            start.UseShellExecute = true;
                            System.Diagnostics.Process.Start(start);
                            return true;
                        }
                    }
                    else
                    {
                        Properties.Settings.Default.UpdateInstallerVersion = "";
                        Properties.Settings.Default.UpdateInstallerPath = "";
                        Properties.Settings.Default.Save();

                        Properties.Settings.Default.Reload();
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
