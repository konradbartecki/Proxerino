using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Networking.Sockets;
using Windows.UI.Xaml.Documents;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Proxerino.Connection_Handling
{
    class StreamingProxy
    {
        // 1. await get data from client
        // 2. send data to destination
        // 3. await data from destination
        // 4. send data to client

        //async loops
        // get data from client
        // { send data to destination }
        // get data from destination
        // { send data to client }

        private StreamSocket clientSocket;
        private StreamSocket destinationSocket;
        private CancellationTokenSource clientTokenSource;
        private CancellationTokenSource destinationTokenSource;

        public StreamingProxy(StreamSocket clientStreamSocket, StreamSocket destinationStreamSocket)
        {
            clientSocket = clientStreamSocket;
            destinationSocket = destinationStreamSocket;

            clientTokenSource = new CancellationTokenSource();
            destinationTokenSource = new CancellationTokenSource();
        }



    }
}
