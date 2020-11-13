using System;

namespace Mcma
{
    public class JobExecution : JobBase
    {
        public string JobAssignmentId { get; set; }
        
        public DateTimeOffset? ActualStartDate { get; set; }
        
        public DateTimeOffset? ActualEndDate { get; set; }
        
        public long? ActualDuration { get; set; }
    }
}