using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Proxerino.Socks_v5;

namespace Proxerino.Proxy
{
    public static class Exceptions
    {
        public static bool CheckForRecoverableSocketsExceptions(Exception exception)
        {
            // If this is an unknown status it means that the error is fatal and retry will likely fail.
            switch (SocketError.GetStatus(exception.HResult))
            {
                case SocketErrorStatus.Unknown:
                    Debug.WriteLine("Unknown socket exception");
                    throw exception;
                    //return false;
                case SocketErrorStatus.SoftwareCausedConnectionAbort:
                    Debug.WriteLine("Software aborted");
                    return true;
                case SocketErrorStatus.ConnectionResetByPeer:
                    Debug.WriteLine("Connection reset by peer");
                    return true;
                default:
                    throw exception;
            }
        }

        //public static byte MapSocketExceptionToReply(Exception e)
        //{
        //    switch (SocketError.GetStatus(e.HResult))
        //    {
        //            case SocketErrorStatus.ConnectionResetByPeer:
        //            return Map.
        //    }
        //}
    }
}
