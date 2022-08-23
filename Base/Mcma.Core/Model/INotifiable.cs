namespace Mcma.Model;

/// <summary>
/// Implemented by objects that might have notifications associated to them, e.g jobs and job assignments
/// </summary>
public interface INotifiable
{
    /// <summary>
    /// Gets the notification endpoint to which notifications related to this resource should be sent
    /// </summary>
    NotificationEndpoint NotificationEndpoint { get; }
}