using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.SigningForm
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
        /// <param name="samlArtifact"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> IsArtifactValid(string samlArtifact, CancellationToken token);
        
        /// <summary>
        /// Dostarcza identyfikator użytkownika na podstawie Artefaktu SAML
        /// </summary>
        /// <param name="samlArtifact">Artefakt SAML dostarczony pod adres przekazany w polu AssertionConsumerServiceURL do metody BuildFormMOdel</param>
        /// <param name="token">Token anulowania</param>
        /// <returns></returns>
        Task<string> GetUserId(string samlArtifact, CancellationToken token);
    }
}