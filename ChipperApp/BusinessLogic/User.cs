using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class User
    {
        private string username;
        private string password;
        private string name;
        private string lastname;
        private string picture;
        private string sessionToken;
        private bool blocked;
        private List<User> followers;
        private List<User> following;
        private List<Chip> chips;
        private List<Notification> notifications;

        public User()
        {
        }

        public User(string username, string password, string name, string lastname, string picture, List<User> followers, List<User> following, List<Chip> chips, List<Notification> notifications)
        {
            this.username = username;
            this.password = password;
            this.name = name;
            this.lastname = lastname;
            this.picture = picture;
            this.followers = followers;
            this.following = following;
            this.chips = chips;
            this.notifications = notifications;
            blocked = false;
        }


        public string Username { get { return username; } set { username = value; } }

        public string Password { get { return password; } set { password = value; } }

        public string Name { get { return name; } set { name = value; } }

        public string Lastname { get { return lastname; } set { lastname = value; } }

        public string Picture { get { return picture; } set { picture = value; } }

        public bool Blocked { get { return blocked; } set { blocked = value; } }
        
        public string SessionToken { get { return sessionToken; } set { sessionToken = value; } }

        public List<User> Followers { get { return followers; } }

        public List<User> Following { get { return following; } }

        public List<Chip> Chips { get { return chips; } }

        public List<Notification> Notifications { get { return notifications; } }

        public override bool Equals(object user)
        {
            var newUser = user as User;
            return this.Username == newUser.Username;
        }

    }
}
