using System.Xml.Schema;
using System.Xml.Serialization;

namespace ProfilZaufany.Sign
{
    [XmlRoot("addDocumentToSigningResponse", Namespace = "http://signing.ws.comarch.gov")]
    internal class AddDocumentToSigningResponse
    {
        [XmlElement("addDocumentToSigningReturn", Form = XmlSchemaForm.Unqualified)]
        public string AddDocumentToSigningReturn { get; set; }
    }
}