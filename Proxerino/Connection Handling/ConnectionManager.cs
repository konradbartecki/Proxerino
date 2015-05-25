using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Core;
using Proxerino.Connection_Handling;
using Proxerino.Types;

namespace Proxerino
{
    public class ConnectionManager
    {
        private List<Client> clientsList = new List<Client>();

        public void AddConnection(StreamSocket incomingSocket)
        {
            NewConnection tempNewConnection = new NewConnection(incomingSocket);

            lock (clientsList)
            {
                if (clientsList.Any(client => client.ClientsHostName.CanonicalName == incomingSocket.Information.RemoteHostName.CanonicalName))
                {
                    //We have got such client already.
                    Client client =
                        clientsList.Single(x => x.ClientsHostName.CanonicalName == incomingSocket.Information.RemoteHostName.CanonicalName);
                    client.AddNewConnection(tempNewConnection);
                }
                else
                {
                    clientsList.Add(new Client(tempNewConnection));
                }
            }
            tempNewConnection.Handle();
        }

        public List<Client> GetClients()
        {
            return clientsList;
        }
    }
}
