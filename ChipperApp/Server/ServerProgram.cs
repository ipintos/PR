using Server;

namespace ChipperApp.Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            var server = new ServerSocket();
            server.StartServer();
        }
    }
}
