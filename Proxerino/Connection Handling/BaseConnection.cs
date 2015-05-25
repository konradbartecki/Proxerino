using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Proxerino.Socks_v5;
using Stage = Proxerino.Socks_v5.StageEnum.Stage;
using Buffer = Windows.Storage.Streams.Buffer;
using Exceptions = Proxerino.Proxy.Exceptions;

namespace Proxerino.Connection_Handling
{
    public class BaseConnection : IDisposable
    {
        protected ConnectionPair ParentConnectionPair;

        protected StreamSocket Socket;
        protected byte[] Bytes;
        protected Stage stage;
        private string _type;

        public BaseConnection(StreamSocket streamSocket, ConnectionPair connectionPair, string Type)
        {
            if (streamSocket == null) 
                throw new ArgumentNullException("streamSocket");
            if (connectionPair == null) 
                throw new ArgumentNullException("connectionPair");

            Socket = streamSocket;
            ParentConnectionPair = connectionPair;
            _type = Type;
        }

        protected internal async Task SendData(byte[] bytesToSend)
        {
            if (bytesToSend.Length == 0)
                throw new ArgumentNullException("bytesToSend");          
            try
            {
                var buffer = bytesToSend.AsBuffer();
                Debug.WriteLine("Sending buffer of length {0}", buffer.Length);
                await Socket.OutputStream.WriteAsync(buffer).AsTask(ParentConnectionPair.WriterTokenSource.Token);
                await Socket.OutputStream.FlushAsync().AsTask(ParentConnectionPair.WriterTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Writer is cancelling");
                throw;
            }
            catch (Exception exception)
            {
                if (_type == "incomingConnection")
                {
                    Debug.WriteLine("Client has dropped connection while receiving data of {0} bytes", bytesToSend.Length);
                    stage = Stage.ClientDisconnected;
                }
                if (_type == "outgoingConnection")
                {
                    Debug.WriteLine("Exit node has dropped connection");
                    stage = Stage.ExitNodeDisconnected;
                }
                //if (Exceptions.CheckForRecoverableSocketsExceptions(exception))
                //{
                    
                //}
                //StopGracefully((uint) bytesToSend.Length);
                //ParentConnectionPair.WriterTokenSource.Cancel();
                //if(!ParentConnectionPair.ReaderTokenSource.IsCancellationRequested)
                
                throw exception;
            }
        }

        protected internal async Task Listen()
        {
            var buffer = new Windows.Storage.Streams.Buffer(Config.Listener.BufferSize);
            try
            {                
                await Socket.InputStream.ReadAsync(buffer,
                        Config.Listener.BufferSize,
                        InputStreamOptions.Partial)
                        .AsTask(ParentConnectionPair.ReaderTokenSource.Token);

                    //Reader.LoadAsync(Config.Listener.BufferSize)
                    //    .AsTask(ParentConnectionPair.cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Task canceled");
            }
            catch (Exception exception)
            {
                if (_type == "incomingConnection")
                {
                    if (buffer.Length > 0)
                    {
                        Debug.WriteLine("There's still data from client but I don't know how to handle it");
                    }
                }
                if (_type == "outgoingConnection")
                {
                    if (buffer.Length > 0)
                    {
                        stage = Stage.ExitNodeDisconnected;
                        CustomListen();
                    }
                }

                if (Exceptions.CheckForRecoverableSocketsExceptions(exception))
                {
                    //StopGracefully(buffer.Length);
                    return;
                }
                throw;
            }
                
            //read complete message
            if (buffer.Length > 0)
            {
                Bytes = new byte[buffer.Length];
                Bytes = buffer.ToArray();
                CustomListen();
            }
            else
            {
                Debug.WriteLine("Disconnected");
            }

            //uint byteCount = Reader.UnconsumedBufferLength;
            //if (Reader.UnconsumedBufferLength > 0)
            //{
            //    Bytes = new byte[byteCount];
            //    Reader.ReadBytes(Bytes);
            //    CustomListen();
            //}

        }

        //private void StopGracefully(uint bytesLeft)
        //{
        //    if (bytesLeft > 0)
        //    {
        //        Debug.WriteLine("There's still {0} bytes of data left to be sent", bytesLeft);
        //        throw new Exception("there is data to be send");
        //    }
        //    ParentConnectionPair.cancellationTokenSource.Cancel();
        //}

        protected virtual Task CustomListen()
        {
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                Socket.Dispose();
                //Reader.Dispose();
                //Writer.Dispose();
                ParentConnectionPair.Dispose();
            }
        }
    }
}
