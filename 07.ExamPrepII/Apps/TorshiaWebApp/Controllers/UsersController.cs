namespace TorshiaWebApp.Controllers
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Services;
    using System.Linq;
    using TorshiaWebApp.Models;
    using TorshiaWebApp.Models.Enums;
    using ViewModels.Users;

    public class UsersController : BaseController
    {
        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        public IHttpResponse Login()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(UserLogInInputModel model)
        {
            var hashedPassword = this.hashService.Hash(model.Password);

            var user = this.Db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == hashedPassword);

            if (user == null)
            {
                return this.BadRequestErrorWithView("Invalid Username or Password");
            }

            var mvcUser = new MvcUserInfo
            {
                Username = model.Username,
                Role = user.Role.ToString(),
                Info = user.Email
            };

            var cookieContent = this.UserCookieService.GetUserCookie(mvcUser);

            var cookie = new HttpCookie(".auth-cakes", cookieContent, 7) { HttpOnly = true };

            this.Response.Cookies.Add(cookie);

            return this.Redirect("/");
        }

        public IHttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Register(UserRegisterInputModel model)
        {
            if (model == null)
            {
                return this.BadRequestErrorWithView("All fields must be filled!");
            }

            if (this.Db.Users.Any(u => u.Username == model.Username))
            {
                return this.BadRequestErrorWithView($"User {model.Username} already exists!");
            }

            Role role = Role.User;

            if (!this.Db.Users.Any())
            {
                role = Role.Admin;
            }

            var user = new User
            {
                Username = model.Username,
                Password = this.hashService.Hash(model.Password),
                Role = role,
                Email = model.Email
            };

            this.Db.Users.Add(user);
            this.Db.SaveChanges();

            return this.Redirect("/Users/Login");
        }

        public IHttpResponse Logout()
        {
            if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return this.Redirect("/");
            }

            var cookie = this.Request.Cookies.GetCookie(".auth-cakes");

            cookie.Delete();

            this.Response.Cookies.Add(cookie);

            return this.Redirect("/");
        }
    }
}
