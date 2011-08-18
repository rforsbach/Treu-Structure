using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Controller.Snap;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to copy the selected items to the clipboard
    /// </summary>
    public class CopyCmd : Canguro.Commands.ModelCommand
    {
        //public override void Run(Canguro.Controller.CommandServices services)
        //{
        //    List<Item> selection = GetSelection(services);
        //    if (selection.Count == 0)
        //        return;
        //    Stream stream = new MemoryStream();
        //    try
        //    {
        //        BinaryFormatter bformatter = new BinaryFormatter();
        //        Magnet magnet = services.GetPoint("selectPivot");
        //        bformatter.Serialize(stream, magnet.SnapPosition);
        //        bformatter.Serialize(stream, selection.Count);
        //        foreach (Item item in selection)
        //        {
        //            bformatter.Serialize(stream, item);
        //            item.IsSelected = true;
        //        }
        //        Clipboard.SetData("Canguro", stream);
        //    }
        //    finally
        //    {
        //        stream.Close();
        //    }
        //}

        /// <summary>
        /// Executes the command. 
        /// Gets the selection and a pivot point, and adds them to the Clipboard with the key "Canguro"
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Dictionary<uint, Joint> joints = new Dictionary<uint, Joint>();
            List<LineElement> lines = new List<LineElement>();
            List<AreaElement> areas = new List<AreaElement>();
            bool haveSelection = false;
            haveSelection = services.GetSelection(joints, lines, areas);
            if (!haveSelection)
            {
                services.GetMany(Culture.Get("selectItems"));
                haveSelection = services.GetSelection(joints, lines, areas);
            }
            if (haveSelection)
            {
                Magnet magnet = services.GetPoint(Culture.Get("selectPivot"));
                if (magnet != null)
                {
                    Microsoft.DirectX.Vector3 pivot = magnet.SnapPosition;
                    Clipboard.Clear();
                    object[] objs = new object[] { joints, lines, areas, pivot };

                    //// Test Serialization
                    //System.IO.MemoryStream s = new MemoryStream();
                    //new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(s, objs);

                    Clipboard.SetData("Canguro", objs);
                }
            }

            foreach (Item item in joints.Values)
                if (item != null)
                    item.IsSelected = true;
            foreach (Item item in lines)
                if (item != null)
                    item.IsSelected = true;
            foreach (Item item in areas)
                if (item != null)
                    item.IsSelected = true;
        }
    }
}
