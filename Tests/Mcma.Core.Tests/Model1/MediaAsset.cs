using Mcma.Model;

namespace Mcma.Core.Tests.Model1;

public class MediaAsset : McmaResource
{
    public required MetadataBase Metadata { get; set; }
}
