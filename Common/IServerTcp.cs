using System;

namespace MyTCPSocketLibrary.Service.ServerTcp
{
    public interface IServerTcp
    {
        event ServerTcpSocketEventHandler.Listening delListening;
        event ServerTcpSocketEventHandler.ClientConnected delClientConnected;
        event ServerTcpSocketEventHandler.ClientDisconnected delClientDisconnected;
        event ServerTcpSocketEventHandler.ReceivedMsgFromClient delReceivedMsgFromClient;
        event ServerTcpSocketEventHandler.ErrorOccur delErrorOccur;
        event ServerTcpSocketEventHandler.MsgReplied delMsgReplied;
        event ServerTcpSocketEventHandler.DebuggingInfo delDebuggingInfo;
    }

    public class ServerTcpSocketEventHandler
    {
        public delegate void Listening(ServerTcpEventArgs args);
        public delegate void ClientConnected(ServerTcpEventArgs args);
        public delegate void ClientDisconnected(ServerTcpEventArgs args);
        public delegate void ReceivedMsgFromClient(ServerTcpEventArgs args);
        public delegate void ErrorOccur(ServerTcpErrorArgs args);
        public delegate void MsgReplied(ServerTcpEventArgs args);
        public delegate void DebuggingInfo(ServerTcpEventArgs args);
    }

    public class ServerTcpEventArgs : EventArgs
    {
        public string ResultMsg { get; private set; }
        public bool Is_ClientConnected { get; private set; }
        public int ClientRefNumber { get; private set; }
        public ServerTcpEventArgs(string msg, bool isClientConnected, int clientRefNumber)
        {
            ResultMsg = msg;
            Is_ClientConnected = isClientConnected;
            ClientRefNumber = clientRefNumber;
        }
    }

    public class ServerTcpCustomException : Exception
    {
        public Exception ExceptionTrace { get; private set; }
        public string ResultMsg { get; private set; }
        public ServerTcpCustomException(string msg, Exception ex)
        {
            ResultMsg = msg;
            ExceptionTrace = ex;
        }
    }

    public class ServerTcpErrorArgs : EventArgs
    {
        public string ResultMsg { get; private set; }
        public ServerTcpErrorArgs(string msg)
        {
            ResultMsg = msg;
        }
    }
}