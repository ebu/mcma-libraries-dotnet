using System.Threading.Tasks;
using Mcma.Model.Jobs;

namespace Mcma.Worker.Jobs;

public interface IJobProfile<TJob> where TJob : Job
{
    string Name { get; }

    Task ExecuteAsync(ProcessJobAssignmentHelper<TJob> workerJobHelper, McmaWorkerRequestContext requestContext);
}