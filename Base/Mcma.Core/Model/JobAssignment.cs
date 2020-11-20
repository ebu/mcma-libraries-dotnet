namespace Mcma
{
    /// <summary>
    /// Represents an assignment of a job to a service for processing
    /// </summary>
    public class JobAssignment : JobBase, INotifiable
    {
        /// <summary>
        /// Gets or sets the ID of the job that's been assigned
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets the tracking information associated with this assignment
        /// </summary>
        public McmaTracker Tracker { get; set; }

        /// <summary>
        /// Gets or sets the endpoint to which to send notifications about the processing of this job assignment
        /// </summary>
        public NotificationEndpoint NotificationEndpoint { get; set; }
    }
}