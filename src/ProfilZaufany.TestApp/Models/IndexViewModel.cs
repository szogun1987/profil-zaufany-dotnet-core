using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProfilZaufany.TestApp.Models
{
    public class IndexViewModel
    {
        [Required]
        public IFormFile Certificate { get; set; }

        [Required]
        public string CertificatePassword { get; set; }

        public bool IsCertificatePresent { get; set; }
    }
}