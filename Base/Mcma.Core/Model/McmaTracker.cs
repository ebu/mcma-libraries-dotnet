using System.Collections.Generic;

namespace Mcma.Model
{
    /// <summary>
    /// Used for tracking calls across multiple operations in an MCMA environment, particularly for the purpose of logging. Typically this will be stored on job-like objects that support it.
    /// It will also be serialized to json and converted to base-64 to be passed in the headers of calls made from a resource manager.  
    /// </summary>
    public class McmaTracker : McmaObject
    {
        /// <summary>
        /// Gets or sets the tracker's ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a human-readable label for the tracker
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets any custom properties that might also be useful for tracking purposes
        /// </summary>
        public IDictionary<string, string> Custom { get; set; }
    }
}