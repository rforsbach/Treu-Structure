using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    public class AnalysisManifest
    {        
        private ManifestItem summary;
        private ManifestItem design;
        private List<ManifestItem> items;
        private string model;
        private byte[] decryptionKey;
        private byte[] decryptionVector;

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

        public ManifestItem Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        public ManifestItem Design
        {
            get { return design; }
            set { design = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public List<ManifestItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public AnalysisManifest()
        {
            items = new List<ManifestItem>();
        }
    }

    public class ManifestItem
    {
        private string filePath;
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public ManifestItem() { }
        public ManifestItem(string filePath, string description)
        {
            this.filePath = filePath;
            this.description = description;
        }
    }
}
