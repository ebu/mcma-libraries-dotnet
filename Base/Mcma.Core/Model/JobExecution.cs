using System;

namespace Mcma.Model;

/// <summary>
/// Represents an attempt by the job processor to execute a job. Differs from a job assignment in that it is not necessarily associated with a service for processing and will
/// exist even if no service is found to process a job.
/// </summary>
public class JobExecution : JobBase
{
    /// <summary>
    /// Gets or sets the job assignment ID, if any. If a job has not been assigned to a service for processing, this will not be set.
    /// </summary>
    public string JobAssignmentId { get; set; }
        
    /// <summary>
    /// Gets or sets the date and time that processing of the job actually began (as opposed to when it was requested or scheduled)
    /// </summary>
    public DateTimeOffset? ActualStartDate { get; set; }
        
    /// <summary>
    /// Gets or sets the date and time that processing of the job finished
    /// </summary>
    public DateTimeOffset? ActualEndDate { get; set; }
        
    /// <summary>
    /// Gets or sets the time it took to process the job. This is typically calculated as the difference between <see cref="ActualEndDate"/> and <see cref="ActualStartDate"/>
    /// </summary>
    public long? ActualDuration { get; set; }
}