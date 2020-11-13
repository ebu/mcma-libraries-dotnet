using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public interface ICosmosDbContainerProvider
    {
        Task<ContainerProperties> GetPropertiesAsync();
        
        Task<Container> GetAsync();
    }
}