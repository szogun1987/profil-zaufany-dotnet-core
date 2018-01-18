using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ProfilZaufany.Helpers;
using ProfilZaufany.SigningForm.DTO;
using ProfilZaufany.X509;
using SimpleSOAPClient;
using SimpleSOAPClient.BinarySecurityToken;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;

namespace ProfilZaufany.SigningForm
{
    public class SigningForm : ISigningForm
    {
        private readonly SigningFormSettings _profileSettings;
        
        public SigningForm(SigningFormSettings profileSettings)
        {
            _profileSettings = profileSettings;
        }

        public SigningForm(
            Environment environment,
            string samlIssuer,
            IX509Provider x509Provider)
        {
            _profileSettings = new SigningFormSettings(
                environment,
                samlIssuer,
                x509Provider);
        }

        #region Build Form model
        private const string AuthnRequestSkeletonXml = @"<?xml version=""1.0""?>
<samlp:AuthnRequest xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"" Version=""2.0"" IsPassive=""false"" ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"" xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
  <samlp:NameIDPolicy AllowCreate=""true"" Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified"" />
</samlp:AuthnRequest>";

        public async Task<SigningFormModel> BuildFormModel(SigningFormBuildingArguments buildingArguments, CancellationToken token)
        {
            var environmentUri = _profileSettings.Environment.ToUri();
            var formActionUri = new Uri(environmentUri, "dt/SingleSignOnService");

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            doc.LoadXml(AuthnRequestSkeletonXml);

            var rootId = "id-" + Guid.NewGuid().ToString("N");

            AddCommonRootAttributes(doc, rootId);

            AddRootAttributes(doc, formActionUri, buildingArguments);

            AddIssuer(doc);

            var certificate = await _profileSettings.X509Provider.Provide(token);

            XmlHelpers.SignSamlDocument(doc, rootId, certificate);

            return new SigningFormModel
            {
                FormAction = formActionUri.AbsoluteUri,
                SAMLRequest = doc.ToBase64String()
            };
        }
        
        private void AddRootAttributes(XmlDocument document, Uri formActionUri, SigningFormBuildingArguments arguments)
        {
            var root = document.DocumentElement;

            var destination = document.CreateAttribute("Destination");
            destination.Value = formActionUri.AbsoluteUri;
            root.Attributes.Append(destination);
            
            var assertionConsumerServiceURL = document.CreateAttribute("AssertionConsumerServiceURL");
            assertionConsumerServiceURL.Value = arguments.AssertionConsumerServiceURL;
            root.Attributes.Append(assertionConsumerServiceURL);
        }

        #endregion

        #region Saml artifact proceed

        private const string ResolveArtifactSkeletonXml = @"<saml2p:ArtifactResolve xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" Version=""2.0""></saml2p:ArtifactResolve>";

        private readonly Random _idGenerator = new Random();

        public async Task<string> GetUserId(string samlAssertionId, CancellationToken token)
        {
            var x509Certificate = await _profileSettings.X509Provider.Provide(token);
            
            var userIdReq = new ResolveUserIdRequest
            {
                AssertionId = samlAssertionId
            };
            
            using (var soapClient = SoapClient.Prepare())
            {
                soapClient.OnSoapEnvelopeRequest(async (arguments, cancellationToken) =>
                {
                    var bodyElement = arguments.Envelope.Body.Value;

                    bodyElement.Add(new XAttribute("callId", _idGenerator.Next()));
                    bodyElement.Add(new XAttribute("requestTimestamp", DateTime.Now));
                });

                soapClient.WithBinarySecurityTokenHeader(x509Certificate);

                var envelope = SoapEnvelope.Prepare();
                envelope.Body(userIdReq.ToXElement());

                var pzUri = _profileSettings.Environment.ToUri();
                var identityInfoServiceUri = new Uri(pzUri, "dt-services/idpIdentityInfoService");

                var response2 = await soapClient.SendAsync(
                    identityInfoServiceUri.AbsoluteUri,
                    "ResolveUserId",
                    envelope,
                    token);

                var userIdResp = response2.Body<ResolveUserIdResponse>();

                return userIdResp.UserId;
            }
        }
        
        public async Task<string> ResolveAsserionId(string samlArtifact, CancellationToken token)
        {
            var x509Certificate = await _profileSettings.X509Provider.Provide(token);

            var resolveEnvelope = await ResolveArtifact(samlArtifact, x509Certificate, token);

            var body = resolveEnvelope.Body.Value;
            var smth = body.XPathEvaluate("/*[local-name()=\'Response\']/*[local-name()=\'Assertion\']");
            
            var assertion = ((IEnumerable)smth).Cast<XElement>().SingleOrDefault();

            if (assertion == null)
            {
                return null;
            }

            var attribute = assertion.Attribute("ID");
            return attribute.Value;
        }

        private async Task<SoapEnvelope> ResolveArtifact(string samlArtifact, X509Certificate2 x509Certificate, CancellationToken token)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(ResolveArtifactSkeletonXml);

            var rootId = "id-" + Guid.NewGuid().ToString("N");

            AddCommonRootAttributes(doc, rootId);

            AddIssuer(doc);

            var artifact = doc.CreateElement("saml2p", "Artifact", Saml20Constants.Protocol);
            artifact.InnerText = samlArtifact;
            doc.DocumentElement.AppendChild(artifact);
            
            XmlHelpers.SignSamlDocument(doc, rootId, x509Certificate);

            var pzUri = _profileSettings.Environment.ToUri();
            
            using (var soapClient = SoapClient.Prepare())
            {
                soapClient.WithBinarySecurityTokenHeader(x509Certificate);

                var envelope = SoapEnvelope.Prepare().Body(doc.ToXElement());

                var artifactResolutionUri = new Uri(pzUri, "dt-services/idpArtifactResolutionService");

                var response = await soapClient.SendAsync(
                    artifactResolutionUri.AbsoluteUri,
                    "ResolveArtifact",
                    envelope,
                    token);

                return response;
            }
        }

        #endregion

        private void AddCommonRootAttributes(XmlDocument document, string rootId)
        {
            var root = document.DocumentElement;

            var id = document.CreateAttribute("ID");
            id.Value = rootId;
            root.Attributes.Append(id);

            var issueInstant = document.CreateAttribute("IssueInstant");
            issueInstant.Value = DateTime.Now.ToString("O");
            root.Attributes.Append(issueInstant);
        }

        private void AddIssuer(XmlDocument document)
        {
            var issuer = document.CreateElement("samlp", "Issuer", Saml20Constants.Assertion);
            issuer.InnerText = _profileSettings.SamlIssuer;
            document.DocumentElement.PrependChild(issuer);
        }
    }
}