using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProfilZaufany.TestApp.Models
{
    public class AuthorizationForm
    {
        [Required]
        public string SamlIssuer { get; set; }

        [Required]
        public IFormFile Certificate { get; set; }

        [Required]
        public string CertificatePassword { get; set; }
    }
}