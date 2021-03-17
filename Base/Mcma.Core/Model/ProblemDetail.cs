using Newtonsoft.Json;

namespace Mcma
{
    /// <summary>
    /// Represents an error as defined by RFC 7807 (https://tools.ietf.org/html/rfc7807)
    /// </summary>
    public class ProblemDetail : McmaObject
    {
        /// <summary>
        /// Gets or sets 
        /// </summary>
        [JsonProperty("type")]
        public string ProblemType { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the error encountered
        /// </summary>
        public string Title { get; set;}

        /// <summary>
        /// Gets or sets a more detailed message about the error encountered 
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets a reference that identifies the specific occurrence of the problem
        /// </summary>
        public string Instance { get; set; }
    }
}