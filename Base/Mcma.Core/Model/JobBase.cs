namespace Mcma
{
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
        public ProblemDetail Error { get; set; }

        public JobParameterBag JobOutput { get; set; }

        public double? Progress { get; set; }
    }
}