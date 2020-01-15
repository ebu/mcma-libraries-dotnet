using System.Threading.Tasks;
using Mcma.Core.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Client.AccessTokens
{
    public static class AccessTokenAuthExtensions
    {
        public static IAuthProvider AddAccessTokenAuth<T>(this IAuthProvider authProvider, string authType, IAccessTokenProvider<T> accessTokenProvider)
            =>
            authProvider.Add(
                authType,
                authCtx => Task.FromResult<IAuthenticator>(new AccessTokenAuthenticator<T>(accessTokenProvider, JToken.Parse(authCtx).ToMcmaObject<T>())));
    }
}