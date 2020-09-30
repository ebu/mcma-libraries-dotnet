using System;

namespace Mcma
{
    public class JobExecution : JobBase
    {
        public string JobAssignmentId { get; set; }
        
        public DateTime? ActualStartDate { get; set; }
        
        public DateTime? ActualEndDate { get; set; }
        
        public long? ActualDuration { get; set; }
    }
}