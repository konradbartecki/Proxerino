using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Proxerino.Socks_v5;
using Proxerino.Types;
using Proxerino.Proxy;
using Buffer = Windows.Storage.Streams.Buffer;
using Exceptions = Proxerino.Proxy.Exceptions;

namespace Proxerino
{
	public static class SocketExtension
	{
		public static Task<bool> SendDataTask(this StreamSocket socket, byte[] bytes, CancellationToken token)
		{
			if (socket == null) throw new ArgumentNullException("socket");
			if (bytes == null) throw new ArgumentNullException("bytes");

			IBuffer buffer = bytes.AsBuffer();
			return SendDataTask(socket, buffer, token);
		}

		public static async Task<bool> SendDataTask(this StreamSocket socket, IBuffer dataBuffer, CancellationToken token)
		{
			if (socket == null) throw new ArgumentNullException("socket");
			if (dataBuffer == null) throw new ArgumentNullException("dataBuffer");
			if (dataBuffer.Length <= 0)
				throw new ArgumentNullException("dataBuffer");

			try
			{
				await socket.OutputStream.WriteAsync(dataBuffer).AsTask(token);
				bool result = await socket.OutputStream.FlushAsync().AsTask(token);
				return result;
			}
			catch (Exception exception)
			{
				Exceptions.CheckForRecoverableSocketsExceptions(exception);
				Debug.WriteLine("Exception while sending data: {0}", exception.Message);
				return false;
			}
		}

		public static async Task<IBuffer> GetDataTask(this StreamSocket socket, CancellationToken token)
		{
			if (socket == null) throw new ArgumentNullException("socket");
			IBuffer buffer = new Buffer(Config.Listener.BufferSize);
			try
			{

				buffer = await socket.InputStream.ReadAsync(buffer,
					Config.Listener.BufferSize,
					InputStreamOptions.Partial)
					.AsTask(token);
				return buffer;
			}
            //catch (TaskCanceledException)
            //{
            //    Debug.WriteLine("Task canceled: Get data from socket");
            //    return buffer;
            //}
            //catch (ObjectDisposedException)
            //{
            //    return null;
            //}
			catch (Exception exception)
			{      
				if (buffer.Length > 0)
				{
					Debug.WriteLine("There is still data from socket to be handled");
					return buffer;
				}
                if (Exceptions.CheckForRecoverableSocketsExceptions(exception))
                    return null;
                throw;
			}
		}

		public static async Task ConnectTask(this StreamSocket socket, ClientsRequest clientsRequest,
			CancellationToken token)
		{
			IPv4 destinationIPv4 = new IPv4(clientsRequest.GetDestinationAddressBytes);
					Port destinationPort = clientsRequest.DestinationPort;
					if (Config.Listener.KeepConnectionsAlive)
						socket.Control.KeepAlive = true;
					else
					    socket.Control.KeepAlive = false;

			try
			{
				if (Config.Listener.UseCellularDataOnly)
				{
					NetworkAdapter wanNetworkAdapter = Utilities.GetWanNetworkAdapter();
					if (wanNetworkAdapter != null)
						await
							socket.ConnectAsync(destinationIPv4.ToHostName(), destinationPort.ToString(),
								SocketProtectionLevel.PlainSocket,
								wanNetworkAdapter).AsTask(token);
				}
				else
				{
					await socket.ConnectAsync(destinationIPv4.ToHostName(), destinationPort.ToString()).AsTask(token);
				}
			}
			catch (Exception e)
			{
				Exceptions.CheckForRecoverableSocketsExceptions(e);
			}
		}
	}
}
