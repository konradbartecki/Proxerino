using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Proxerino.Connection_Handling;
using Proxerino.Types;

namespace Proxerino.Socks_v5
{
    public class ClientsRequest
    {
        //       The SOCKS request is formed as follows:
        //   +----+-----+-------+------+----------+----------+
        //   |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
        //   +----+-----+-------+------+----------+----------+
        //   | 1  |  1  | X'00' |  1   | Variable |    2     |
        //   +----+-----+-------+------+----------+----------+
        //Where:
        //     o  VER    protocol version: X'05'
        //     o  CMD
        //        o  CONNECT X'01'
        //        o  BIND X'02'
        //        o  UDP ASSOCIATE X'03'
        //     o  RSV    RESERVED
        //     o  ATYP   address type of following address
        //        o  IP V4 address: X'01'
        //        o  DOMAINNAME: X'03'
        //        o  IP V6 address: X'04'
        //     o  DST.ADDR       desired destination address
        //     o  DST.PORT desired destination port in network octet
        //        order

        private byte[] _incBytes;

        public ClientsRequest(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentNullException("bytes");
            }
            if(bytes.Length < Map.Lenghts.MinimalClientsRequestLength)
                throw new ArgumentException("Client's request constructor received not enough bytes", "bytes");
                
            _incBytes = bytes;

        }

        public byte ClientsVersion
        {
            get { return _incBytes[0]; }
        }

        public byte CommandByte
        {
            get { return _incBytes[1]; }
        }

        public byte AddressTypeByte
        {
            get { return _incBytes[3]; }
        }

        public Port DestinationPort
        {
            get
            {
                byte[] portBytes = new byte[2];

                switch (AddressTypeByte)
                {
                    case Map.AddressType.IPV4:
                        portBytes[0] = _incBytes[8];
                        portBytes[1] = _incBytes[9];
                        break;
                    case Map.AddressType.IPV6:
                        return null;
                    case Map.AddressType.DOMAIN_NAME:
                        //int portIndex = 4 + _incBytes[4];

                        //portBytes[0] = _incBytes[portIndex + 1];
                        //portBytes[1] = _incBytes[portIndex + 2];
                        return null;
                    default:
                        return null;
                }
                return new Port(portBytes);
            }
        }

        public byte[] GetDestinationAddressBytes
        {
            get
            {
                if (AddressTypeByte == Map.AddressType.IPV4)
                {
                    byte[] ipAddressBytes = new byte[4];
                    ipAddressBytes[0] = _incBytes[4];
                    ipAddressBytes[1] = _incBytes[5];
                    ipAddressBytes[2] = _incBytes[6];
                    ipAddressBytes[3] = _incBytes[7];

                    return ipAddressBytes;
                }
                if (AddressTypeByte == Map.AddressType.IPV6)
                {
                    //We do not support IPv6 yet :-(
                    return null;
                }
                if (AddressTypeByte == Map.AddressType.DOMAIN_NAME)
                {
                    int domainLength = _incBytes[4];
                    byte[] domainBytes = new byte[domainLength];
                    //Copy domain bytes from where domain address always begins (that's 5th byte)
                    //to new domainBytes array starting from 0 index
                    Array.Copy(_incBytes, 5, domainBytes, 0, domainLength);

                    return domainBytes;
                }
                return null;
            }
        }
    }
}
