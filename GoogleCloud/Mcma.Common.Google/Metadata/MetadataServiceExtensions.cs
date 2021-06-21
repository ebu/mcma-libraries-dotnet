using System.Linq;
using System.Threading.Tasks;

namespace Mcma.Common.Google.Metadata
{
    public static class MetadataServiceExtensions
    {
        public static async Task<bool> IsAvailableAsync(this IMetadataService metadataService)
            => (await metadataService.GetAsync(string.Empty)).Any();
        
        public static async Task<string> GetSingleAsync(this IMetadataService metadataService, string path)
            => (await metadataService.GetAsync(path)).FirstOrDefault();
        
        public static async Task<string> GetSingleForInstanceAsync(this IMetadataService metadataService, string path)
            => (await metadataService.GetForInstanceAsync(path)).FirstOrDefault();
        
        public static async Task<string> GetSingleForProjectAsync(this IMetadataService metadataService, string path)
            => (await metadataService.GetForProjectAsync(path)).FirstOrDefault();
        
        public static Task<string[]> GetForInstanceAsync(this IMetadataService metadataService, string path)
            => metadataService.GetAsync("instance/" + path.TrimStart('/'));

        public static Task<string[]> GetForProjectAsync(this IMetadataService metadataService, string path)
            => metadataService.GetAsync("project/" + path.TrimStart('/'));

        public static Task<string> GetProjectIdAsync(this IMetadataService metadataService)
            => metadataService.GetSingleForProjectAsync("projectId");

        public static Task<string> GetKubernetesClusterNameAsync(this IMetadataService metadataService)
            => metadataService.GetSingleForInstanceAsync("attributes/cluster-name");

        public static async Task<bool> IsOnKubernetesClusterAsync(this IMetadataService metadataService)
            => !string.IsNullOrWhiteSpace(await metadataService.GetKubernetesClusterNameAsync());

        public static async Task<string> GetInstanceZoneAsync(this IMetadataService metadataService)
            => (await metadataService.GetSingleForInstanceAsync("zone"))?.Split('/').LastOrDefault();

        public static Task<string> GetInstanceIdAsync(this IMetadataService metadataService)
            => metadataService.GetSingleForInstanceAsync("id");
    }
}