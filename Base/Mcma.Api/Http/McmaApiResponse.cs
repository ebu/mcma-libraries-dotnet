using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mcma.Api
{
    public class McmaApiResponse
    {
        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public byte[] Body { get; set; }

        public JToken JsonBody { get; set; }
    }
}