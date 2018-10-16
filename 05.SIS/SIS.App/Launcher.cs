namespace SIS.App
{
    using WebServer;

    using Framework.Routes;
    using Framework;

    public class Launcher
    {
        public static void Main()
        {
            Server server = new Server(8000, new ControllerRouter());

            MvcEngine.Run(server);
        }
    }
}