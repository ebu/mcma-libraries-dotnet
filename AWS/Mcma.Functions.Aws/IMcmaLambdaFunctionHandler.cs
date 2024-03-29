﻿using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Mcma.Functions.Aws;

public interface IMcmaLambdaFunctionHandler<in TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, ILambdaContext context);
}
    
public interface IMcmaLambdaFunctionHandler<in TInput>
{
    Task ExecuteAsync(TInput input, ILambdaContext context);
}
    
public interface IMcmaLambdaFunctionHandler
{
    Task ExecuteAsync(ILambdaContext context);
}