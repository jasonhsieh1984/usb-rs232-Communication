// -----------------------------------------------------------------------
// <copyright file="ClsComPort.cs" company="Bionime">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel; // for BackgroundWorker
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using Bionime.Util;

/// <summary>
/// TODO: Update summary.
/// </summary>
public class ClsComPort
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
    public event EventHandler ReceiveMessage;

    /// <summary>
    /// TransmitMessage EventHandler
    /// </summary>
    public event EventHandler TransmitMessage;

    /// <summary>
    /// COM Port 
    /// </summary>
    private SerialPort comPort;

    /// <summary>
    /// Initial and Open Com Port
    /// </summary>
    /// <param name="portName"> Port Name</param>
    /// <param name="baudrate"> Baud rate</param>
    /// <returns>success of fail</returns>
    public bool Initialize(string portName, int baudrate)
    {
        this.comPort = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);
        this.comPort.ReadTimeout = 2000;
        this.comPort.DataReceived += new SerialDataReceivedEventHandler(this.ComPortDataReceived);
        this.comPort.ErrorReceived += new SerialErrorReceivedEventHandler(this.ComPortErrorReceived);
        this.comPort.NewLine = Environment.NewLine;
        this.PortName = portName;

        return this.OpenComPort();
    }

    /// <summary>
    /// Gets or Port of the COM Port
    /// </summary>
    public string PortName
    {
        get;
        private set;
    }

    /// <summary>
    /// Initialize com Port
    /// </summary>
    /// <param name="portName">port name</param>
    /// <returns>success or fail</returns>
    public bool Initialize(string portName)
    {
        this.comPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        this.comPort.ReadTimeout = 2000;
        this.comPort.DataReceived += new SerialDataReceivedEventHandler(this.ComPortDataReceived);
        this.comPort.ErrorReceived += new SerialErrorReceivedEventHandler(this.ComPortErrorReceived);
        this.comPort.NewLine = Environment.NewLine;
        this.PortName = portName;

        return this.OpenComPort();
    }

    /// <summary>
    /// Open com Port
    /// </summary>
    /// <returns>success of fail</returns>
    public bool OpenComPort()
    {
        try
        {
            if ((this.comPort != null) && (!this.comPort.IsOpen))
            {
                this.comPort.Open();
                this.UpdateTXMessage(this.PortName + ",開啟成功");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            this.UpdateTXMessage(this.PortName + ",開啟失敗!!!");
            this.UpdateTXMessage(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// Close Com Port
    /// </summary>
    public void CloseComPort()
    {
        try
        {
            if ((this.comPort != null) && this.comPort.IsOpen)
            {
                this.comPort.Close();
            }
        }
        catch (Exception ex)
        {
            // 這邊你可以自訂發生例外的處理程序
            Debug.WriteLine(ex.ToString());
        }
    }
    
    /// <summary>
    /// Send Data to Com Port
    /// </summary>
    /// <param name="data"> string data</param>
    /// <returns> success or fail </returns>
    public bool SendData(string data)
    {
        lock ("Flag")
        {
            try
            {
                // Flush the serial buffer
                while (this.comPort.BytesToRead > 0)
                {
                    this.ComPortDataReceived(this, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

            // make sure the command is valid
            if ((data == null) || (data == string.Empty))
            {
                // not valid ignore
                return false;
            }

            try
            {
                ////System.Threading.Thread.Sleep(100);
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                this.comPort.Write(buffer, 0, buffer.Length);
                this.UpdateTXMessage(this.PortName + " [TX] " + data);
            }
            catch (Exception ex)
            {
                // 這邊你可以自訂發生例外的處理程序
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Com Port Data Received event
    /// </summary>
    /// <param name="sender">object sender</param>
    /// <param name="e">event Args</param>
    private void ComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        lock ("Flag")
        {
            StringBuilder result = new StringBuilder();
            while (this.comPort.BytesToRead > 0)
            {
                System.Threading.Thread.Sleep(100);
                try
                {
                    byte[] bufferData = new byte[this.comPort.BytesToRead];
                    this.comPort.Read(bufferData, 0, bufferData.Length); 
                    result.Append(System.Text.Encoding.UTF8.GetString(bufferData));
                }
                catch (TimeoutException timeoutEx)
                {
                    // 以下這邊請自行撰寫你想要的例外處理
                    this.UpdateTXMessage("[Debug] Timeout");
                    this.UpdateTXMessage("[Debug]" + timeoutEx.ToString());
                }
                catch (Exception ex)
                {
                    this.UpdateTXMessage("[Debug]" + ex.ToString());
                    Debug.WriteLine(ex.ToString());
                }
            }

            string context = result.ToString();
            this.UpdateRXMessage(this.PortName + " [RX] " + context);

            if (context.Contains("meter_"))
            {
                int intStart = 0, intEnd = 0;
                string strNewResponse = string.Empty;
                intStart = context.IndexOf("meter");
                strNewResponse = context.Substring(intStart, context.Length - intStart);
                intEnd = strNewResponse.IndexOf("\r");
                strNewResponse = strNewResponse.Substring(0, intEnd);
                string[] strTmp = strNewResponse.Split('_');

                string ackString = "dbu_" + strTmp[1] + Environment.NewLine;
                this.SendData(ackString);
            }
            else
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(context);
            }
        }
    }

    /// <summary>
    /// check sum check 
    /// </summary>
    /// <param name="byteArray">byte array to check</param>
    /// <returns>correct or not</returns>
    private bool CheckSum(byte[] byteArray)
    {
        int intSum = 0;

        for (int i = 0; i < byteArray.Length - 1; i++)
        {
            intSum += byteArray[i];
        }

        intSum = intSum & 0xff;
        int receiveSum = byteArray[byteArray.Length - 1];

        if (intSum == receiveSum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Com Port Error Received event
    /// </summary>
    /// <param name="sender">object sender</param>
    /// <param name="e">event Args</param>
    private void ComPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
    }

    /// <summary>
    /// Update RX Message
    /// </summary>
    /// <param name="updateMessage">update message</param>
    private void UpdateRXMessage(string updateMessage)
    {
        if (this.ReceiveMessage != null)
        {
            // if OnQueryEnd was implement 
            this.ReceiveMessage(null, null, updateMessage);
        }
    }

    /// <summary>
    /// Update TX Message
    /// </summary>
    /// <param name="updateMessage">update message</param>
    private void UpdateTXMessage(string updateMessage)
    {
        if (this.TransmitMessage != null)
        {
            // if OnQueryEnd was implement 
            this.TransmitMessage(null, null, updateMessage);
        }
    }
}