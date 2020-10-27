using System.Threading.Tasks;

namespace Mcma.Client
{
    public interface IAuthProvider
    {
        Task<IAuthenticator> GetAsync(string authType, object authContext);
    }
}