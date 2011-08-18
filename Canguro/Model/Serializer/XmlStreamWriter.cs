using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Canguro.Model.Serializer
{
    internal class XmlStreamWriter : System.Xml.XmlWriter
    {
        TextWriter writer;
        Stack<string> tags = new Stack<string>();
        bool tagIsOpen = false;

        public XmlStreamWriter(Stream stream)
        {
            writer = new StreamWriter(stream, Encoding.UTF8);
        }

        public override void Close()
        {
            //writer.Close();
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteCData(string text)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteCharEntity(char ch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteComment(string text)
        {
            if (tagIsOpen)
                writer.Write(">");
            tagIsOpen = false;
            writer.Write("<!--{0}-->", EncodeValue(text));
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteEndAttribute()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteEndDocument()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteEndElement()
        {
            string tag = tags.Pop();
            if (tagIsOpen)
                writer.Write(" />");
            else
                writer.Write("</{0}>", tag);
            tagIsOpen = false;
        }

        public override void WriteEntityRef(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteFullEndElement()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteRaw(string data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            writer.Write(" {0}=", EncodeValue(localName));
        }

        public override void WriteStartDocument(bool standalone)
        {
            writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        }

        public override void WriteStartDocument()
        {
            WriteStartDocument(true);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (tagIsOpen)
                writer.Write(">");
            tags.Push(localName);
            tagIsOpen = true;
            writer.Write("<{0}", localName);
        }

        public override System.Xml.WriteState WriteState
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override void WriteString(string text)
        {
            writer.Write("\"{0}\"", EncodeValue(text));
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteWhitespace(string ws)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        StringBuilder buffStr = new StringBuilder();
        private string EncodeValue(string text)
        {
            buffStr.Remove(0, buffStr.Length);

            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text)
                {
                    if (c == '\'')
                        buffStr.Append("&apos;");
                    else if (c == '"')
                        buffStr.Append("&quot;");
                    else if (c < '0' || c > 'z' || (c > '9' && c < 'A'))
                        buffStr.Append(string.Format("&#x{0:x};", (int)c));
                    else
                        buffStr.Append(c);
                }
            }
            return buffStr.ToString();
        }
    }
}
