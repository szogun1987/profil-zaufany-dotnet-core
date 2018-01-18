using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProfilZaufany.TestApp.Helpers
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> ToByteArray(this IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                return stream.ToArray();
            }
        }
    }
}