using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.IdentityInfoService
{
    public interface IIdentityInfoService
    {
        /// <summary>
        /// Dostarcza identyfikator użytkownika na podstawie Artefaktu SAML
        /// </summary>
        /// <param name="samlAssertionId">Id asercji zwrócony przez metodę ResolveAssertionId</param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns></returns>
        Task<string> GetUserId(string samlAssertionId, CancellationToken token);
    }
}