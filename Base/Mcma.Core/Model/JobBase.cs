using Mcma.Model.Jobs;

namespace Mcma.Model;

/// <summary>
/// Represents a job-like object (<see cref="Job"/>, <see cref="JobAssignment"/>, <see cref="JobExecution"/>, etc)
/// </summary>
public class JobBase : McmaResource
{
    /// <summary>
    /// Gets or sets the status of the job-like object
    /// </summary>
    public JobStatus Status { get; set; }
 
    /// <summary>
    /// Gets or sets the error 
    /// </summary>
    public ProblemDetail? Error { get; set; }

    /// <summary>
    /// Gets or sets a collection of key-value pairs representing the output of the job, if any 
    /// </summary>
    public JobParameterBag? JobOutput { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage of the job (0%-100%)
    /// </summary>
    public double? Progress { get; set; }
}