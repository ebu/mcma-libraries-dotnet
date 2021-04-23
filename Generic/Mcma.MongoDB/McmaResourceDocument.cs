using Newtonsoft.Json.Linq;

namespace Mcma.MongoDb
{
    public class McmaResourceDocument
    {
        public string Id { get; set; }
        
        public string Path { get; set; }
        
        public JObject Resource { get; set; }
    }
}