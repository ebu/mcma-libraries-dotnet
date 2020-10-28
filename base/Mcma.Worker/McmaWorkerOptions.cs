using Mcma.Client;

namespace Mcma.Worker
{
    public class McmaWorkerOptions
    {
        public string TableName { get; set; }
        
        public ResourceManagerOptions ResourceManager { get; set; }
    }
}