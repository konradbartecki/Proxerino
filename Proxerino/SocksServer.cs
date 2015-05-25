using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Proxerino.Connection_Handling;
using Proxerino.Proxy;

namespace Proxerino
{
    /// <summary>
    /// Socks v5 server implementation class
    /// </summary>
    public class SocksServer
    {
        private StreamSocketListener _listener;
        private SocketQualityOfService _qualityOfService;
        private string _serviceName;
        private string _listeningAddress;
        public ConnectionManager ConnectionManager;

        private int ConnectionID;

        /// <summary>
        /// Starts an asynchronous StreamListenerService
        /// </summary>
        /// <param name="hostname">Hostname or IP Address</param>
        /// <param name="port">Port or service name</param>
        public async void Start(HostName hostname, string port)
        {
            //If hostname is not empty then try to obtain our current IP
            if (string.IsNullOrWhiteSpace(hostname.CanonicalName))
                throw new ArgumentNullException("Hostname");
            if (string.IsNullOrWhiteSpace(port)) 
                throw new ArgumentNullException("Port");
            
            //Proceed only if we did not create listener earlier
            if (_listener == null)
            {
                try
                {
                    //Create new listener, OnConnection event and start listening
                    if(ConnectionManager == null)
                        ConnectionManager = new ConnectionManager();
                    _listener = new StreamSocketListener();
                    _listener.ConnectionReceived += OnConnection;
                    await _listener.BindEndpointAsync(hostname, port);
                }
                catch (Exception exception)
                {
                    //TODO: Message Log: SocketError.GetStatus(exception.HResult).ToString();
                    
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                }
            }
            else
            {
                //TODO: Message Log: "Server already running"
            }
        }

        public void Start(string port)
        {

            
            //TODO: Message Log: Using port only. Trying to determine IP Address
            //TODO: Message Log: Starting server at IP:PORT
            
            Start(Utilities.CurrentHostName(), port);
            Debug.WriteLine("Started listening on {0}:{1}", Utilities.CurrentHostName().CanonicalName, _listener.Information.LocalPort);
        }

        public void Stop()
        {
            _listener.Dispose();
        }

        /// <summary>
        /// Invoked once a connection is accepted by StreamSocketListener.
        /// </summary>
        /// <param name="sender">The listener that accepted the connection.</param>
        /// <param name="args">Parameters associated with the accepted connection.</param>
        private void OnConnection(
            StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Task connectionTask = new Task(() =>
            {
                ConnectionManager.AddConnection(args.Socket);
            });
            connectionTask.Start();
        }
        
    }
}
