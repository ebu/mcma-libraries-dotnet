using Google.Cloud.Functions.Hosting;
using Mcma.GoogleCloud.Firestore;
using Mcma.Worker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.Worker
{
    public abstract class McmaWorkerStartup : FunctionsStartup
    {
        protected abstract string ApplicationName { get; }

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected virtual void BuildFirestore(FirestoreTableBuilder builder)
        {
        }

        protected abstract void BuildWorker(McmaWorkerBuilder workerBuilder);
        
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionWorker(ApplicationName, BuildWorker, BuildFirestore);
    }
}