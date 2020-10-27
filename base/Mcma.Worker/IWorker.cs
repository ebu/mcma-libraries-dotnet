using System.Threading.Tasks;

namespace Mcma.Worker
{
    public interface IWorker
    {
        Task DoWorkAsync(WorkerRequestContext requestContext);
    }
}
