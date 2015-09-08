namespace GM760_pc_util_communication_test_tool_win
{
    partial class FrmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtHistory = new System.Windows.Forms.TextBox();
            this.grpBoxHistory = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.BtnOpenNetwork = new System.Windows.Forms.Button();
            this.BtnCloseNetwork = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWifiPort = new System.Windows.Forms.TextBox();
            this.txtEthernetPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblHint = new System.Windows.Forms.Label();
            this.grpBoxHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtHistory
            // 
            this.txtHistory.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHistory.Location = new System.Drawing.Point(7, 28);
            this.txtHistory.Multiline = true;
            this.txtHistory.Name = "txtHistory";
            this.txtHistory.ReadOnly = true;
            this.txtHistory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHistory.Size = new System.Drawing.Size(572, 231);
            this.txtHistory.TabIndex = 0;
            // 
            // grpBoxHistory
            // 
            this.grpBoxHistory.Controls.Add(this.txtHistory);
            this.grpBoxHistory.Location = new System.Drawing.Point(12, 65);
            this.grpBoxHistory.Name = "grpBoxHistory";
            this.grpBoxHistory.Size = new System.Drawing.Size(585, 265);
            this.grpBoxHistory.TabIndex = 2;
            this.grpBoxHistory.TabStop = false;
            this.grpBoxHistory.Text = "歷史訊息";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(501, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 48);
            this.button1.TabIndex = 3;
            this.button1.Text = "Simulate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BtnOpenNetwork
            // 
            this.BtnOpenNetwork.Location = new System.Drawing.Point(270, 350);
            this.BtnOpenNetwork.Name = "BtnOpenNetwork";
            this.BtnOpenNetwork.Size = new System.Drawing.Size(91, 50);
            this.BtnOpenNetwork.TabIndex = 4;
            this.BtnOpenNetwork.Text = "開啟網路";
            this.BtnOpenNetwork.UseVisualStyleBackColor = true;
            this.BtnOpenNetwork.Click += new System.EventHandler(this.BtnOpenNetwork_Click);
            // 
            // BtnCloseNetwork
            // 
            this.BtnCloseNetwork.Location = new System.Drawing.Point(386, 350);
            this.BtnCloseNetwork.Name = "BtnCloseNetwork";
            this.BtnCloseNetwork.Size = new System.Drawing.Size(91, 50);
            this.BtnCloseNetwork.TabIndex = 5;
            this.BtnCloseNetwork.Text = "關閉網路";
            this.BtnCloseNetwork.UseVisualStyleBackColor = true;
            this.BtnCloseNetwork.Click += new System.EventHandler(this.BtnCloseNetwork_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 341);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "WIFI Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 385);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Ethernet Port";
            // 
            // txtWifiPort
            // 
            this.txtWifiPort.Location = new System.Drawing.Point(132, 338);
            this.txtWifiPort.Name = "txtWifiPort";
            this.txtWifiPort.Size = new System.Drawing.Size(106, 29);
            this.txtWifiPort.TabIndex = 8;
            this.txtWifiPort.Text = "8701";
            this.txtWifiPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            // 
            // txtEthernetPort
            // 
            this.txtEthernetPort.Location = new System.Drawing.Point(132, 385);
            this.txtEthernetPort.Name = "txtEthernetPort";
            this.txtEthernetPort.Size = new System.Drawing.Size(106, 29);
            this.txtEthernetPort.TabIndex = 9;
            this.txtEthernetPort.Text = "8700";
            this.txtEthernetPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "提示: ";
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHint.ForeColor = System.Drawing.Color.Red;
            this.lblHint.Location = new System.Drawing.Point(177, 27);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(61, 21);
            this.lblHint.TabIndex = 11;
            this.lblHint.Text = "lblHint";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 424);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEthernetPort);
            this.Controls.Add(this.txtWifiPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCloseNetwork);
            this.Controls.Add(this.BtnOpenNetwork);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grpBoxHistory);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.Text = "GM760 通訊測試";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            this.grpBoxHistory.ResumeLayout(false);
            this.grpBoxHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHistory;
        private System.Windows.Forms.GroupBox grpBoxHistory;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button BtnOpenNetwork;
        private System.Windows.Forms.Button BtnCloseNetwork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWifiPort;
        private System.Windows.Forms.TextBox txtEthernetPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblHint;
    }
}

