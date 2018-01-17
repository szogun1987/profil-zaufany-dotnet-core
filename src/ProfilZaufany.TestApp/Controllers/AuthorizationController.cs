using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.SigningForm;
using ProfilZaufany.TestApp.Models;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IX509Provider _x509Provider;

        public AuthorizationController(IX509Provider x509Provider)
        {
            _x509Provider = x509Provider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GoToPz(AuthorizationForm authorizationForm)
        {
            var settings = new SigningFormSettings(
                Environment.Test, 
                authorizationForm.SamlIssuer, 
                _x509Provider);
            var signingForm = new SigningForm.SigningForm(settings);

            var signingFormModel = await signingForm.BuildFormModel(new SigningFormBuildingArguments
            {
                AssertionConsumerServiceURL = "http://localhost:64685" + Url.Action("ConsumePzArtifact")
            });

            return View(signingFormModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConsumePzArtifact([FromForm(Name = "SAMLArt")] string samlArt)
        {
            return View();
        }
    }
}