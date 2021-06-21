using System.Threading.Tasks;

namespace Mcma.Client.Auth
{
    public interface IAuthProvider
    {
        Task<IAuthenticator> GetAsync(string authType, object authContext);
    }
}