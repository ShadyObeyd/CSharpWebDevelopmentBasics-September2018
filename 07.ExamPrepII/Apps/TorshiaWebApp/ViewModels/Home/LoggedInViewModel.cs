using System.Collections.Generic;
using TorshiaWebApp.ViewModels.Tasks;

namespace TorshiaWebApp.ViewModels.Home
{
    public class LoggedInViewModel
    {
        public ICollection<TaskViewModel> Tasks { get; set; }
    }
}
