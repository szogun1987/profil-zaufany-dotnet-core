using System.Xml.Serialization;

namespace ProfilZaufany.UserInfo.DTO
{
    [XmlRoot("getTpUserInfoResponse", Namespace = "http://userinfo.zp.epuap.gov.pl")]
    public class UserInfoResponse
    {
        [XmlElement("getTpUserInfoReturn", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public UserInfo GetTpUserInfoReturn { get; set; }
    }
}