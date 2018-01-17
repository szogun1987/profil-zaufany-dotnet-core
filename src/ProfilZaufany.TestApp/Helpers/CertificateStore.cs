using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp.Helpers
{
    /// <summary>
    /// W środowisku produkcyjnym certyfikat powinien znajdować się w bezpiecznym magazynie
    /// </summary>
    public class CertificateStore : ICertificateStore, IX509Provider
    {
        private X509Certificate2 _certificate2;

        public X509Certificate2 Get()
        {
            return _certificate2;
        }

        public void Set(X509Certificate2 certificate2)
        {
            _certificate2 = certificate2;
        }

        public Task<X509Certificate2> Provide()
        {
            return Task.FromResult(_certificate2);
        }
    }
}