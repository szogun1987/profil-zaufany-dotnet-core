using System;
using System.Threading;
using System.Threading.Tasks;
using ProfilZaufany.Helpers;
using ProfilZaufany.X509;
using SimpleSOAPClient;
using SimpleSOAPClient.BinarySecurityToken;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;

namespace ProfilZaufany.Sign
{
    public class SigningService : ISigningService
    {
        private readonly Uri _signingServiceUri;
        private readonly IX509Provider _x509Provider;

        public SigningService(
            Environment environment,
            IX509Provider x509Provider)
        {
            _signingServiceUri = environment.GetServiceUri("pz-services/tpSigning");
            _x509Provider = x509Provider;
        }

        public async Task<string> AddDocumentToSign(AddDocumentToSigningRequest request, CancellationToken token)
        {
            var certificate = await _x509Provider.Provide(token);
            using (var client = SoapClient
                .Prepare())
            {
                client
                    .AddCommonHeaders()
                    .WithBinarySecurityTokenHeader(certificate);

                var envelope = SoapEnvelope
                    .Prepare()
                    .Body(request.ToXElement());
                
                var soapResponse = await client.SendAsync(
                    _signingServiceUri.AbsoluteUri,
                    "addDocumentToSigning",
                    envelope,
                    token);

                var response = soapResponse.Body<AddDocumentToSigningResponse>();

                return response.AddDocumentToSigningReturn;
            }
        }

        public async Task<byte[]> GetSignedDocument(string documentId, CancellationToken token)
        {
            var certificate = await _x509Provider.Provide(token);
            using (var client = SoapClient
                .Prepare())
            {
                client
                    .AddCommonHeaders()
                    .WithBinarySecurityTokenHeader(certificate);

                var request = new GetSignedDocumentRequest
                {
                    Id = documentId
                };

                var envelope = SoapEnvelope
                    .Prepare()
                    .Body(request.ToXElement());

                var soapResponse = await client.SendAsync(
                    _signingServiceUri.AbsoluteUri,
                    "getSignedDocument",
                    envelope,
                    token);

                var response = soapResponse.Body<GetSignedDocumentResponse>();

                return response.SignedDocumentReturn;
            }
        }
    }
}