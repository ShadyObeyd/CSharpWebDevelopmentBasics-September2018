namespace SIS.App
{
    using WebServer;

    using Framework.Routes;
    using Framework;
    using Framework.Services;

    public class Launcher
    {
        public static void Main()
        {
            var dependencyContainer = new DependencyContainer();

            var handlingContext = new HttpRouteHandlingContext(new ControllerRouter(dependencyContainer), new ResourceRouter());

            Server server = new Server(8000, handlingContext);

            MvcEngine.Run(server);
        }
    }
}