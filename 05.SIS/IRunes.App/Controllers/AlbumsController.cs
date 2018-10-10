namespace IRunes.App.Controllers
{
    using Controllers;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.Linq;

    public class AlbumsController : BaseController
    {
        public IHttpResponse All(IHttpRequest request)
        {
            if (!this.IsAuthenticated(request))
            {
                return new RedirectResult("/users/login");
            }

            var albums = this.Context.Albums;

            var albumsList = string.Empty;

            if (albums.Any())
            {
                foreach (var album in albums)
                {
                    // TODO concat album string!
                }
            }

            return this.View();
        }
    }
}
