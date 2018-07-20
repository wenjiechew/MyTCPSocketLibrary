
using MyTCPSocketLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConnection
{
    class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static List<TcpClient> _tcpClients = new List<TcpClient>();
        private static IPEndPoint iPEndPoint = new
                IPEndPoint(IPAddress.Any, 13000);

        private static TcpListener _serverSocket = new TcpListener(iPEndPoint);



        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }
        private static void SetupServer()
        {
            Console.Write("Settings up server...");
            _serverSocket.Start();
            _serverSocket.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

        private static void AcceptCallBack(IAsyncResult asyncResult)
        {
            TcpClient tcpClient = _serverSocket.EndAcceptTcpClient(asyncResult);
            _tcpClients.Add(tcpClient);


            Console.Write("Connected to Client");
            tcpClient.GetStream().BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(ReceivedCallback), tcpClient);
            _serverSocket.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

        private static void ReceivedCallback(IAsyncResult asyncResult)
        {
            TcpClient tcpClient = (TcpClient)asyncResult.AsyncState;
            int received = tcpClient.GetStream().EndRead(asyncResult);

            byte[] dataBuf = new byte[received];

            Buffer.BlockCopy(_buffer, 0, dataBuf, 0, received);
            string message = Decode(_buffer);
            message.Trim();
            Console.WriteLine("Text received: " + message);

            string res = string.Empty;
            if(message.ToLower() != "get time")
            {
                res = "Invalid Request";
            }
            else
            {
                res = DateTime.Now.ToLongDateString();
            }

            byte[] data = Encoding.ASCII.GetBytes(res);
            tcpClient.GetStream().BeginWrite(data, 0, data.Length, new AsyncCallback(SendCallback), tcpClient);
        }

        private static void SendCallback(IAsyncResult asyncResult)
        {
            TcpClient tcpClient = (TcpClient)asyncResult.AsyncState;
            tcpClient.GetStream().EndWrite(asyncResult);


        }

        private static string Decode(byte[] buffer) => Encoding.ASCII.GetString(buffer);
    }
}
