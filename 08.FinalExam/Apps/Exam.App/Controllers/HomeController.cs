using Exam.App.ViewModels.Home;
using Exam.App.ViewModels.Packages;
using Exam.Models.Enums;
using SIS.HTTP.Responses;
using System.Linq;

namespace Exam.App.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            var user = this.Db.Users.FirstOrDefault(u => u.Username == this.User.Username);

            if (user != null)
            {
                var viewModel = new LoggedInViewModel();

                viewModel.PendingPackages = this.Db.Packages
                    .Where(p => p.Status == Status.Pending && p.RecipientId == user.Id)
                    .Select(p => new PackageViewModel
                    {
                        Id = p.Id,
                        Title = p.Description
                    }).ToList();

                viewModel.ShippedPackages = this.Db.Packages
                    .Where(p => p.Status == Status.Shipped && p.RecipientId == user.Id)
                    .Select(p => new PackageViewModel
                    {
                        Id = p.Id,
                        Title = p.Description
                    }).ToList();

                viewModel.DeliveredPackages = this.Db.Packages
                    .Where(p => p.Status == Status.Delivered && p.RecipientId == user.Id)
                    .Select(p => new PackageViewModel
                    {
                        Id = p.Id,
                        Title = p.Description
                    }).ToList();

                return this.View("/Home/Index-LoggedIn", viewModel);
            }
            else
            {
                return this.View();
            }
        }
    }
}
