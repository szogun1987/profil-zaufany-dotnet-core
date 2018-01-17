using System.Security.Cryptography.X509Certificates;

namespace ProfilZaufany.TestApp.Helpers
{
    public interface ICertificateStore
    {
        X509Certificate2 Get();

        void Set(X509Certificate2 certificate2);
    }
}