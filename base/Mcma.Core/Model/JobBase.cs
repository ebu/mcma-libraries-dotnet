namespace Mcma
{
    public class JobBase : McmaResource
    {
        public JobStatus Status { get; set; }
 
        public ProblemDetail Error { get; set; }

        public JobParameterBag JobOutput { get; set; }

        public double? Progress { get; set; }
    }
}