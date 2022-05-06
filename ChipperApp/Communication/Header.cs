using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public  class Header
    {
        private int _dataLength;
        private string _sessionToken;

        public Header()
        {
        }

        public Header(int dataLength, string sessionToken)
        {
            _dataLength = dataLength;
            _sessionToken = sessionToken;
        }

        public int Length { get { return _dataLength; } set { _dataLength = value; } }

        public string Session { get { return _sessionToken; } set { _sessionToken = value; } }
    }
}

