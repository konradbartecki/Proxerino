using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Proxerino.Types;
using Proxerino.Socks_v5;

namespace Proxerino
{
    public class GeneralReply
    {
     //   +----+-----+-------+------+----------+----------+
     //   |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
     //   +----+-----+-------+------+----------+----------+
     //   | 1  |  1  | X'00' |  1   | Variable |    2     |
     //   +----+-----+-------+------+----------+----------+

     //Where:

     //     o  VER    protocol version: X'05'
     //     o  REP    Reply field:
     //        o  X'00' succeeded
     //        o  X'01' general SOCKS server failure
     //        o  X'02' connection not allowed by ruleset
     //        o  X'03' Network unreachable
     //        o  X'04' Host unreachable
     //        o  X'05' Connection refused
     //        o  X'06' TTL expired
     //        o  X'07' Command not supported
     //        o  X'08' Address type not supported
     //        o  X'09' to X'FF' unassigned
     //     o  RSV    RESERVED
     //     o  ATYP   address type of following address
     //        o  IP V4 address: X'01'
     //        o  DOMAINNAME: X'03'
     //        o  IP V6 address: X'04'
     //     o  BND.ADDR       server bound address
     //     o  BND.PORT       server bound port in network octet order

     //     5.  Addressing

     //       In an address field (DST.ADDR, BND.ADDR), the ATYP field specifies
     //       the type of address contained within the field:

     //           o  X'01'

     //       the address is a version-4 IP address, with a length of 4 octets

     //           o  X'03'

     //        the address field contains a fully-qualified domain name.  The first
     //        octet of the address field contains the number of octets of name that
     //        follow, there is no terminating NUL octet.

     //           o  X'04'

     //       the address is a version-6 IP address, with a length of 16 octets.

        private readonly byte[] _bytes;

        public GeneralReply(byte replyByte, StreamSocket streamSocket)
        {
            if (replyByte == Map.Reply.SUCCEEDED)
            {
                //Create new networking types for more easy way to convert a socket IP and PORT to bytes
                IPv4 newServerSocketAddress = new IPv4(streamSocket.Information.LocalAddress);
                Port newServerSocketPort = new Port(streamSocket.Information.LocalPort);
                
                //Get new byte array length and create it
                _bytes =
                    new byte[4 + newServerSocketAddress.ToBytes().Count()
                    + newServerSocketPort.ToBytes().Count()];

                //Start filling the byte array
                _bytes[0] = Map.SOCKS_VERSION;
                _bytes[1] = replyByte;
                //byte[2] is reserved 0x00
                _bytes[3] = Map.AddressType.IPV4;
                newServerSocketAddress.ToBytes().CopyTo(_bytes, 4);
                newServerSocketPort.ToBytes().CopyTo(_bytes, 8);
                if (_bytes.Count() != 10)
                {
                    //Expected byte array length is 10. Something is clearly wrong.
                    throw new Exception("Something went wrong when preparing reply message");
                }
            }
        }

        /// <summary>
        /// New SOCKS v5 Reply message. Getting only replybyte means we have encountred an error
        /// </summary>
        /// <param name="replyByte"></param>
        public GeneralReply(byte replyByte)
        {
            //Getting only replybyte means we have encountred an error
            _bytes = new byte[2];
            _bytes[0] = Map.SOCKS_VERSION;
            _bytes[1] = replyByte;
        }

        public byte[] ToBytes()
        {
            return _bytes;
        }
    }
}
