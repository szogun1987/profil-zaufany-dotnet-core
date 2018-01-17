using System;
using System.Threading.Tasks;
using System.Xml;
using ProfilZaufany.Helpers;

namespace ProfilZaufany.SigningForm
{
    public class SigningForm : ISigningForm
    {
        private readonly SigningFormSettings _profileSettings;

        private const string SkeletonXml = @"<?xml version=""1.0""?>
<samlp:AuthnRequest xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"" Version=""2.0"" IsPassive=""false"" ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"" xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
  <samlp:NameIDPolicy AllowCreate=""true"" Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified"" />
</samlp:AuthnRequest>";

        public SigningForm(SigningFormSettings profileSettings)
        {
            _profileSettings = profileSettings;
        }

        public async Task<SigningFormModel> BuildFormModel(SigningFormBuildingArguments buildingArguments)
        {
            var environmentUri = _profileSettings.Environment.ToUri();
            var formActionUri = new Uri(environmentUri, "dt/SingleSignOnService");

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            doc.LoadXml(SkeletonXml);

            var rootId = "id-" + Guid.NewGuid().ToString("N");

            AddRootAttributes(doc, formActionUri, rootId, buildingArguments);

            AddIssuer(doc);

            var certificate = await _profileSettings.X509Provider.Provide();

            XmlHelpers.SignSamlDocument(doc, rootId, certificate);
            
            return new SigningFormModel
            {
                FormAction = formActionUri.AbsoluteUri,
                SAMLRequest = doc.ToBase64String()
            };
        }

        private void AddRootAttributes(XmlDocument document, Uri formActionUri, string rootId, SigningFormBuildingArguments arguments)
        {
            var root = document.DocumentElement;

            var destination = document.CreateAttribute("Destination");
            destination.Value = formActionUri.AbsoluteUri;
            root.Attributes.Append(destination);

            var id = document.CreateAttribute("ID");
            id.Value = rootId;
            root.Attributes.Append(id);

            var issueInstant = document.CreateAttribute("IssueInstant");
            issueInstant.Value = DateTime.Now.ToString("O");
            root.Attributes.Append(issueInstant);

            var assertionConsumerServiceURL = document.CreateAttribute("AssertionConsumerServiceURL");
            assertionConsumerServiceURL.Value = arguments.AssertionConsumerServiceURL;
            root.Attributes.Append(assertionConsumerServiceURL);
        }

        private void AddIssuer(XmlDocument document)
        {
            var issuer = document.CreateElement("samlp", "Issuer", Saml20Constants.Assertion);
            issuer.InnerText = _profileSettings.SamlIssuer;
            document.DocumentElement.PrependChild(issuer);
        }
    }
}