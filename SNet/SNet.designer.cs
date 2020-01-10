namespace SNet
{
    partial class f_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f_Main));
            this.txtCapture = new System.Windows.Forms.TextBox();
            this.MainToolbar = new System.Windows.Forms.ToolStrip();
            this.tbCapture = new System.Windows.Forms.ToolStripButton();
            this.tbStop = new System.Windows.Forms.ToolStripButton();
            this.tbSave = new System.Windows.Forms.ToolStripButton();
            this.tbClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbIgnoreResources = new System.Windows.Forms.ToolStripButton();
            this.lblCaptureDomain = new System.Windows.Forms.ToolStripLabel();
            this.txtCaptureDomain = new System.Windows.Forms.ToolStripTextBox();
            this.tblblProcessId = new System.Windows.Forms.ToolStripLabel();
            this.txtProcessId = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.la_certState = new System.Windows.Forms.Label();
            this.la_State = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MainToolbar.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCapture
            // 
            this.txtCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCapture.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtCapture.Location = new System.Drawing.Point(10, 28);
            this.txtCapture.Multiline = true;
            this.txtCapture.Name = "txtCapture";
            this.txtCapture.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCapture.Size = new System.Drawing.Size(463, 483);
            this.txtCapture.TabIndex = 1;
            // 
            // MainToolbar
            // 
            this.MainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbCapture,
            this.tbStop,
            this.tbSave,
            this.tbClear,
            this.toolStripSeparator1,
            this.tbIgnoreResources,
            this.lblCaptureDomain,
            this.txtCaptureDomain,
            this.tblblProcessId,
            this.txtProcessId,
            this.toolStripSeparator3});
            this.MainToolbar.Location = new System.Drawing.Point(0, 0);
            this.MainToolbar.Name = "MainToolbar";
            this.MainToolbar.Size = new System.Drawing.Size(717, 25);
            this.MainToolbar.TabIndex = 5;
            // 
            // tbCapture
            // 
            this.tbCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbCapture.Image = ((System.Drawing.Image)(resources.GetObject("tbCapture.Image")));
            this.tbCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbCapture.Name = "tbCapture";
            this.tbCapture.Size = new System.Drawing.Size(23, 22);
            this.tbCapture.Text = "Capture";
            this.tbCapture.ToolTipText = "Start capturing HTTP requests.";
            this.tbCapture.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // tbStop
            // 
            this.tbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbStop.Image = ((System.Drawing.Image)(resources.GetObject("tbStop.Image")));
            this.tbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbStop.Name = "tbStop";
            this.tbStop.Size = new System.Drawing.Size(23, 22);
            this.tbStop.Text = "Stop Capture";
            this.tbStop.ToolTipText = "Stop capturing HTTP Requests";
            this.tbStop.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // tbSave
            // 
            this.tbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSave.Image = ((System.Drawing.Image)(resources.GetObject("tbSave.Image")));
            this.tbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSave.Name = "tbSave";
            this.tbSave.Size = new System.Drawing.Size(23, 22);
            this.tbSave.Text = "Save";
            this.tbSave.ToolTipText = "Saves captured URLs to a file";
            this.tbSave.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // tbClear
            // 
            this.tbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbClear.Image = ((System.Drawing.Image)(resources.GetObject("tbClear.Image")));
            this.tbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbClear.Name = "tbClear";
            this.tbClear.Size = new System.Drawing.Size(23, 22);
            this.tbClear.Text = "Clear";
            this.tbClear.ToolTipText = "Clears the captured Urls";
            this.tbClear.Click += new System.EventHandler(this.ButtonHandler);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbIgnoreResources
            // 
            this.tbIgnoreResources.CheckOnClick = true;
            this.tbIgnoreResources.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbIgnoreResources.Image = ((System.Drawing.Image)(resources.GetObject("tbIgnoreResources.Image")));
            this.tbIgnoreResources.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbIgnoreResources.Name = "tbIgnoreResources";
            this.tbIgnoreResources.Size = new System.Drawing.Size(23, 22);
            this.tbIgnoreResources.ToolTipText = "Ignore Images, CSS and JavaScript links";
            // 
            // lblCaptureDomain
            // 
            this.lblCaptureDomain.Name = "lblCaptureDomain";
            this.lblCaptureDomain.Size = new System.Drawing.Size(52, 22);
            this.lblCaptureDomain.Text = "Domain:";
            // 
            // txtCaptureDomain
            // 
            this.txtCaptureDomain.Name = "txtCaptureDomain";
            this.txtCaptureDomain.Size = new System.Drawing.Size(150, 25);
            this.txtCaptureDomain.ToolTipText = "Capture only content in this domain";
            // 
            // tblblProcessId
            // 
            this.tblblProcessId.Name = "tblblProcessId";
            this.tblblProcessId.Size = new System.Drawing.Size(63, 22);
            this.tblblProcessId.Text = "Process Id:";
            // 
            // txtProcessId
            // 
            this.txtProcessId.Name = "txtProcessId";
            this.txtProcessId.Size = new System.Drawing.Size(150, 25);
            this.txtProcessId.ToolTipText = "Limit capturing to a specific process on this machine";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "SSL certificate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Writing:";
            // 
            // la_certState
            // 
            this.la_certState.AutoSize = true;
            this.la_certState.Location = new System.Drawing.Point(82, 16);
            this.la_certState.Name = "la_certState";
            this.la_certState.Size = new System.Drawing.Size(38, 13);
            this.la_certState.TabIndex = 7;
            this.la_certState.Text = "label3";
            // 
            // la_State
            // 
            this.la_State.AutoSize = true;
            this.la_State.Location = new System.Drawing.Point(52, 29);
            this.la_State.Name = "la_State";
            this.la_State.Size = new System.Drawing.Size(38, 13);
            this.la_State.TabIndex = 8;
            this.la_State.Text = "label4";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "SSL CERTIFICATE",
            "CAPTURING"});
            this.listBox1.Location = new System.Drawing.Point(5, 45);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(219, 433);
            this.listBox1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.la_State);
            this.groupBox1.Controls.Add(this.la_certState);
            this.groupBox1.Location = new System.Drawing.Point(478, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 486);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stat";
            // 
            // f_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 521);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.MainToolbar);
            this.Controls.Add(this.txtCapture);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "f_Main";
            this.Text = "School.NET";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FiddlerCapture_FormClosing);
            this.Load += new System.EventHandler(this.fEventLoad);
            this.MainToolbar.ResumeLayout(false);
            this.MainToolbar.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCapture;
        private System.Windows.Forms.ToolStrip MainToolbar;
        private System.Windows.Forms.ToolStripButton tbCapture;
        private System.Windows.Forms.ToolStripButton tbStop;
        private System.Windows.Forms.ToolStripButton tbSave;
        private System.Windows.Forms.ToolStripButton tbIgnoreResources;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbClear;
        private System.Windows.Forms.ToolStripLabel tblblProcessId;
        private System.Windows.Forms.ToolStripLabel lblCaptureDomain;
        private System.Windows.Forms.ToolStripTextBox txtCaptureDomain;
        private System.Windows.Forms.ToolStripComboBox txtProcessId;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label la_certState;
        private System.Windows.Forms.Label la_State;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}