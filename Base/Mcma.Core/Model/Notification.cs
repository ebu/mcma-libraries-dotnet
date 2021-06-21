using Newtonsoft.Json.Linq;

namespace Mcma.Model
{
    /// <summary>
    /// Represents a notification sent for an MCMA resource
    /// </summary>
    public class Notification : McmaResource
    {
        /// <summary>
        /// Gets or sets the source from which the notification was sent
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the content of the notification. Typically this will be deserialized to an MCMA object.
        /// </summary>
        public JToken Content { get; set; }
    }
}