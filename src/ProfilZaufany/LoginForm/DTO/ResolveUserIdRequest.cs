using System.Xml.Serialization;

namespace ProfilZaufany.LoginForm.DTO
{
    [XmlRoot(ElementName = "reqResolveUserId", Namespace = "http://www.cpi.gov.pl/dt/IdpIdentityInfoServiceSchema")]
    public class ResolveUserIdRequest
    {
        [XmlElement(ElementName = "assertionId")]
        public string AssertionId { get; set; }
    }
}