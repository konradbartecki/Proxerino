using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Proxerino.Socks_v5
{
    public static class Exceptions
    {
        public static GeneralReply CatchSocketExceptions(Exception exception)
        {
            // If this is an unknown status it means that the error is fatal and retry will likely fail.
            switch (SocketError.GetStatus(exception.HResult))
            {
                case SocketErrorStatus.Unknown:
                    throw exception;
                case SocketErrorStatus.NetworkIsUnreachable:
                    return new GeneralReply(Map.Reply.NETWORK_UNREACHABLE);
                case SocketErrorStatus.UnreachableHost:
                    return new GeneralReply(Map.Reply.HOST_UNREACHABLE);
                case SocketErrorStatus.ConnectionRefused:
                    return new GeneralReply(Map.Reply.CONNECTION_REFUSED);
                case SocketErrorStatus.CertificateExpired:
                    return new GeneralReply(Map.Reply.TTL_EXPIRED);
            }
            //When a reply (REP value other than X'00') indicates a failure, the
            //SOCKS server MUST terminate the TCP connection shortly after sending
            //the reply.  This must be no more than 10 seconds after detecting the
            //condition that caused a failure.
            throw exception;
        }
    }
}
