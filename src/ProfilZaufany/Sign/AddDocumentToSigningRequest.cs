using System.Xml.Serialization;

namespace ProfilZaufany.Sign
{
    [XmlRoot("addDocumentToSigning", Namespace = "http://signing.ws.comarch.gov")]
    public class AddDocumentToSigningRequest
    {
        /// <summary>
        /// Podpisywany dokument
        /// </summary>
        [XmlElement("doc", DataType = "base64Binary")]
        public byte[] Doc { get; set; }

        /// <summary>
        /// Adres URL do którego użytkownik zostanie przekierowany gdy podpisanie dokumentu zakończy się pomyślnie
        /// </summary>
        [XmlElement("successURL")] public string SuccessUrl { get; set; }

        /// <summary>
        /// Adres URL do którego używkonik zostanie przekierowany jeżeli podpis zakończy się porażką
        /// </summary>
        [XmlElement("failureURL")] public string FailureUrl { get; set; }

        /// <summary>
        /// Dodatkowa informacja tekstowa wyświetlana użytkownikowi
        /// </summary>
        [XmlElement("additionalInfo")] public string AdditionalInfo { get; set; }
    }
}