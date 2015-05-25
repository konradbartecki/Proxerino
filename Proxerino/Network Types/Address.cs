using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;

namespace Proxerino
{
    public class Address
    {
        private HostName _hostName;

        public Address(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            
            _hostName = new HostName(value);
        }

        public Address(byte[] valueBytes)
        {
            
        }
    }
}
