using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Proxerino.Socksv5;
using Proxerino.Socks_v5;
using Buffer = Windows.Storage.Streams.Buffer;

namespace Proxerino.Connection_Handling
{
    public class NewConnection
    {
        private StreamSocket clientSocket;
        private StreamSocket outgoingSocket;

        private CancellationTokenSource tokenSource;

        public HostName ClientsHostname { get; private set; }

        /// <summary>
        /// Contains IsAlive state of both connections
        /// so if both [0] and [1] == dead
        /// we can close the connection
        /// </summary>
        private byte[] byteLock = new byte[2];

        public NewConnection(StreamSocket socket)
        {
            ClientsHostname = socket.Information.RemoteHostName;
            tokenSource = new CancellationTokenSource();
            clientSocket = socket;      
            //TODO: socket.InputStream.AsStreamForRead(.)
        }

        public async Task Handle()
        {
            var methods = await clientSocket.GetDataTask(tokenSource.Token);
            if (methods == null || methods.Length < 3)
                return;
            byte[] selectedMethodMessage = new MethodReply(SelectBestMethod.FromAvailableMethods(methods.ToArray())).ToBytes();
            await clientSocket.SendDataTask(selectedMethodMessage, tokenSource.Token);
            var destinationData = await clientSocket.GetDataTask(tokenSource.Token);
            if (destinationData == null || destinationData.Length < 8)
                return;
            ClientsRequest clientsRequestData = new ClientsRequest(destinationData.ToArray());
            outgoingSocket = new StreamSocket();
            await outgoingSocket.ConnectTask(clientsRequestData, tokenSource.Token);
            await clientSocket.SendDataTask(new GeneralReply(Map.Reply.SUCCEEDED, outgoingSocket).ToBytes(), tokenSource.Token);
            StartStreaming();
        }

        private void StartStreaming()
        {
            Parallel.Invoke(() => clienttoDestinationLoopTask(), () => destinationToClientLoopTask());
        }

        /// <summary>
        /// This is some kind of heartbeat for connections so 
        /// we will close connection only if both of them have disconnected
        /// </summary>
        /// <param name="i">Connection ID</param>
        /// <param name="b">IsAlive? 0 = Alive, 1 = Dead</param>
        private void CheckLock(int i, byte b)
        {
            byteLock[i] = b;
            if (byteLock[0] == 1 && byteLock[1] == 1)
            {
                Debug.WriteLine("Both failed - disconnecting");
                clientSocket.Dispose();
                outgoingSocket.Dispose();
            }
        }

        private async Task clienttoDestinationLoopTask()
        {
            bool ConnectionIsActive = true;
            while (ConnectionIsActive)
            {
                ConnectionIsActive = await SendDataWrapperTask(clientSocket,
                                                     outgoingSocket,
                                                     tokenSource.Token);
                CheckLock(0,0);
            }
            Debug.WriteLine("Client to destination failed");
            CheckLock(0, 1);
        }

        private async Task destinationToClientLoopTask()
        {
            bool ConnectionIsActive = true;
            while (ConnectionIsActive)
            {
                ConnectionIsActive = await SendDataWrapperTask(outgoingSocket,
                                     clientSocket,
                                     tokenSource.Token);
                CheckLock(1, 0);
            }
            Debug.WriteLine("Destination to Client failed");
            CheckLock(1, 1);
        }

        private async Task<bool> SendDataWrapperTask(StreamSocket sourceSocket, StreamSocket destinationSocket, CancellationToken cancellationToken)
        {
            var buffer = await sourceSocket.GetDataTask(cancellationToken);
                
            if (buffer == null || buffer.Length == 0)
                return false;
            bool result = await destinationSocket.SendDataTask(buffer, cancellationToken);
            return result;
        }
    }
}
