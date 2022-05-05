using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientActivity
    {
        private string user;
        private int activity;
        

        public ClientActivity(string user, int activity)
        {
            this.user = user;
            this.activity = activity;
        }
        public string User { get { return user; } set { user = value; } }
        public int Activity { get { return activity; } set { activity = value; } }        

    }
}
