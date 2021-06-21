using System.Threading.Tasks;

namespace Mcma.Common.Google.Metadata
{
    public interface IMetadataService
    {
        Task<string[]> GetAsync(string path);
    }
}