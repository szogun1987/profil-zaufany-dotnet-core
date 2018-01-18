using System.Xml.Schema;
using System.Xml.Serialization;

namespace ProfilZaufany.Sign
{
    [XmlRoot("getSignedDocumentResponse", Namespace = "http://signing.ws.comarch.gov")]
    public class GetSignedDocumentResponse
    {
        [XmlElement("getSignedDocumentReturn", DataType = "base64Binary", Form = XmlSchemaForm.Unqualified)]
        public byte[] SignedDocumentReturn { get; set; }
    }
}