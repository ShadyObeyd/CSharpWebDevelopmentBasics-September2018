namespace SIS.App
{
    using WebServer;

    using Framework.Routes;
    using Framework;

    public class Launcher
    {
        public static void Main()
        {
            var handlingContext = new HttpRouteHandlingContext(new ControllerRouter(), new ResourceRouter());

            Server server = new Server(8000, handlingContext);

            MvcEngine.Run(server);
        }
    }
}