using Mcma.Model;
using Mcma.Serialization;

namespace Mcma.Core.Tests;

[TestClass]
public sealed class McmaTypesTests
{
    [TestMethod]
    public void McmaTypes_ShouldDisambiguateByObjectType()
    {
        McmaTypes.Add<Model1.MetadataBase>()
                 .Add<Model1.Metadata>()
                 .Add<Model1.MediaAsset>()
                 .Add<Model1.Workflow>();

        McmaTypes.Add<Model2.MetadataBase>()
                 .Add<Model2.Metadata>()
                 .Add<Model2.MediaAsset>()
                 .Add<Model2.Workflow>();

        var mediaAsset = new Model1.MediaAsset { Metadata = new Model1.Metadata { Title = "Test", Description = "Test" } };
        var workflow = new Model1.Workflow { Outputs = { [nameof(mediaAsset)] = mediaAsset } };

        var workflowJson = workflow.ToMcmaJsonObject().ToString();

        McmaJson.Parse(workflowJson)?.ToMcmaObject<Model1.Workflow>();
    }
}
