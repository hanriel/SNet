using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SNet
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new f_Main());


            var limit = ServicePointManager.DefaultConnectionLimit;
            if (ServicePointManager.DefaultConnectionLimit < 10) ServicePointManager.DefaultConnectionLimit = 200;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            try
            {
                Application.Run(new f_Main());
            }
            catch (Exception ex)
            {
                Application_ThreadException(null, new ThreadExceptionEventArgs(ex));
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            //App.Log(ex);

            var msg = string.Format("Yikes! Something went wrong...\r\n\r\n{0}\r\n" +
                "The error has been recorded and written to a log file and you can\r\n" +
                "review the details or report the error via Help | Show Error Log\r\n\r\n" +
                "Do you want to continue?", ex.Message);

            DialogResult res = MessageBox.Show(msg, " Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (res == DialogResult.No) Application.Exit();
        }

    }
}
