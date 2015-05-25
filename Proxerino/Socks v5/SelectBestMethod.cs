using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proxerino.Socks_v5;

namespace Proxerino.Socksv5
{
    static public class SelectBestMethod
    {
        public static byte FromAvailableMethods(byte[] incBytes)
        {
            // SERVER AUTHENTICATION REQUEST
            // The client connects to the server, and sends a version
            // identifier/method selection message:
            //
            //      +----+----------+----------+
            //      |VER | NMETHODS | METHODS  |
            //      +----+----------+----------+
            //      | 1  |    1     | 1 to 255 |
            //      +----+----------+----------+
            if (incBytes.Length < 3)
                throw new ArgumentOutOfRangeException("incBytes", "Not enough bytes received to select method");

            //Create new list where we will store all methods that client supports
            var clientMethodsList = new List<byte>();

            //Allow only socks v5 connections
            if (Config.Listener.AllowOnlySocksV5Connections)
                if (incBytes[0] != Map.SOCKS_VERSION)
                {
                    //Allow only Socks v5 compatible clients
                    return Map.Method.NO_ACCEPTABLE_METHOD;
                }
            //Get number of how many methods client supports
            var numberOfMethods = incBytes[1];
            //Add methods to list
            for (var i = 0; i < numberOfMethods; i++)
            {
                //i+2 because actual available methods identifiers begin on 2nd byte of array
                clientMethodsList.Add(incBytes[i + 2]);
            }

            //This is priority list of which socks method we want to choose first
            //if (clientMethodsList.Contains(Socksv5Map.Method.USERNAME_PASSWORD))
            //    return Socksv5Map.Method.USERNAME_PASSWORD;
            if (clientMethodsList.Contains(Map.Method.NO_AUTHENTICATION_REQUIRED))
                return Map.Method.NO_AUTHENTICATION_REQUIRED;
            if (clientMethodsList.Contains(Map.Method.GSSAPI))
                Debug.WriteLine("This server does not support GSSAPI");
            
            //For everything else
            Debug.WriteLine("Can't agree on method with client");
            return Map.Method.NO_ACCEPTABLE_METHOD;
        }          
    }
}
