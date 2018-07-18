using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyTCPSocketLibrary.Variable
{
    public class Settings
    {
        public Settings(IPEndPoint iPEndPoint)
        {
            IPEndPoint = iPEndPoint;
        }

        /// <summary>
        /// Set the IP End Point of the Connection
        /// </summary>
        public IPEndPoint IPEndPoint { get; private set; }

    }
}
