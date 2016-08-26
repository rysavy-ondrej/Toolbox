//
//  UdpSocket.cs
//
//  Author:
//       Ondrej Rysavy <rysavy@fit.vutbr.cz>
//
//  Copyright (c) 2016 (c) Brno University of Technology
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace System.Net.Sockets
{

    /// <summary>
    ///  Provides User Datagram Protocol (UDP) network services. This class is a direct implementation over Socket class. 
    ///  It provides asynchronous operations.
    /// </summary>
	public sealed class UdpSocket : IDisposable
	{
		Socket m_socket;
        /// <summary>
        /// Initializes a new instance of the UdpClient class.
        /// </summary>
        /// <param name="af">One of the AddressFamily values that specifies the addressing scheme of the socket. </param>
        /// <remarks>
        /// The family parameter determines whether the listener uses an IP version 4 address (IPv4) or an IP version 6 (IPv6) address. 
        /// To use an IPv4 address, pass the InterNetwork value. To use an IPv6 address, pass the InterNetworkV6 value. 
        /// Passing any other value will cause the method to throw an ArgumentException.
        /// </remarks>
		public UdpSocket (AddressFamily af)
		{
			m_socket = new Socket (af, SocketType.Dgram, ProtocolType.Udp);
		}

        /// <summary>
        /// Binds the current socket to the specified local endpoint. 
        /// </summary>
        /// <param name="localEP">An IPEndPoint that respresents the local endpoint to which you bind the UDP connection.</param>
		internal void Bind (EndPoint localEP)
		{
			m_socket.Bind (localEP);
		}

		/// <summary>
		/// Receives the specified number of bytes of data into the specified location of the data buffer, 
		/// and stores the endpoint.
		/// </summary>
		/// <returns>The number of bytes received.</returns>
		/// <param name="buffer">An array of type Byte that is the storage location for received data.</param>
		/// <param name="offset">The position in the buffer parameter to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="remoteEP">An EndPoint, passed by reference, that represents the remote server.</param>
		public Task<int> RecvFromAsync (byte [] buffer, int offset, int size, ref EndPoint remoteEP)
		{
			TaskCompletionSource<int> tcs = new TaskCompletionSource<int> ();
			var saea = new SocketAsyncEventArgs ();
			saea.SetBuffer (buffer, offset, size);
			saea.RemoteEndPoint = remoteEP;
			saea.Completed += (object sender, SocketAsyncEventArgs args) => { tcs.SetResult (args.BytesTransferred); };

			if (!m_socket.ReceiveFromAsync (saea)) {
				tcs.SetResult (saea.BytesTransferred);
			}
			return tcs.Task;
		}

		/// <summary>
		/// Sends the specified number of bytes of data to the specified endpoint, starting at the specified location in the buffer.
		/// </summary>
		/// <returns>The number of bytes sent.</returns>
		/// <param name="buffer">An array of type Byte that contains the data to be sent.</param>
		/// <param name="offset">The position in the data buffer at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="remoteEP">The EndPoint that represents the destination location for the data.</param>
		internal Task<int> SendToAsync (byte [] buffer, int offset, int size, EndPoint remoteEP)
		{
			TaskCompletionSource<int> tcs = new TaskCompletionSource<int> ();
			var saea = new SocketAsyncEventArgs ();
			saea.SetBuffer (buffer, offset, size);
			saea.RemoteEndPoint = remoteEP;
			saea.Completed += (object sender, SocketAsyncEventArgs args) => { tcs.SetResult (args.BytesTransferred); };

			if (!m_socket.SendToAsync (saea)) {
				tcs.SetResult (saea.BytesTransferred);
			}
			return tcs.Task;
		}


        /// <summary>
        /// Closes the UDP connection.
        /// </summary>
		public void Close ()
		{
			m_socket.Shutdown (SocketShutdown.Both);
		}
		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		void Dispose (bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					m_socket.Dispose ();
				}
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~UdpSocket() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose ()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose (true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}

