using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Fiddler;

namespace SNet
{   public partial class f_Main : Form
    {
        private UrlCaptureConfiguration CaptureConfiguration { get; set; }

        public f_Main()
        {
            InitializeComponent();
            CaptureConfiguration = new UrlCaptureConfiguration();
            Start();
        }

        private void FiddlerCapture_FormClosing(object sender, FormClosingEventArgs e) { Stop(); }
       

        void Start()
        {
            InstallCertificate();

            if (tbIgnoreResources.Checked)
            {
                CaptureConfiguration.IgnoreResources = true;
            }
            else
            {
                CaptureConfiguration.IgnoreResources = false;
            }

            string strProcId = txtProcessId.Text;
            if (strProcId.Contains('-'))
                strProcId = strProcId.Substring(strProcId.IndexOf('-') + 1).Trim();

            strProcId = strProcId.Trim();

            int procId = 0;
            if (!string.IsNullOrEmpty(strProcId))
            {
                if (!int.TryParse(strProcId, out procId))
                    procId = 0;
            }

            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            Fiddler.CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.Startup(8888, true, true, false);
        }

        void Stop()
        {
            FiddlerApplication.AfterSessionComplete -= FiddlerApplication_AfterSessionComplete;

            if (FiddlerApplication.IsStarted())
                FiddlerApplication.Shutdown();
            UninstallCertificate();
        }

        public static bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;

                if (!CertMaker.trustRootCert())
                    return false;
            }

            return true;
        }

        public static bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }
            return true;
        }

        private void ButtonHandler(object sender, EventArgs e)
        {
            if (sender == tbCapture) Start();
            else if (sender == tbStop) Stop();
            else if (sender == tbSave)
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "txt",
                    Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Save Fiddler Capture File",
                    RestoreDirectory = true
                };
                var res = diag.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    File.WriteAllText(diag.FileName, txtCapture.Text);


                }
            }
            else if (sender == tbClear) txtCapture.Text = string.Empty;

            UpdateButtonStatus();
        }

        private void FiddlerApplication_AfterSessionComplete(Session sess)
        {
            // Ignore HTTPS connect requests
            if (sess.RequestMethod == "CONNECT") return;

            if (CaptureConfiguration.ProcessId > 0)
            {
                if (sess.LocalProcessID != 0 && sess.LocalProcessID != CaptureConfiguration.ProcessId) return;
            }

            if (!string.IsNullOrEmpty(CaptureConfiguration.CaptureDomain))
            {
                if (sess.hostname.ToLower() != CaptureConfiguration.CaptureDomain.Trim().ToLower()) return;
            }

            if (CaptureConfiguration.IgnoreResources)
            {
                string url = sess.fullUrl.ToLower();

                var extensions = CaptureConfiguration.ExtensionFilterExclusions;
                foreach (var ext in extensions)
                {
                    if (url.Contains(ext))
                        return;
                }

                var filters = CaptureConfiguration.UrlFilterExclusions;
                foreach (var urlFilter in filters)
                {
                    if (url.Contains(urlFilter))
                        return;
                }
            }

            if (sess == null || sess.oRequest == null || sess.oRequest.headers == null)
                return;

            //string headers = sess.oRequest.headers.ToString();
            //var reqBody = Encoding.UTF8.GetString(sess.RequestBody);

            // if you wanted to capture the response
            //string respHeaders = session.oResponse.headers.ToString();
            //var respBody = Encoding.UTF8.GetString(session.ResponseBody);

            // replace the HTTP line to inject full URL
            string firstLine = sess.RequestMethod + " " + sess.fullUrl + "\r\n";
            //int at = headers.IndexOf("\r\n");
            //if (at < 0)
            //    return;
            //headers = firstLine + "\r\n" + headers.Substring(at + 1);

            // must marshal to UI thread
            BeginInvoke(new Action<string>((text) =>
            {
                txtCapture.AppendText(text);
                UpdateButtonStatus();
            }), firstLine);

        }


        

        public void UpdateButtonStatus()
        {
            tbCapture.Enabled = !FiddlerApplication.IsStarted();
            tbStop.Enabled = !tbCapture.Enabled;
            tbSave.Enabled = txtCapture.Text.Length > 0;
            tbClear.Enabled = tbSave.Enabled;

            if (CertMaker.rootCertExists()) la_certState.Text = "Installed";
            else la_certState.Text = "Don't Installed";

            if (!tbCapture.Enabled) la_State.Text = "Started";
            else la_State.Text = "Stopped";

            CaptureConfiguration.IgnoreResources = tbIgnoreResources.Checked;
        }

        private void fEventLoad(object sender, EventArgs e)
        {
            tbIgnoreResources.Checked = CaptureConfiguration.IgnoreResources;
            txtCaptureDomain.Text = CaptureConfiguration.CaptureDomain;

            UpdateButtonStatus();

            try
            {
                var processes = Process.GetProcesses().OrderBy(p => p.ProcessName);
                foreach (var process in processes)
                {
                    txtProcessId.Items.Add(process.ProcessName + "  - " + process.Id);
                }
            }
            catch { }
        }
    }


}
