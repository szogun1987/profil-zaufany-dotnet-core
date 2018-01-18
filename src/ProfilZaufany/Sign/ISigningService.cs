using System.Threading;
using System.Threading.Tasks;

namespace ProfilZaufany.Sign
{
    public interface ISigningService
    {
        /// <summary>
        /// Dodaje dokument do kolejki podpisywanych dokumentów
        /// </summary>
        /// <param name="request">Informacje o podpisywanym dokumencie</param>
        /// <param name="token"></param>
        /// <returns>Identyfikator dokumentu - url pod który należy przekierować użytkownika</returns>
        Task<string> AddDocumentToSign(AddDocumentToSigningRequest request, CancellationToken token);

        /// <summary>
        /// Pobiera podpisany dokument
        /// </summary>
        /// <param name="documentId">Identyfikator dokumentu</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<byte[]> GetSignedDocument(string documentId, CancellationToken token);
    }
}