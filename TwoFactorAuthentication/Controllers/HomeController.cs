using Microsoft.AspNetCore.Mvc;

namespace TwoFactorAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
