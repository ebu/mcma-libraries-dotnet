using System.Threading.Tasks;

namespace Mcma.Worker.Common
{
    public interface IMcmaWorker
    {
        Task DoWorkAsync(McmaWorkerRequest request, string requestId);
    }
}
