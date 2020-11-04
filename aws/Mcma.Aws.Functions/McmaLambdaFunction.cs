using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Mcma.Aws.S3;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions
{
    public abstract class McmaLambdaFunction<TFunctionHandler, TInput, TOutput>
        where TFunctionHandler : class, IMcmaLambdaFunctionHandler<TInput, TOutput>
    {
        static McmaLambdaFunction() => AwsS3LocatorHelper.AddTypes();
        
        private Lazy<IServiceProvider> ServiceProvider { get; }

        protected McmaLambdaFunction()
        {
            ServiceProvider = new Lazy<IServiceProvider>(BuildServiceProvider);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            Configure(services);
            services.AddSingleton<TFunctionHandler>();
            return services.BuildServiceProvider();
        }

        protected abstract void Configure(IServiceCollection services);
        
        public Task<TOutput> ExecuteAsync(TInput request, ILambdaContext context)
                => ServiceProvider.Value.GetRequiredService<TFunctionHandler>().ExecuteAsync(request, context);
    }
    
    public abstract class McmaLambdaFunction<TFunctionHandler, TInput>
        where TFunctionHandler : class, IMcmaLambdaFunctionHandler<TInput>
    {
        private Lazy<IServiceProvider> ServiceProvider { get; }

        protected McmaLambdaFunction()
        {
            ServiceProvider = new Lazy<IServiceProvider>(BuildServiceProvider);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            Configure(services);
            services.AddSingleton<TFunctionHandler>();
            return services.BuildServiceProvider();
        }

        protected abstract void Configure(IServiceCollection services);

        public Task ExecuteAsync(TInput request, ILambdaContext context)
            => ServiceProvider.Value.GetRequiredService<TFunctionHandler>().ExecuteAsync(request, context);
    }
    
    public abstract class McmaLambdaFunction<TFunctionHandler>
        where TFunctionHandler : class, IMcmaLambdaFunctionHandler
    {
        private Lazy<IServiceProvider> ServiceProvider { get; }

        protected McmaLambdaFunction()
        {
            ServiceProvider = new Lazy<IServiceProvider>(BuildServiceProvider);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            Configure(services);
            services.AddSingleton<TFunctionHandler>();
            return services.BuildServiceProvider();
        }

        protected abstract void Configure(IServiceCollection services);

        public Task ExecuteAsync(ILambdaContext context)
            => ServiceProvider.Value.GetRequiredService<TFunctionHandler>().ExecuteAsync(context);
    }
}