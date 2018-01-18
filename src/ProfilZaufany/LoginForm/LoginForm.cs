using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ProfilZaufany.Helpers;
using ProfilZaufany.LoginForm.DTO;
using ProfilZaufany.X509;
using SimpleSOAPClient;
using SimpleSOAPClient.BinarySecurityToken;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;

namespace ProfilZaufany.LoginForm
{
    public class LoginForm : ILoginForm
    {
        private readonly Environment _environment;
        private readonly string _samlIssuer;
        private readonly IX509Provider _x509Provider;

        public LoginForm(
            Environment environment,
            string samlIssuer,
            IX509Provider x509Provider)
        {
            _environment = environment;
            _samlIssuer = samlIssuer;
            _x509Provider = x509Provider;
        }

        #region Build Form model
        private const string AuthnRequestSkeletonXml = @"<?xml version=""1.0""?>
<samlp:AuthnRequest xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"" Version=""2.0"" IsPassive=""false"" ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"" xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
  <samlp:NameIDPolicy AllowCreate=""true"" Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified"" />
</samlp:AuthnRequest>";

        public async Task<LoginFormModel> BuildFormModel(LoginFormBuildingArguments buildingArguments, CancellationToken token)
        {
            var formActionUri = _environment.GetServiceUri("dt/SingleSignOnService");

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            doc.LoadXml(AuthnRequestSkeletonXml);

            var rootId = "id-" + Guid.NewGuid().ToString("N");

            AddCommonRootAttributes(doc, rootId);

            AddRootAttributes(doc, formActionUri, buildingArguments);

            AddIssuer(doc);

            var certificate = await _x509Provider.Provide(token);

            XmlHelpers.SignSamlDocument(doc, rootId, certificate);

            return new LoginFormModel
            {
                FormAction = formActionUri.AbsoluteUri,
                SAMLRequest = doc.ToBase64String()
            };
        }
        
        private void AddRootAttributes(XmlDocument document, Uri formActionUri, LoginFormBuildingArguments arguments)
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

        #region GetUserId

        private const string ResolveArtifactSkeletonXml = @"<saml2p:ArtifactResolve xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" Version=""2.0""></saml2p:ArtifactResolve>";
        
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

        #region ResolveAsserionId

        public async Task<string> ResolveAsserionId(string samlArtifact, CancellationToken token)
        {
            var x509Certificate = await _x509Provider.Provide(token);

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
            
            using (var soapClient = SoapClient.Prepare())
            {
                soapClient
                    .AddCommonHeaders()
                    .WithBinarySecurityTokenHeader(x509Certificate);

                var envelope = SoapEnvelope.Prepare().Body(doc.ToXElement());

                var artifactResolutionUri = _environment.GetServiceUri("dt-services/idpArtifactResolutionService");

                var response = await soapClient.SendAsync(
                    artifactResolutionUri.AbsoluteUri,
                    "ResolveArtifact",
                    envelope,
                    token);

                var body = response.Body.Value;
                var smth = body.XPathEvaluate("/*[local-name()=\'Response\']/*[local-name()=\'Assertion\']");

                var assertion = ((IEnumerable)smth).Cast<XElement>().SingleOrDefault();

                if (assertion == null)
                {
                    return null;
                }

                var attribute = assertion.Attribute("ID");
                return attribute.Value;
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
            issuer.InnerText = _samlIssuer;
            document.DocumentElement.PrependChild(issuer);
        }
    }
}