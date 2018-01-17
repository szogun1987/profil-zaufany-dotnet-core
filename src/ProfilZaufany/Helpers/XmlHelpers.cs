using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace ProfilZaufany.Helpers
{
    internal static class XmlHelpers
    {
        public static string ToBase64String(this XmlDocument document)
        {
            using (var stream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    document.WriteTo(xmlWriter);
                    xmlWriter.Flush();

                    stream.Position = 0;

                    return Convert.ToBase64String(stream.ToArray());
                }
            }
        }
        public static void SignSamlDocument(XmlDocument doc, string id, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SigningKey = cert.PrivateKey;
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

            // Retrieve the value of the "ID" attribute on the root assertion element.
            var reference = new Reference("#" + id);
            reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly));

            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            if (doc.DocumentElement != null)
            {
                var nodes = doc.DocumentElement.GetElementsByTagName("Issuer", Saml20Constants.Assertion);
                
                var parentNode = nodes[0].ParentNode;
                if (parentNode != null)
                {
                    parentNode.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), nodes[0]);
                }
            }
        }
    }
}