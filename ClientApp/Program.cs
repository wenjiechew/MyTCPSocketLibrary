using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Program
    {
        private static string myMessage = string.Empty;
        private static TcpClient client = new TcpClient();
        private static IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, 3000);

        static void Main(string[] args)
        {
            Console.Write("Connecting to Server...");
            while (!client.Connected)
            {
                try
                {
                    client.Connect(iPEndPoint);
                }
                catch (SocketException ex)
                {
                    Console.Clear();
                    Console.Write("Error: {0}", ex);
                }
                
            }            
            Console.WriteLine("Connected to Server");
            SendMessage();
        }

        private static void SendMessage()
        {
            NetworkStream clientStream = client.GetStream();
            while (true)
            {
                Console.WriteLine("Write to Server: ");
                //Writing to Server
                ASCIIEncoding encoder = new ASCIIEncoding();
                string msg = Console.ReadLine();
                byte[] buffer = encoder.GetBytes(msg);

                clientStream.Write(buffer,0,buffer.Length);
                clientStream.Flush();


                //Receiving from Server
                byte[] data = new byte[4096];

                //string to store the respones ASCII representation
                String responseData = string.Empty;
                Int32 bytes = clientStream.Read(data, 0, data.Length);
                responseData = encoder.GetString(data, 0, bytes);

                Console.WriteLine("Incoming from Server; ", responseData);
            }



        }
    }
}
