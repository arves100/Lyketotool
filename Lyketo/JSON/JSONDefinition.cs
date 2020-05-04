using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lyketo.JSON
{
    /// <summary>
    /// This object represents a custom type that might be used
    /// inside any proto type.
    /// </summary>
    [JsonObject]
    public class JSONTypedef
    {
        /// <summary>
        /// Name of the custom type (for example Apply).
        /// </summary>
        public string name;

        /// <summary>
        /// The data contained inside the custom type.
        /// 
        /// The format of the data works the same as
        /// JSONProto, which the exception that is does
        /// support only basic data.
        /// </summary>
        public Dictionary<string, string> content;
    }

    /// <summary>
    /// This object defines a YAMT JSON Proto definition file.
    /// 
    /// This JSON file is responsable for defining the structure of the Item or Mob Proto.
    /// </summary>
    [JsonObject]
    public class JSONDefinition
    {
        /// <summary>
        /// List of custom definitions like Item Proto Apply table.
        /// </summary>
        public List<JSONTypedef> Typedefs;

        /// <summary>
        /// Item Proto definition
        /// </summary>
        [JsonProperty(PropertyName = "Item_Proto")]
        public Dictionary<string, string> ItemProto;

        /// <summary>
        /// Mob Proto definition
        /// </summary>
        [JsonProperty(PropertyName = "Mob_Proto")]
        public Dictionary<string, string> MobProto;
    }
}
