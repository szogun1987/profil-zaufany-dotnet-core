using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.LoginForm
{
    public interface ISigningForm
    {
        /// <summary>
        /// Tworzy model formularza 
        /// </summary>
        /// <param name="buildingArguments"><include file='SigningFormBuildingArguments.cs' path='[@name="SigningFormBuildingArguments"]'/></param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns></returns>
        Task<SigningFormModel> BuildFormModel(SigningFormBuildingArguments buildingArguments, CancellationToken token);

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