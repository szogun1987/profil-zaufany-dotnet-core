using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProfilZaufany.TestApp.Models
{
    public class DocumentToSignViewModel
    {
        [Required]
        public IFormFile DocumentToSign { get; set; }

        public string AdditionalInfo { get; set; }
    }
}