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
    using System.IO;

    public abstract class Controller
    {
        protected Controller()
        {
            this.ViewModel = new ViewModel();
        }

        private ViewEngine ViewEngine { get; } = new ViewEngine();

        public Model ModelState { get; set; } = new Model();

        public IHttpRequest Request { get; set; }

        public ViewModel ViewModel { get; set; }

        public IIdentity Identity => (IIdentity)this.Request.Session.GetParameter("auth");

        protected IViewable View([CallerMemberName] string caller = "")
        {
            string controllerName = ControllerUtilities.GetControllerName(this);

            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, caller);
            }
            catch(FileNotFoundException e)
            {
                this.ViewModel.Data["Error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            string renderedContent = this.ViewEngine.RenderHtml(viewContent, this.ViewModel.Data);

            return new ViewResult(new View(renderedContent));
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