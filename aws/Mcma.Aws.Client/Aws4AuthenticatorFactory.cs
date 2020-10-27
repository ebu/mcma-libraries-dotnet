using System.Threading.Tasks;
using Mcma.Client;

namespace Mcma.Aws.Client
{
    internal class Aws4AuthenticatorFactory : AuthenticatorFactory<Aws4AuthContext>
    {
        protected override Task<IAuthenticator> GetAsync(Aws4AuthContext authContext)
            => Task.FromResult<IAuthenticator>(new Aws4Authenticator(authContext));
    }
}