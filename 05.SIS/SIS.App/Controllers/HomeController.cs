namespace SIS.App.Controllers
{
    using Framework.Controllers;
    using Framework.ActionResults.Contracts;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index.html");
        }
    }
}
