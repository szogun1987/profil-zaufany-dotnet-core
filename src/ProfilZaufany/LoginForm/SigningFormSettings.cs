using ProfilZaufany.X509;

namespace ProfilZaufany.LoginForm
{
    public class SigningFormSettings
    {
        /// <summary>
        /// Tworzy nową instancję parametrów obiektu signing form
        /// </summary>
        /// <param name="environment">Środowisko PZ</param>
        /// <param name="samlIssuer">Wartość elementu Issuer z wniosku o przyznanie crtyfikatu</param>
        /// <param name="x509Provider">Dostawca certyfikatu do podpisu komunikacji z systemem PZ</param>
        public SigningFormSettings(
            Environment environment, 
            string samlIssuer, 
            IX509Provider x509Provider)
        {
            Environment = environment;
            SamlIssuer = samlIssuer;
            X509Provider = x509Provider;
        }

        /// <summary>
        /// Środowisko PZ
        /// </summary>
        public Environment Environment { get; private set; }

        /// <summary>
        /// Wartość elementu Issuer z wniosku o przyznanie crtyfikatu
        /// </summary>
        public string SamlIssuer { get; private set; }

        /// <summary>
        /// Dostawca certyfikatu do podpisu komunikacji z systemem PZ
        /// </summary>
        public IX509Provider X509Provider { get; private set; }
    }
}