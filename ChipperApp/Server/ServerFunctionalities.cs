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
                Console.WriteLine(user.Username);
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
                Console.WriteLine("Usuario: " + chip.User);
                Console.WriteLine("Chip: " + chip.Content);
            }            
        }

        internal void TopUsersByFollowers()
        {
            Console.WriteLine("Usuarios con mas seguidores");
            Console.WriteLine("---------------------------");
            var orderedList = _chipper.users.OrderBy(u => u.Followers.Count).Reverse();
            foreach (User u in orderedList) //Falta acotar a 5
            {
                Console.WriteLine("Usuario: " + u.Username + " Cantidad de seguidores: " + u.Followers.Count);
            }
        }

        internal void TopUsersByActivity()
        {
            Console.WriteLine("Usuarios más activos");
            Console.WriteLine("Periodo - Fecha desde:  ");
            string sDate = Console.ReadLine();
            Console.WriteLine("Periodo - Fecha hasta:  ");
            string eDate = Console.ReadLine();            
            DateTime startDate = Convert.ToDateTime(sDate);
            DateTime endDate = Convert.ToDateTime(eDate);

            List<ClientActivity> topUsersActivity = new List<ClientActivity>();

            foreach(Chip c in _chipper.Chips)
            {
                bool inRange = DateInRange(c.DatePosted, startDate, endDate);
                if (inRange)
                {
                    User user = _chipper.Users.Find(u => u.Username == c.User);
                    ClientActivity cl = topUsersActivity.Find(cl => cl.User == c.User);
                    if (cl == null)
                    {
                        cl.Activity = 1;
                        topUsersActivity.Add(cl);
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
    }
}
