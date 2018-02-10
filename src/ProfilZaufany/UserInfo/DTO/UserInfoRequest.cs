using System.Xml.Serialization;

namespace ProfilZaufany.UserInfo.DTO
{
    [XmlRoot("getTpUserInfo", Namespace = "http://userinfo.zp.epuap.gov.pl")]
    public class UserInfoRequest
    {
        [XmlElement("tgsid", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string Tgsid { get; set; }
        
        [XmlElement("systemOrganisationId", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string SystemOrganisationId { get; set; }
    }
}