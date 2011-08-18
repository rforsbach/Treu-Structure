using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using Canguro.Model;
using Canguro.Model.Results;

namespace Canguro.Utility
{
    class AnalysisUtils
    {
        internal static bool CanAnalyze(Canguro.Model.Model model, ref string message, out bool isConnected)
        {
            List<LinkedList<int>> adjList = model.GetConnectivityGraph();
            int count = model.JointList.Count;
            bool[] connected = new bool[count];
            Stack<int> stack = new Stack<int>();
            isConnected = true;

            // Check for zero length lines
            float minLength = Properties.Settings.Default.JoinTolerance;
            foreach (LineElement line in model.LineList)
                if (line != null && line.LengthInt < minLength)
                {
                    message = Culture.Get("zeroLengthLines");
                    isConnected = false;
                    return false;
                }

            // Check Connectivity
            for (int jid = 0; jid < model.JointList.Count; jid++)
            {
                if (model.JointList[jid] == null)
                    connected[jid] = true;
                else if (stack.Count == 0)
                    stack.Push(jid);
            }
            if (stack.Count == 0)
            {
                message = Culture.Get("noJointsToAnalyze");
                return false;
            }

            connected[stack.Peek()] = true;
            while (stack.Count > 0)
            {
                int curr = stack.Pop();
                if (adjList[curr] != null)
                {
                    foreach (int i in adjList[curr])
                    {
                        if (!connected[i])
                        {
                            connected[i] = true;
                            stack.Push(i);
                        }
                    }
                }
            }

            for (int i = 0; i < connected.Length; i++)
            {
                if (!connected[i])
                {
                    isConnected = false;
                    model.JointList[i].IsVisible = true;
                    model.JointList[i].IsSelected = true;
                }
                else if (model.JointList[i] != null)
                    model.JointList[i].IsSelected = false;
            }

            if (!isConnected)
            {
                foreach (LineElement line in model.LineList)
                    if (line != null && line.I.IsSelected)
                        line.IsSelected = true;

                message = Culture.Get("notConnected");
                return false;
            }

            bool[] restraints = new bool[] { false, false, false };
            foreach (Joint joint in model.JointList)
                if (joint != null)
                {
                    restraints[0] |= (joint.DoF.T1 != JointDOF.DofType.Free);
                    restraints[1] |= (joint.DoF.T2 != JointDOF.DofType.Free);
                    restraints[2] |= (joint.DoF.T3 != JointDOF.DofType.Free);
                }
            if (!(restraints[0] && restraints[1] && restraints[2]))
            {
                message = Culture.Get("notEnoughRestraints");
                return false;
            }

            return true;
        }

        internal static byte[] GetCompressedModel(string file)
        {
            byte[] buffer = new byte[8192];
            MemoryStream ms = new MemoryStream();

            using (ZipOutputStream s = new ZipOutputStream(ms))
            {
                s.SetLevel(9); // 0 (store only) to 9 (best compression)
                ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                s.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    StreamUtils.Copy(fs, s, buffer);
                }
            }

            return ms.GetBuffer();
        }

        internal class LoadFileThreadParams
        {
            private Controller.CommandServices services;
            private DownloadProps fileProps;
            private StringBuilder sb;
            private RijndaelManaged decryptor;

            public LoadFileThreadParams(Controller.CommandServices services, DownloadProps props, StringBuilder sb, RijndaelManaged decryptor)
            {
                this.services = services;
                this.fileProps = props;
                this.sb = sb;
                this.decryptor = decryptor;
            }

            public RijndaelManaged Decryptor
            {
                get { return decryptor; }
                set { decryptor = value; }
            }

            public DownloadProps FileProps
            {
                get { return fileProps; }
                set { fileProps = value; }
            }

            public Controller.CommandServices Services
            {
                get { return services; }
            }

            public StringBuilder Sb
            {
                get { return sb; }
            }
        }

        internal static void loadFileFromWeb(object parameters)
        {
            try
            {
                LoadFileThreadParams loadParams = (LoadFileThreadParams)parameters;
                DownloadProps file = loadParams.FileProps;
                RijndaelManaged decryptor = loadParams.Decryptor;
                if (file.Finished) return;

                getFileFromWebServer(Properties.Settings.Default.ResultsDownloadUrl + "/" + file.FileName, loadParams.Sb, loadParams.Services, file, decryptor);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e, "GetResultsDownload");
            }
        }

        internal static void getFileFromWebServer(string uri, StringBuilder sb, Controller.CommandServices services, DownloadProps task, RijndaelManaged decryptor)
        {
            int retry = 1;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            uri = uri.Replace("#", "%23");

            while (retry > 0)
            {
                try
                {
                    // used on each read operation
                    byte[] buf = new byte[8192];

                    // prepare the web page we will be asking for
                    request = (HttpWebRequest)
                        WebRequest.Create(uri);

                    request.Timeout = 10000;

                    // execute the request
                    response = (HttpWebResponse)
                        request.GetResponse();


                    // we will read data via the encrypted compressed response stream
                    Stream ws = response.GetResponseStream();
                    ICryptoTransform ct = decryptor.CreateDecryptor(decryptor.Key, decryptor.IV);
                    CryptoStream cs = new CryptoStream(ws, ct, CryptoStreamMode.Read);
                    ZipInputStream resStream = new ZipInputStream(cs);
                    if (resStream.GetNextEntry() == null)
                        throw new FormatException(Culture.Get("EA0001"));

                    string tempString = null;
                    int totalCount = 0;
                    int count = 0;

                    do
                    {
                        // fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);
                        totalCount += count;

                        // make sure we read some data
                        if (count != 0)
                        {
                            // translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // continue building the string
                            sb.Append(tempString);

                            // TODO: Set percentage
                            //DownloadProgress progress = services.Model.Results.Downloaded;
                            services.ReportProgress(/*progress.TotalProgress*/ 0, task.CaseName, Convert.ToUInt32((float)totalCount / response.ContentLength * 100.0f));
                        }
                    }
                    while (count > 0); // any more data to read?

                    retry = 0;
                }
                catch (TimeoutException)
                {
                    System.Diagnostics.Debug.WriteLine("retry #" + retry);
                    retry++;
                }
                catch (WebException)
                {
                    System.Diagnostics.Debug.WriteLine("retry #" + retry);
                    retry++;
                }
                finally
                {
                    if (response != null)
                        response.Close();
                }
            }
        }

        internal static StringBuilder buffStr = new StringBuilder();
        internal static string DecodeStream(StringBuilder text)
        {
            buffStr.Remove(0, buffStr.Length);
            int textLen = text.Length;
            int state = 0;
            int code = 0;
            char[] codebuffer = new char[4];

            for (int i = 0; i < textLen; i++)
            {
                char c = text[i];

                if (c == '&')
                    state = 1;
                else if (state == 1 && c == '#')
                    state = 2;
                else if (state == 2 && c == 'x')
                    state = 3;
                else if (state >= 3 && c != ';')
                {
                    if (state > 6)
                        throw new FormatException("Incorrectly coded character");

                    codebuffer[state - 3] = c;
                    ++state;
                }
                else if (state >= 3 && c == ';')
                {
                    string codeStr = new string(codebuffer);
                    code = int.Parse(codeStr, System.Globalization.NumberStyles.AllowHexSpecifier);
                    buffStr.Append((char)code);
                    state = 0;
                }
                else
                {
                    buffStr.Append(c);
                    state = 0;
                }
            }

            return buffStr.ToString();
        }

        internal static string DecodeStream(string text)
        {
            buffStr.Remove(0, buffStr.Length);
            int state = 0;
            int code = 0;
            char[] codebuffer = new char[4];

            foreach (char c in text)
            {
                if (c == '&')
                    state = 1;
                else if (state == 1 && c == '#')
                    state = 2;
                else if (state == 2 && c == 'x')
                    state = 3;
                else if (state >= 3 && c != ';')
                {
                    if (state > 6)
                        throw new FormatException("Incorrectly coded character");

                    codebuffer[state - 3] = c;
                    ++state;
                }
                else if (state >= 3 && c == ';')
                {
                    string codeStr = new string(codebuffer);
                    code = int.Parse(codeStr, System.Globalization.NumberStyles.AllowHexSpecifier);
                    buffStr.Append((char)code);
                    state = 0;
                }
                else
                {
                    buffStr.Append(c);
                    state = 0;
                }
            }

            return buffStr.ToString();
        }
    }
}
