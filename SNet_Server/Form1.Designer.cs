namespace SNet_Server
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
            this.labelMyIP = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.ClientIP = new System.Windows.Forms.ColumnHeader();
            this.Computer = new System.Windows.Forms.ColumnHeader();
            this.Version = new System.Windows.Forms.ColumnHeader();
            this.ConnectionID = new System.Windows.Forms.ColumnHeader();
            this.ClientName = new System.Windows.Forms.ColumnHeader();
            this.PingTime = new System.Windows.Forms.ColumnHeader();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CommunicationsDisplay = new System.Windows.Forms.WebBrowser();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxUnusedIPs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMyIP
            // 
            this.labelMyIP.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.labelMyIP.AutoSize = true;
            this.labelMyIP.ForeColor = System.Drawing.Color.LightYellow;
            this.labelMyIP.Location = new System.Drawing.Point(20, 225);
            this.labelMyIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMyIP.Name = "labelMyIP";
            this.labelMyIP.Size = new System.Drawing.Size(37, 15);
            this.labelMyIP.TabIndex = 68;
            this.labelMyIP.Text = "My IP";
            // 
            // listView1
            // 
            this.listView1.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
                                                         System.Windows.Forms.AnchorStyles.Bottom) |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                {this.ClientIP, this.Computer, this.Version, this.ConnectionID, this.ClientName, this.PingTime});
            this.listView1.Location = new System.Drawing.Point(18, 54);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(697, 160);
            this.listView1.TabIndex = 67;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // ClientIP
            // 
            this.ClientIP.Text = "Client IP";
            this.ClientIP.Width = 136;
            // 
            // Computer
            // 
            this.Computer.Text = "Computer";
            this.Computer.Width = 120;
            // 
            // Version
            // 
            this.Version.Text = "Version";
            this.Version.Width = 88;
            // 
            // ConnectionID
            // 
            this.ConnectionID.Text = "ClientID";
            this.ConnectionID.Width = 56;
            // 
            // ClientName
            // 
            this.ClientName.Text = "Name";
            this.ClientName.Width = 127;
            // 
            // PingTime
            // 
            this.PingTime.Text = "Ping Time";
            this.PingTime.Width = 164;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom |
                                                        System.Windows.Forms.AnchorStyles.Left) |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.CommunicationsDisplay);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.groupBox2.ForeColor = System.Drawing.Color.LightYellow;
            this.groupBox2.Location = new System.Drawing.Point(14, 295);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(845, 278);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Communication Events";
            // 
            // CommunicationsDisplay
            // 
            this.CommunicationsDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommunicationsDisplay.Location = new System.Drawing.Point(4, 20);
            this.CommunicationsDisplay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CommunicationsDisplay.MinimumSize = new System.Drawing.Size(23, 23);
            this.CommunicationsDisplay.Name = "CommunicationsDisplay";
            this.CommunicationsDisplay.Size = new System.Drawing.Size(838, 255);
            this.CommunicationsDisplay.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.LightYellow;
            this.label2.Location = new System.Drawing.Point(723, 133);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 72;
            this.label2.Text = "IP6 addresses";
            // 
            // listBoxUnusedIPs
            // 
            this.listBoxUnusedIPs.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxUnusedIPs.Enabled = false;
            this.listBoxUnusedIPs.FormattingEnabled = true;
            this.listBoxUnusedIPs.ItemHeight = 15;
            this.listBoxUnusedIPs.Location = new System.Drawing.Point(723, 151);
            this.listBoxUnusedIPs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBoxUnusedIPs.Name = "listBoxUnusedIPs";
            this.listBoxUnusedIPs.Size = new System.Drawing.Size(136, 64);
            this.listBoxUnusedIPs.TabIndex = 71;
            // 
            // label1
            // 
            this.label1.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.LightYellow;
            this.label1.Location = new System.Drawing.Point(723, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 15);
            this.label1.TabIndex = 70;
            this.label1.Text = "Host IP4 Address";
            // 
            // listBox1
            // 
            this.listBox1.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(723, 54);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(136, 64);
            this.listBox1.TabIndex = 69;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 248);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 38);
            this.button1.TabIndex = 73;
            this.button1.Text = "Debug";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(872, 624);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxUnusedIPs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.labelMyIP);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Form1";
            this.Text = "TCPiP Server App";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelMyIP;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader ClientIP;
        private System.Windows.Forms.ColumnHeader Computer;
        private System.Windows.Forms.ColumnHeader Version;
        private System.Windows.Forms.ColumnHeader ConnectionID;
        private System.Windows.Forms.ColumnHeader ClientName;
        private System.Windows.Forms.ColumnHeader PingTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.WebBrowser CommunicationsDisplay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxUnusedIPs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
    }
}