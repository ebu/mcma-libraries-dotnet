namespace Mcma.Model
{
    /// <summary>
    /// Represents an endpoint that receives notifications about MCMA resources
    /// </summary>
    public class NotificationEndpoint : McmaResource
    {
        /// <summary>
        /// Gets or sets the HTTP url at which notifications are expected
        /// </summary>
        public string HttpEndpoint { get; set; }
    }
}