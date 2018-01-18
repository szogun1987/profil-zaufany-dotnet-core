using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.X509
{
    public interface IX509Provider
    {
        Task<X509Certificate2> Provide(CancellationToken token = default (CancellationToken));
    }
}