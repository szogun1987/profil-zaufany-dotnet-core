using System.Security.Cryptography.X509Certificates;

namespace ProfilZaufany.TestApp.Helpers
{
    public interface ISecretsStore
    {
        X509Certificate2 GetCertificate();

        void SetCertificate(X509Certificate2 certificate2);

        string GetSamlIssuer();

        void SetSamlIssuer(string issuer);

    }
}