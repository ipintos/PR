using Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ConnectionHandler
    {
        

        public static void HandleClient(Socket connection)
        {
            var socketHelper = new SocketHelper(connection);
            Console.WriteLine($"Inició el cliente: {Thread.CurrentThread.ManagedThreadId}");
            bool connected = true;
            while (connected)
            {
                try
                {
                    Header requestHeader = ReceiveHeader(socketHelper);
                    int dataSize = requestHeader.Length;
                    string session = requestHeader.Session;
                    var content = ReceiveContent(socketHelper, dataSize);
                   /* Console.WriteLine($"content {content}");*/
                    if (string.Equals(Parser.GetActionString(content), Protocol.CLIENT_END_CONNECTION_REQUEST_COMMAND))
                    {
                        CloseClientConnection(connection);
                    }
                    else
                    {
                        string resultMessage = ExecuteClientRequest(requestHeader, content);
                        SendResponse(resultMessage, socketHelper);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine($"El cliente {Thread.CurrentThread.ManagedThreadId} cerró la conexión.");
                    connected = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error en la conexión con el cliente.");
                    connected = false;
                }
            }
        }

        public static void SendResponse(string message, SocketHelper helper)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);// Conversión de datos a bytes
            byte[] dataLength = BitConverter.GetBytes(data.Length);// Conversión del largo de los datos a bytes
            SendResponseHeader(helper, message);//antes de enviar los datos, se manda el largo al servidor
            SendResponseContent(helper, data);
        }

        private static byte[] BuildHeader(string message)
        {
            string headerMessage = message.Length + Protocol.MESSAGE_SEPARATOR;// + "00000000";
            while (headerMessage.Length < Protocol.HEADER_DATA_SIZE)
                headerMessage += Protocol.MESSAGE_SEPARATOR;
            byte[] header = Encoding.UTF8.GetBytes(headerMessage);
            return header;
        }

        private static void SendResponseHeader(SocketHelper helper, string resultMessage)
        {
            byte[] header = BuildHeader(resultMessage);
            helper.Send(header);
        }
        
        private static void SendResponseContent(SocketHelper helper, byte[] resultMessage)
        {
            helper.Send(resultMessage);
        }

        private static string ExecuteClientRequest(Header header, string content)
        {
            int action = Parser.GetAction(content); //Parsear las acciones los parámetros según nuestro formato
            string response;
            ClientFunctionalitiesResponse execute = new();
            string username;
            string password;
            string name;
            string chip;
            string image;

            if (!(execute.IsLoggedUser(header.Session)) && (action != Protocol.ACTION_CLIENT_LOGIN) && (action != Protocol.ACTION_CLIENT_ADD_USER) )
            {
                return BuildResponse(Protocol.METHOD_REQUEST, action, Protocol.ERROR_STATE, $"Debe iniciar sesión para realizar la acción {action}"); ;
            }
            switch (action)
            {
                case Protocol.ACTION_CLIENT_ADD_USER:
                    username = Parser.GetParameterAt(content, 0);
                    password = Parser.GetParameterAt(content, 1);
                    string description = execute.CreateNewUser(username, password);
                    response = execute.CreateNewUser(username, password);
                    break;

                case Protocol.ACTION_CLIENT_LOGIN:
                    username = Parser.GetParameterAt(content, 0);
                    password = Parser.GetParameterAt(content, 1);
                    response = execute.Login(username, password);
                    break;

                case Protocol.ACTION_SEARCH:
                    username = Parser.GetParameterAt(content, 0);
                    name = Parser.GetParameterAt(content, 1);                    
                    response = execute.SearchUsers(username, name, header.Session);
                    break;                
                case Protocol.ACTION_FOLLOW:
                    //falta el usuario logueado?
                    username = Parser.GetParameterAt(content, 0);
                    name = Parser.GetParameterAt(content, 1);
                    response = execute.FollowUser(username, header.Session);//falta el usuario logueado?
                    break;
                case Protocol.ACTION_PUBLISH_CHIP:
                    //falta el usuario logueado?
                    chip = Parser.GetParameterAt(content, 0);
                    image = Parser.GetParameterAt(content, 1);
                    response = execute.PublishChip( "usuario logueado", chip, image, header.Session);//falta el usuario logueado?
                    break;
                case Protocol.ACTION_NOTIFICATION:
                    //falta el usuario logueado?
                    response = execute.GetNotifications(header.Session);//falta el usuario logueado?
                    break;
                case Protocol.ACTION_VIEW_PROFILE:
                    username = Parser.GetParameterAt(content, 0); //el username del usuario al cual quiero ver el perfil
                    response = execute.GetUserProfile(username);
                    break;
                case Protocol.ACTION_REPLY_CHIP_LIST:
                    username = Parser.GetParameterAt(content, 0); //el username del usuario al que se desea responder
                    response = execute.ReplyChipList(username, header.Session);
                    break;
                case Protocol.ACTION_REPLY_CHIP:
                    username = Parser.GetParameterAt(content, 0); //el username del usuario logueado
                    string chipid = Parser.GetParameterAt(content, 1);
                    string chipreply = Parser.GetParameterAt(content, 2);
                    response = execute.ReplyChip(chipid, chipreply, header.Session);
                    break;

                case Protocol.ACTION_LOGOUT:
                case Protocol.ACTION_DISCONNECT:

                default:
                    response = BuildResponse(Protocol.METHOD_REQUEST, Protocol.ACTION_CLIENT_LOGIN, Protocol.ERROR_STATE, $"Ocurrió un error al procesar la acción {action}");
                    break;
            }
            return response;
        }

        private static string BuildResponse(string method, int action, string state, string description)
        {
            return $"{method}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{action}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{state}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{description}";
        }

        private static string ReceiveContent(SocketHelper helper, int size)
        {
            byte[] data = helper.Receive(size);//recibe el cuerpo del mensaje
            var msg = Encoding.UTF8.GetString(data);
            return msg; 
        }

        private static Header ReceiveHeader(SocketHelper helper)
        {
            byte[] headerBytes = helper.Receive(Protocol.HEADER_DATA_SIZE);
            string headerString = Encoding.UTF8.GetString(headerBytes);
            string[] headerParams = headerString.Split(Protocol.MESSAGE_SEPARATOR);
            string sessionToken = headerParams[1];
            int dataLength = Int32.Parse(headerParams[0]);
            Header header = new(dataLength, sessionToken);
            /*Console.WriteLine($"sessionToken: {sessionToken}");*/
            return header;
        }

        public static void CloseClientConnection(Socket client)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar la conexión del cliente.");
            }
        }
    }
}
