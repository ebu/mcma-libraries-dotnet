using System;

namespace Mcma.Model.Jobs
{
    public class Job : JobBase, INotifiable
    {
        public string ParentId { get; set; }

        public string JobProfileId { get; set; }

        public JobParameterBag JobInput { get; set; }

        public long? Timeout { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public McmaTracker Tracker { get; set; }

        public NotificationEndpoint NotificationEndpoint { get; set; }
    }
}