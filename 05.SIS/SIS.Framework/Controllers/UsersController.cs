namespace SIS.Framework.Controllers
{
    using ActionResults.Contracts;
    using Security;

    public class UsersController : Controller
    {
        public IActionResult LogIn()
        {
            this.SignIn(new IdentityUser { Username = "Pesho", Password = "123" });
            return this.View();
        }

        public IActionResult Authorized()
        {
            this.ViewModel["username"] = this.Identity.Username;

            return this.View();
        }
    }
}
