using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcma.Logging;

namespace Mcma.Worker
{
    public class McmaWorker : IMcmaWorker
    {
        public McmaWorker(ILoggerProvider loggerProvider, IEnumerable<IMcmaWorkerOperation> operations)
        {
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            Operations = operations?.ToArray() ?? throw new McmaException("No operations registered for worker.");
        }
        
        private ILoggerProvider LoggerProvider { get; }

        private IMcmaWorkerOperation[] Operations { get; }

        public async Task DoWorkAsync(McmaWorkerRequestContext requestContext)
        {
            if (requestContext == null)
                throw new ArgumentNullException(nameof(requestContext));
            
            requestContext.SetLogger(LoggerProvider);
            
            var operation = Operations.FirstOrDefault(op => op.Accepts(requestContext));
            if (operation == null)
                throw new McmaException($"No handler found for '{requestContext.OperationName}' that can handle this request.");

            requestContext.Logger.Debug("Handling worker operation '" + requestContext.OperationName + "' with handler of type '" + operation.GetType().Name + "'");
            
            try
            {
                await operation.ExecuteAsync(requestContext);
            }
            catch (Exception ex)
            {
                requestContext.Logger.Error($"Failed to process worker operation '{requestContext.OperationName}'. Exception: {ex}");
                throw;
            }
        }
    }
}
