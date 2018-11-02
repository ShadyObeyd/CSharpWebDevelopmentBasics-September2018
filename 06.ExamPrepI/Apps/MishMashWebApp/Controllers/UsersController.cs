namespace MishMashWebApp.Controllers
{
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses;

    using SIS.MvcFramework;
    using SIS.MvcFramework.Services;
    
    using ViewModels.Users;
    using Models.Enums;
    using Models;

    using System.Linq;
    using System;
    

    public class UsersController : BaseController
    {
        private readonly IHashService hashService;

        public UsersController(IHashService hashService)
        {
            this.hashService = hashService;
        }

        public IHttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Register(RegisterViewModel model)
        {
            if (model == null)
            {
                return this.BadRequestErrorWithView("All fields must be filled!");
            }

            if (this.Db.Users.Any(u => u.Username == model.Username.Trim()))
            {
                return this.BadRequestErrorWithView($"User with username {model.Username} already exists!");
            }

            string hashedPassword = this.hashService.Hash(model.Password);

            Role role = Role.User;

            if (!this.Db.Users.Any())
            {
                role = Role.Admin;
            }

            User user = new User
            {
                Username = model.Username,
                Password = hashedPassword,
                Role = role,
                Email = model.Email
            };

            this.Db.Users.Add(user);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                return this.BadRequestErrorWithView(e.Message);
            }

            return this.Redirect("/Users/Login");
        }

        public IHttpResponse Login()
        {
            return this.View();
        }

        [HttpPost]
        public IHttpResponse Login(LoginViewModel model)
        {
            string hashedPassword = this.hashService.Hash(model.Password);

            User user = this.Db.Users.FirstOrDefault(u => u.Username == model.Username.Trim() && 
                                                          u.Password == hashedPassword);

            if (user == null)
            {
                return this.BadRequestErrorWithView("Invalid username or passoword!");
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