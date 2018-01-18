using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.LoginForm;
using ProfilZaufany.TestApp.Helpers;
using ProfilZaufany.TestApp.Models;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IX509Provider _x509Provider;
        private readonly ISecretsStore _secretsStore;

        public AuthorizationController(
            IX509Provider x509Provider,
            ISecretsStore secretsStore)
        {
            _x509Provider = x509Provider;
            _secretsStore = secretsStore;
        }

        public IActionResult Index()
        {
            var form = new AuthorizationForm
            {
                SamlIssuer = _secretsStore.GetSamlIssuer()
            };
            return View(form);
        }

        [HttpPost]
        public async Task<IActionResult> GoToPz(AuthorizationForm authorizationForm, CancellationToken token)
        {
            var settings = new SigningFormSettings(
                Environment.Test, 
                authorizationForm.SamlIssuer, 
                _x509Provider);

            _secretsStore.SetSamlIssuer(authorizationForm.SamlIssuer);

            var signingForm = new SigningForm(settings);

            var signingFormModel = await signingForm.BuildFormModel(new SigningFormBuildingArguments
            {
                AssertionConsumerServiceURL = "http://localhost:64685" + Url.Action("ConsumePzArtifact")
            }, token);
            
            return View(signingFormModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConsumePzArtifact([FromForm(Name = "SAMLArt")] string samlArt, CancellationToken token)
        {
            string samlIssuer = _secretsStore.GetSamlIssuer();

            var settings = new SigningFormSettings(
                Environment.Test,
                samlIssuer,
                _x509Provider);
            var signingForm = new SigningForm(settings);

            bool isValid;
            string userName;
            var assertionId = await signingForm.ResolveAsserionId(samlArt, token);
            if (assertionId != null)
            {
                userName = await signingForm.GetUserId(assertionId, token);
                isValid = true;
            }
            else
            {
                userName = null;
                isValid = false;
            }

            return View(new ConsumePzArtifactViewModel
            {
                IsValid = isValid,
                UserName = userName
            });
        }
    }
}