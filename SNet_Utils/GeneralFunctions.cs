using System;

namespace SNet_Utils
{
    public static class GeneralFunctions
    {
        public static string GetAppPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TCPIPServerClient");

        public static string GetDateTimeFormatted => DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToLongTimeString();
    }
}