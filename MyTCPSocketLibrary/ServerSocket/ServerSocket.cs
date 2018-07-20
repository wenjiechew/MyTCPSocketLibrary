using MyTCPSocketLibrary.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace MyTCPSocketLibrary.ServerSocket
{
    public class ServerSocket : Connectivity
    {
        private static Settings settings;
        private static TcpListener _serverSocket;
        private static TcpClient _tcpClient;

        public TcpListener TcpServer => _serverSocket;

        public ServerSocket(IPEndPoint iPEndPoint)
        {
            settings = new Settings(iPEndPoint);
        }

        public virtual void StartListening()
        {
            _serverSocket = new TcpListener(settings.IPAddress, settings.Port);
            _serverSocket.Start();
        }
    }
}
