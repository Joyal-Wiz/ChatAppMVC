using Microsoft.AspNetCore.Mvc;

namespace ChatAppMVC.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}