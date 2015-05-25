using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxerino
{
    public class Port
    {
        private int _port;

        private void Set(int value)
        {
            if (value > 1 && value < 65535)
            {
                _port = value;
            }
        }
        /// <summary>
        /// Port class which converts port number between int, bytes and string.
        /// </summary>
        /// <param name="value">Int, String, or 2 bytes</param>
        public Port(int value)
        {
            Set(value);
        }

        public Port(string value)
        {
            int i = Convert.ToInt32(value);
            Set(i);
        }

        public Port(byte[] valueBytes)
        {
            if (valueBytes.Length == 2)
            {
                int i = 0;
                i = i + valueBytes[0] * 256;
                i = i + valueBytes[1];
                Set(i);
            }
            else
            {
                throw new ArgumentException("valueBytes");
            }
        }


        //  Benton Stark snippet
        //  http://code.openhub.net/file?fid=KNqQMl-VtaK9vd2d3xUuBlFhfeo&cid=gFz0OrNq4e4&s=&fp=265574&projSelected=true&fp=265574&projSelected=true#L487

        public byte[] ToBytes()
        {
            byte[] array = new byte[2];
            array[0] = Convert.ToByte(_port / 256);
            array[1] = Convert.ToByte(_port % 256);
            return array;
        }

        public override string ToString()
        {
            return _port.ToString();
        }

        public int ToInt32()
        {
            return _port;
        }
    }
}
