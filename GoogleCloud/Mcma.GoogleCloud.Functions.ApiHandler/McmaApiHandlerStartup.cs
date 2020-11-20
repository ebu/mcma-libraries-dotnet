﻿using System;
using Google.Cloud.Functions.Hosting;
using Mcma.Api;
using Mcma.Aws.Functions.ApiHandler;
using Mcma.GoogleCloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.ApiHandler
{
    public abstract class McmaApiHandlerStartup : FunctionsStartup
    {
        public abstract string ApplicationName { get; }

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected virtual void BuildFirestore(FirestoreTableBuilder builder)
        {
        }

        public abstract void BuildApi(McmaApiBuilder apiBuilder);
        
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionApiHandler(ApplicationName, BuildApi, BuildFirestore);
    }
}