using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Proxerino.Connection_Handling;
using Proxerino.Types;

namespace Proxerino
{
    public class Client
    {
        public HostName ClientsHostName { get; private set; }

        /// <summary>
        /// Returns IP Address of client and number of connections: IPAddress (connections)
        /// </summary>
        public string DisplayName
        {
            get { return string.Format("{0} ({1})", ClientsHostName.CanonicalName, GetConnectionsNumber()); }
        }

        private readonly List<NewConnection> _connectionPairs = new List<NewConnection>();
        private BandwidthStatistics _bandwidthStatistics;
        //private ulong _totalOutboundBitsPerSecond;
        //private ulong _totalInboundBitsPerSecond;

        public Client(NewConnection connection)
        {
            ClientsHostName = connection.ClientsHostname;
            AddNewConnection(connection);
        }

        public void AddNewConnection(NewConnection connection)
        {
            _connectionPairs.Add(connection);          
        }

        public int GetConnectionsNumber()
        {
            return _connectionPairs.Count;
        }


        //I am not really sure if implementing IDispose is needed here
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        foreach (var connection in _connectionPairs)
        //        {
        //            connection.Dispose();
        //        }
        //    }

        //}
    }
}
