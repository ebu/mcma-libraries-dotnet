using Mcma.Model;

namespace Mcma.Core.Tests.Model1;

public class Workflow : McmaResource
{
    public JobParameterBag Outputs { get; set; } = new();
}