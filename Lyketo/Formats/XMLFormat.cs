using System.Xml;

namespace Lyketo.Formats
{
    /// <summary>
    /// Implements an XML format for YAMT Proto tools.
    /// </summary>
    public class XMLFormat : IFormat
    {
        private XmlDocument Doc;
        private bool Write;
        private XmlNode CurrentNode;
        private string Path;
        private XmlNodeList Nodes;

        public bool Finalize()
        {
            if (Write)
            {
                Doc.Save(Path);
            }

            return true;
        }

        public bool Initialize(string fileName, bool write)
        {
            Write = write;
            Path = fileName;

            Doc = new XmlDocument
            {
                PreserveWhitespace = false
            };

            if (!write)
            {
                Doc.Load(fileName);

                var first = Doc.FirstChild;

                if (first.Name == "xml")
                {
                    first = first.NextSibling;
                }

                if (first.Name != "ItemProto")
                    return false;

                Nodes = first.ChildNodes;
                CurrentNode = first.FirstChild;
            }
            else
            {
                var first = Doc.CreateElement("ItemProto");
                first.SetAttribute("Version", "1");
                first.SetAttribute("Convert", "YAMT");
                Doc.AppendChild(first);

                CurrentNode = Doc.CreateElement("ItemDef");
            }

            return true;
        }

        public void Next()
        {
            if (!Write)
            {
                CurrentNode = CurrentNode.NextSibling;
            }
            else
            {
                Doc.FirstChild.AppendChild(CurrentNode);
                CurrentNode = Doc.CreateElement("ItemDef");
            }
        }

        public int Count()
        {
            if (!Write)
                return Nodes.Count;

            return 0;
        }

        public string FilterName()
        {
            return "XML files (*.xml)|*.xml";
        }

        #region Get functions
        public string GetString(string field, int len)
        {
            // Length is not important in XML strings

            return CurrentNode.Attributes.GetNamedItem(field).Value;
        }

        public double GetDouble(string field)
        {
            return double.Parse(GetString(field, 0));
        }

        public float GetFloat(string field)
        {
            return float.Parse(GetString(field, 0));
        }

        public short GetInt16(string field)
        {
            return short.Parse(GetString(field, 0));
        }

        public int GetInt32(string field)
        {
            return int.Parse(GetString(field, 0));
        }

        public long GetInt64(string field)
        {
            return long.Parse(GetString(field, 0));
        }

        public char GetInt8(string field)
        {
            return char.Parse(GetString(field, 0));
        }

        public ushort GetUInt16(string field)
        {
            return ushort.Parse(GetString(field, 0));
        }

        public uint GetUint32(string field)
        {
            return uint.Parse(GetString(field, 0));
        }

        public ulong GetUInt64(string field)
        {
            return ulong.Parse(GetString(field, 0));
        }

        public byte GetUInt8(string field)
        {
            return byte.Parse(GetString(field, 0));
        }
        #endregion

        #region Set functions
        public void Set(string field, string value, int len)
        {
            // Length is not important in XML strings

            XmlAttribute attr = Doc.CreateAttribute(field);
            attr.Value = value;
            CurrentNode.Attributes.Append(attr);
        }

        public void Set(string field, uint value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, int value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, short value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, ushort value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, float value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, double value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, char value)
        {
            Set(field, ((int)value).ToString(), 0);
        }

        public void Set(string field, byte value)
        {
            Set(field, ((int)value).ToString(), 0);
        }

        public void Set(string field, long value)
        {
            Set(field, value.ToString(), 0);
        }

        public void Set(string field, ulong value)
        {
            Set(field, value.ToString(), 0);
        }
        #endregion
    }
}
