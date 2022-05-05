using BusinessLogic;
using Communication;
using Settings;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class ServerSocket
    {
        private Socket _serverSocket;
        private IPEndPoint _serverIPEndPoint;
        static readonly SettingsManager settings = new();
        private bool _keepConnection = true;
        private Thread _threadServerMenu;
        private List<Client> _connectedClients = new();

        public ServerSocket()
        {
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverIPEndPoint = new IPEndPoint(IPAddress.Parse(settings.GetSetting(ServerConfig.ServerIpConfigKey)), Int32.Parse(settings.GetSetting(ServerConfig.SeverPortConfigKey)));
                _serverSocket.Bind(_serverIPEndPoint);
                _serverSocket.Listen(Protocol.BACKLOG);
            }
            catch
            {
                Console.WriteLine("Error al crear el Server.");
            }
            finally
            {

            }
        }

        public void StartServer()
        {
            try
            {
                _threadServerMenu = new Thread(() => StartServerMenu());//creo un thread para manejar el menú del servidor
                _threadServerMenu.Start();
                while (_keepConnection)//el main thread espera las conexiones del cliente
                {
                    ListenForClients();
                }
                CloseServerConnection();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al iniciar el Server.{ex}");
            }
        }

        private void ListenForClients()
        {
            try
            {
                var newClient = _serverSocket.Accept();
                StartClient(newClient);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error al iniciar ListenForClients.{e}");
            }
        }

        private void StartClient(Socket newClient)
        {
            try
            {
                var threadIncomingConn = new Thread(() => ConnectionHandler.HandleClient(newClient)); //creo varios thread para poder conectar varios clientes.
                threadIncomingConn.Start();
                Client client = new()
                {
                    Id = threadIncomingConn.ManagedThreadId,
                    Connection = newClient
                };
                _connectedClients.Add(client);
            }
            catch (Exception e)
            {
                Console.WriteLine($"StartClient {e}");
            }
        }

        private void StartServerMenu()
        {
            var option = "";
            ServerMenu.MenuServer();//inicia el servidor mostrando el menú
            while (_keepConnection)//manejo las acciones del server
            {
                Console.Write(">");
                option = Console.ReadLine();
                if (IsExitCommand(option))
                {
                    _keepConnection = false;
                    break;
                }
                if (IsShowMenuCommand(option))
                {
                    ServerMenu.MenuServer();
                    continue;
                }
                if (IsClearMenuCommand(option))
                {
                    Console.Clear();
                    continue;
                }
                try
                {
                    ServerMenu.ExecuteMenuOption(Int32.Parse(option));
                }
                catch (FormatException)
                {
                    Console.WriteLine("No se reconoce el comando.");
                }
            }
            CloseServerConnection();
        }

        public void CloseServerConnection()
        {
            try
            {
                foreach(var client in _connectedClients)
                {
                    client.Connection.Shutdown(SocketShutdown.Both);
                    client.Connection.Close();
                }
                Console.WriteLine("cerré las conexiones ahora cierro el server?");
                _serverSocket.Shutdown(SocketShutdown.Both);
                _serverSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar la conexión desde el Server. {ex}");
            }
        }

        private static bool IsShowMenuCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.SERVER_SHOW_MENU_COMMAND);
        }

        private static bool IsExitCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.SERVER_EXIT_COMMAND);
        }

        private static bool IsClearMenuCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.CLEAR_CONSOLE_COMMAND);
        }
    }
}
