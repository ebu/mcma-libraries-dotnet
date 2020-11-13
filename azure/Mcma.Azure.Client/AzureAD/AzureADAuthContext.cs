using System;

namespace Mcma.Azure.Client.AzureAD
{
    public class AzureADAuthContext
    {
        public string Scope { get; set; }

        public void ValidateScope()
        {
            if (string.IsNullOrWhiteSpace(Scope))
                throw new Exception("Azure AD auth context must specify a scope.");
        }
    }
}