using System.Collections.Generic;

namespace Mcma.Model
{
    /// <summary>
    /// Represents a service in an MCMA environment
    /// </summary>
    public class Service : McmaResource
    {
        /// <summary>
        /// Gets or sets the name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of authentication required to communicate with the endpoints exposed by this service.
        /// This may be overridden at the <see cref="ResourceEndpoint"/> level
        /// </summary>
        public string AuthType { get; set; }

        /// <summary>
        /// Gets or sets the context to use for authenticating requests made to the endpoints exposed by this service.
        /// This may be overridden at the <see cref="ResourceEndpoint"/> level
        /// </summary>
        public string AuthContext { get; set; }

        /// <summary>
        /// Gets or sets the collection of endpoints exposed by this service
        /// </summary>
        public ICollection<ResourceEndpoint> Resources { get; set; }

        /// <summary>
        /// Gets or sets the type of jobs expected by this service (if any)
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Gets or sets the collection of IDs for job profiles that this service supports
        /// </summary>
        public string[] JobProfileIds { get; set; }

        /// <summary>
        /// Gets or sets the collection input locations this service is aware of
        /// </summary>
        public ICollection<Locator> InputLocations { get; set; }

        /// <summary>
        /// Gets or sets the collection output locations this service is aware of
        /// </summary>
        public ICollection<Locator> OutputLocations { get; set; }
    }
}