using Mcma.Serialization;
using Newtonsoft.Json;

namespace Mcma.Model
{
    /// <summary>
    /// An MCMA object that's strongly-typed, i.e it specifies a "Type" property
    /// </summary>
    public class McmaObject : McmaExpandoObject
    {
        public McmaObject()
        {
            Type = GetType().Name;
        }
        
        /// <summary>
        /// Gets or sets the type of object
        /// </summary>
        /// <remarks>This maps to the "@type" property in JSON. When creating a new object, it will be set automatically to the type of the object.</remarks>
        [JsonProperty(McmaJson.TypePropertyName)]
        public string Type { get; set; }
    }
}