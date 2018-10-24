namespace SIS.Framework.Controllers
{
    using HTTP.Requests.Contracts;

    using ActionResults.Contracts;
    using ActionResults;
    using Views;
    using Utilities;
    using Models;
    using Security.Contracts;

    using System.Runtime.CompilerServices;

    public abstract class Controller
    {
        protected Controller()
        {
            this.ViewModel = new ViewModel();
        }

        public Model ModelState { get; set; } = new Model();

        public IHttpRequest Request { get; set; }

        public ViewModel ViewModel { get; set; }

        public IIdentity Identity => (IIdentity)this.Request.Session.GetParameter("auth");

        protected IViewable View([CallerMemberName] string caller = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, caller);

            var view = new View(fullyQualifiedName, this.ViewModel.Data);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
        {
            return new RedirectResult(redirectUrl);
        }

        protected void SignIn(IIdentity auth)
        {
            this.Request.Session.AddParameter("auth", auth);
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }
    }
}