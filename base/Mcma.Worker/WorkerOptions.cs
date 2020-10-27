using Mcma.Client;

namespace Mcma.Worker
{
    public class WorkerOptions
    {
        public string TableName { get; set; }
        
        public ResourceManagerOptions ResourceManager { get; set; }
    }
}