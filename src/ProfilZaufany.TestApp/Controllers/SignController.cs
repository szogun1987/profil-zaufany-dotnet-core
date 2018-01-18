using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.Sign;
using ProfilZaufany.TestApp.Helpers;
using ProfilZaufany.TestApp.Models;

namespace ProfilZaufany.TestApp.Controllers
{
    public class SignController : Controller
    {
        private readonly ISigningService _signingService;
        private static ConcurrentDictionary<Guid, string> _documentInfoRepository = new ConcurrentDictionary<Guid, string>();

        public SignController(
            ISigningService signingService)
        {
            _signingService = signingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DocumentToSignViewModel viewModel, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var documentId = Guid.NewGuid();

            try
            {
                var bytes = await viewModel.DocumentToSign.ToByteArray();
                var request = new AddDocumentToSigningRequest
                {
                    Doc = bytes,
                    AdditionalInfo = viewModel.AdditionalInfo,
                    SuccessUrl = Constants.PublicEndpoint + Url.Action("OnSignSuccess", new { documentId }),
                    FailureUrl = Constants.PublicEndpoint + Url.Action("OnSignFailure", new { documentId }),
                };

                var signingUrl = await _signingService.AddDocumentToSign(request, token);
                
                _documentInfoRepository.TryAdd(documentId, signingUrl);

                return Redirect(signingUrl);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(viewModel);
            }
        }
        
        public IActionResult OnSignSuccess([FromQuery] Guid documentId)
        {
            return View(new SignedDocumentInfo
            {
                Id = documentId
            });
        }

        public async Task<IActionResult> DownloadSigned([FromQuery] Guid documentId, CancellationToken token)
        {
            if (!_documentInfoRepository.TryGetValue(documentId, out string documentUrl))
            {
                return BadRequest();
            }

            var document = await _signingService.GetSignedDocument(documentUrl, token);

            return File(document, "application/xml", "singed.xml");
        }

        public IActionResult OnSignFailure([FromQuery] Guid documentId)
        {
            _documentInfoRepository.TryRemove(documentId, out string _);
            return View();
        }
    }
}