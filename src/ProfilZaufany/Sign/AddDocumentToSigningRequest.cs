using System.Xml.Serialization;

namespace ProfilZaufany.Sign
{
    [XmlRoot("addDocumentToSigning", Namespace = "http://signing.ws.comarch.gov")]
    public class AddDocumentToSigningRequest
    {
        [XmlElement("doc", DataType = "base64Binary")]
        public byte[] Doc { get; set; }

        [XmlElement("successURL")] public string SuccessUrl { get; set; }

        [XmlElement("failureURL")] public string FailureUrl { get; set; }

        [XmlElement("additionalInfo")] public string AdditionalInfo { get; set; }
    }
}