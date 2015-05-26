using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxerino.Socks_v5
{
    public static class Map
    {
        /// <summary>
        /// This file contains variable namings copied from Benton Stark's .NET Socks client
        /// See: https://github.com/bentonstark/starksoft-aspen/blob/master/Starksoft.Aspen/Proxy/Socks5ProxyClient.cs
        /// </summary>
        #region OriginalSocksv5Implementation
        public const byte SOCKS_VERSION = 0x05;
          
        public static class Request
        {
            public const byte CMD_CONNECT = 0x01;
            public const byte CMD_BIND = 0x02;
            public const byte CMD_UDP_AASSOCIATE = 0x03;
        }

        public static class Method
        {
            public const byte NO_AUTHENTICATION_REQUIRED = 0x00;
            public const byte GSSAPI = 0x01;
            public const byte USERNAME_PASSWORD = 0x02;
            public const byte IANA_ASSIGNED_RANGE_BEGIN = 0x03;
            public const byte IANA_ASSIGNED_RANGE_END = 0x7F;
            public const byte PRIVATE_METHODS_RANGE_BEGIN = 0x80;
            public const byte PRIVATE_METHODS_RANGE_END = 0xFE;
            public const byte NO_ACCEPTABLE_METHOD = 0xFF;
        }

        public static class AddressType
        {
            public const byte IPV4 = 0x01;
            public const byte DOMAIN_NAME = 0x03;
            public const byte IPV6 = 0x04;
        }

        public static class Reply
        {
            public const byte SUCCEEDED = 0x00;
            public const byte GENERAL_SOCKS_SERVER_FAILURE = 0x01;
            public const byte CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
            public const byte NETWORK_UNREACHABLE = 0x03;
            public const byte HOST_UNREACHABLE = 0x04;
            public const byte CONNECTION_REFUSED = 0x05;
            public const byte TTL_EXPIRED = 0x06;
            public const byte COMMAND_NOT_SUPPORTED = 0x07;
            public const byte ADRESS_TYPE_NOT_SUPPORTED = 0x08;
        }

        #endregion
        #region CustomSocksv5Implementation

        public static class Lenghts
        {

            /// <summary>
            /// Minimal client's request message lenght
            /// anything below means we have recieved too few bytes
            /// and we will display an error :-)
            /// </summary>
            public const int MinimalClientsRequestLength = 8;
        }

        public static class Indexes
        {
            
        }

        public static class Ranges
        {
            
        }
        #endregion

    }
}
