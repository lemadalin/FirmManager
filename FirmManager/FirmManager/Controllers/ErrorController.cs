using Microsoft.AspNetCore.Mvc;

namespace FirmManager.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/Error/{code}")]
        public IActionResult Index(int code)
        {
            return View(code);
        }
    }
}