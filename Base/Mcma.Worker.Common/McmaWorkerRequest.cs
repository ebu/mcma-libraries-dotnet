using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mcma.Worker.Common
{
    public class McmaWorkerRequest
    {
        public string OperationName { get; set; }
        
        public JObject Input { get; set; }
        
        public McmaTracker Tracker { get; set; }
    }
}
