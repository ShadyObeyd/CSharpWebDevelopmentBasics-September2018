using SIS.MvcFramework;
using TorshiaWebApp.Data;

namespace TorshiaWebApp.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            this.Db = new TorshiaDbContext();
        }

        protected TorshiaDbContext Db { get; }
    }
}
