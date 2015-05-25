using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;

namespace Proxerino.Types
{
    public class IPv4
    {
        private HostName _hostName;

        public IPv4(string value)
        {
            _hostName = new HostName(value);
        }

        public IPv4(byte[] valueBytes)
        {
            if (valueBytes.Length == 4)
            {
                string value = String.Format("{0}.{1}.{2}.{3}", 
                    valueBytes[0], 
                    valueBytes[1], 
                    valueBytes[2],
                    valueBytes[3]);
                _hostName = new HostName(value);
            }
        }

        public IPv4(HostName hostname)
        {
            _hostName = hostname;
        }

        public byte[] ToBytes()
        {
            string[] strings = _hostName.CanonicalName.Split('.');
            byte[] returnBytes = new byte[4];
            int i = 0;
            foreach (string s in strings)
            {
                returnBytes[i] = Convert.ToByte(s);
                i++;
            }
            return returnBytes;
        }

        //Using "new string ToString()" below is probably bad but I'll commit it to github without touching anything
        public new string ToString()
        {
            return _hostName.CanonicalName;
        }

        public HostName ToHostName()
        {
            return _hostName;
        }
    }
}
