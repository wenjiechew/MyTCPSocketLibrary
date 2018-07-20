using MyTCPSocketLibrary.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace MyTCPSocketLibrary.ClientSocket
{
    public class ClientSocket : Connectivity
    {
        #region Variables
        #region Private Variables
        private static Settings settings;
        private static TcpClient _clientSocket;

        #endregion
        public TcpClient Client => _clientSocket;
        public NetworkStream Stream => Client.GetStream();
        public Settings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
            }
        }
        public bool IsConnected => _clientSocket == null ? false : _clientSocket.Connected;

        #endregion
        public ClientSocket() { }

        public ClientSocket(IPEndPoint iPEndPoint)
        {
            settings = new Settings(iPEndPoint);
        }

        /// <summary>
        /// Connect to Server Socket
        /// </summary>
        public virtual void Connect()
        {
            _clientSocket = new TcpClient();
            int attempts = 0;

            while (!IsConnected)
            {
                try
                {
                    attempts++;
                    Client.Connect(settings.IPAddress, settings.Port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.Write("Connected!!");
        }

        public virtual void Disconnect()
        {
            try
            {
                Stream.Close();
                if (IsConnected)
                {
                    Client.LingerState = settings.LingerOption;
                    Client.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
