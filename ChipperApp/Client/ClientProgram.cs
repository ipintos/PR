using System.Net;
using System.Net.Sockets;
using System.Text;
using Client;
using Communication;

namespace ChipperApp.Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            var client = new ClientSocket();
            client.ListenForMessages();
        }
    }

}