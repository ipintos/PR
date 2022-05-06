using BusinessLogic;
using Communication;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientSocket
    {
        private Socket _clientSocket;
        private IPEndPoint _serverIPEndPoint;
        private IPEndPoint _clientIPEndPoint;
        static readonly SettingsManager settings = new();
        private bool _keepConnection = true;
        private SocketHelper _socketHelper;
        private string _sessionToken;


        public ClientSocket()
        {
            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientIPEndPoint = new IPEndPoint(IPAddress.Parse(settings.GetSetting(ClientConfig.ServerIpConfigKey)), Int32.Parse(settings.GetSetting(ClientConfig.ClientPortConfigKey)));
                _serverIPEndPoint = new IPEndPoint(IPAddress.Parse(settings.GetSetting(ClientConfig.ServerIpConfigKey)), Int32.Parse(settings.GetSetting(ClientConfig.SeverPortConfigKey)));
                _clientSocket.Bind(_clientIPEndPoint);
                _clientSocket.Connect(_serverIPEndPoint);
                _socketHelper = new SocketHelper(_clientSocket);
                _sessionToken = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar la conexión desde el Client. {ex}");
            }
        }

        public void ListenForMessages()
        {
            try
            {                
                ClientMenu.MenuClient();
                while (_keepConnection)
                {
                    Console.Write(">");
                    var message = Console.ReadLine();
                    if (IsExitCommand(message))
                    {
                        _keepConnection = false;
                        CloseConnection();
                        break;
                    }
                    if (IsShowMenuCommand(message))
                    {
                        ClientMenu.MenuClient();
                        continue;
                    }
                    if (IsClearMenuCommand(message))
                    {
                        Console.Clear();
                        continue;
                    }
                    try
                    {
                        ClientMenu.ExecuteMenuOption(Int32.Parse(message), this);
                        var dataSize = ReceiveHeader(_socketHelper);
                        string response = ReceiveContent(_socketHelper, dataSize);
                        ProcessResponse(response); //devuelve un string o se manda a pantalla directamente?
                        
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("No se reconoce el comando.");
                    }
                    catch
                    {
                        _keepConnection = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al conectarse al servidor: {ex}");
                CloseConnection();
            }
        }

        private void ProcessResponse(string response) //es un string o se manda a pantalla directamente?
        {
            if (string.Equals(Parser.GetState(response), Protocol.OK_STATE))
            {
                int action = Parser.GetAction(response);
                if (action == Protocol.ACTION_CLIENT_LOGIN)
                {
                    _sessionToken = Parser.GetParameterAt(response, 0);
                }

                switch (action) //según la petición que se haya realizado es como se muestra la respuesta que dio el servidor
                {
                    case Protocol.ACTION_CLIENT_ADD_USER:
                        Console.WriteLine(Parser.GetDescription(response)); 
                        break;
                    case Protocol.ACTION_CLIENT_LOGIN:
                        //Console.WriteLine(Parser.GetDescription(response));
                        break;
                    case Protocol.ACTION_SEARCH:
                        string[] users = Parser.GetDescription(response).Split("&");
                        foreach (string user in users)
                        {
                            Console.WriteLine(user);
                        }
                        break;
                    case Protocol.ACTION_FOLLOW:
                        Console.WriteLine(Parser.GetDescription(response));
                        break;
                    case Protocol.ACTION_PUBLISH_CHIP:
                        Console.WriteLine(Parser.GetDescription(response));
                        break;
                    case Protocol.ACTION_NOTIFICATION:                                        
                        string[] notifications = Parser.GetDescription(response).Split("&");
                        foreach (String n in notifications)
                        {
                            string[] notificationFields = n.Split("|");
                            Console.WriteLine("idNotificacion: " + notificationFields[0] + "  chip: " + notificationFields[1]);
                        }
                        //Desde aqui se larga la posibilidad de responder a las notificaciones        
                        ClientMenu.ExecuteMenuOption(Protocol.ACTION_NOTIFICATION_REPLY, this);
                        break;
                    case Protocol.ACTION_VIEW_PROFILE:
                        string[] userinfo = Parser.GetDescription(response).Split("&");                        
                        Console.WriteLine("Usuario: " + userinfo[0]);
                        Console.WriteLine("Nombre: " + userinfo[1] + " " + userinfo[2]);
                        Console.WriteLine("Cantidad de seguidores: " + userinfo[3]);
                        Console.WriteLine("Cantidad de cuentas que sigue: " + userinfo[4]);                        
                        Console.WriteLine("Publicaciones:");
                        if (userinfo.Length > 5)
                        {
                            string[] chips =userinfo[5].Split("|");
                            
                            foreach (String c in chips)
                            {
                                string[] notificationFields = c.Split("|");
                                Console.WriteLine(c);
                            }
                        } else
                        {
                            Console.WriteLine("0");
                        }                          

                        break;
                    case Protocol.ACTION_REPLY_CHIP_LIST:
                        //user.Username + "@"+ c.ChipId + "|" + c.Content + "&";
                        string [] replyChipListInfo = Parser.GetDescription(response).Split("@");
                        string userOriginal = replyChipListInfo[0]; //usuario que hizo la publicacion original y al cual se va a responder
                        string[] chipinfo = replyChipListInfo[1].Split("&");
                        foreach (String c in chipinfo)
                        {
                            string[] chipFields = c.Split("|");
                            Console.WriteLine("idchip: " + chipFields[0] + "  chip: " + chipFields[1]);
                        }
                        //Desde aqui se ejcuta la opcion para responder a la publicación que se selecciona
                         ClientMenu.ExecuteMenuOption(Protocol.ACTION_REPLY_CHIP, this);                      
                        break;
                    case Protocol.ACTION_REPLY_CHIP:
                        Console.WriteLine(Parser.GetDescription(response));
                        break;
                    case Protocol.ACTION_LOGOUT:
                        Console.WriteLine(Parser.GetDescription(response));
                        _sessionToken = String.Empty;
                        break;
                    case Protocol.ACTION_DISCONNECT:
                        CloseConnection();
                        break;
                }
                

            }
            //return Parser.GetParameterAt(response, 0);
        }

        public void CloseConnection()
        {
            try
            {
                ClientFunctionalities.SendRequest($"REQ#{Protocol.CLIENT_END_CONNECTION_REQUEST_COMMAND}", this);
                Console.WriteLine("Client connection closed request send");
            }
            catch
            {
                Console.WriteLine("Error al cerrar la conexión desde el Client.");
            }
        }

        public void SendHeader(byte[] message/*, string sessionToken*/)
        {
            _socketHelper.Send(message);
        }

        public void SendMessage(byte[] message)
        {
            _socketHelper.Send(message);
        }

        internal void SendFile(string path)
        {
            FileCommsHandler fileCommHandler = new FileCommsHandler(_socketHelper);
            fileCommHandler.SendFile(path);
        }

        private static int ReceiveHeader(SocketHelper helper)
        {
            byte[] headerBytes = helper.Receive(Protocol.HEADER_DATA_SIZE);
            string header = Encoding.UTF8.GetString(headerBytes);
            string[] headerParams = header.Split(Protocol.MESSAGE_SEPARATOR);
            int dataLength = Int32.Parse(headerParams[0]);// helper.Receive(Protocol.HEADER_DATA_SIZE);//recibe el header
            return dataLength;
        }

        private static string ReceiveContent(SocketHelper helper, int size)
        {
            byte[] data = helper.Receive(size);
            var msg = Encoding.UTF8.GetString(data);
            return msg;
        }

        private bool IsExitCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.CLIENT_EXIT_COMMAND);
        }

        private static bool IsShowMenuCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.CLIENT_SHOW_MENU_COMMAND);
        }

        private bool IsClearMenuCommand(string message)
        {
            return message.Trim().ToLower().Equals(Protocol.CLEAR_CONSOLE_COMMAND);
        }

        public string SessionToken { get { return _sessionToken; } set { _sessionToken = value; } }

        private static byte[] BuildHeader(string message, string sessionToken)
        {
            string headerMessage = message.Length + Protocol.MESSAGE_SEPARATOR + sessionToken;
            while (headerMessage.Length < Protocol.HEADER_DATA_SIZE)
                headerMessage += Protocol.MESSAGE_SEPARATOR;
            byte[] header = Encoding.UTF8.GetBytes(headerMessage);
            return header;
        }
    }
}

