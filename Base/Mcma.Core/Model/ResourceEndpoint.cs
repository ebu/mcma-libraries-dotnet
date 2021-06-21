namespace Mcma.Model
{
    /// <summary>
    /// Represents an endpoint exposed by an MCMA service for performing operations against a specific type of resource
    /// </summary>
    public class ResourceEndpoint : McmaObject
    {
        /// <summary>
        /// Gets or sets the type of resource this endpoint handles
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP url for 
        /// </summary>
        public string HttpEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the type of authentication required to communicate with this endpoint.
        /// This is optional at this level and will override the auth type defined at the <see cref="Service"/> level
        /// </summary>
        public string AuthType { get; set; }

        /// <summary>
        /// Gets or sets the context to use for authenticating requests made to this endpoint.
        /// This is optional at this level and will override the auth context defined at the <see cref="Service"/> level
        /// </summary>
        public object AuthContext { get; set; }
    }
}