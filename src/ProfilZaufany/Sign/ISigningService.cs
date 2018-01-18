using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.Sign
{
    public interface ISigningService
    {
        Task<AddDocumentToSigningResponse> AddDocumentToSign(AddDocumentToSigningRequest request, CancellationToken token);
    }
}