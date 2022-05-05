using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        private int _id;
        private Socket _socket;

        public Client()
        {
        }

        public Client(int id, Socket socket)
        {
            _id = id;
            _socket = socket;
        }

        public int Id { get { return _id; } set { _id = value; } }

        public Socket Connection { get { return _socket; } set { _socket = value; } }
    }

}