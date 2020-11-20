﻿using System;
using System.Threading.Tasks;
using Mcma.Logging;

namespace Mcma.Worker
{
    public abstract class McmaWorkerOperation<T> : IMcmaWorkerOperation
    {   
        public abstract string Name { get; }

        public Type InputType => typeof(T);

        private bool IsValidRequest(McmaWorkerRequestContext reqCtx)
            => Name.Equals(reqCtx.OperationName, StringComparison.OrdinalIgnoreCase) && reqCtx.TryGetInputAs<T>(out _);

        bool IMcmaWorkerOperation.Accepts(McmaWorkerRequestContext reqCtx) => IsValidRequest(reqCtx) && Accepts(reqCtx);

        protected virtual bool Accepts(McmaWorkerRequestContext reqCtx) => true;

        Task IMcmaWorkerOperation.ExecuteAsync(McmaWorkerRequestContext requestContext)
        {
            if (requestContext == null)
                throw new ArgumentNullException(nameof(requestContext));

            var input = requestContext.GetInputAs<T>();

            requestContext.Logger.Debug("Got input of type '" + typeof(T).Name + "' from worker request.");
            
            return ExecuteAsync(requestContext, input);
        }

        protected abstract Task ExecuteAsync(McmaWorkerRequestContext requestContext, T requestInput);
    }
}
