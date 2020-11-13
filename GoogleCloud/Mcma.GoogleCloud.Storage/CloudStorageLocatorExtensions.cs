using Mcma.Serialization;
using StorageObject = Google.Apis.Storage.v1.Data.Object;

namespace Mcma.GoogleCloud.Storage
{
    public static class CloudStorageLocatorHelper
    {
        public static IMcmaTypeRegistrations AddTypes() => McmaTypes.Add<CloudStorageFileLocator>().Add<CloudStorageFolderLocator>();
        
        public static string GetFilePath(this CloudStorageFolderLocator folderLocator, string fileName)
            => string.IsNullOrWhiteSpace(folderLocator?.FolderPath) ? fileName : $"{folderLocator.FolderPath.TrimEnd('/')}/{fileName.TrimStart('/')}";

        public static CloudStorageFileLocator GetFileLocator(this CloudStorageFolderLocator folderLocator, string fileName)
            => new CloudStorageFileLocator
            {
                Bucket = folderLocator.Bucket,
                FilePath = folderLocator.GetFilePath(fileName)
            };

        public static StorageObject ToStorageObject(this CloudStorageFileLocator fileLocator)
            => new StorageObject
            {
                Bucket = fileLocator.Bucket,
                Name = fileLocator.FilePath
            };
    }
}