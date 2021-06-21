using System;
using Mcma.Logging;
using Mcma.Model;
using Mcma.Serialization;
using Mcma.Worker.Common;
using Newtonsoft.Json.Linq;

namespace Mcma.Worker
{
    public class McmaWorkerRequestContext
    {
        public McmaWorkerRequestContext(McmaWorkerRequest request, string requestId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            OperationName = !string.IsNullOrWhiteSpace(request.OperationName)
                                ? request.OperationName
                                : throw new McmaException("OperationName must be a non-empty string");
            
            Input = request.Input;
            Tracker = request.Tracker;
            RequestId = requestId;
        }

        public string OperationName { get; }

        public JObject Input { get;  }
        
        public McmaTracker Tracker { get; }

        public string RequestId { get; }
        
        public ILogger Logger { get; private set; }

        internal void SetLogger(ILoggerProvider loggerProvider) => Logger = loggerProvider.Get(RequestId, Tracker);

        public object GetInputAs(Type type)
        {
            try
            {
                return Input?.ToMcmaObject(type);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Worker request input could not be deserialized to type {type.Name}. See inner exception for details.", ex);
            }
        }

        public T GetInputAs<T>() => (T)GetInputAs(typeof(T));

        public bool TryGetInputAs(Type type, out object dataObject)
        {
            dataObject = null;
            try
            {
                dataObject = GetInputAs(type);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetInputAs<T>(out T dataObject)
        {
            dataObject = default;

            var result = TryGetInputAs(typeof(T), out var obj);
            if (result)
                dataObject = (T)obj;

            return result;
        }
    }
}