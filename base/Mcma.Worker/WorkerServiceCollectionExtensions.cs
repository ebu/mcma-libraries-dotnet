using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public static class WorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddOperation<TOperation>(this IServiceCollection services) where TOperation : class, IWorkerOperation
            =>  services.AddSingleton<IWorkerOperation, TOperation>();

        public static IServiceCollection AddOperation<TInput>(this IServiceCollection services, 
                                                              string operationName,
                                                              Func<WorkerRequestContext, TInput, Task> handler,
                                                              Func<WorkerRequestContext, bool> accepts = null)
            => services.AddSingleton<IWorkerOperation>(new DelegateWorkerOperation<TInput>(operationName, handler, accepts));
    }
}