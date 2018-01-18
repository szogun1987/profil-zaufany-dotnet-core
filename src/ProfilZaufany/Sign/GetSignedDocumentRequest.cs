using System.Xml.Serialization;

namespace ProfilZaufany.Sign
{
    [XmlRoot("getSignedDocument", Namespace = "http://signing.ws.comarch.gov")]
    public class GetSignedDocumentRequest
    {
        [XmlElement("id")]
        public string Id { get; set; }
    }
}