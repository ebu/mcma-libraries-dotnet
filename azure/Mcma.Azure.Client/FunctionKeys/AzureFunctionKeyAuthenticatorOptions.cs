using System;

namespace Mcma.Azure.Client.FunctionKeys
{
    public class AzureFunctionKeyAuthenticatorOptions
    {
        public string DecryptionKey { get; set; } = Environment.GetEnvironmentVariable(AzureConstants.FunctionKeyEncryptionKeySetting);
    }
}