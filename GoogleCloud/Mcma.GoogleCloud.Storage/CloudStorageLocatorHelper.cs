using Mcma.Serialization;
using StorageObject = Google.Apis.Storage.v1.Data.Object;

namespace Mcma.GoogleCloud.Storage
{
    public static class CloudStorageLocatorHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<CloudStorageLocator>();

        public static StorageObject ToStorageObject(this CloudStorageLocator fileLocator)
            => new()
            {
                Bucket = fileLocator.Bucket,
                Name = fileLocator.Name
            };
    }
}