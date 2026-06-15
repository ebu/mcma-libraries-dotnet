using Mcma.Model;

namespace Mcma.Core.Tests.Model2;

public class Workflow : McmaResource
{
    public JobParameter Outputs { get; set; } = new();
}