using Exam.App.ViewModels.Packages;
using System.Collections.Generic;

namespace Exam.App.ViewModels.Home
{
    public class LoggedInViewModel
    {
        public ICollection<PackageViewModel> PendingPackages { get; set; }

        public ICollection<PackageViewModel> ShippedPackages { get; set; }

        public ICollection<PackageViewModel> DeliveredPackages { get; set; }
    }
}
