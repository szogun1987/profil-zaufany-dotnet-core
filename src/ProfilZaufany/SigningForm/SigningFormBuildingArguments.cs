namespace ProfilZaufany.SigningForm
{
    public class SigningFormBuildingArguments
    {
        /// <summary>
        /// Adres url pod który zostanie dostarczony artefakt SAML
        /// Będzie on otwarty metodą POST, zaś artefakt będzie znajdował się w polu formularza SAMLArt
        /// </summary>
        public string AssertionConsumerServiceURL { get; set; }
    }
}