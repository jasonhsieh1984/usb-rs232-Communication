// -----------------------------------------------------------------------
// <copyright file="ClsNetworkFunction.cs" company="Bionime">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace GM760_pc_util_communication_test_tool_win
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// used to estblish TCP Listener and accept connect
    /// </summary>
    public class ClsNetworkFunction
    {
        /// <summary>
        /// EventHandler delegate
        /// </summary>
        /// <param name="sender">system sender</param>
        /// <param name="e">system DoWorkEventArgs</param>
        /// <param name="message">string that we want to trans</param>
        public delegate void EventHandler(object sender, EventArgs e, string message);

        /// <summary>
        /// ReceiveMessage EventHandler
        /// </summary>
        public event EventHandler PushMessage;

        /// <summary>
        /// Gets or Port of the Host Name
        /// </summary>
        public string HostName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets of the Listen Port
        /// </summary>
        public int ListenPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Port of the IP Address
        /// </summary>
        public IPAddress HostIPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Tcp Listener
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Volatile is used as hint to the compiler that this data
        /// member will be accessed by multiple threads.
        /// </summary>
        private volatile bool shouldStop;

        /// <summary>
        /// HandleClient function
        /// </summary>
        private List<HandleClient> listHandleClient = new List<HandleClient>();

        /// <summary>
        /// Initialize com Port
        /// </summary>
        /// <returns>success or fail</returns>
        /// <param name="port">port</param>
        public bool Initial(int port)
        {
            ////取得本機名稱
            this.HostName = Dns.GetHostName();

            ////取得本機IP
            IPAddress[] ipa = Dns.GetHostAddresses(this.HostName);
            this.HostIPAddress = ipa[0];

            foreach (IPAddress item in ipa)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork && System.Net.IPAddress.IsLoopback(item) == false)
                {
                    this.HostIPAddress = item;
                    break;
                }
            }

            this.ListenPort = port;
            this.shouldStop = false;

            return true;
        }

        /// <summary>
        /// StartListen.
        /// </summary>
        public void StartListen()
        {
            Thread myThread = new Thread(new ThreadStart(this.DoListen));            
            myThread.IsBackground = true;
            myThread.Start();
        }

        /// <summary>
        /// StopListen.
        /// </summary>
        public void StopListen()
        {
            for (int i = this.listHandleClient.Count - 1; i >= 0; i--)
            {
                this.listHandleClient[i].Stop();
                this.listHandleClient.RemoveAt(i);
            }

            this.tcpListener.Stop();
            this.shouldStop = true;            
        }

        /// <summary>
        /// Listen Process.
        /// </summary>
        private void DoListen()
        {
            ////建立本機端的IPEndPoint物件
            IPEndPoint ipe = new IPEndPoint(this.HostIPAddress, this.ListenPort);

            ////建立TcpListener物件
            this.tcpListener = new TcpListener(ipe);

            TcpClient tmpTcpClient = new TcpClient();

            ////開始監聽port
            this.tcpListener.Start();
            this.GetMessage("等待客戶端連線中... \n");

            while (this.shouldStop == false)
            {
                try
                {   
                    ////建立與客戶端的連線
                    tmpTcpClient = this.tcpListener.AcceptTcpClient();

                    if (tmpTcpClient.Connected)
                    {
                        string clientIP = ((IPEndPoint)tmpTcpClient.Client.RemoteEndPoint).Address.ToString();

                        this.GetMessage("連線成功!");
                        HandleClient handleClient = new HandleClient(tmpTcpClient);
                        
                        handleClient.PushMessage += (object objectSender, EventArgs eventArgs, string updateMessage) =>
                        {
                            this.GetMessage("[client: " + clientIP + "] " + updateMessage);
                        };
                        
                        this.listHandleClient.Add(handleClient);

                        Thread myThread = new Thread(new ThreadStart(handleClient.Communicate));                        
                        myThread.IsBackground = true;
                        myThread.Start();
                        myThread.Name = tmpTcpClient.Client.RemoteEndPoint.ToString();
                    }
                }
                catch (Exception ex)
                {
                    this.shouldStop = true;
                    this.GetMessage(ex.Message);
                }
            }

            tmpTcpClient.Close();
            this.GetMessage("監聽結束");
        }

        /// <summary>
        /// Get Message.
        /// </summary>
        /// <param name="message">message</param>
        private void GetMessage(string message)
        {
            if (this.PushMessage != null)
            {
                this.PushMessage(null, null, message);
            }
        }
    }

    /// <summary>
    /// CommunicationBase給客戶端和主機端共用，可傳送接收訊息
    /// </summary>
    public class CommunicationBase
    {
        /// <summary>
        /// 傳送訊息
        /// </summary>
        /// <param name="msg">要傳送的訊息</param>
        /// <param name="tmpTcpClient">TcpClient</param>
        public void SendMsg(string msg, TcpClient tmpTcpClient)
        {
            NetworkStream ns = tmpTcpClient.GetStream();
            if (ns.CanWrite)
            {
                byte[] msgByte = Encoding.Default.GetBytes(msg);
                ns.Write(msgByte, 0, msgByte.Length);
            }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        /// <param name="tmpTcpClient">TcpClient</param>
        /// <returns>接收到的訊息</returns>
        public string ReceiveMsg(TcpClient tmpTcpClient)
        {
            string receiveMsg = string.Empty;
            byte[] receiveBytes = new byte[tmpTcpClient.ReceiveBufferSize];
            int numberOfBytesRead = 0;
            NetworkStream ns = tmpTcpClient.GetStream();

            if (ns.CanRead)
            {
                do
                {
                    numberOfBytesRead = ns.Read(receiveBytes, 0, tmpTcpClient.ReceiveBufferSize);
                    receiveMsg = Encoding.Default.GetString(receiveBytes, 0, numberOfBytesRead);
                }
                while (ns.DataAvailable);
            }

            return receiveMsg;
        }
    }

     /// <summary>
    /// HandleClient
    /// </summary>
    public class HandleClient
    {
        /// <summary>
        /// private attribute of HandleClient class
        /// </summary>
        private TcpClient mTcpClient;

        /// <summary>
        /// Volatile is used as hint to the compiler that this data
        /// member will be accessed by multiple threads.
        /// </summary>
        private volatile bool shouldStop;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tmpTcpClient">傳入TcpClient參數</param>
        public HandleClient(TcpClient tmpTcpClient)
        {
            this.mTcpClient = tmpTcpClient;
            this.shouldStop = false;
        }

        /// <summary>
        /// EventHandler delegate
        /// </summary>
        /// <param name="sender">system sender</param>
        /// <param name="e">system DoWorkEventArgs</param>
        /// <param name="message">string that we want to trans</param>
        public delegate void EventHandler(object sender, EventArgs e, string message);

        /// <summary>
        /// PushMessage EventHandler
        /// </summary>
        public event EventHandler PushMessage;

        /// <summary>
        /// Stop connection.
        /// </summary>
        public void Stop()
        {
            this.shouldStop = true;
        }

        /// <summary>
        /// Communicate
        /// </summary>
        public void Communicate()
        {
            while (this.shouldStop == false)
            {
                try
                {
                    CommunicationBase comm = new CommunicationBase();
                    string msg = comm.ReceiveMsg(this.mTcpClient);

                    if (msg == "Hello Server!")
                    {
                        this.GetMessage(msg);
                        comm.SendMsg("Hi Meter\n", this.mTcpClient);
                        this.GetMessage("主機回傳:Hi Meter");
                    }
                    else
                    {
                        this.GetMessage(msg);
                        comm.SendMsg("主機回傳:測試", this.mTcpClient);
                        this.GetMessage("主機回傳:測試!");
                    }                    
                }
                catch
                {
                    this.shouldStop = true;
                    this.GetMessage("強制關閉連線!");
                    this.mTcpClient.Close();                    
                }                
            }
        }

        /// <summary>
        /// Get Message From client.
        /// </summary>
        /// <param name="message">message</param>
        private void GetMessage(string message)
        {
            if (this.PushMessage != null)
            {
                this.PushMessage(null, null, message);
            }
        }
    }
}
