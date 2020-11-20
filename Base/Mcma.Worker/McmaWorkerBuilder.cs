﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public class McmaWorkerBuilder
    {
        internal McmaWorkerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }

        public McmaWorkerBuilder AddOperation<TOperation>() where TOperation : class, IMcmaWorkerOperation
        {
            Services.AddSingleton<IMcmaWorkerOperation, TOperation>();
            return this;
        }

        public McmaWorkerBuilder AddOperation<TInput>(string operationName,
                                                  Func<McmaWorkerRequestContext, TInput, Task> handler,
                                                  Func<McmaWorkerRequestContext, bool> accepts = null)
        {
            Services.AddSingleton<IMcmaWorkerOperation>(new DelegateMcmaWorkerOperation<TInput>(operationName, handler, accepts));
            return this;
        }
    }
}