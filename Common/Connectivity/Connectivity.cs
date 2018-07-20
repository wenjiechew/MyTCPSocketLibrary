using System.Net.Sockets;
using System.Text;

namespace MyTCPSocketLibrary.Common
{
    public abstract class Connectivity
    {
        public virtual void Encryption() { }
        public virtual void Decryption() { }
        public virtual string Decode(byte[] receiveMsg) => Encoding.ASCII.GetString(receiveMsg, 0, receiveMsg.Length);
        public virtual byte[] Encode(string text) => Encoding.ASCII.GetBytes(text);

        public virtual void Send(string toSend, NetworkStream stream)
        {
            stream.Write(Encode(toSend.Trim()), 0, toSend.Trim().Length);
        }
        public virtual string Read(byte[] readMessage, NetworkStream stream)
        {
            stream.Read(readMessage, 0, readMessage.Length);
            return Decode(readMessage);
        }

    }
}