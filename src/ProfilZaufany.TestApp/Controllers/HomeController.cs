using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfilZaufany.TestApp.Helpers;
using ProfilZaufany.TestApp.Models;

namespace ProfilZaufany.TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICertificateStore _certificateStore;

        public HomeController(ICertificateStore certificateStore)
        {
            _certificateStore = certificateStore;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                IsCertificatePresent = _certificateStore.Get() != null
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel certificateForm)
        {
            certificateForm.IsCertificatePresent = _certificateStore.Get() != null;
            if (!ModelState.IsValid)
            {
                return View(certificateForm);
            }
            
            using (var stream = new MemoryStream())
            {
                await certificateForm.Certificate.CopyToAsync(stream);
                var certificateBytes = stream.ToArray();
                try
                {
                    var certificate = new X509Certificate2(certificateBytes, certificateForm.CertificatePassword);
                    _certificateStore.Set(certificate);
                    certificateForm.IsCertificatePresent = true;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }
            
            return View(certificateForm);
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
