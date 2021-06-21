namespace Mcma.Model
{
    /// <summary>
    /// Enumeration of all the possible statuses an MCMA job might have
    /// </summary>
    public enum JobStatus
    {
        New,
        Pending,
        Assigned,
        Queued,
        Scheduled,
        Running,
        Completed,
        Failed,
        Canceled
    }
}