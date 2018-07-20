using System.Net;
using System.Net.Sockets;

namespace MyTCPSocketLibrary
{
    public class Settings
    {
        public Settings(IPEndPoint iPEndPoint)
        {
            IPEndPoint = iPEndPoint;
        }

        /// <summary>
        /// Set the IP EndPoint Connection 
        /// (Address, Port)
        /// </summary>
        private IPEndPoint IPEndPoint { get; set; }

        public LingerOption LingerOption
        {
            get
            {
                return LingerOption ?? new LingerOption(false, 0);
            }
            set
            {
                LingerOption = value;
            }
        }
        public IPAddress IPAddress => IPEndPoint.Address;
        public int Port => IPEndPoint.Port;

    }
}
