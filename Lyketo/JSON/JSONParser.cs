using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lyketo.JSON
{
    /// <summary>
    /// This class defines a basic object used for proto parsing.
    /// </summary>
    public class BasicObjectDefine
    {
        /// <summary>
        /// The object type, BasicObjectDefine if it's a complex or custom object.
        /// </summary>
        public string type;

        /// <summary>
        /// The name of the object.
        /// </summary>
        public string name;

        /// <summary>
        /// The number of such object. (used in array types)
        /// </summary>
        public int count;
    }

    /// <summary>
    /// This class defines a list of BasicObjectDefine, used in custom typedefs.
    /// </summary>
    public class ListObjectDefine
    {
        /// <summary>
        /// The name of the object list.
        /// </summary>
        public string name;

        /// <summary>
        /// The list containing the objects.
        /// </summary>
        public List<BasicObjectDefine> list;
    }

    /// <summary>
    /// This class converts JSON Proto definition into
    /// custom JSONObject classes that will be used by the 
    /// ProtoFactory to perform actions for our protos.
    /// </summary>
    public class JSONParser
    {
        /// <summary>
        /// Item Proto object list.
        /// </summary>
        public List<BasicObjectDefine> ItemProto { get; protected set; }

        /// <summary>
        /// Mob proto object list.
        /// </summary>
        public List<BasicObjectDefine> MobProto { get; protected set; }

        /// <summary>
        /// Custom types.
        /// </summary>
        private List<ListObjectDefine> Typedefs;

        /// <summary>
        /// Default costructor
        /// </summary>
        public JSONParser()
        {
            ItemProto = new List<BasicObjectDefine>();
            MobProto = new List<BasicObjectDefine>();
            Typedefs = new List<ListObjectDefine>();
        }

        /// <summary>
        /// This function returns the specific Typedef object from
        /// a Typdef name
        /// </summary>
        /// <param name="name">The Typedef name</param>
        /// <returns>A JSONObjectList containing the definition of the Typedef, or null if the type does not exists.</returns>
        public ListObjectDefine GetTypedefObjectFromName(string name)
        {
            foreach (var typedef in Typedefs)
            {
                if (typedef.name == name)
                {
                    return typedef;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a JSON from a file.
        /// </summary>
        /// <param name="fileName">The name of the file to be parsed.</param>
        /// <returns>True if the parsing succeeded, otherwise false.</returns>
        public bool Parse(string fileName)
        {
            JSONDefinition def = JsonConvert.DeserializeObject<JSONDefinition>(File.ReadAllText(fileName));

            if (def.MobProto == null && def.ItemProto == null)
            {
                return false; // Invalid definition file.
            }

            // Custom typedef parser.
            if (def.Typedefs != null && def.Typedefs.Count > 0)
            {
                Typedefs.Clear();

                foreach (var typedef in def.Typedefs)
                {
                    ParseTypedef(typedef);
                }
            }

            // A definition might only include item or mob proto.

            if (def.ItemProto != null)
            {
                ParseProto(def.ItemProto, ItemProto);
            }

            if (def.MobProto != null)
            {
                ParseProto(def.MobProto, MobProto);
            }

            return true;
        }

        /// <summary>
        /// Convert a JSONTypedef into a ListObjectDefine and store it
        /// inside the Typedef array.
        /// </summary>
        /// <param name="typedef">A JSON Typedef object.</param>
        private void ParseTypedef(JSONTypedef typedef)
        {
            ListObjectDefine obj = new ListObjectDefine();

            obj.name = typedef.name;
            obj.list = new List<BasicObjectDefine>();

            foreach (var data in typedef.content)
            {
                BasicObjectDefine cobj = new BasicObjectDefine();
                cobj.name = data.Key;

                string typeName;
                GetTypeCountFromString(data.Value, out typeName, out cobj.count);

                if (!ValidateBaseType(typeName))
                {
                    throw new Exception($"Invalid type or complex type {typeName} in typedef {obj.name}");
                }

                cobj.type = typeName;

                obj.list.Add(cobj);
            }

            Typedefs.Add(obj);
        }

        /// <summary>
        /// Gets from the type string the type and it's count.
        /// 
        /// For example "uint32 4" will be splitted as "uint32" with count 4.
        /// 
        /// In case "uint32" is passed the count will be defauted to 1.
        /// </summary>
        /// <param name="baseString">The string to gets the two types.</param>
        /// <param name="typeName">An output string that will contain the type name.</param>
        /// <param name="count">An output int that will contain the count of such type.</param>
        private void GetTypeCountFromString(string baseString, out string typeName, out int count)
        {
            if (baseString.Contains(" "))
            {
                var splitValue = baseString.Split(' ');

                if (splitValue.Length > 2)
                {
                    throw new Exception($"Invalid type array definition for string {baseString}");
                }

                typeName = splitValue[0];
                count = int.Parse(splitValue[1]);

                return;
            }

            typeName = baseString;
            count = 1;
        }

        /// <summary>
        /// Validates a base type.
        /// 
        /// NOTE: This function does not convert complex types (Typedef types),
        /// it only converts generic or basic types such as int16, int32
        /// </summary>
        /// <param name="dataType">The type name to verify.</param>
        /// <returns>True if the data is valid, otherwise false.</returns>
        private bool ValidateBaseType(string dataType)
        {
            return dataType == "int32" || dataType == "uint32" || dataType == "uint64" ||
                dataType == "int64" || dataType == "uint16" || dataType == "int16" ||
                dataType == "byte" || dataType == "int8" || dataType == "uint8" ||
                dataType == "uchar" || dataType == "char" || dataType == "string";
        }

        /// <summary>
        /// Verify if the user Typedef exists.
        /// </summary>
        /// <param name="dataType">The type name to verify.</param>
        /// <returns>True if the complex type exists, otherwise false.</returns>
        private bool ValidateComplexType(string dataType)
        {
            foreach (var typedef in Typedefs)
            {
                if (typedef.name == dataType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts a JSONProto into a List of BasicObjectDefines.
        /// </summary>
        /// <param name="jsonProto">The JSON proto to convert.</param>
        /// <param name="outProto">The list that will receive the results.</param>
        private void ParseProto(Dictionary<string, string> jsonProto, List<BasicObjectDefine> outProto)
        {
            outProto.Clear();

            foreach (var data in jsonProto)
            {
                BasicObjectDefine obj = new BasicObjectDefine();
                string dataType;

                obj.name = data.Key;

                GetTypeCountFromString(data.Value, out dataType, out obj.count);

                if (!ValidateBaseType(dataType))
                {
                    if (!ValidateComplexType(dataType))
                    {
                        throw new Exception($"Invalid type specified. {dataType}");

                    }
                }

                obj.type = dataType;

                outProto.Add(obj);
            }
        }
    }
}
