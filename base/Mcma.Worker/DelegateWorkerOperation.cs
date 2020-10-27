using System;
using System.Threading.Tasks;

namespace Mcma.Worker
{
    internal class DelegateWorkerOperation<T> : WorkerOperation<T>
    {
        public DelegateWorkerOperation(string name,
                                       Func<WorkerRequestContext, T, Task> executeAsync,
                                       Func<WorkerRequestContext, bool> accepts = null)
        {
            Name = name;
            ExecuteAsyncFunc = executeAsync;
            AcceptsFunc = accepts ?? (req => true);
        }

        public override string Name { get; }

        private Func<WorkerRequestContext, T, Task> ExecuteAsyncFunc { get; }

        private Func<WorkerRequestContext, bool> AcceptsFunc { get; }

        protected override bool Accepts(WorkerRequestContext reqCtx) => AcceptsFunc(reqCtx);

        protected override Task ExecuteAsync(WorkerRequestContext requestContext, T requestInput) => ExecuteAsyncFunc(requestContext, requestInput);
    }
}
