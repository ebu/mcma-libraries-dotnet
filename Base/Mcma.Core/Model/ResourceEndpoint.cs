namespace Mcma.Model;

/// <summary>
/// Represents an endpoint exposed by an MCMA service for performing operations against a specific type of resource
/// </summary>
public class ResourceEndpoint : McmaObject
{
    /// <summary>
    /// Gets or sets the type of resource this endpoint handles
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP url for 
    /// </summary>
    public string HttpEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of authentication required to communicate with this endpoint.
    /// This is optional at this level and will override the auth type defined at the <see cref="Service"/> level
    /// </summary>
    public string? AuthType { get; set; }
}