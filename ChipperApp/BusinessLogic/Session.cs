using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public  class Session
    {
        private User _user;
        private string _sessionToken;

        public Session()
        {
        }

        public Session(User user, string sessionToken)
        {
            _user = user;
            _sessionToken = sessionToken;
        }

        public User User { get { return _user; } set { _user = value; } }

        public string Token { get { return _sessionToken; } set { _sessionToken = value; } }
    }
}
