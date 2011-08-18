using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    [Serializable]
    public class DownloadProgress
    {
        private string model;
        private DownloadProps summary;
        private DownloadProps design;
        private LinkedList<DownloadProps> items = new LinkedList<DownloadProps>();
        [NonSerialized]
        private LinkedListNode<DownloadProps> lastFinishedItem = null;
        [NonSerialized]
        private LinkedListNode<DownloadProps> lastWorkingItem = null;
        private string locked = "";
        private bool started = false;
        private uint totalFinished = 0;
        private uint totalItems = 0;
        private byte[] decryptionKey;
        private byte[] decryptionVector;

        public DownloadProgress()
        {
        }

        public DownloadProgress(uint totalFinished, bool started)
        {
            this.totalFinished = totalFinished;
            this.started = started;
        }

        public byte[] DecryptionKey
        {
            get { return decryptionKey; }
            set { decryptionKey = value; }
        }

        public byte[] DecryptionVector
        {
            get { return decryptionVector; }
            set { decryptionVector = value; }
        }

        public bool Started
        {
            get { return started; }
        }

        public DownloadProps PeekNextDownload()
        {
            lock (locked)
            {
                if (lastWorkingItem == null)
                {
                    if (lastFinishedItem == null)
                    {
                        if (items.First == null)
                            return null;
                        else
                            return items.First.Value;
                    }
                    else if (lastFinishedItem.Next == null)
                        return null;
                    else
                        return lastFinishedItem.Next.Value;
                }
                else if (lastWorkingItem.Next == null)
                    return null;

                return lastWorkingItem.Next.Value;
            }
        }

        public DownloadProps GetNextDownload()
        {
            lock (locked)
            {
                if (lastWorkingItem == null)
                {
                    if (lastFinishedItem == null)
                    {
                        if (items.First == null)
                            return null;
                        else
                            lastWorkingItem = items.First;
                    }
                    else if (lastFinishedItem.Next == null)
                        return null;
                    else
                        lastWorkingItem = lastFinishedItem.Next;
                }
                else if (lastWorkingItem.Next == null)
                    return null;

                lastWorkingItem = lastWorkingItem.Next;
                return lastWorkingItem.Value;
            }
        }

        public void PrioritizeItem(DownloadProps item)
        {
            lock (locked)
            {
                LinkedListNode<DownloadProps> node = items.Find(item);
                items.Remove(node);
                items.AddAfter(lastWorkingItem, node);
            }
        }

        public void FinishedItem(DownloadProps item)
        {
            lock (locked)
            {
                if (item == null) return;

                item.Finished = true;
                LinkedListNode<DownloadProps> node = items.Find(item);
                items.Remove(node);
                items.AddAfter(lastFinishedItem, node);
                lastFinishedItem = lastFinishedItem.Next;
            }
        }

        public void Start(CanguroServer.AnalysisManifest manifest)
        {
            started = false;
            lock (locked)
            {
                // Reset state
                items.Clear();
                lastFinishedItem = null;
                lastWorkingItem = null;
                summary = new DownloadProps();
                design = new DownloadProps();
                totalFinished = 0;
                decryptionKey = null;
                decryptionVector = null;
                
                // Load manifest
                summary.LoadManifestItem(manifest.Summary);
                design.LoadManifestItem(manifest.Design);
                foreach (CanguroServer.ManifestItem item in manifest.Items)
                    items.AddLast(new DownloadProps(item));

                decryptionKey = manifest.DecryptionKey;
                decryptionVector = manifest.DecryptionVector;
                
                totalItems = (uint)items.Count;
                started = true;
            }
        }

        public uint TotalProgress
        {
            get
            {
                uint progress = totalFinished * 100;                

                LinkedListNode<DownloadProps> item = lastFinishedItem;
                if (item != null)
                    item = item.Next;

                LinkedListNode<DownloadProps> endItem = lastWorkingItem;
                if (endItem != null)
                    endItem = endItem.Next;

                while (item != null && item != endItem)
                {
                    progress += item.Value.Percentage;
                    item = item.Next;
                }

                if (totalItems > 0)
                    return progress / (totalItems * 100);
                
                return 100;
            }
        }
        
        public LinkedList<DownloadProps> Items
        {
            get { return items; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public DownloadProps Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        public DownloadProps Design
        {
            get { return design; }
            set { design = value; }
        }
   }
}
