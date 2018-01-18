using Microsoft.AspNetCore.Mvc;

namespace ProfilZaufany.TestApp.Controllers
{
    public class SignController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}