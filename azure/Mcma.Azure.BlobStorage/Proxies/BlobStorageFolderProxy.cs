using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public class BlobStorageFolderProxy : BlobStorageProxy<BlobStorageFolderLocator>
    {
        internal BlobStorageFolderProxy(BlobStorageFolderLocator locator, string connectionString)
            : base(locator, connectionString)
        {
        }

        public async Task<BlobStorageFileLocator> PutAsync(string fileName, Stream readFrom, BlobHttpHeaders headers = null)
        {
            var fileLocator = Locator.FileLocator(fileName);
            var blobClient = ContainerClient.GetBlobClient(fileLocator.FilePath);
            await blobClient.UploadAsync(readFrom, headers);
            return fileLocator;
        }

        public async Task<BlobStorageFileLocator> PutAsTextAsync(string fileName, string content, BlobHttpHeaders headers = null)
            => await PutAsync(fileName, new MemoryStream(Encoding.UTF8.GetBytes(content)), headers);
    }
}
