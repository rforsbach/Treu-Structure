using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Canguro.Model.Serializer
{
    class LayerSerializer : Serializer
    {
        public LayerSerializer(Model model)
            : base(model)
        {
        }

        public void XmlSerialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Layer));
            foreach (Layer val in model.Layers)
                serializer.Serialize(stream, val);
        }

        public void XmlDeserialize(XmlNode xml)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
