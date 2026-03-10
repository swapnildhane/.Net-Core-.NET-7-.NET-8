using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EmployController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
