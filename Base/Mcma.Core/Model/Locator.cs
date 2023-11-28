namespace Mcma.Model;

public class Locator : McmaResource
{
    /// <summary>
    /// Gets or sets the url of the location
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the locator
    /// </summary>
    public LocatorStatus? Status { get; set; }
}
