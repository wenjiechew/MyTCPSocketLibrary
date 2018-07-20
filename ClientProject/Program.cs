using MyTCPSocketLibrary.ClientSocket;
using MyTCPSocketLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientProject
{
    class Program
    {
        private static IPEndPoint iPEndPoint =
            new IPEndPoint(IPAddress.Loopback, 13000);
        private static ClientSocket _client = new ClientSocket(iPEndPoint);

        static void Main(string[] args)
        {
            _client.Connect();
            SendLoop();
            Console.ReadLine();


            //client.Connect();


            //    string text = Console.ReadLine();
            //    client.Send(text, client.Stream);

            //    if (text == "exit") break;
            //}
            //client.Disconnect();
        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.WriteLine("Enter Request: ");
                string req = Console.ReadLine();
                _client.Send(req, _client.Stream);

                byte[] receiveBuf = new byte[1024];
                int msg = _client.Stream.Read(receiveBuf, 0, receiveBuf.Length);


                byte[] dataBuf = new byte[msg];
                Buffer.BlockCopy(receiveBuf, 0, dataBuf, 0, msg);
                Console.WriteLine("Received: " + _client.Decode(dataBuf));

            }
        }
    }
}
