using System.ComponentModel.DataAnnotations;

namespace ProfilZaufany.TestApp.Models
{
    public class AuthorizationForm
    {
        [Required]
        public string SamlIssuer { get; set; }
    }
}