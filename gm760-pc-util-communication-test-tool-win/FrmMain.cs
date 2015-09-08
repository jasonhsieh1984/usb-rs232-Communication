// -----------------------------------------------------------------------
// <copyright file="FrmMain.cs" company="Bionime">
// TODO: Update copyright text.
// History:
//     v0.0.1: Jason, 2015/03/12, First coding.
//     v0.0.2: Jason, 2015/03/18, COM PORT, WIFI, Ethernet
//     v1.0.0: Jason, 2015/03/23, 每個命令後面增加換行符號
// </copyright>
// -----------------------------------------------------------------------
namespace GM760_pc_util_communication_test_tool_win
{
    using System;
    using System.Collections;    
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <summary>
    /// Main Form
    /// </summary>
    public partial class FrmMain : Form
    {
        /// <summary>
        /// METER CONNECT LIMIT
        /// </summary>
        private static int meterConnectLimit = 5;

        /// <summary>
        /// Version Description
        /// </summary>
        private string strVersion = "v1.0.0";

        /// <summary>
        /// Boolean network is opened
        /// </summary>
        private bool bNetworkOpen;

        /// <summary>
        /// USB Monitor 
        /// </summary>
        private USB getUSB = new USB();

        /// <summary>
        /// Monitor Com Port List
        /// </summary>
        private MonitorComPort[] monitorComPorts = new MonitorComPort[meterConnectLimit];

        /// <summary>
        /// last Com Port List
        /// </summary>
        private ArrayList lastComPortList;

        /// <summary>
        /// TCP/IP Function for WIFI
        /// </summary>
        private ClsNetworkFunction wifiServer = new ClsNetworkFunction();

        /// <summary>
        /// TCP/IP Function for Ethernet
        /// </summary>
        private ClsNetworkFunction ethernetServer = new ClsNetworkFunction();

        /// <summary>
        /// Set Text Callback.
        /// </summary>
        /// <param name="message">message</param> 
        delegate void SetTextCallback(string message);

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmMain" /> class.
        /// </summary>
        public FrmMain()
        {
            this.InitializeComponent();
            this.Text += " " + this.strVersion;
            this.CenterToScreen();
            this.bNetworkOpen = false;
            this.txtEthernetPort.ReadOnly = true;
            this.txtWifiPort.ReadOnly = true;

            this.lastComPortList = new ArrayList();

            for (int i = 0; i < meterConnectLimit; i++)
            {
                this.monitorComPorts[i] = new MonitorComPort();                
                this.monitorComPorts[i].Index = i;
                this.monitorComPorts[i].Tag = string.Empty;
                this.monitorComPorts[i].OnUpdateEnd += (object sender, EventArgs e, string message) =>
                {
                    // parser to show serialnumber
                    MonitorComPort monitor = sender as MonitorComPort;
                    this.PushMessage(message);

                    if (message.Contains("meter_"))
                    {
                        string[] result = message.Split('_');
                        string serialNumber = string.Empty;

                        if (result.Length == 4)
                        {
                            serialNumber = result[1];
                        }                       
                    }
                };
            }

            this.wifiServer.PushMessage += (object sender, EventArgs e, string message) =>
            {
                this.PushMessage("WIFI: " + message);
            };

            this.ethernetServer.PushMessage += (object sender, EventArgs e, string message) =>
            {
                this.PushMessage("Ethernet: " + message);
            };
        }

        /// <summary>
        /// FrmMain Load event 
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            ////3 seconds to wait for com port ready.
            this.getUSB.AddUSBEventWatcher(this.USBEventHandler, this.USBEventHandler, new TimeSpan(0, 0, 3));

            this.BtnOpenNetwork.Enabled = true;
            this.BtnCloseNetwork.Enabled = false;

            this.lblHint.Text = "初始化成功，請開啟網路。";
        }

        /// <summary>
        /// USB EventHandler
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void USBEventHandler(object sender, EventArrivedEventArgs e)
        {
            ////find current bionime device 
            ArrayList currentBionimeDevice = new ArrayList();

            ////get the serial ports whitch connected
            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
                string[] str = registryKey.GetValueNames();
                foreach (string valuename in registryKey.GetValueNames())
                {
                    if (valuename.Contains("ProlificSerial"))
                    {
                        if (registryKey.GetValue(valuename).ToString().Equals(portName))
                        {
                            currentBionimeDevice.Add(portName as object);
                        }
                    }
                }
            }

            //// add or remove device here
            if (this.lastComPortList.Count < currentBionimeDevice.Count)
            {
                foreach (var item in currentBionimeDevice)
                {
                    if (this.lastComPortList.Contains(item))
                    {
                        Console.WriteLine("device exit: " + item.ToString());
                    }
                    else
                    {
                        Console.WriteLine("add device: " + item.ToString());
                        this.PushMessage("增加Meter: " + item.ToString());
                        this.lastComPortList.Add(item);
                        this.AddMeter(item.ToString());
                    }
                }
            }
            else
            {
                ArrayList templist = (ArrayList)this.lastComPortList.Clone();

                foreach (var item in templist)
                {
                    if (currentBionimeDevice.Contains(item))
                    {
                        Console.WriteLine("device exit:" + item.ToString());
                    }
                    else
                    {
                        Console.WriteLine("remove device:" + item.ToString());
                        this.PushMessage("移除Meter: " + item.ToString());
                        this.lastComPortList.Remove(item);
                        this.RemoveMeter(item.ToString());
                    }
                }
            }
        }
        
        /// <summary>
        /// Add Meter to system 
        /// </summary>
        /// <param name="comPortName">com port name</param>
        private void AddMeter(string comPortName)
        {
            // start thread
            try
            {
                for (int i = 0; i < this.monitorComPorts.Length; i++)
                {
                    if (this.monitorComPorts[i].Tag == string.Empty)
                    {
                        this.monitorComPorts[i].Tag = comPortName;
                        this.monitorComPorts[i].Execute(comPortName);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.PushMessage("ERR:AddMeter, " + ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Remove Meter from system 
        /// </summary>
        /// <param name="comPortName">com port name</param>
        private void RemoveMeter(string comPortName)
        {
            // stop thread
            try
            {
                for (int i = 0; i < this.monitorComPorts.Length; i++)
                {
                    if (this.monitorComPorts[i].Tag == comPortName)
                    {
                        this.monitorComPorts[i].Tag = string.Empty;
                        this.monitorComPorts[i].Cancel();                        
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.PushMessage("ERR:RemoveMeter, " + ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Get Message From COM PORT
        /// </summary>
        /// <param name="message">message</param>
        private void PushMessage(string message)
        {
            if (this.txtHistory.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(this.PushMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                ////this.txtHistory.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ")  + message + "\r\n" + this.txtHistory.Text;
                this.txtHistory.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + message + "\r\n");
                if (message.Contains("[RX]"))
                {
                    this.DoProcess(message);
                }
            }
        }

        /// <summary>
        /// DoProcess for COM Port event
        /// </summary>
        /// <param name="message">message</param>
        private void DoProcess(string message)
        {
            if (this.FindKeyword(message, "Hello Server!"))
            {
                foreach (MonitorComPort item in this.monitorComPorts)
                {
                    if (this.FindKeyword(message, item.Tag))
                    {
                        item.Send("Hi Meter\n");
                        break;
                    }
                }
            }
            else if (this.FindKeyword(message, "Get Setting"))
            {
                if (this.bNetworkOpen == false)
                {
                    MessageBox.Show("網路尚未開啟，請重新測試。");
                    return;
                }

                foreach (MonitorComPort item in this.monitorComPorts)
                {
                    if (this.FindKeyword(message, item.Tag))
                    {
                        string info = this.wifiServer.HostIPAddress.ToString() + "," + this.wifiServer.ListenPort.ToString() + ","
                            + this.ethernetServer.ListenPort.ToString() + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                        item.Send(info);
                        break;
                    }
                }
            }
            else if (this.FindKeyword(message, "Hi Meter"))
            {
                ////for simulation

                foreach (MonitorComPort item in this.monitorComPorts)
                {
                    if (this.FindKeyword(message, item.Tag))
                    {
                        item.Send("Get Setting");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Find keywords in received message
        /// </summary>
        /// <param name="message">Message</param> 
        /// <param name="keyword">Keyword</param> 
        /// <returns>Boolean of equal or not</returns>
        private bool FindKeyword(string message, string keyword) ////定義正則表達式函數  
        {
            Regex regex = new Regex(keyword, RegexOptions.IgnoreCase);
            return regex.IsMatch(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.monitorComPorts.Length; i++)
                {
                    if (this.monitorComPorts[i].Tag != string.Empty)
                    {
                        this.monitorComPorts[i].Send("Hello Server!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// BtnOpenNetwork Click event 
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void BtnOpenNetwork_Click(object sender, EventArgs e)
        {
            if (this.txtEthernetPort.Text == string.Empty || this.txtWifiPort.Text == string.Empty)
            {
                MessageBox.Show("Port錯誤，請確認。");
                return;
            }

            this.ethernetServer.Initial(Convert.ToInt32(this.txtEthernetPort.Text));
            this.wifiServer.Initial(Convert.ToInt32(this.txtWifiPort.Text));
            this.ethernetServer.StartListen();
            this.wifiServer.StartListen();

            this.PushMessage("Host Name= " + this.ethernetServer.HostName);
            this.PushMessage("IP Address= " + this.ethernetServer.HostIPAddress.ToString());
            this.PushMessage("Ethernet Port= " + this.ethernetServer.ListenPort);
            this.PushMessage("WIFI Port= " + this.wifiServer.ListenPort);
            this.PushMessage("開始監聽");

            this.bNetworkOpen = true;
            this.BtnOpenNetwork.Enabled = false;
            this.BtnCloseNetwork.Enabled = true;
            this.lblHint.Text = "開啟網路完成，等待連線...";
        }

        /// <summary>
        /// BtnCloseNetwork Click event 
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void BtnCloseNetwork_Click(object sender, EventArgs e)
        {
            this.bNetworkOpen = false;

            this.wifiServer.StopListen();
            this.ethernetServer.StopListen();

            this.BtnOpenNetwork.Enabled = true;
            this.BtnCloseNetwork.Enabled = false;

            this.lblHint.Text = "關閉已網路，請重新開啟網路。";
        }

        /// <summary>
        /// Ctrl + Alt + E to enable edit port
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.Alt == true && e.KeyCode == Keys.E)
            {
                this.txtEthernetPort.ReadOnly = false;
                this.txtWifiPort.ReadOnly = false;
                MessageBox.Show("開啟PORT修改功能。");
            }
        }

        /// <summary>
        /// FrmMain Closing event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">sender event args</param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.getUSB.RemoveUSBEventWatcher();
        }
    }
}
