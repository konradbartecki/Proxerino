using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxerino.Socks_Universal
{
    public class Message
    {
        private byte[] _messageBytes;

        public Message()
        {
            
        }

        void SetVersionByte(byte Version)
        {
            _messageBytes[0] = Version;
        }
    }
}
