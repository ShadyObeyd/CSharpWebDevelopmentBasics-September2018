using SIS.HTTP.Responses;
using System.Linq;
using TorshiaWebApp.ViewModels.Home;
using TorshiaWebApp.ViewModels.Tasks;

namespace TorshiaWebApp.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            var user = this.Db.Users.FirstOrDefault(u => u.Username == this.User.Username);

            if (user != null)
            {
                var viewModel = new LoggedInViewModel
                {
                    Tasks = this.Db.Tasks.Where(t => t.IsReported == false).Select(t => new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Level = t.AffectedSectors.Count
                    }).ToList()
                };

                return this.View("Home/Index-LoggedIn", viewModel);
            }
            else
            {
                return this.View();
            }
        }
    }
}
