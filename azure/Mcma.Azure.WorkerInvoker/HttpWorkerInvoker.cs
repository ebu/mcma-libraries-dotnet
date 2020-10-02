using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mcma.Context;
using Mcma.Serialization;
using Mcma.WorkerInvoker;

namespace Mcma.Azure.WorkerInvoker
{
    public class HttpWorkerInvoker : Mcma.WorkerInvoker.WorkerInvoker
    {
        
        private const string FunctionKeyHeader = "x-functions-key";

        public HttpWorkerInvoker(IContextVariableProvider contextVariableProvider)
            : base(contextVariableProvider)
        {
        }

        private HttpClient HttpClient { get; } = new HttpClient();

        protected override async Task InvokeAsync(string workerFunctionId, WorkerRequest request)
        {
            // create a POST request with the worker request as the body
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, workerFunctionId)
            {
                Content = new StringContent(request.ToMcmaJson().ToString(), Encoding.UTF8, "application/json")
            };

            // if we have a function key
            var functionKey = ContextVariableProvider.GetOptionalContextVariable("WorkerFunctionKey");
            if (functionKey != null)
                httpRequest.Headers.Add(FunctionKeyHeader, functionKey);

            // send the request
            var resp = await HttpClient.SendAsync(httpRequest);
            
            resp.EnsureSuccessStatusCode();
        }
    }
}
