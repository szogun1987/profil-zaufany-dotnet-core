using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.SigningForm;
using ProfilZaufany.TestApp.Models;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GoToPz(AuthorizationForm authorizationForm)
        {
            byte[] certificateBytes;

            using (var stream = new MemoryStream())
            {
                await authorizationForm.Certificate.CopyToAsync(stream);
                certificateBytes = stream.ToArray();
            }

            var settings = new SigningFormSettings(
                Environment.Test, 
                authorizationForm.SamlIssuer, 
                new Callback509Provider(() => Task.FromResult(new X509Certificate2(certificateBytes, authorizationForm.CertificatePassword))));
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