using ProfilZaufany.X509;

namespace ProfilZaufany.SigningForm
{
    public class SigningFormSettings
    {
        public SigningFormSettings(
            Environment environment, 
            string samlIssuer, 
            IX509Provider x509Provider)
        {
            Environment = environment;
            SamlIssuer = samlIssuer;
            X509Provider = x509Provider;
        }

        public Environment Environment { get; private set; }

        public string SamlIssuer { get; private set; }

        public IX509Provider X509Provider { get; private set; }
    }
}