using System.Threading.Tasks;

namespace Mcma.WorkerInvoker
{
    public interface IMcmaWorkerInvoker
    {
        Task InvokeAsync(string operationName, object input, McmaTracker tracker = null);
    }
}
