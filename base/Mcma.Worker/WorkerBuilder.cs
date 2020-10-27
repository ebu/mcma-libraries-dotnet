using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public class WorkerBuilder
    {
        internal WorkerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }

        public WorkerBuilder AddOperation<TOperation>() where TOperation : class, IWorkerOperation
        {
            Services.AddSingleton<IWorkerOperation, TOperation>();
            return this;
        }

        public WorkerBuilder AddOperation<TInput>(string operationName,
                                                  Func<WorkerRequestContext, TInput, Task> handler,
                                                  Func<WorkerRequestContext, bool> accepts = null)
        {
            Services.AddSingleton<IWorkerOperation>(new DelegateWorkerOperation<TInput>(operationName, handler, accepts));
            return this;
        }
    }
}