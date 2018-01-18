using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.X509
{
    public class Callback509Provider : IX509Provider
    {
        private readonly Func<CancellationToken, Task<X509Certificate2>> _callback;

        public Callback509Provider(Func<CancellationToken, Task<X509Certificate2>> callback)
        {
            _callback = callback;
        }

        public Task<X509Certificate2> Provide(CancellationToken token = default (CancellationToken))
        {
            return _callback(token);
        }
    }
}