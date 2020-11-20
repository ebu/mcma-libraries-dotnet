﻿namespace Mcma.Azure.Client.AzureAD.ManagedIdentity
{
    public class AzureManagedIdentityBearerTokenProviderOptions
    {
        public string ConnectionString { get; set; }
        
        public string AzureAdInstance { get; set; }
    }
}