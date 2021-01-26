namespace Mcma.Azure.BlobStorage
{
    public class BlobStorageFileLocator : BlobStorageLocator
    {
        public string FilePath => GetParsedUrl().Path;
    }
}
