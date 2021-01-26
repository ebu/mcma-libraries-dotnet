namespace Mcma.Azure.BlobStorage
{
    public class BlobStorageFolderLocator : BlobStorageLocator
    {
        public string FolderPath => GetParsedUrl().Path;
    }
}
