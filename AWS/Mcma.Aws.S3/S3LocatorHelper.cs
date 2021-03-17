using Mcma.Serialization;

namespace Mcma.Aws.S3
{
    public static class S3LocatorHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<S3Locator>();
    }
}