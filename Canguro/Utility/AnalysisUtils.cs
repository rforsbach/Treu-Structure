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
    }
}
