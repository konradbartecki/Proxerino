using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxerino.Socks_v5;

namespace Proxerino
{
    public class MethodReply
    {
        //        The server selects from one of the methods given in METHODS, and
        //sends a METHOD selection message:
        //                      +----+--------+
        //                      |VER | METHOD |
        //                      +----+--------+
        //                      | 1  |   1    |
        //                      +----+--------+
        //If the selected METHOD is X'FF', none of the methods listed by the
        //client are acceptable, and the client MUST close the connection.

        //The values currently defined for METHOD are:
        //       o  X'00' NO AUTHENTICATION REQUIRED
        //       o  X'01' GSSAPI
        //       o  X'02' USERNAME/PASSWORD
        //       o  X'03' to X'7F' IANA ASSIGNED
        //       o  X'80' to X'FE' RESERVED FOR PRIVATE METHODS
        //       o  X'FF' NO ACCEPTABLE METHODS

        //The client and server then enter a method-specific sub-negotiation.

        private byte[] _bytes;

        /// <summary>
        /// The server selects from one of the methods given in METHODS, and sends a METHOD selection message
        /// </summary>
        /// <param name="SelectedMethod">
        /// </param>
        public MethodReply(byte SelectedMethod)
        {
            _bytes = new byte[2]; //2 bytes
            _bytes[0] = Map.SOCKS_VERSION;
            _bytes[1] = SelectedMethod;
        }

        public byte[] ToBytes()
        {
            return _bytes;
        }
    }
        

    }
