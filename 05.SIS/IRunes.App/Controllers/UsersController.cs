namespace IRunes.App.Controllers
{
    using Models;
    using Services;
    
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using SIS.HTTP.Enums;

    using System;
    using System.Linq;

    public class UsersController : BaseController
    {
        private const string InvalidUsernameMessage = "Please provide a valid username with at least 4 characters!";
        private const string UsernameExistsMessage = "User with given username already exists!";
        private const string InvalidPasswordMessage = "Please provide a valid password with at least 6 characters!";
        private const string PasswordsDoNotMatchMessage = "Passwords do not match!";

        public IHttpResponse Login(IHttpRequest request)
        {
            return this.View();
        }

        public IHttpResponse PostLogin(IHttpRequest request)
        {
            string username = request.FormData["username"].ToString().Trim();
            string password = request.FormData["password"].ToString();

            var hashService = new HashService();

            string hashedPassword = hashService.Hash(password);

            var user = this.Context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user == null)
            {
                return new RedirectResult("/login");
            }

            this.SignInUser(username, request);

            return new RedirectResult("/Home/index");
        }

        public IHttpResponse Register(IHttpRequest request)
        {
            return this.View();
        }

        public IHttpResponse PostRegister(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();
            var email = request.FormData["email"].ToString();

            if (string.IsNullOrEmpty(username) || username.Length < 4)
            {
                throw new ArgumentException(InvalidUsernameMessage);
            }

            if (this.Context.Users.Any(u => u.Username == username))
            {
                throw new InvalidOperationException(UsernameExistsMessage);
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                throw new ArgumentException(InvalidPasswordMessage);
            }

            if (password != confirmPassword)
            {
                throw new ArgumentException(PasswordsDoNotMatchMessage);
            }

            var hashService = new HashService();

            string hashedPassword = hashService.Hash(password);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            this.Context.Users.Add(user);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                return new BadRequestResult(e.Message, HttpResponseStatusCode.InternalServerError);
            }

            this.SignInUser(username, request);

            return new RedirectResult("/");
        }
    }
}
