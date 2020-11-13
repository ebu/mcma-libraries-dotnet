using System.Threading.Tasks;

namespace Mcma.Worker
{
    public interface IMcmaWorkerOperation
    {
        string Name { get; }
        
        bool Accepts(McmaWorkerRequestContext request);

        Task ExecuteAsync(McmaWorkerRequestContext request);
    }
}
