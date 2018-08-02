using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerApp
{
    public class Program
    {
        private static TcpListener tcpListener;
        private static Thread listenThread;
        private static List<TcpClient> ClientList = new List<TcpClient>();


        static void Main(string[] args)
        {
            Server();
        }

        private static void Server()
        {
            Console.Write("Setting up Server...");
            tcpListener = new TcpListener(IPAddress.Loopback, 3000);
            listenThread = new Thread(new ThreadStart(ListenForClient));
            listenThread.Start();
        }

        private static void ListenForClient()
        {
            Console.Write("Waiting for Clients");
            tcpListener.Start();

            while (true)
            {
                //blocks until client has connected to server.
                TcpClient client = tcpListener.AcceptTcpClient();
                ClientList.Add(client);

                Console.WriteLine("Client Connected: {0}", ClientList.Count().ToString());
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private static void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int byteRead;
            Console.WriteLine("Reading Msg...");
            while (true)
            {
                byteRead = 0;

                try
                {
                    //blocks until a client sends a message
                    byteRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    Console.WriteLine("Error Reading Message");
                    break;
                }

                if(byteRead == 0)
                {
                    ClientList.Remove(tcpClient);
                    Console.WriteLine("No of Clients Conneted: {0}", ClientList.Count().ToString());
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                //Convert the Bytes received to a string and display
                string msg = encoder.GetString(message, 0, byteRead);
                Console.WriteLine("No of Clients Connected: {0}", ClientList.Count().ToString());
                Console.WriteLine("Message Incoming: {0}", msg);

                //Echo the msg back to client
                Echo(msg, encoder, clientStream);
            }
        }

        private static void Echo(string msg, ASCIIEncoding encoder, NetworkStream clientStream)
        {
            //Echo Back to Client
            byte[] buffer = encoder.GetBytes(msg);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }
    }
}
