using System;
using System.Threading.Tasks;

namespace Mcma.Worker
{
    internal class DelegateMcmaWorkerOperation<T> : McmaWorkerOperation<T>
    {
        public DelegateMcmaWorkerOperation(string name,
                                       Func<McmaWorkerRequestContext, T, Task> executeAsync,
                                       Func<McmaWorkerRequestContext, bool> accepts = null)
        {
            Name = name;
            ExecuteAsyncFunc = executeAsync;
            AcceptsFunc = accepts ?? (req => true);
        }

        public override string Name { get; }

        private Func<McmaWorkerRequestContext, T, Task> ExecuteAsyncFunc { get; }

        private Func<McmaWorkerRequestContext, bool> AcceptsFunc { get; }

        protected override bool Accepts(McmaWorkerRequestContext reqCtx) => AcceptsFunc(reqCtx);

        protected override Task ExecuteAsync(McmaWorkerRequestContext requestContext, T requestInput) => ExecuteAsyncFunc(requestContext, requestInput);
    }
}
