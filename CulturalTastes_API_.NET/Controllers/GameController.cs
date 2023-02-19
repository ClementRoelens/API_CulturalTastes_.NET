using Microsoft.AspNetCore.Mvc;

namespace CulturalTastes_API_.NET.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
