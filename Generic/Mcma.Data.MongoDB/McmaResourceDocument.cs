using Newtonsoft.Json.Linq;

namespace Mcma.Data.MongoDB
{
    public class McmaResourceDocument
    {
        public string Id { get; set; }
        
        public string Path { get; set; }
        
        public JObject Resource { get; set; }
    }
}