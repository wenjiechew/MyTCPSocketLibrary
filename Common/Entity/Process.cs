using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common.Entity
{
    public class Process
    {
        public Process()
        {

        }
        public string iD { get; set; }
        public string status { get; set; }
        public TcpClient tcpClient { get; set; }
        public DateTime dateTime { get; set; }
    }
}
