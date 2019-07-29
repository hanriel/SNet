using System;
using System.Net;
using System.Net.Sockets;

namespace SNet_Client
{
    public class Client
    {
        /// <summary>
        /// A delegate type called when a client receives data from a server.  Void return type.
        /// </summary>
        /// <param name="message">A byte array representing the message received from the server.</param>
        /// <param name="messageSize">The size, in bytes of the message.</param>
        public delegate void ReceiveDataCallback(byte[] message, int messageSize);

        /// <summary>
        /// A delegate type called when a client receives a broadcast message.  Void return type.
        /// </summary>
        /// <param name="message">A byte array representing the message received from the server.</param>
        /// <param name="messageSize">The size, in bytes of the message.</param>
        private delegate void ReceiveBroadcastCallback(byte[] message, int messageSize);

        /// <summary>
        /// A delegate called when disconnected from the server.
        /// </summary>
        public delegate void DisconnectCallback();

        private Socket _broadcastSocket;
        private bool _receiveBroadcasts;
        private int _broadcastPort;
        public DateTime LastDataFromServer;

        /// <summary>
        /// Modify the callback function used when data is received from the server.
        /// </summary>
        public ReceiveDataCallback OnReceiveData { get; set; }

        /// <summary>
        /// Modify the callback function used when data is received from the server.
        /// </summary>
        private ReceiveBroadcastCallback OnReceiveBroadcast { get; } = null;

        /// <summary>
        /// Modify the callback function used when data is received from the server.
        /// </summary>
        public DisconnectCallback OnDisconnected { get; set; }

        public bool Connected => GetTheSocket != null && GetTheSocket.Connected;

        public bool ReceiveBroadcasts
        {
            get => _receiveBroadcasts;

            set
            {
                if (_receiveBroadcasts == value) return;

                _receiveBroadcasts = value;
                if (!_receiveBroadcasts)
                    _broadcastSocket?.Close();
                else if (_broadcastPort > 0)
                    SetupBroadcastSocket();
            }
        }

        public int BroadcastPort
        {
            get => _broadcastPort;

            set
            {
                if (_broadcastPort == value) return;

                _broadcastPort = value;

                if (_receiveBroadcasts)
                    SetupBroadcastSocket();
            }
        }

        ///// <summary>
        ///// Returns the Socket owned by this class. I may not need it!
        ///// </summary>
        private Socket GetTheSocket { get; set; }

        /// <summary>
        /// Construct a Client setting the callback.
        /// </summary>
        public Client()
        {
            LastDataFromServer = DateTime.Now; //initialize to current time
        }

        /// <summary>
        /// Connect to a server at a specific IP address and port.
        /// </summary>
        /// <param name="address">The IP address of the server to connect to.  
        /// To get an IP address from a string use System.Net.IPAddress.Parse("0.0.0.0")</param>
        /// <param name="port">The port number on the server to connect to.</param>
        public void Connect(IPAddress address, int port)
        {
            try
            {
                Disconnect();

                GetTheSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GetTheSocket.Connect(new IPEndPoint(address, port));

                if (GetTheSocket.Connected)
                    WaitForData();
            }

            catch (SocketException se)
            {
                Console.WriteLine(@"Client EXCEPTION in Connect: " + se.Message);
                ToFile(se.Message);
            }
        }

        /// <summary>
        /// Disconnect a client from a server.  If the client is not connected to a server when this function is called, there is no effect.
        /// </summary>
        public void Disconnect()
        {
            GetTheSocket?.Close();
        }

        /// <summary>
        /// Send a message to the server we are connected to.
        /// </summary>
        /// <param name="message">A byte array representing the message to send.</param>
        public void SendMessage(byte[] message)
        {
            if (GetTheSocket == null) return;

            if (GetTheSocket.Connected)
                GetTheSocket.Send(message);
        }

        /// <summary>
        /// Start an asynchronous wait for data from the server.  When data is received, a callback will be triggered.
        /// </summary>
        private void WaitForData()
        {
            try
            {
                var pack = new Packet(GetTheSocket);
                GetTheSocket.BeginReceive(pack.DataBuffer, 0, pack.DataBuffer.Length, SocketFlags.None,
                    OnDataReceived, pack);
            }

            catch (SocketException se)
            {
                Console.WriteLine(@"Client EXCEPTION in WaitForData: " + se.Message);
                ToFile(se.Message);
            }
        }

        /// <summary>
        /// A callback triggered by receiving data from the server.
        /// </summary>
        /// <param name="async">The packet object received from the server containing the received message.</param>
        private void OnDataReceived(IAsyncResult async)
        {
            try
            {
                var socketData = (Packet) async.AsyncState;
                var dataSize = socketData.CurrentSocket.EndReceive(async);

                OnReceiveData?.Invoke(socketData.DataBuffer, dataSize);

                WaitForData();
            }

            catch (ObjectDisposedException)
            {
                Console.WriteLine(@"Client EXCEPTION in OnDataReceived: Socket has been closed");
            }

            catch (SocketException se)
            {
                Console.WriteLine(@"Client EXCEPTION in OnDataReceived: " + se.Message);

                OnDisconnected?.Invoke();

                ToFile(se.Message);
            }
        }

        private void SetupBroadcastSocket()
        {
            _broadcastSocket?.Close();

            _broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _broadcastSocket.Bind(new IPEndPoint(IPAddress.Any, _broadcastPort));

            WaitForBroadcast();
        }

        /// <summary>
        /// Start an asynchronous wait for data from the server.  When data is received, a callback will be triggered.
        /// </summary>
        private void WaitForBroadcast()
        {
            try
            {
                var pack = new Packet(_broadcastSocket);
                EndPoint port = new IPEndPoint(IPAddress.Any, _broadcastPort);

                _broadcastSocket.BeginReceiveFrom(pack.DataBuffer, 0, pack.DataBuffer.Length, SocketFlags.None,
                    ref port, OnBroadcastReceived, pack);
            }

            catch (SocketException se)
            {
                Console.WriteLine(@"Client EXCEPTION in WaitForBroadcast: " + se.Message);
            }
        }

        private void OnBroadcastReceived(IAsyncResult async)
        {
            try
            {
                var socketData = (Packet) async.AsyncState;
                var dataSize = socketData.CurrentSocket.EndReceive(async);

                OnReceiveBroadcast?.Invoke(socketData.DataBuffer, dataSize);

                WaitForBroadcast();
            }

            catch (ObjectDisposedException)
            {
                Console.WriteLine(@"Client EXCEPTION in OnBroadcastReceived: Socket has been closed");
            }

            catch (SocketException se)
            {
                Console.WriteLine(@"Client EXCEPTION in OnBroadcastReceived: " + se.Message);
            }
        }

        /// <summary>
        /// Represents a TCP/IP transmission containing the socket it is using, the clientNumber
        ///  (used by server communication only), and a data buffer representing the message.
        /// </summary>
        private class Packet
        {
            public readonly Socket CurrentSocket;
            public readonly byte[] DataBuffer = new byte[1024];

            /// <summary>
            /// Construct a Packet Object
            /// </summary>
            /// <param name="sock">The socket this Packet is being used on.</param>
            public Packet(Socket sock)
            {
                CurrentSocket = sock;
            }
        }

        private static void ToFile(string message)
        {
            var appPath = SNet_Utils.GeneralFunctions.GetAppPath;

            System.IO.StreamWriter sw = null;
            try
            {
                sw = System.IO.File.AppendText(System.IO.Path.Combine(appPath, "SockClient.txt"));
                sw.WriteLine($"{DateTime.Now:G}: {message}.");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($@"ERROR: {ex.Message}, Possible Permissions Issue!");
            }
            finally
            {
                try
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}