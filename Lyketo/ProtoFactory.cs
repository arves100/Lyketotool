using System;
using Lyketo.JSON;
using Lyketo.Formats;
using System.Collections.Generic;

namespace Lyketo
{
    /// <summary>
    /// This class performs the logic behind dynamic Proto conversion
    /// </summary>
    public class ProtoFactory
    {
        /// <summary>
        /// Sets the dst base field from the src base field by knowing it's type.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="src">The source format.</param>
        /// <param name="dst">The destination format.</param>
        /// <returns>True if the type was setted up, otherwise false.</returns>
        private static bool SetBaseType(string type, string name, IFormat src, IFormat dst)
        {
            if (type == "int32")
            {
                dst.Set(name, src.GetInt32(name));
                return true;
            }
            else if (type == "uint32")
            {
                dst.Set(name, src.GetUint32(name));
                return true;
            }
            else if (type == "int16")
            {
                dst.Set(name, src.GetInt16(name));
                return true;
            }
            else if (type == "uint16")
            {
                dst.Set(name, src.GetUInt16(name));
                return true;
            }
            else if (type == "int8" || type == "char")
            {
                dst.Set(name, src.GetInt8(name));
                return true;
            }
            else if (type == "uint8" || type == "uchar" || type == "byte")
            {
                dst.Set(name, src.GetUInt8(name));
                return true;
            }
            else if (type == "int64")
            {
                dst.Set(name, src.GetInt64(name));
                return true;
            }
            else if (type == "uint64")
            {
                dst.Set(name, src.GetUInt64(name));
                return true;
            }
            else if (type == "float")
            {
                dst.Set(name, src.GetFloat(name));
                return true;
            }
            else if (type == "double")
            {
                dst.Set(name, src.GetDouble(name));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the dst complex field from the src complex field by knowing it's type.
        /// </summary>
        /// <param name="complexObj">The complex object.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="src">The source format.</param>
        /// <param name="dst">The destination format.</param>
        /// <returns>True if the type was setted up, otherwise false.</returns>
        private static bool SetComplexType(ListObjectDefine complexObj, string name, IFormat src, IFormat dst, int k, int c)
        {
            foreach (var obj in complexObj.list)
            {
                string newName = GetNewName(name + obj.name, k, c);

                if (obj.type == "string")
                {
                    SetStringType(obj.count, newName, src, dst);
                    continue;
                }

                for (int i = 0; i < obj.count; i++)
                {
                    if (obj.count > 1)
                    {
                        newName += "_" + i.ToString();
                    }

                    if (!SetBaseType(obj.type, newName, src, dst))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Append the number to an existing name if the number is greater than 0.
        /// </summary>
        /// <param name="oldName">The string that will be appended.</param>
        /// <param name="i">The number.</param>
        /// <returns>The newly modified string.</returns>
        private static string GetNewName(string oldName, int i, int c)
        {
            if (c > 1)
                return oldName + i.ToString();

            return oldName;
        }

        private static void SetStringType(int strlen, string name, IFormat src, IFormat dst)
        {
            dst.Set(name, src.GetString(name, strlen), strlen);
        }

        /// <summary>
        /// Process a proto entry from format A to format B by using the dynamic definition.
        /// 
        /// NOTE: Both src and dst MUST BE initialized before this function should be called.
        /// NOTE: Both src and dst finalize MUST BE called after this function, this function
        /// will not call them.
        /// </summary>
        /// <param name="def">A JSONParser class that stores the definition.</param>
        /// <param name="src">The source format.</param>
        /// <param name="dst">The destination format.</param>
        /// <param name="list">The list to get the defines from.</param>
        private static void ProcessEntry(JSONParser def, IFormat src, IFormat dst, List<BasicObjectDefine> list)
        {
            foreach (var objdef in list)
            {
                if (objdef.type == "string")
                {
                    SetStringType(objdef.count, objdef.name, src, dst);
                    continue;
                }

                for (int i = 0; i < objdef.count; i++)
                {
                    if (SetBaseType(objdef.type, GetNewName(objdef.name, i, objdef.count), src, dst))
                        continue;

                    var complexType = def.GetTypedefObjectFromName(objdef.type);

                    if (complexType == null)
                    {
                        throw new Exception($"Complex type not defined {objdef.type}");
                    }

                    if (!SetComplexType(complexType, objdef.name, src, dst, i, objdef.count))
                    {
                        throw new Exception($"Invalid type {objdef.type} for {objdef.name}");
                    }
                }
            }
        }

        /// <summary>
        /// Converts an Item Proto from format A to format B by using the dynamic definition.
        /// 
        /// NOTE: Both src and dst MUST BE initialized before this function should be called.
        /// NOTE: Both src and dst finalize MUST BE called after this function, this function
        /// will not call them.
        /// </summary>
        /// <param name="def">A JSONParser class that stores the definition.</param>
        /// <param name="src">The source format.</param>
        /// <param name="dst">The destination format.</param>
        public static void ProcessItemProto(JSONParser def, IFormat src, IFormat dst)
        {
            for (int i = 0; i < src.Count(); i++)
            {
                ProcessEntry(def, src, dst, def.ItemProto);
                src.Next();
                dst.Next();
            }
        }
    }
}
