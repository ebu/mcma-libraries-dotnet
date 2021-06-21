using System;
using System.Threading.Tasks;

namespace Mcma.Client.Auth
{
    internal interface IAuthenticatorFactory
    {
        Type ContextType { get; }
        
        Task<IAuthenticator> GetAsync(object authContext);
    }
}