using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientMenu
    {
        public static void MenuClient()
        {
            Console.WriteLine("1. Alta de Usuario");
            Console.WriteLine("2. Iniciar sesión");
            Console.WriteLine("3. Búsqueda de Usuarios");
            Console.WriteLine("4. Seguir a otro Usuario");
            Console.WriteLine("5. Publicar un Chip");
            Console.WriteLine("6. Ver Notificaciones");
            Console.WriteLine("7. Ver Perfil y Publicaciones de un Usuario");
            Console.WriteLine("8. Responder a una Publicación");
            Console.WriteLine("9. Cerrar sesión");
            Console.WriteLine("10. Desconectarse");
            Console.WriteLine($"{Protocol.CLIENT_SHOW_MENU_COMMAND} - para mostrar el menú.");
            Console.WriteLine($"{Protocol.CLIENT_EXIT_COMMAND} - para cerrar la conexión.");
            Console.WriteLine("");
        }

        public static void ExecuteMenuOption(int option, ClientSocket connection)
        {
            switch (option)
            {
                case 1:
                    ClientFunctionalities.CreateNewUser(connection);
                    break;

                case 2:
                    ClientFunctionalities.Login(connection);
                    break;

                case 3:
                    ClientFunctionalities.SearchUsers(connection);
                    break;

                case 4:
                    ClientFunctionalities.FollowUser(connection);
                    break;

                case 5:
                    ClientFunctionalities.PublishChip(connection);
                    break;

                case 6:
                    ClientFunctionalities.GetNotifications(connection);
                    break;

                case 7:
                    ClientFunctionalities.GetUserProfile(connection);
                    break;

                case 8:
                    ClientFunctionalities.ReplyChip(connection);
                    break;

                case 9:
                    ClientFunctionalities.Logout(connection);
                    break;

                default:
                    Console.WriteLine("La opción ingresada no es válida");
                    break;
            }
        }
    }
}
