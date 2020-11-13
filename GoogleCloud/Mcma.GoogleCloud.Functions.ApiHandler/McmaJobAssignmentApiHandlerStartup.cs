using System;
using Google.Cloud.Functions.Hosting;
using Mcma.Api;
using Mcma.Aws.Functions.ApiHandler;
using Mcma.GoogleCloud.Firestore;
using Mcma.GoogleCloud.PubSubWorkerInvoker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.ApiHandler
{
    public abstract class McmaJobAssignmentApiHandlerStartup : FunctionsStartup
    {
        public abstract string ApplicationName { get; }

        public virtual string WorkerPubSubTopic => McmaPubSubWorkerInvokerEnvironmentVariables.WorkerTopicName;

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected virtual void BuildFirestore(FirestoreTableBuilder builder)
        {
        }

        protected virtual void AddAdditionalRoutes(McmaApiBuilder builder)
        {
        }
        
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionJobAssignmentApiHandler(ApplicationName, WorkerPubSubTopic, BuildFirestore, AddAdditionalRoutes);
    }
}