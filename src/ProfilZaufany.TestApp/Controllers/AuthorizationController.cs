using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.LoginForm;
using ProfilZaufany.TestApp.Helpers;
using ProfilZaufany.TestApp.Models;
using ProfilZaufany.UserInfo;
using ProfilZaufany.X509;

namespace ProfilZaufany.TestApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IX509Provider _x509Provider;
        private readonly ISecretsStore _secretsStore;
        private readonly IUserInfoService _userInfoService;

        public AuthorizationController(
            IX509Provider x509Provider,
            ISecretsStore secretsStore,
            IUserInfoService userInfoService)
        {
            _x509Provider = x509Provider;
            _secretsStore = secretsStore;
            _userInfoService = userInfoService;
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
            _secretsStore.SetSamlIssuer(authorizationForm.SamlIssuer);

            var signingForm = new LoginForm.LoginForm(
                Environment.Test,
                authorizationForm.SamlIssuer,
                _x509Provider);

            var signingFormModel = await signingForm.BuildFormModel(new LoginFormBuildingArguments
            {
                AssertionConsumerServiceURL = Constants.PublicEndpoint + Url.Action("ConsumePzArtifact")
            }, token);
            
            return View(signingFormModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConsumePzArtifact([FromForm(Name = "SAMLArt")] string samlArt, CancellationToken token)
        {
            string samlIssuer = _secretsStore.GetSamlIssuer();
            
            var signingForm = new LoginForm.LoginForm(
                Environment.Test,
                samlIssuer,
                _x509Provider);

            bool isValid;
            string userName;
            var assertionId = await signingForm.ResolveAsserionId(samlArt, token);
            if (assertionId != null)
            {
                var info = await _userInfoService.GetUserInfo(assertionId, token);
                userName = info.AccountEmailAddress;
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