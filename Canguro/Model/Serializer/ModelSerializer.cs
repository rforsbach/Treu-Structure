using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Canguro.Model.Serializer
{
    class ModelSerializer : Serializer
    {
        XmlDocument document = new XmlDocument();
        string filePath;

        public ModelSerializer(Model model, string filePath)
            : base(model)
        {
            this.filePath = filePath;
        }

        public void XmlSerialize(Stream stream)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void XmlDeserialize(XmlNode xml)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
