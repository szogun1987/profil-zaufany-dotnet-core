using System.Xml.Serialization;

namespace ProfilZaufany.UserInfo.DTO
{
    [XmlType(Namespace = "http://sign.zp.comarch.gov")]
    public class UserInfo
    {
        /// <remarks/>
        [XmlElement("accountEmailAddress", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public string AccountEmailAddress { get; set; }

        [XmlElement("claimedRole", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string ClaimedRole { get; set; }
    }
}