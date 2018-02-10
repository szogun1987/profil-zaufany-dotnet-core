using System.Threading;
using System.Threading.Tasks;
using ProfilZaufany.Helpers;
using ProfilZaufany.UserInfo.DTO;
using ProfilZaufany.X509;
using SimpleSOAPClient;
using SimpleSOAPClient.BinarySecurityToken;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;

namespace ProfilZaufany.UserInfo
{
    public class UserInfoService : IUserInfoService
    {
        private readonly Environment _environment;
        private readonly IX509Provider _x509Provider;

        public UserInfoService(
            Environment environment,
            IX509Provider x509Provider)
        {
            _environment = environment;
            _x509Provider = x509Provider;
        }

        public async Task<DTO.UserInfo> GetUserInfo(string samlAssertionId, CancellationToken token)
        {
            var x509Certificate = await _x509Provider.Provide(token);

            var userInfoRequest = new UserInfoRequest
            {
                Tgsid = samlAssertionId
            };

            using (var soapClient = SoapClient.Prepare())
            {
                soapClient
                    .WithBinarySecurityTokenHeader(x509Certificate);

                var envelope = SoapEnvelope.Prepare();
                envelope.Body(userInfoRequest.ToXElement());

                var identityInfoServiceUri = _environment.GetServiceUri("pz-services/tpUserInfo");

                var response = await soapClient.SendAsync(
                    identityInfoServiceUri.AbsoluteUri,
                    "GetTpUserInfo",
                    envelope,
                    token);

                var userInfoResponse = response.Body<UserInfoResponse>();

                return userInfoResponse.GetTpUserInfoReturn;
            }
        }
    }
}