namespace SNet_Client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

#region Windows Form Designer generated code

/// <summary>
/// Required method for Designer support - do not modify
/// the contents of this method with the code editor.
/// </summary>
private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();
    System.ComponentModel.ComponentResourceManager resources =
        new System.ComponentModel.ComponentResourceManager(typeof(SNet_Client.Form1));
    this.buttonConnectToServer = new System.Windows.Forms.Button();
    this.textBoxServer = new System.Windows.Forms.TextBox();
    this.labelStatusInfo = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.textBoxClientName = new System.Windows.Forms.TextBox();
    this.label1 = new System.Windows.Forms.Label();
    this.textBoxServerListeningPort = new System.Windows.Forms.TextBox();
    this.label3 = new System.Windows.Forms.Label();
    this.buttonDisconnect = new System.Windows.Forms.Button();
    this.labelConnectionStuff = new System.Windows.Forms.Label();
    this.buttonSendDataToServer = new System.Windows.Forms.Button();
    this.textBoxText = new System.Windows.Forms.TextBox();
    this.label4 = new System.Windows.Forms.Label();
    this.textBoxNum1 = new System.Windows.Forms.TextBox();
    this.textBoxNum2 = new System.Windows.Forms.TextBox();
    this.label5 = new System.Windows.Forms.Label();
    this.label6 = new System.Windows.Forms.Label();
    this.la_info = new System.Windows.Forms.Label();
    this.progressBar1 = new System.Windows.Forms.ProgressBar();
    this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
    this.SuspendLayout();
    this.buttonConnectToServer.Location = new System.Drawing.Point(14, 93);
    this.buttonConnectToServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.buttonConnectToServer.Name = "buttonConnectToServer";
    this.buttonConnectToServer.Size = new System.Drawing.Size(223, 27);
    this.buttonConnectToServer.TabIndex = 0;
    this.buttonConnectToServer.Text = "Connect To Server";
    this.buttonConnectToServer.UseVisualStyleBackColor = true;
    this.buttonConnectToServer.Click += new System.EventHandler(this.buttonConnectToServer_Click);
    this.textBoxServer.Location = new System.Drawing.Point(121, 6);
    this.textBoxServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxServer.Name = "textBoxServer";
    this.textBoxServer.Size = new System.Drawing.Size(116, 23);
    this.textBoxServer.TabIndex = 1;
    this.textBoxServer.Text = "localhost";
    this.labelStatusInfo.AutoSize = true;
    this.labelStatusInfo.Location = new System.Drawing.Point(110, 132);
    this.labelStatusInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.labelStatusInfo.Name = "labelStatusInfo";
    this.labelStatusInfo.Size = new System.Drawing.Size(178, 15);
    this.labelStatusInfo.TabIndex = 2;
    this.labelStatusInfo.Text = "Click \'Connect to Server\'  button";
    this.label2.AutoSize = true;
    this.label2.Location = new System.Drawing.Point(13, 9);
    this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label2.Name = "label2";
    this.label2.Size = new System.Drawing.Size(52, 15);
    this.label2.TabIndex = 3;
    this.label2.Text = "Server IP";
    this.textBoxClientName.Location = new System.Drawing.Point(121, 63);
    this.textBoxClientName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxClientName.Name = "textBoxClientName";
    this.textBoxClientName.Size = new System.Drawing.Size(116, 23);
    this.textBoxClientName.TabIndex = 4;
    this.textBoxClientName.Text = "John Smith";
    this.label1.AutoSize = true;
    this.label1.Location = new System.Drawing.Point(14, 67);
    this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label1.Name = "label1";
    this.label1.Size = new System.Drawing.Size(73, 15);
    this.label1.TabIndex = 5;
    this.label1.Text = "Client Name";
    this.textBoxServerListeningPort.Location = new System.Drawing.Point(121, 35);
    this.textBoxServerListeningPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxServerListeningPort.Name = "textBoxServerListeningPort";
    this.textBoxServerListeningPort.Size = new System.Drawing.Size(58, 23);
    this.textBoxServerListeningPort.TabIndex = 6;
    this.textBoxServerListeningPort.Text = "5678";
    this.label3.AutoSize = true;
    this.label3.Location = new System.Drawing.Point(13, 38);
    this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label3.Name = "label3";
    this.label3.Size = new System.Drawing.Size(64, 15);
    this.label3.TabIndex = 7;
    this.label3.Text = "Server port";
    this.buttonDisconnect.Enabled = false;
    this.buttonDisconnect.Location = new System.Drawing.Point(14, 126);
    this.buttonDisconnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.buttonDisconnect.Name = "buttonDisconnect";
    this.buttonDisconnect.Size = new System.Drawing.Size(88, 27);
    this.buttonDisconnect.TabIndex = 9;
    this.buttonDisconnect.Text = "Disconnect";
    this.buttonDisconnect.UseVisualStyleBackColor = true;
    this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
    this.labelConnectionStuff.AutoSize = true;
    this.labelConnectionStuff.Location = new System.Drawing.Point(14, 283);
    this.labelConnectionStuff.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.labelConnectionStuff.Name = "labelConnectionStuff";
    this.labelConnectionStuff.Size = new System.Drawing.Size(16, 15);
    this.labelConnectionStuff.TabIndex = 10;
    this.labelConnectionStuff.Text = "...";
    this.buttonSendDataToServer.Enabled = false;
    this.buttonSendDataToServer.Location = new System.Drawing.Point(14, 159);
    this.buttonSendDataToServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.buttonSendDataToServer.Name = "buttonSendDataToServer";
    this.buttonSendDataToServer.Size = new System.Drawing.Size(216, 27);
    this.buttonSendDataToServer.TabIndex = 11;
    this.buttonSendDataToServer.Text = "Send Data To Server";
    this.buttonSendDataToServer.UseVisualStyleBackColor = true;
    this.buttonSendDataToServer.Click += new System.EventHandler(this.buttonSendDataToServer_Click);
    this.textBoxText.Location = new System.Drawing.Point(14, 192);
    this.textBoxText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxText.Multiline = true;
    this.textBoxText.Name = "textBoxText";
    this.textBoxText.Size = new System.Drawing.Size(510, 85);
    this.textBoxText.TabIndex = 12;
    this.label4.AutoSize = true;
    this.label4.Location = new System.Drawing.Point(450, 174);
    this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label4.Name = "label4";
    this.label4.Size = new System.Drawing.Size(73, 15);
    this.label4.TabIndex = 13;
    this.label4.Text = "Text to send:";
    this.textBoxNum1.Location = new System.Drawing.Point(532, 192);
    this.textBoxNum1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxNum1.MaxLength = 10;
    this.textBoxNum1.Name = "textBoxNum1";
    this.textBoxNum1.Size = new System.Drawing.Size(94, 23);
    this.textBoxNum1.TabIndex = 14;
    this.textBoxNum1.Text = "123456";
    this.textBoxNum2.Location = new System.Drawing.Point(532, 254);
    this.textBoxNum2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.textBoxNum2.MaxLength = 10;
    this.textBoxNum2.Name = "textBoxNum2";
    this.textBoxNum2.Size = new System.Drawing.Size(94, 23);
    this.textBoxNum2.TabIndex = 15;
    this.textBoxNum2.Text = "54321";
    this.label5.AutoSize = true;
    this.label5.Location = new System.Drawing.Point(532, 174);
    this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label5.Name = "label5";
    this.label5.Size = new System.Drawing.Size(96, 15);
    this.label5.TabIndex = 16;
    this.label5.Text = "Number to send:";
    this.label6.AutoSize = true;
    this.label6.Location = new System.Drawing.Point(532, 237);
    this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.label6.Name = "label6";
    this.label6.Size = new System.Drawing.Size(95, 15);
    this.label6.TabIndex = 17;
    this.label6.Text = "Another number";
    this.la_info.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold,
        System.Drawing.GraphicsUnit.Point, ((byte) (0)));
    this.la_info.ForeColor = System.Drawing.Color.Purple;
    this.la_info.Location = new System.Drawing.Point(245, 9);
    this.la_info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
    this.la_info.Name = "la_info";
    this.la_info.Size = new System.Drawing.Size(382, 48);
    this.la_info.TabIndex = 18;
    this.la_info.Text = "la_info";
    this.progressBar1.Location = new System.Drawing.Point(237, 159);
    this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.progressBar1.Name = "progressBar1";
    this.progressBar1.Size = new System.Drawing.Size(206, 27);
    this.progressBar1.TabIndex = 19;
    this.notifyIcon1.Icon = ((System.Drawing.Icon) (resources.GetObject("notifyIcon1.Icon")));
    this.notifyIcon1.Text = "SNet - Client";
    this.notifyIcon1.Visible = true;
    this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(637, 302);
    this.Controls.Add(this.progressBar1);
    this.Controls.Add(this.la_info);
    this.Controls.Add(this.label6);
    this.Controls.Add(this.label5);
    this.Controls.Add(this.textBoxNum2);
    this.Controls.Add(this.textBoxNum1);
    this.Controls.Add(this.label4);
    this.Controls.Add(this.textBoxText);
    this.Controls.Add(this.buttonSendDataToServer);
    this.Controls.Add(this.labelConnectionStuff);
    this.Controls.Add(this.buttonDisconnect);
    this.Controls.Add(this.label3);
    this.Controls.Add(this.textBoxServerListeningPort);
    this.Controls.Add(this.label1);
    this.Controls.Add(this.textBoxClientName);
    this.Controls.Add(this.label2);
    this.Controls.Add(this.labelStatusInfo);
    this.Controls.Add(this.textBoxServer);
    this.Controls.Add(this.buttonConnectToServer);
    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
    this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
    this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
    this.Name = "Form1";
    this.Text = "SNet - Client ";
    this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
    this.Load += new System.EventHandler(this.Form1_Load);
    this.ResumeLayout(false);
    this.PerformLayout();
}

#endregion

private System.Windows.Forms.Button buttonConnectToServer;
private System.Windows.Forms.TextBox textBoxServer;
private System.Windows.Forms.Label labelStatusInfo;
private System.Windows.Forms.Label label2;
private System.Windows.Forms.TextBox textBoxClientName;
private System.Windows.Forms.Label label1;
private System.Windows.Forms.TextBox textBoxServerListeningPort;
private System.Windows.Forms.Label label3;
private System.Windows.Forms.Button buttonDisconnect;
private System.Windows.Forms.Label labelConnectionStuff;
private System.Windows.Forms.Button buttonSendDataToServer;
private System.Windows.Forms.TextBox textBoxText;
private System.Windows.Forms.Label label4;
private System.Windows.Forms.TextBox textBoxNum1;
private System.Windows.Forms.TextBox textBoxNum2;
private System.Windows.Forms.Label label5;
private System.Windows.Forms.Label la_info;
private System.Windows.Forms.ProgressBar progressBar1;
private System.Windows.Forms.NotifyIcon notifyIcon1;
private System.Windows.Forms.Label label6;
    }
}