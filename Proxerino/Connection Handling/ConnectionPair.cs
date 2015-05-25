using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Text;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Proxerino.Socks_v5;
using Proxerino.Types;
using Stage = Proxerino.Socks_v5.StageEnum.Stage;

namespace Proxerino.Connection_Handling
{
	/// <summary>
	/// Creates new pair of incoming-outgoing connection
	/// </summary>
	public class ConnectionPair : IDisposable
	{
		protected IncomingConnection _incomingConnection;
		protected OutgoingConnection _outgoingConnection;
		protected internal CancellationTokenSource ReaderTokenSource = new CancellationTokenSource();
		protected internal CancellationTokenSource WriterTokenSource = new CancellationTokenSource();
		protected internal Stage stage;

		/// <summary>
		/// Creates and listens at socket from new received connection
		/// </summary>
		/// <param name="socket">Socket from ConnectionReceived event from socket listener</param>
		public ConnectionPair(StreamSocket socket)
		{
			ClientsHostname = socket.Information.RemoteHostName;
			_incomingConnection = new IncomingConnection(socket, this);
		}

		public void Start()
		{
			stage = Stage.ClientConnected;
			_incomingConnection.Listen();
		}

		public HostName ClientsHostname { get; private set; }

		protected async Task SendDataToClient(byte[] dataBytes)
		{
			await _incomingConnection.SendData(dataBytes);
		}

		protected async Task SendDataToDestination(byte[] dataBytes)
		{
			await _outgoingConnection.SendData(dataBytes);
		}

		//public void Disconnect()
		//{
		//    cancellationTokenSource.Cancel();
		//    if (_outgoingConnection != null)
		//    {
		//        _outgoingConnection.IsConnected = false;
		//    }
				
		//}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_incomingConnection.Dispose();
				_outgoingConnection.Dispose();
				ReaderTokenSource.Dispose();
				WriterTokenSource.Dispose();
			}

		}

		/// <summary>
		/// Class that will handle client's data.
		/// </summary>
		protected class IncomingConnection : BaseConnection
		{
			public IncomingConnection(StreamSocket streamSocket, ConnectionPair connectionPair)
				: base(streamSocket, connectionPair, "incomingConnection")
			{
				if (streamSocket == null) throw new ArgumentNullException("streamSocket");
				if (connectionPair == null) throw new ArgumentNullException("connectionPair");
				//This constructor will send streamSocket to base class and set reference to outer class
			}

			protected override async Task CustomListen()
			{
				if (stage == Stage.StreamingMode)
				{
					await ParentConnectionPair.SendDataToDestination(Bytes);
					await Listen();
				}
				else
				{
					if (stage == Stage.ClientConnected)
					{
						//Select method for newly connected clients
						//Send received bytes to SelectBestMethod for evaluation and choosing the best method
						//then build new reply message and send it through writer stream
						byte bestMethod = Socksv5.SelectBestMethod.FromAvailableMethods(Bytes);
						MethodReply methodReply = new MethodReply(bestMethod);
						await SendData(methodReply.ToBytes());
					    stage = Stage.SendBestMethod;
						Listen();
					}
					else if (stage == Stage.SendBestMethod)
					{
						//We will await for client's request here
                        //That is a minimal length for proper Socks v5 client's request
					    bool errorsEncountered = Bytes.Length <= 9;
					    
                        if(Config.Listener.AllowOnlySocksV5Connections)
                            if (Bytes[0] != Map.SOCKS_VERSION)
                                errorsEncountered = true;

						if (!errorsEncountered)
						{
                            ClientsRequest clientsRequest = new ClientsRequest(Bytes);
							Task newOutgoingConnectionTask = new Task(() =>
							{
								ParentConnectionPair._outgoingConnection =
									new OutgoingConnection(ParentConnectionPair, clientsRequest);
								//This one below should not be awaited as it will cause deadlock
								ParentConnectionPair._outgoingConnection.Connect();

							}, ParentConnectionPair.ReaderTokenSource.Token);
							newOutgoingConnectionTask.Start();
							stage = Stage.StreamingMode;
							Listen();
						}
					}
				}                
			}

		}


			/// <summary>
			/// Class that will handle outgoing aka destination data
			/// </summary>
			protected class OutgoingConnection : BaseConnection
			{
				public bool IsConnected;
				private ClientsRequest _clientsRequest;

				public OutgoingConnection(ConnectionPair connectionPair, ClientsRequest clientsRequest)
					: base(new StreamSocket(), connectionPair, "outgoingConnection")
				{
					if (connectionPair == null) throw new ArgumentNullException("connectionPair");
					if (clientsRequest == null) throw new ArgumentNullException("clientsRequest");
					_clientsRequest = clientsRequest;
				}

				private async Task SendDataToClient(byte[] dataBytes)
				{
					await ParentConnectionPair.SendDataToClient(dataBytes);
				}

				public async Task Connect()
				{
					IPv4 destinationIPv4 = new IPv4(_clientsRequest.GetDestinationAddressBytes);
					Port destinationPort = _clientsRequest.DestinationPort;
					if (Config.Listener.KeepConnectionsAlive)
						Socket.Control.KeepAlive = true;

					try
					{
						if (Config.Listener.UseCellularDataOnly)
						{
							NetworkAdapter wanNetworkAdapter = Utilities.GetWanNetworkAdapter();
							if (wanNetworkAdapter != null)
								await
									Socket.ConnectAsync(destinationIPv4.ToHostName(), destinationPort.ToString(),
										SocketProtectionLevel.PlainSocket,
										wanNetworkAdapter);
							else
							{
                                //ParentConnectionPair.Disconnect();
								return;
							}
						}
						else
						{
							await Socket.ConnectAsync(destinationIPv4.ToHostName(), destinationPort.ToString());
						}

                        //IsConnected = true;
						//We don't want to await listen() as it will lock the server here.
                        //IsInStreamingMode = true;
						Listen();
						await SendDataToClient(new GeneralReply(Map.Reply.SUCCEEDED, Socket).ToBytes());
					}
					catch (Exception exception)
					{
						SendDataToClient(Exceptions.CatchSocketExceptions(exception).ToBytes());
                        //ParentConnectionPair.Disconnect();
					}
				}

				protected override async Task CustomListen()
				{
					await SendDataToClient(Bytes);
					Listen();
				}
			}
		}
	}
