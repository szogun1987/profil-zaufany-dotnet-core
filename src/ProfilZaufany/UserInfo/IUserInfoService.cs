using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.UserInfo
{
    public interface IUserInfoService
    {
        /// <summary>
        /// Zwraca informację o zalogowanym użytkowniku
        /// </summary>
        /// <param name="samlAssertionId">Id asercji zwrócony przez metodę ResolveAssertionId</param>
        /// <param name="token">Token anulowania zadania</param>
        /// <returns></returns>
        Task<DTO.UserInfo> GetUserInfo(string samlAssertionId, CancellationToken token);
    }
}