using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyTCPSocketLibrary.Service.ServerTcp
{
    public class ServerTcp : IServerTcp
    {
        private TcpListener _Server;
        private AsyncCallback nWorkerCallBack;
        private IPEndPoint iPEndPoint;
        private ArrayList m_tcpWorkerList = ArrayList.Synchronized(new ArrayList());

        private string welcomeMsg = String.Empty;

        private class SocketPacket
        {
            private TcpClient currentTcpClient;
            private int _clientNumber;
            private byte[] dataBuffer = new byte[8192];

            public TcpClient CurrentClient => currentTcpClient;
            public int ClientNo => _clientNumber;
            public byte[] DataBuffer => dataBuffer;

            public SocketPacket(TcpClient client, int clientNumber)
            {
                this.currentTcpClient = client;
                this._clientNumber = clientNumber;
            }
        }
        public int ClientConnectedCount => m_tcpWorkerList.Count;

        public ServerTcp(IPEndPoint iPEndPoint, String welcomeMsg = "")
        {
            this.iPEndPoint = iPEndPoint;
            this.welcomeMsg = welcomeMsg;
            _Server = new TcpListener(this.iPEndPoint);            
        }
        public void StopServer()
        {
            if (_Server != null)
            {
                _Server.Server.Close();

                TcpClient client = null;
                for (int i = 0; i < m_tcpWorkerList.Count; i++)
                {
                    client = (TcpClient)m_tcpWorkerList[i];
                    if (client != null)
                    {
                        client.Close();
                        client = null;
                    }
                }
            }
        }
        public void HostServer()
        {
            if(_Server != null)
            {
                try
                {
                    if(_Server.LocalEndpoint == null)
                    {
                        _Server.BeginAcceptTcpClient(new AsyncCallback(CallBack_ClientConnected), null);
                    }
                    else
                    {
                        TcpClient client = null;
                        for(int i = 0; i< m_tcpWorkerList.Count; i++)
                        {
                            client = (TcpClient)m_tcpWorkerList[i];
                            if (client != null)
                            {
                                OnClientConnected(String.Format("Continue Communicate with Connected Client: {0}|{1}",
                                    i.ToString(), 
                                    client.Client.RemoteEndPoint,ToString()));
                            }
                        }                        
                    }                    
                }
                catch (Exception ex)
                {
                    OnErrorOccured("(HostServer): ", ex);
                }
            }
        }
        private void CallBack_ClientConnected(IAsyncResult AR)
        {
            try
            {
                TcpClient workerClient = _Server.EndAcceptTcpClient(AR);
                m_tcpWorkerList.Add(workerClient);

                OnClientConnected(String.Format("Continue Communicate with Connected Client: {0}|{1}",
                                    m_tcpWorkerList.Count.ToString(),
                                    workerClient.Client.RemoteEndPoint, ToString()));

                if (!welcomeMsg.Equals(String.Empty)) SendMsgToClient(welcomeMsg, m_tcpWorkerList.Count.ToString());

                WaitForData(workerClient, m_tcpWorkerList.Count);

                _Server.BeginAcceptTcpClient(new AsyncCallback(CallBack_ClientConnected), null);
            }
            catch (SocketException ex)
            {
                OnErrorOccured("(Callback_ClientConnected)", ex);
            }
        }
        public void PublishMsgToClients(string msg)
        {
            if (msg.Equals(string.Empty)) return;
            try
            {
                byte[] DataInByte = Encoding.ASCII.GetBytes(msg);
                for(int i = 0; i < m_tcpWorkerList.Count; i++)
                {
                    TcpClient workerClient = (TcpClient)m_tcpWorkerList[i];
                    if (workerClient != null)
                    {
                        if (workerClient.Connected)
                        {
                            workerClient.GetStream().Write(DataInByte, 0, DataInByte.Length);
                            OnMessageRepliedCompleted(msg, i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured("(PublishMsgToClients): ", ex);
            }
        }
        protected virtual void SendMsgToClient(string welcomeMsg, int clientNumber)
        {
            try
            {
                if (welcomeMsg.Equals(String.Empty)) return;

                byte[] DataInByte = Encoding.ASCII.GetBytes(welcomeMsg);
                TcpClient client = (TcpClient)m_tcpWorkerList[clientNumber - 1];
                if(client != null)
                {
                    client.GetStream().Write(DataInByte, 0, DataInByte.Length);
                    OnMessageRepliedCompleted(welcomeMsg, clientNumber);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured("(SendMsgToClient): ", ex);
            }
        }
        private void OnMessageRepliedCompleted(string msg, int i)
        {
            throw new NotImplementedException();
        }

        private void WaitForData(TcpClient workerClient, int clientNo)
        {
            try
            {
                if (nWorkerCallBack == null) nWorkerCallBack = new AsyncCallback(CallBack_OnDataReceived);

                SocketPacket socketPacket = new SocketPacket(workerClient, clientNo);

                workerClient.GetStream().BeginRead(socketPacket.DataBuffer, 0,
                    socketPacket.DataBuffer.Length,
                    nWorkerCallBack,
                    socketPacket);
            }
            catch(SocketException ex)
            {
                OnErrorOccured("(WaitForData): ", ex);
            }
        }

        private void CallBack_OnDataReceived(IAsyncResult AR)
        {
            SocketPacket socketData = (SocketPacket)AR.AsyncState;
            try
            {
                int iRxData = socketData.CurrentClient.GetStream().EndRead(AR);
                char[] chars = new char[iRxData + 1];
                int charLen = 0;

                Decoder decoder = Encoding.ASCII.GetDecoder();
                charLen = decoder.GetChars(socketData.DataBuffer, 
                    0, iRxData, chars, 0);

                String msg = new String(chars);

                OnReceivedMsgFromClient(msg, socketData.ClientNo);

                WaitForData(socketData.CurrentClient, socketData.ClientNo);
            }
            catch (ObjectDisposedException ex)
            {

            }
            catch (SocketException ex)
            {
                if(ex.ErrorCode == 10054)
                {
                    OnClientDisconnected(ex.Message, socketData.ClientNo);

                    m_tcpWorkerList[socketData.ClientNo - 1] = null;
                }
            }
        }

        #region Delgate Functions
        public event ServerTcpSocketEventHandler.Listening delListening;
        public event ServerTcpSocketEventHandler.ClientConnected delClientConnected;
        public event ServerTcpSocketEventHandler.ClientDisconnected delClientDisconnected;
        public event ServerTcpSocketEventHandler.ReceivedMsgFromClient delReceivedMsgFromClient;
        public event ServerTcpSocketEventHandler.ErrorOccur delErrorOccur;
        public event ServerTcpSocketEventHandler.MsgReplied delMsgReplied;
        public event ServerTcpSocketEventHandler.DebuggingInfo delDebuggingInfo;

        private void OnClientDisconnected(string message, int clientNo)
        {
            if (delClientDisconnected != null)
            {
                try
                {
                    delClientDisconnected(new ServerTcpEventArgs(message, false, clientNo));
                }
                catch (Exception ex)
                {

                    OnErrorOccured(ex.Message, new ServerTcpCustomException("delClientDisconnected", ex));
                }
            }
        }

        private void OnReceivedMsgFromClient(string msg, int clientNo)
        {
            throw new NotImplementedException();
        }

        private void OnErrorOccured(string v, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void OnClientConnected(string v)
        {
            if(delClientConnected != null)
            {
                try
                {
                    delClientConnected(new ServerTcpEventArgs(v, true, 0));
                }
                catch (Exception ex)
                {
                    OnErrorOccured(ex.Message, new ServerTcpCustomException("delClientConnected", ex));
                }
            }
        }
        #endregion  

    }
}
