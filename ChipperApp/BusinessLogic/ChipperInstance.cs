using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Communication;

namespace BusinessLogic
{
    public class ChipperInstance
    {
        private static ChipperInstance _instance;
        public List<User> users;
        public List<Chip> chips;
        public List<Notification> notifications;
        public List<User> usersLogged;
        public List<Session> _sessions;
        /*public List<User> usersBlocked;*/
        /*public static string _userLogged;*/
        
        public static int _chipId = 0;
        public static int _notificationId = 0;


        private ChipperInstance()
        {
            //TODO: idealmente aquí iría el código a la base de datos
            users = new List<User>();
            chips = new List<Chip>();
            notifications = new List<Notification>();
            usersLogged = new List<User>();
            _sessions = new List<Session>();
        }

        public static ChipperInstance GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ChipperInstance();
            }
            return _instance;

        }

        public List<User> Users { get { return users; } set { users = value; } }
        public List<Chip> Chips { get { return chips; } set { chips = value; } }
        public List<Notification> Notification { get { return notifications; } set { notifications = value; } }
        public List<User> UsersLoggd { get { return usersLogged; } set { usersLogged = value; } }
       /* public List<User> UsersBlocked { get { return usersBlocked; } set { usersBlocked = value; } }*/

        public void AddUserToList(User user)
        {
            users.Add(user);
        }

        public List<User> AllUsers()
        {
            return users;
        }

        public bool ValidateUser(string user, string password)
        {
            User userStored = users.Find(u => (u.Username == user));
            return (userStored is not null) && (userStored.Password == password);
        }

        public bool UserIsRegistered(string user)
        {
            bool registered = false;
            User userRegistered = users.Find(u => (u.Username == user));
            if (userRegistered != null)
            {
                registered = true;
            }
            return registered;
        }

        public bool IsBlockedUser(string user)
        {
            User userBlocked = users.Find(u => (u.Username == user));
            return userBlocked.Blocked;
        }

        public bool IsLoggedUser(string user)
        {
            bool logged = false;
            User userLogged = usersLogged.Find(u => (u.Username == user));
            if (userLogged != null)
            {
                logged = true;
            }
            return logged;
        }

        public void BlockUser(string user)
        {
            User userBlocked = users.Find(u => (u.Username == user));
            if (userBlocked != null)
            {
                userBlocked.Blocked = true;
                Console.WriteLine("Usuario bloquedo");
            }
            else
            {
                Console.WriteLine("Usuario no válido");
            }
        }

        public void UnBlockUser(string user)
        {
            if (IsBlockedUser(user))
            {
                UserBLockedRemove(user);
                Console.WriteLine("Usuario desbloquedo");
            }
            else
            {
                Console.WriteLine("El usuario no esta en la lista de bloquados");
            }
        }

        public void UserBLockedRemove(string user)
        {
            User userBlocked = users.Find(u => (u.Username == user));
            userBlocked.Blocked = false;
        }

        public void UserLoggedRemove(string user)
        {
            User userLogged = usersLogged.Find(u => (u.Username == user));
            usersLogged.Remove(userLogged);
        }

        public void BlockedUsersList()
        {
            foreach (User u in users.Where(user => user.Blocked))
            {
                Console.WriteLine(u.Username);
            }
        }

        public void LoggedUsersList()
        {
            foreach (User u in usersLogged)
            {
                Console.WriteLine(u.Username);
            }
        }

        public User GetUser(string username)
        {
            return users.Find(user => user.Username == username);
        }

        public string CreateSessionToken(string username)
        {
            User user = GetUser(username);
            user.SessionToken = CreateToken(username);
            usersLogged.Add(user);
            return user.SessionToken;
        }


        public string CreateToken(string key)
        {
            Random random = new Random();
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

         /*   Console.WriteLine($"token para {key}: {Enumerable.Repeat(characters, Protocol.TOKEN_DATA_SIZE).Select(s => s[random.Next(s.Length)]).ToArray()}");*/

            return new string(Enumerable.Repeat(characters, Protocol.TOKEN_DATA_SIZE).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<User> GetFollowers(User user)
        {
            return user.Followers;
        }

        public List<Notification> GetNotifications(User user)
        {
            return user.Notifications;
        }

        public int GetChipId()
        {
            return _chipId;
        }

        public void SetChipId(int id)
        {
            _chipId = id;
        }
        public int GetNotificationId()
        {
            return _notificationId;
        }

        public void SetNotificationId(int id)
        {
            _notificationId = id;
        }
    }

}
