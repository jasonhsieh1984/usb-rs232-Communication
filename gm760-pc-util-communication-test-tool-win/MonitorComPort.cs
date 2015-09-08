// -----------------------------------------------------------------------
// <copyright file="MonitorComPort.cs" company="Bionime">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.ComponentModel; // for BackgroundWorker
using System.Data;

/// <summary>
/// TODO: Update summary.
/// </summary>
public class MonitorComPort
{
    /// <summary>
    /// time out limit
    /// </summary>
    private const int TimeoutLimit = 20;
    
    /// <summary>
    /// BackgroundWorker to monitor com port
    /// </summary>
    private BackgroundWorker backgroundWorkerMonitorComPort;
    
    /// <summary>
    /// comPort object
    /// </summary>
    private ClsComPort comPort;
    
    /// <summary>
    /// time count
    /// </summary>
    private int timeCount;

    /// <summary>
    /// Initializes a new instance of the MonitorComPort class 
    /// build connectionString in constructor
    /// </summary>
    public MonitorComPort()
    {
        this.timeCount = 0;
        this.backgroundWorkerMonitorComPort = new BackgroundWorker();
        this.backgroundWorkerMonitorComPort.WorkerReportsProgress = true;
        this.backgroundWorkerMonitorComPort.WorkerSupportsCancellation = true;
        this.backgroundWorkerMonitorComPort.DoWork += new DoWorkEventHandler(this.MonitorComPortDoWork);
        this.backgroundWorkerMonitorComPort.ProgressChanged += new ProgressChangedEventHandler(this.MonitorComPortProgress);
        ////backControlTableUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwStationRunWorkerCompleted);
    }

    /// <summary>
    /// EventHandler delegate
    /// </summary>
    /// <param name="sender">system sender</param>
    /// <param name="e">system DoWorkEventArgs</param>
    /// <param name="message">string that we want to trans</param>
    public delegate void EventHandler(object sender, EventArgs e, string message);

    /// <summary>
    /// OnUpdateEnd EventHandler
    /// </summary>
    public event EventHandler OnUpdateEnd;

    /// <summary>
    /// Gets or sets thread index
    /// </summary>
    public int Index
    {
        get;
        set;
    }
    
    /// <summary>
    /// Gets or sets thread Tag
    /// </summary>
    public string Tag
    {
        get;
        set;
    }

    /// <summary>
    /// send data to com port
    /// </summary>
    /// <param name="message">message</param>
    public void Send(string message)
    {
        this.comPort.SendData(message);
    }

    /// <summary>
    /// Cancel Control table Start method
    /// </summary>
    public void Cancel()
    {
        if (this.backgroundWorkerMonitorComPort.WorkerSupportsCancellation == true)
        {
            this.backgroundWorkerMonitorComPort.CancelAsync();
        }
    }

    /// <summary>
    /// Update Control table Start method
    /// </summary>
    /// <param name="comPortName">target comPort</param>
    public void Execute(string comPortName)
    {
        this.backgroundWorkerMonitorComPort.RunWorkerAsync(comPortName as object);
    }

    /// <summary>
    /// Control table Update DoWork
    /// </summary>
    /// <param name="sender">system sender</param>
    /// <param name="e">system DoWorkEventArgs</param>
    private void MonitorComPortDoWork(object sender, DoWorkEventArgs e)
    {
        BackgroundWorker worker = sender as BackgroundWorker;
        string comPortName = e.Argument as string;
        bool nextRun = true;

        while (nextRun)
        {
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
                worker.ReportProgress(0, "Close!!" as object);
                break;
            }
            else
            {
                if (this.comPort != null)
                {
                    if (this.timeCount > TimeoutLimit)
                    {
                        // time out 
                        this.comPort.CloseComPort();
                        this.comPort = null;
                    }
                    else
                    {
                        this.timeCount++;
                    }
                }
                else
                {
                    this.comPort = new ClsComPort();
                    this.comPort.ReceiveMessage += (object objectSender, EventArgs eventArgs, string updateMessage) =>
                    {
                        worker.ReportProgress(0, updateMessage as object);
                        this.timeCount = 0;
                    };

                    this.comPort.TransmitMessage += (object objectSender, EventArgs eventArgs, string updateMessage) =>
                    {
                        worker.ReportProgress(0, updateMessage as object);
                        this.timeCount = 0;
                    };

                    this.comPort.Initialize(comPortName);
                }

                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    /// <summary>
    /// Control table Progress Changed
    /// </summary>
    /// <param name="sender">system sender</param>
    /// <param name="e">system ProgressChangedEventArgs</param>
    private void MonitorComPortProgress(object sender, ProgressChangedEventArgs e)
    {
        try
        {
            string message;
            message = e.UserState as string;
            if (this.OnUpdateEnd != null)
            {
                // if OnQueryEnd was implement 
                this.OnUpdateEnd(this, null, message);
            }
        }
        catch (Exception exception)
        {
            exception.ToString();
        }
    }
}
