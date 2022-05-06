using BusinessLogic;
using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerFunctionalities
    {
        private ChipperInstance _chipper;

        public ServerFunctionalities()
        {
            _chipper = ChipperInstance.GetInstance();
        }

        public void GetUsers()
        {
            Console.WriteLine("Lista de usuarios del sistema");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("");            
            List<User> users = _chipper.AllUsers();
            foreach (User user in users)
            {
                Console.WriteLine("User: " + user.Username + " Nombre: " + user.Name + " bloqueado: " + user.Blocked);
            }
            Console.WriteLine(""); Console.WriteLine("");
        }

        internal void LockUser()
        {
            string userName;
            Console.WriteLine("Bloqueo de usuarios");
            Console.WriteLine("------------------");
            Console.WriteLine("");
            Console.Write("Ingrese el usuario a bloquear: ");
            userName = Console.ReadLine();
            lock(_chipper)
            {
                _chipper.BlockUser(userName);
            }
        }

        internal void UnlockUser()
        {
            Console.WriteLine("Lista de usuarios bloqueados");
            Console.WriteLine("---------------------------------------------");
            _chipper.BlockedUsersList();
            Console.WriteLine("");
            Console.Write("Ingrese el usuario a desbloquear: ");
            string userName = Console.ReadLine();
            lock (_chipper)
            {
                _chipper.UnBlockUser(userName);
            }
        }

        internal void SearchChipByKey()
        {            
            Console.Write("Ingrese texto a buscar: ");
            string text = Console.ReadLine();
            Console.WriteLine("");
            //busqueda del texto
            Console.WriteLine("Chips que incluyen el texto buscado:");
            Console.WriteLine("");
            List<Chip> textFind = new List<Chip>();
            foreach (Chip chip in _chipper.Chips)
            {
                if (chip.Content.Contains(text))
                {                   
                    textFind.Add(chip);
                }
            }
            foreach(Chip chip in textFind)
            {
                Console.WriteLine("Chip: " + chip.Content);
            }            
        }

        internal void TopUsersByFollowers()
        {
            Console.WriteLine("Usuarios con mas seguidores");
            Console.WriteLine("---------------------------");
            var orderedList = _chipper.Users.OrderBy(u => u.Followers.Count).Reverse();
            foreach (User u in orderedList) //Falta acotar a 5
            {
                Console.WriteLine("Usuario: " + u.Username + " Cantidad de seguidores: " + u.Followers.Count);
            }
        }

        internal void TopUsersByActivity()
        {
            Console.WriteLine("Usuarios más activos");
            Console.WriteLine("Periodo - Fecha desde (DD/MM/AA):  ");
            string sDate = Console.ReadLine();
            Console.WriteLine("Periodo - Fecha hasta (DD/MM/AA):  ");
            string eDate = Console.ReadLine();            
            DateTime startDate = Convert.ToDateTime(sDate);
            DateTime endDate = Convert.ToDateTime(eDate);

            List<ClientActivity> topUsersActivity = new List<ClientActivity>();

            foreach(Chip c in _chipper.Chips)
            {
                bool inRange = DateInRange(c.DatePosted, startDate, endDate);
                if (inRange)
                {
                    User user = c.User;
                    ClientActivity cl = topUsersActivity.Find(cl => cl.User == c.User.Username);
                    if (cl == null)
                    {
                        ClientActivity clnew = new ClientActivity(user.Username,1);                        
                        topUsersActivity.Add(clnew);
                    }
                    else
                    {
                        cl.Activity++;
                    }                    
                }
            }
            var orderedList = topUsersActivity.OrderBy(cl => cl.Activity).Reverse(); //Ordenada por el campo entero Activity
            foreach (ClientActivity cl in orderedList)
            {
                Console.WriteLine("Usuario: " + cl.User + "  Actividad: " + cl.Activity);
            }

        }

        public bool DateInRange(DateTime chipDate, DateTime startDate, DateTime endDate)
        {
            bool inRange = false;
            int compareStart = DateTime.Compare(startDate, chipDate);
            int compareEnd = DateTime.Compare(endDate, chipDate); ;

            if (compareStart <= 0 && compareEnd >= 0)
            {
                inRange = true;
            }
            return inRange;
        }

        public void LoginClient()
        {
            Console.WriteLine("Lista de usuarios del sistema");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("");
            List<User> users = _chipper.AllUsers();
            foreach (User user in users)
            {
                Console.WriteLine(user.Username + user.Picture);
            }
            Console.WriteLine(""); Console.WriteLine("");
        }

        //AGREGADO POR FUERA DE LOS REQUERIMIENTOS PARA PRUEBAS
        public void CARGARDATOS()
        {
            List<User> followers1 = new List<User>();
            List<User> followers2 = new List<User>();
            List<User> followers3 = new List<User>();
            List<User> followers4 = new List<User>();
            List<User> followers5 = new List<User>();

            List<User> following1 = new List<User>();
            List<User> following2 = new List<User>();
            List<User> following3 = new List<User>();
            List<User> following4 = new List<User>();
            List<User> following5 = new List<User>();

            List<Chip> chips1 = new List<Chip>();
            List<Chip> chips2 = new List<Chip>();
            List<Chip> chips3 = new List<Chip>();
            List<Chip> chips4 = new List<Chip>();
            List<Chip> chips5 = new List<Chip>();

            List<Notification> notification1 = new List<Notification>();
            List<Notification> notification2 = new List<Notification>();
            List<Notification> notification3 = new List<Notification>();
            List<Notification> notification4 = new List<Notification>();
            List<Notification> notification5 = new List<Notification>();


            User user1 = new User("user1", "pass1", "name1", "lastname1", "picture1", followers1, following1, chips1, notification1);
            User user2 = new User("user2", "pass2", "name2", "lastname2", "picture2", followers2, following2, chips2, notification2);
            User user3 = new User("user3", "pass3", "name3", "lastname3", "picture3", followers3, following3, chips3, notification3);
            User user4 = new User("user4", "pass4", "name4", "lastname4", "picture4", followers4, following4, chips4, notification4);
            User user5 = new User("user5", "pass5", "name5", "lastname5", "picture5", followers5, following5, chips5, notification5);
            _chipper.AddUserToList(user1);
            _chipper.AddUserToList(user2);
            _chipper.AddUserToList(user3);
            _chipper.AddUserToList(user4);
            _chipper.AddUserToList(user5);

            List<Chip> replies1 = new List<Chip>();
            List<Chip> replies2 = new List<Chip>();
            List<Chip> replies3 = new List<Chip>();
            List<Chip> replies4 = new List<Chip>();
            List<Chip> replies5 = new List<Chip>();

            List<string> images1 = new List<string>();
            List<string> images2 = new List<string>();
            List<string> images3 = new List<string>();
            List<string> images4 = new List<string>();
            List<string> images5 = new List<string>();

            User u1 = _chipper.Users.Find(u => (u.Username == "user1"));
            User u2 = _chipper.Users.Find(u => (u.Username == "user2"));
            User u3 = _chipper.Users.Find(u => (u.Username == "user3"));
            User u4 = _chipper.Users.Find(u => (u.Username == "user4"));
            User u5 = _chipper.Users.Find(u => (u.Username == "user5"));

            u1.Followers.Add(u2); u2.Following.Add(u1);
            u1.Followers.Add(u3); u3.Following.Add(u1);
            u2.Followers.Add(u3); u3.Following.Add(u2);
            u3.Followers.Add(u4); u4.Following.Add(u3);
            u3.Followers.Add(u5); u5.Following.Add(u3);
            u4.Followers.Add(u5); u5.Following.Add(u4);
            

            Console.WriteLine("Datos"); 
            foreach(User u in _chipper.Users)
            {
                Console.WriteLine("");
                Console.WriteLine("usuario: " + u.Username + " Nombre: " + u.Name);
                foreach (Chip c in u.Chips)
                {
                    Console.WriteLine("Chip id: " + c.ChipId + "  Contenido: " + c.Content);
                }
                foreach (User uf in u.Followers)
                {
                    Console.WriteLine("Seguidor:" + uf.Name);
                }
                foreach (User ufg in u.Following)
                {
                    Console.WriteLine("Seguiendo:" + ufg.Name);
                }
                foreach (Notification n in u.Notifications)
                {
                    Console.WriteLine("notificacion desde el usuario: " + n.Chip.Content);
                }
                Console.WriteLine("hora Now: " + DateTime.Now);
                Console.WriteLine("hora today: " + DateTime.Today);                
            }
            Console.WriteLine("datos cargados");
        }

        public void MOSTRARCHIPS()
        {
            Console.WriteLine("Datos de los chips:");
            foreach(Chip c in _chipper.Chips)
            {
                Console.WriteLine("chip id: " + c.ChipId + " Contenido: " + c.Content);
                foreach(Chip r in c.Replies)
                {
                    Console.WriteLine("Respuesta: " + r.Content + " con ID:" + r.ChipId);
                }
            }

        }
/*
        public void LEERFECHA()
        {
            Console.Write("ingresar fecha DD/MM/AA :");
            string sFecha = Console.ReadLine();
            DateTime fecha = Convert.ToDateTime(sFecha);
            Console.WriteLine("fecha convertida: " + sFecha);
            DateTime start = Convert.ToDateTime("10 / 02 / 2021");
            DateTime end = Convert.ToDateTime("10 / 02 / 2022");
            bool enRango = DateInRange(fecha, start, end);
            Console.WriteLine("empieza: " + start);
            Console.WriteLine("end: " + end);
            if (enRango)
            {
                Console.WriteLine("en rango");
            }
            else
            {
                Console.WriteLine("por fuera");
            }
        }*/
    }
}
