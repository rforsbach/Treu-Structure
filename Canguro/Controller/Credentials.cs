using System;
using System.IO;
using System.Net;
using System.IO.IsolatedStorage;
using System.Security.Policy;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Security.Cryptography;

namespace Canguro.Controller
{
    [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    internal class Credentials
    {
        private string user;
        private string password;
        private string displayName;
        private string hostName;
        private string serial;
        const string filename = "credentials.txt";

        public Credentials(string usr, string psw)
        {
            user = usr;
            password = psw;
            Authenticate();
        }

        public Credentials()
        {
            Load();
        }

        public void Load()
        {
            user = "";
            password = "";
            try
            {
                TryReadCredentials();
                Authenticate();
                hostName = Dns.GetHostName();
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.ToString()); 
#endif
            }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public string UserName
        {
            get { return user; }
            set { user = value; }
        }

        public string Description
        {
            get { return hostName; }
        }

        public string Serial
        {
            get 
            {
                return "";
            }
        }

        public string Password
        {
            internal get { return password; }
            set { password = value; }
        }

        public bool Authenticate()
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                displayName = "";
                return false;
            }
            try
            {
                CanguroServer.Analysis ws = new CanguroServer.Analysis();
                displayName = ws.Authenticate(user, password);
              //displayName = ws.Authenticate(user, password, hostName, LocalIP);
                return !string.IsNullOrEmpty(displayName);
            }
            catch (Exception) { } // Ignore

            // Call web service, get credit.
            displayName = user;
            return true;
        }

        public void Save()
        {
            IsolatedStorageFile file = null;
            StreamWriter writer = null;
            try
            {
                file = IsolatedStorageFile.GetUserStoreForAssembly();
                //string[] filenames = file.GetFileNames(filename);
                //if (filenames.Length > 0)
                //    file.DeleteFile(filename);

                IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, file);
                writer = new StreamWriter(isoStream);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                RijndaelManaged encriptor = new RijndaelManaged();
                encriptor.Padding = PaddingMode.None;
                encriptor.Key = encoding.GetBytes("blahblahblahblahblahblahblahblah");
                encriptor.IV = encoding.GetBytes("hblahblahblahbla");
                ICryptoTransform crypto = encriptor.CreateEncryptor();
                string usrPass = user + "|||" + password + "|||";
                usrPass = usrPass.PadRight(256);
                byte[] cUsr = crypto.TransformFinalBlock(encoding.GetBytes(usrPass), 0, usrPass.Length);

                isoStream.Write(cUsr, 0, cUsr.Length);
            }
            catch (IsolatedStorageException ex)
            {
                throw new Exception(Culture.Get("cantSaveFile"), ex);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
        }

        private bool TryReadCredentials()
        {
            try
            {
                // Retrieve an IsolatedStorageFile for the current Domain and Assembly.
                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForAssembly();

                if (!isoFile.FileExists(filename))
                    return false;

                IsolatedStorageFileStream isoStream =
                    new IsolatedStorageFileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    isoFile);

                StreamReader reader = new StreamReader(isoStream);
                // Read the data.

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                RijndaelManaged encriptor = new RijndaelManaged();
                encriptor.Padding = PaddingMode.None;
                encriptor.Key = encoding.GetBytes("blahblahblahblahblahblahblahblah");
                encriptor.IV = encoding.GetBytes("hblahblahblahbla");
                ICryptoTransform crypto = encriptor.CreateDecryptor(encoding.GetBytes("blahblahblahblahblahblahblahblah"), encoding.GetBytes("hblahblahblahbla"));

                byte[] usrPass = new byte[256];
                int c = isoStream.Read(usrPass, 0, 256);
//                byte[] decrypted = crypto.TransformFinalBlock(usrPass, 0, 256);

                MemoryStream memoryStream = new MemoryStream(usrPass);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, crypto, CryptoStreamMode.Read);
                byte[] plainText = new byte[c];
                int decryptedCount = cryptoStream.Read(plainText, 0, c);
                memoryStream.Close();
                cryptoStream.Close();

                string str = encoding.GetString(plainText, 0, c);
                string[] values = str.Split(new string[] { "|||" }, StringSplitOptions.None);

                user = values[0];
                password = values[1];

                reader.Close();
                isoFile.Close();
                return true;
            }
            catch (System.IO.FileNotFoundException)
            {
                // Expected exception if a file cannot be found. This indicates that we have a new user.
                return false;
            }
        }

        public void DeleteFile()
        {
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForAssembly();
                if (file.GetFileNames(filename).Length > 0)
                {
                    file.DeleteFile(filename);
                }
                file.Dispose();
                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
