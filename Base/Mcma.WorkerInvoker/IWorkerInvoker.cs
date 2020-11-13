using System.Threading.Tasks;

namespace Mcma.WorkerInvoker
{
    public interface IWorkerInvoker
    {
        Task InvokeAsync(string operationName, object input, McmaTracker tracker = null);
    }
}
