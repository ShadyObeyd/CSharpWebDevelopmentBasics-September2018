namespace SIS.Framework.Controllers
{
    using HTTP.Requests.Contracts;

    using ActionResults.Contracts;
    using ActionResults;
    using Views;
    using Utilities;

    using System.Runtime.CompilerServices;

    using Models;

    public abstract class Controller
    {
        protected Controller()
        {

        }

        public Model ModelState { get; set; } = new Model();

        public IHttpRequest Request { get; set; }

        public ViewModel ViewModel { get; set; }

        protected IViewable View([CallerMemberName] string caller = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, caller);

            var view = new View(fullyQualifiedName);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
        {
            return new RedirectResult(redirectUrl);
        }
    }
}