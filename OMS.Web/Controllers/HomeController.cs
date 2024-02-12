using Microsoft.AspNetCore.Mvc;

namespace OMS.Web;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
