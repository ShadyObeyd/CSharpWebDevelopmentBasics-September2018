namespace IRunes.App.Controllers
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.Linq;

    public class AlbumsController : BaseController
    {
        private const string NoAlbumsMessage = "There are currently no albums.";

        public IHttpResponse All(IHttpRequest request)
        {
            if (!this.IsAuthenticated(request))
            {
                return new RedirectResult("/users/login");
            }

            var albums = this.Context.Albums;

            if (albums.Any())
            {
                var albumsList = string.Empty;

                foreach (var album in albums)
                {
                    string albumHtml = $@"<p><a href=""/albums/details/{album.Id}"">{album.Name}</a></p>";

                    albumsList += albumHtml;
                    this.ViewBag["albumsList"] = albumsList;
                }
            }
            else
            {
                this.ViewBag["albumsList"] = NoAlbumsMessage;
            }

            return this.View();
        }
    }
}
