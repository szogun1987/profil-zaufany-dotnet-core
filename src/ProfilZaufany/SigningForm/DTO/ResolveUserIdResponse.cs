using System.Xml.Schema;
using System.Xml.Serialization;

namespace ProfilZaufany.SigningForm.DTO
{
    [XmlRoot(ElementName = "respResolveUserId", Namespace = "http://www.cpi.gov.pl/dt/IdpIdentityInfoServiceSchema")]
    public class ResolveUserIdResponse
    {
        [XmlElement("userId", Form = XmlSchemaForm.Unqualified)]
        public string UserId { get; set; }
    }
}