using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.LoginForm
{
    public interface ILoginForm
    {
        /// <summary>
        /// Tworzy model formularza 
        /// </summary>
        /// <param name="buildingArguments"><include file='LoginFormBuildingArguments.cs' path='[@name="LoginFormBuildingArguments"]'/></param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns></returns>
        Task<LoginFormModel> BuildFormModel(LoginFormBuildingArguments buildingArguments, CancellationToken token);

        /// <summary>
        /// Metoda weryfikuje czy 
        /// </summary>
        /// <param name="samlArtifact">Artefakt SAML dostarczony z systemu PZ</param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns>id asercji SAML albo null w przypadku przekazania błędnego tokenu</returns>
        Task<string> ResolveAsserionId(string samlArtifact, CancellationToken token);

        /// <summary>
        /// Dostarcza identyfikator użytkownika na podstawie Artefaktu SAML
        /// </summary>
        /// <param name="samlAssertionId">Id asercji zwrócony przez metodę ResolveAssertionId</param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns></returns>
        Task<string> GetUserId(string samlAssertionId, CancellationToken token);
    }
}