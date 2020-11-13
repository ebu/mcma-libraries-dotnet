namespace Mcma
{
    /// <summary>
    /// Implemented by <see cref="Locator"/> objects that support exposing a URL
    /// </summary>
    public interface IUrlLocator
    {
        /// <summary>
        /// Gets the url for the locator
        /// </summary>
        string Url { get; }
    }
}