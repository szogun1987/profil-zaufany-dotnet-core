using System.Threading;
using System.Threading.Tasks;
using ProfilZaufany.Helpers;
using ProfilZaufany.IdentityInfoService.DTO;
using ProfilZaufany.X509;
using SimpleSOAPClient;
using SimpleSOAPClient.BinarySecurityToken;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;

namespace ProfilZaufany.IdentityInfoService
{
    public class IdentityInfoService : IIdentityInfoService
    {
        private readonly Environment _environment;
        private readonly IX509Provider _x509Provider;

        public IdentityInfoService(
            Environment environment,
            IX509Provider x509Provider)
        {
            _environment = environment;
            _x509Provider = x509Provider;
        }

        #region GetUserId
        
        public async Task<string> GetUserId(string samlAssertionId, CancellationToken token)
        {
            var x509Certificate = await _x509Provider.Provide(token);

            var userIdReq = new ResolveUserIdRequest
            {
                AssertionId = samlAssertionId
            };

            using (var soapClient = SoapClient.Prepare())
            {
                soapClient
                    .AddCommonHeaders()
                    .WithBinarySecurityTokenHeader(x509Certificate);

                var envelope = SoapEnvelope.Prepare();
                envelope.Body(userIdReq.ToXElement());

                var identityInfoServiceUri = _environment.GetServiceUri("dt-services/idpIdentityInfoService");

                var response2 = await soapClient.SendAsync(
                    identityInfoServiceUri.AbsoluteUri,
                    "ResolveUserId",
                    envelope,
                    token);

                var userIdResp = response2.Body<ResolveUserIdResponse>();

                return userIdResp.UserId;
            }
        }

        #endregion
    }
}