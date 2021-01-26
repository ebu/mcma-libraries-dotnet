using Mcma.Serialization;
using Newtonsoft.Json;

namespace Mcma
{
    public class McmaObject : McmaExpandoObject
    {
        public McmaObject()
        {
            Type = GetType().Name;
        }
        
        [JsonProperty(McmaJson.TypePropertyName)]
        public string Type { get; set; }
    }
}