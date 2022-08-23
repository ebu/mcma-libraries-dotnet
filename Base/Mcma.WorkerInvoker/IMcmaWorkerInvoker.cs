using System.Threading.Tasks;
using Mcma.Model;

namespace Mcma.WorkerInvoker;

public interface IMcmaWorkerInvoker
{
    Task InvokeAsync(string operationName, object input, McmaTracker tracker = null);
}