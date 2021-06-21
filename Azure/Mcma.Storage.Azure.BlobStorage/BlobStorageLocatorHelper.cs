using Mcma.Serialization;

namespace Mcma.Storage.Azure.BlobStorage
{
    public static class BlobStorageLocatorHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<BlobStorageLocator>();
        
        public static string GetChildFilePath(this BlobStorageLocator folderLocator, string fileName)
            => folderLocator.Path?.TrimEnd('/') + (!string.IsNullOrWhiteSpace(folderLocator.Path) ? "/" : "") + fileName;

        public static BlobStorageLocator GetChildLocator(this BlobStorageLocator folderLocator, string fileName)
            => new()
            {
                Url = $"{folderLocator.Url.TrimEnd('/')}/{fileName}"
            };
    }
}
