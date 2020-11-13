namespace Mcma
{
    public class JobAssignment : JobBase, INotifiable
    {
        public string JobId { get; set; }

        public McmaTracker Tracker { get; set; }

        public NotificationEndpoint NotificationEndpoint { get; set; }
    }
}