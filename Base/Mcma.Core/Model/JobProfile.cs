using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mcma
{
    public class JobProfile : McmaResource
    {
        public string Name { get; set; }

        public ICollection<JobParameter> InputParameters { get; set; }

        public ICollection<JobParameter> OutputParameters { get; set; }
        
        public ICollection<JobParameter> OptionalInputParameters { get; set; }
        
        public JObject CustomProperties { get; set; }
    }
}