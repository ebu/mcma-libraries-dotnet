using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Mcma.Utility;

namespace Mcma.Client.Azure.FunctionKeys;

public class AzureFunctionKeyAuthenticator : IAuthenticator
{
    public AzureFunctionKeyAuthenticator(AzureFunctionKeyAuthContext authContext, string decryptionKey)
    {
        FunctionKey = new Lazy<string>(() => GetFunctionKey(authContext));
        DecryptionKey = decryptionKey;
    }

    private Lazy<string> FunctionKey { get; }
        
    private string DecryptionKey { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        request.Headers.Add(AzureConstants.FunctionKeyHeader, FunctionKey.Value);
            
        return Task.CompletedTask;
    }

    private string GetFunctionKey(AzureFunctionKeyAuthContext authContext)
    {
        if (!authContext.IsEncrypted)
            return authContext.FunctionKey;

        if (string.IsNullOrWhiteSpace(DecryptionKey))
            throw new Exception("Function key is encrypted, but a key for decrypting it was not found in the environment variables for this application.");

        return EncryptionHelper.Decrypt(authContext.FunctionKey, DecryptionKey);
    }
}