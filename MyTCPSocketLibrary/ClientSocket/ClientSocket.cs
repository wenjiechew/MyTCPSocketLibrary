using MyTCPSocketLibrary.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyTCPSocketLibrary.ClientSocket
{
    public abstract class ClientSocket
    {
        #region Settings
        private readonly Settings Settings;
        #endregion

        #region Variables
        private Socket _clientSocket;
        #endregion

        public ClientSocket(IPEndPoint iPEndPoint)
        {
            Settings = new Settings(iPEndPoint);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public bool IsConnected => _clientSocket == null ? false : _clientSocket.Connected;
        public EndPoint LocalEndPoint => _clientSocket?.LocalEndPoint;
        public EndPoint RemoteEndPoint => _clientSocket?.RemoteEndPoint;
        public IPAddress IPAddress => Settings.IPEndPoint.Address;
        public int Port => Settings.IPEndPoint.Port;

        /// <summary>
        /// Connecting to Server Socket
        /// </summary>
        public virtual void Connect()
        {


        }
    }
}
