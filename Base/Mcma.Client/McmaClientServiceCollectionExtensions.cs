﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client
{
    public static class McmaClientServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaClient(this IServiceCollection services,
                                                       Action<McmaClientBuilder> build)
        {
            var builder = new McmaClientBuilder(services);
            build(builder);
            return services;
        }
    }
}