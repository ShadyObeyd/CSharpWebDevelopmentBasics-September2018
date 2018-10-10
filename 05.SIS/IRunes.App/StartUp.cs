namespace IRunes.App
{
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    
    using Controllers;

    public class StartUp
    {
        public static void Main()
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            ConfigureRouting(serverRoutingTable);

            Server server = new Server(8000, serverRoutingTable);

            server.Run();
        }

        private static void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {
            // GET
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/index"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/login"] = request => new UsersController().Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/register"] = request => new UsersController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/all"] = request => new AlbumsController().All(request);

            // POST
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/login"] = request => new UsersController().PostLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/register"] = request => new UsersController().PostRegister(request);
        }
    }
}