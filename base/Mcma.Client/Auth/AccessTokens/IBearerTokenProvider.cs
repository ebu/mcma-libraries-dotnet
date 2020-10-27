using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client.AccessTokens
{
    public interface IBearerTokenProvider<in TAuthContext>
    {
        Task<BearerToken> GetAsync(TAuthContext authContext, CancellationToken cancellationToken = default);
    }
}