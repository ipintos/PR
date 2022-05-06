using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class Parser
    {
        public static string GetMethod(string request)
        {
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return tokens[0];
        }

        public static int GetAction(string request)
        {
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return Int32.Parse(tokens[1]);
        }

        public static string GetActionString(string request)
        {
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return tokens[1];
        }

        public static string GetState(string request)
        {
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return tokens[2];
        }

        public static string GetDescription(string request)
        {
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return tokens[3];
        }

        public static string GetParameterAt(string request, int position)
        {
            //debe empezar en el lugar 3 como mínimo ya que los primeros 3 son reservados 
            string[] tokens = request.Split(Protocol.MESSAGE_SEPARATOR);
            return tokens[position + 3];
        }
    }
}

