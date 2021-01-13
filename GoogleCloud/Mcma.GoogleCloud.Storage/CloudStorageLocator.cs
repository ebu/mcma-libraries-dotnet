namespace Mcma.GoogleCloud.Storage
{
    public abstract class CloudStorageLocator : Locator
    {
        public string Bucket { get; set; }
    }
}