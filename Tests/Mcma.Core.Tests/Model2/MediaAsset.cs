using Mcma.Model;

namespace Mcma.Core.Tests.Model2;

public class MediaAsset : McmaResource
{
    public required MetadataBase Metadata { get; set; }
}
