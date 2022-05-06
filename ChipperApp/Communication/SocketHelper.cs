using Settings;
using System.Net.Sockets;


namespace Communication
{
    public class SocketHelper
    {
        private readonly Socket _socket;

        public SocketHelper(Socket socket)
        {
            _socket = socket;
        }

        public void Send(byte[] data)
        {
            int offset = 0;
            int size = data.Length;
            try
            {
                while (offset < size)
                    {
                    int sent = _socket.Send(data, offset, size - offset, SocketFlags.None);
                    if (sent == 0)
                        throw new SocketException();
                    offset += sent;
                }
            }
            catch(Exception e){
                Console.WriteLine($"Ocurrió un error al enviar el mensaje {e}");
            }
        }

        public byte[] Receive(int length)
        {
            byte[] response = new byte[length];
            int offset = 0;
            try
            {
                while (offset < length)
                {
                    int received = _socket.Receive(response, offset, length - offset, SocketFlags.None);
                    if (received == 0)
                        throw new SocketException();//Conexión perdida. Es una exception o socketException?
                    offset += received;
                }
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al recibir el mensaje {ex}");
                 throw;
            }
           
        }
    }
}

