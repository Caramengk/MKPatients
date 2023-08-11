using Microsoft.AspNetCore.Mvc;

namespace MKPatients.Controllers
{
    public class Remotes : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
