using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class ClientFunctionalities
    {
        public static void Login(ClientSocket connection)
        {
            Console.WriteLine("Login");
            Console.WriteLine("-----");
            Console.WriteLine("");
            Console.Write("Ingrese Usuario: ");
            string userName = Console.ReadLine();
            Console.Write("Ingrese Contraseña: ");
            string password = Console.ReadLine();
           
            string parameters = $"{userName}{Protocol.MESSAGE_SEPARATOR}{password}";
            var message = BuildRequest(Protocol.METHOD_REQUEST, Protocol.ACTION_CLIENT_LOGIN, Protocol.OK_STATE, parameters);

            SendRequest(message, connection);
        }

        public static void CreateNewUser(ClientSocket connection)
        {
            string userName, password, name, lastName, picture;
            Console.WriteLine("Alta de Usuario");
            Console.WriteLine("---------------");
            Console.WriteLine("");
            Console.Write("Ingrese Usuario: ");
            userName = Console.ReadLine();
            Console.Write("Ingrese Contraseña: ");
            password = Console.ReadLine();
            Console.Write("Ingrese Nombre: ");
            name = Console.ReadLine();
            Console.Write("Ingrese Apellido: ");
            lastName = Console.ReadLine();
            Console.WriteLine("Ingrese ubicación de la imagen de perfil: ");
            string path = Console.ReadLine();
            picture = new FileInfo(path).Name;
            
            string parameters = $"{userName}{Protocol.MESSAGE_SEPARATOR}" +
                $"{password}{Protocol.MESSAGE_SEPARATOR}" +
                $"{name}{Protocol.MESSAGE_SEPARATOR}" +
                $"{lastName}{Protocol.MESSAGE_SEPARATOR}" +
                $"{picture}";
            string message = BuildRequest(Protocol.METHOD_REQUEST, Protocol.ACTION_CLIENT_ADD_USER, Protocol.OK_STATE, parameters);

            SendFileRequest(message,path, connection);
        }

        public static void FollowUser(ClientSocket connection)
        {
            //Previa busqueda?
            Console.WriteLine("Seguimiento de Usuarios");
            Console.WriteLine("-----------------------");
            Console.WriteLine("");
            Console.Write("Ingrese Usuario a Seguir: ");
            string userName = Console.ReadLine();
            string userLogged = ""; // ChipperInstance._userLogged; el usuario logueado se debe controlar desde el server
            string message = "REQ" + "#" + "04" + "#" + userLogged + "#" + userName;
            SendRequest(message, connection);
        }

        public static void PublishChip(ClientSocket connection)
        {
            Console.WriteLine("Publicar Chip");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.Write("Ingrese Publicación: ");
            string chip = Console.ReadLine();
            //string userLogged = ""; // ChipperInstance._userLogged; el usuario logueado se debe controlar desde el server
            string image = string.Empty;
            string message = "REQ" + "#" + "15" + "#" /*+ userLogged + "#"*/ + chip;
            Console.Write("Desea adjuntar imágenes? (S/N)");
            string resp = Console.ReadLine();

            if (resp.ToLower().Equals("s"))
            {
                message = message + "#";
                List<string> images = new List<string>();
                List<string> paths = new List<string>();
                int imageNumber = 0;
                string path;
                while (resp.ToLower().Equals("s") && imageNumber < 3)
                {
                    Console.WriteLine("Ingrese ubicación de la imagen: ");
                    path = Console.ReadLine();
                    paths.Add(path);
                    image = new FileInfo(path).Name;
                    images.Add(image);
                    imageNumber++;
                    Console.WriteLine(imageNumber);
                    Console.Write("Desea adjuntar otra imagen? (S/N) ");
                    resp = Console.ReadLine();
                }
                message = message + images.Count.ToString() + "#";
                message = message + images[0];
                for (int i = 1; i < images.Count; i++)
                {
                    message = message + "&" + images[i];
                }
                SendFileRequest(message, paths, connection);
            }
            else
            {
                message = "REQ" + "#" + "05" + "#" /*+ userLogged + "#"*/ + chip + "#" + image;
                SendRequest(message, connection);
            }
        }

        public static void SearchUsers(ClientSocket connection)
        {
            Console.WriteLine("Búsqueda de usuarios");
            Console.WriteLine("--------------------");
            Console.WriteLine("");
            Console.Write("Por usuario (opcional): ");
            string userName = Console.ReadLine();
            Console.Write("Por nombre (opcional): ");
            string name = Console.ReadLine();
            if (string.IsNullOrEmpty(userName)) userName = "&";
            if (string.IsNullOrEmpty(name)) name = "&";
            string userLogged = ""; // ChipperInstance._userLogged; el usuario logueado se debe controlar desde el server
            string message = "REQ" + "#" + "03" + "#" + userLogged + "#" + userName + "#" + name;
            SendRequest(message, connection);
        }

        public static void GetNotifications(ClientSocket connection)
        {
            Console.WriteLine("Notificaciones");
            Console.WriteLine("--------------");
            Console.WriteLine(" ");
            string userName = Console.ReadLine();
            string message = "REQ" + "#" + "06" + "#" + userName; //username?
            SendRequest(message, connection);   
        }

        public static void GetUserProfile(ClientSocket connection)
        {
            Console.WriteLine("Perfil y Publicaciones de un Usuario");
            Console.WriteLine("------------------------------------");
            Console.WriteLine(" ");
            Console.WriteLine("Ingrese usuario a visualizar: ");
            string userName = Console.ReadLine();
            string message = "REQ" + "#" + "07" + "#" + userName;
            SendRequest(message, connection);
        }

        public static void ReplyChip(ClientSocket connection)
        {
            Console.WriteLine("Responder a una publicación");
            Console.WriteLine("---------------------------");
            Console.WriteLine(" ");
            Console.WriteLine("Ingrese usuario cuyas publicaciones va a responder: ");
            string userName = Console.ReadLine();
            string message = "REQ" + "#" + "08" + "#" + userName;
            SendRequest(message, connection);
        }

        public static void Logout(ClientSocket connection)
        {
            Console.WriteLine("Usuario desconectado");
        }

        public static void SendRequest(string message, ClientSocket connection)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);// Conversión de datos a bytes
            byte[] header = BuildHeader(message, connection.SessionToken);
            connection.SendHeader(header);
            connection.SendMessage(data);
        }

        private static void SendFileRequest(string message, List<string> paths, ClientSocket connection)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] header = BuildHeader(message, connection.SessionToken);
            connection.SendHeader(header);
            connection.SendMessage(data);
            foreach (string pathItem in paths)
            {
                connection.SendFile(pathItem);
            }
        }
        private static void SendFileRequest(string message, string path, ClientSocket connection)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] header = BuildHeader(message, connection.SessionToken);
            connection.SendHeader(header/*, connection.SessionToken*/);
            connection.SendMessage(data);
            connection.SendFile(path);
        }

        private static string BuildRequest(string method, int action, string state, string description)
        {
            return $"{method}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{action}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{state}{Protocol.MESSAGE_SEPARATOR}" +
                    $"{description}";
        }

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
