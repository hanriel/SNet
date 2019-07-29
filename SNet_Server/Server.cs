using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SNet_Server
{
    /// <summary>
    /// A class that listens on a port for client connections, sends and receives messages from connected clients,
    /// and periodically broadcasts UDP messages.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// A delegate type called when a client initially connects to the server.  Void return type.
        /// </summary>
        /// <param name="clientNumber">A unique identifier of the client that has connected to the server.</param>
        public delegate void ClientConnectCallback(int clientNumber);

        /// <summary>
        /// A delegate type called when a client disconnects from the server.  Void return type.
        /// </summary>
        /// <param name="clientNumber">A unique identifier of the client that has disconnected from the server.</param>
        public delegate void ClientDisconnectCallback(int clientNumber);

        /// <summary>
        /// A delegate type called when the server receives data from a client.
        /// </summary>
        /// <param name="clientNumber">A unique identifier of the client that has disconnected from the server.</param>
        /// <param name="message">A byte array representing the message sent.</param>
        /// <param name="messageSize">The size in bytes of the message.</param>
        public delegate void ReceiveDataCallback(int clientNumber, byte[] message, int messageSize);

        private Socket _mainSocket;
        private Timer _broadcastTimer;
        private int _currentClientNumber;

        public class UserSock
        {
            public UserSock(int nClientId, Socket s)
            {
                ClientId = nClientId;
                UserSocket = s;
                DTimer = DateTime.Now;
                SzStationName = string.Empty;
                SzClientName = string.Empty;
                UserListeningPort = 5678;
                SzAlternateIp = string.Empty;
                PingStatClass = new PingStatsClass();
            }

            public int ClientId { get; }
            public Socket UserSocket { get; }
            public DateTime DTimer { get; set; }
            public string SzClientName { get; set; }
            public string SzStationName { get; set; }
            public ushort UserListeningPort { get; set; }
            public string SzAlternateIp { get; set; }
            public PingStatsClass PingStatClass { get; set; }
            public int ZeroDataCount { get; internal set; }
        }

        // public Dictionary<int, Socket > workerSockets = new Dictionary<int, Socket>();
        public readonly Dictionary<int, UserSock> WorkerSockets = new Dictionary<int, UserSock>();

        public Server()
        {
            OnClientDisconnect = null;
        }


        /// <summary>
        /// Modify the callback function used when a client initially connects to the server.
        /// </summary>
        public ClientConnectCallback OnClientConnect { get; set; }

        /// <summary>
        /// Modify the callback function used when a client disconnects from the server.
        /// </summary>
        public ClientDisconnectCallback OnClientDisconnect { get; set; }

        /// <summary>
        /// Modify the callback function used when the server receives a message from a client.
        /// </summary>
        public ReceiveDataCallback OnReceiveData { get; set; }

        /// <summary>
        /// Whether or not the server is currently listening for new client connections.
        /// </summary>
        public bool IsListening => _mainSocket != null && _mainSocket.IsBound;

        /// <summary>
        /// Make the server listen for client connections on a specific port.
        /// </summary>
        /// <param name="listenPort">The number of the port to listen for connections on.</param>
        public void Listen(int listenPort)
        {
            try
            {
                Stop();

                _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _mainSocket.Bind(new IPEndPoint(IPAddress.Any, listenPort));
                _mainSocket.Listen(100);
                _mainSocket.BeginAccept(OnReceiveConnection, null);
            }

            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        /// <summary>
        /// Остановка принятия новых подключений и закрытие всех текущих открытых соединений.
        /// </summary>
        public void Stop()
        {
            lock (WorkerSockets)
            {
                foreach (UserSock s in WorkerSockets.Values)
                {
                    if (s.UserSocket.Connected)
                        s.UserSocket.Close();
                }

                WorkerSockets.Clear();
            }

            if (IsListening)
                _mainSocket.Close();
        }

        /// <summary>
        /// Send a message to all connected clients.
        /// </summary>
        /// <param name="message">A byte array representing the message to send.</param>
        /// <param name="testConnections"></param>
        public void SendMessage(byte[] message, bool testConnections = false)
        {
            if (testConnections)
            {
                var clientsToRemove = new List<int>();
                foreach (var clientId in WorkerSockets.Keys)
                {
                    if (WorkerSockets[clientId].UserSocket.Connected)
                    {
                        try
                        {
                            WorkerSockets[clientId].UserSocket.Send(message);
                        }
                        catch
                        {
                            clientsToRemove.Add(clientId);
                        }

                        Thread.Sleep(10); // this is for a client Ping so stagger the send messages
                    }
                    else
                        clientsToRemove.Add(clientId);
                }


                if (clientsToRemove.Count > 0)
                {
                    foreach (var cId in clientsToRemove)
                    {
                        OnClientDisconnect?.Invoke(cId);
                    }
                }
                clientsToRemove.Clear();
            }
            else
            {
                foreach (var s in WorkerSockets.Values)
                {
                    try
                    {
                        if (s.UserSocket.Connected)
                            s.UserSocket.Send(message);
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine(se.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Send a message to a specific client.
        /// </summary>
        /// <param name="clientNumber">A unique identifier of the client that has connected to the server.</param>
        /// <param name="message">A byte array representing the message to send.</param>
        public void SendMessage(int clientNumber, byte[] message)
        {
            if (!WorkerSockets.ContainsKey(clientNumber))
            {
                //throw new ArgumentException("Invalid Client Number", "clientNumber");
                Console.WriteLine(@"Invalid Client Number");
                return;
            }

            try
            {
                //workerSockets[clientNumber].Send(message);
                WorkerSockets[clientNumber].UserSocket.Send(message);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        /// <summary>
        /// Begin broadcasting a message over UDP every several seconds.
        /// </summary>
        /// <param name="message">A byte array representing the message to send.</param>
        /// <param name="port">The port over which to send the message.</param>
        /// <param name="frequency">Frequency to send the message in seconds.</param>
        public void BeginBroadcast(byte[] message, int port, int frequency)
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };

            var pack = new Packet(sock, port) {DataBuffer = message};

            _broadcastTimer?.Dispose();

            _broadcastTimer =
                new Timer(BroadcastTimerCallback, pack, 0, frequency * 1000);
        }

        /// <summary>
        /// Stop broadcasting UDP messages.
        /// </summary>
        public void EndBroadcast()
        {
            _broadcastTimer?.Dispose();
        }

        /// <summary>
        /// A callback called by the broadcast timer.  Broadcasts a message.
        /// </summary>
        /// <param name="state">An object representing the byte[] message to be broadcast.</param>
        private void BroadcastTimerCallback(object state)
        {
            ((Packet) state).CurrentSocket.SendTo(((Packet) state).DataBuffer,
                new IPEndPoint(IPAddress.Broadcast, ((Packet) state).ClientNumber));
        }

        /// <summary>
        /// An internal callback triggered when a client connects to the server.
        /// </summary>
        /// <param name="async"></param>
        private void OnReceiveConnection(IAsyncResult async)
        {
            try
            {
                lock (WorkerSockets)
                {
                    Interlocked.Increment(ref _currentClientNumber); // Thread Safe
                    var us = new UserSock(_currentClientNumber, _mainSocket.EndAccept(async));
                    WorkerSockets.Add(_currentClientNumber, us);
                }

                OnClientConnect?.Invoke(_currentClientNumber);

                WaitForData(_currentClientNumber);
                _mainSocket.BeginAccept(OnReceiveConnection, null);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine(@"OnClientConnection: Socket has been closed");
            }
            catch (SocketException se)
            {
                //Console.WriteLine("SERVER EXCEPTION in OnReceiveConnection: " + se.Message);
                Debug.WriteLine("SERVER EXCEPTION in OnReceiveConnection: " +
                                                   se.Message); //pe 4-22-2015

                if (WorkerSockets.ContainsKey(_currentClientNumber))
                {
                    Console.WriteLine(@"RemoteEndPoint: " +
                                      WorkerSockets[_currentClientNumber].UserSocket.RemoteEndPoint);
                    Console.WriteLine(@"LocalEndPoint: " +
                                      WorkerSockets[_currentClientNumber].UserSocket.LocalEndPoint);

                    Console.WriteLine(@"Closing socket from OnReceiveConnection");
                }

                //Socket gets closed and removed from OnClientDisconnect
                OnClientDisconnect?.Invoke(_currentClientNumber);
            }
        }

        /// <summary>
        /// Begins an asynchronous wait for data for a particular client.
        /// </summary>
        /// <param name="clientNumber">A unique identifier of the client that has connected to the server.</param>
        private void WaitForData(int clientNumber)
        {
            if (!WorkerSockets.ContainsKey(clientNumber))
            {
                return;
            }

            try
            {
                var pack = new Packet(WorkerSockets[clientNumber].UserSocket, clientNumber);
                WorkerSockets[clientNumber].UserSocket.BeginReceive(pack.DataBuffer, 0, pack.DataBuffer.Length,
                    SocketFlags.None, OnDataReceived, pack);
            }
            catch (SocketException se)
            {
                try
                {
                    //Socket gets closed and removed from OnClientDisconnect
                    if (OnClientDisconnect != null)
                        OnClientDisconnect(clientNumber);

                    //Console.WriteLine("SERVER EXCEPTION in WaitForClientData: " + se.Message);
                    Debug.WriteLine(
                        $"SERVER EXCEPTION in WaitForClientData: {se.Message}"); //pe 4-22-2015
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception ex)
            {
                //Socket gets closed and removed from OnClientDisconnect
                if (OnClientDisconnect != null)
                    OnClientDisconnect(clientNumber);

                var msg = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Debug.WriteLine($"SERVER EXCEPTION in WaitForClientData2: {msg}"); //pe 5-3-2017
            }
        }

        /// <summary>
        /// An internal callback triggered when the server recieves data from a client.
        /// </summary>
        /// <param name="async"></param>
        private void OnDataReceived(IAsyncResult async)
        {
            var socketData = (Packet) async.AsyncState;

            try
            {
                var dataSize = socketData.CurrentSocket.EndReceive(async);

                if (dataSize.Equals(0))
                {
                    if (WorkerSockets.ContainsKey(socketData.ClientNumber))
                    {
                        if (WorkerSockets[socketData.ClientNumber].ZeroDataCount++ == 10)
                        {
                            OnClientDisconnect?.Invoke(socketData.ClientNumber);
                        }
                    }
                }
                else
                {
                    OnReceiveData(socketData.ClientNumber, socketData.DataBuffer, dataSize);
                    WorkerSockets[socketData.ClientNumber].ZeroDataCount = 0;
                }

                WaitForData(socketData.ClientNumber);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine(@"OnDataReceived: Socket has been closed");

                //Socket gets closed and removed from OnClientDisconnect
                OnClientDisconnect?.Invoke(socketData.ClientNumber);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054 || se.ErrorCode == 10060) //10054 - Error code for Connection reset by peer
                {
                    try
                    {
                        Debug.WriteLine(
                            $"SERVER EXCEPTION in OnClientDataReceived, ServerObject removed:({se.ErrorCode}) {socketData.ClientNumber}, (happens during a normal client exit)");
                        Debug.WriteLine("RemoteEndPoint: " +
                                        WorkerSockets[socketData.ClientNumber].UserSocket
                                            .RemoteEndPoint);
                        Debug.WriteLine("LocalEndPoint: " +
                                        WorkerSockets[socketData.ClientNumber].UserSocket
                                            .LocalEndPoint);
                    }
                    catch
                    {
                        // ignored
                    }

                    //Socket gets closed and removed from OnClientDisconnect
                    OnClientDisconnect?.Invoke(socketData.ClientNumber);

                    Console.WriteLine(@"Closing socket from OnDataReceived");
                }
                else
                {
                    string mess = "CONNECTION BOOTED for reason other than 10054: code = " + se.ErrorCode.ToString() +
                                  ",   " + se.Message;
                    Console.WriteLine(mess);
                    ToFile(mess);
                }
            }
        }

        /// <summary>
        /// Represents a TCP/IP transmission containing the socket it is using, the clientNumber
        ///  (used by server communication only), and a data buffer representing the message.
        /// </summary>
        private class Packet
        {
            public Socket CurrentSocket;
            public int ClientNumber;

            public byte[] DataBuffer = new byte[1024];
            //public byte[] DataBuffer = new byte[4096];

            /// <summary>
            /// Construct a Packet Object
            /// </summary>
            /// <param name="sock">The socket this Packet is being used on.</param>
            /// <param name="client">The client number that this packet is from.</param>
            public Packet(Socket sock, int client)
            {
                CurrentSocket = sock;
                ClientNumber = client;
            }
        }

        private void ToFile(string message)
        {
            var appPath = SNet_Utils.GeneralFunctions.GetAppPath;

            System.IO.StreamWriter sw = null;
            try
            {
                sw = System.IO.File.AppendText(System.IO.Path.Combine(appPath, "ServerSocketIssue.txt"));
                var logLine = $"{DateTime.Now:G}: {message}.";
                sw.WriteLine(logLine);
            }
            catch // (Exception ex)
            {
                //Console.WriteLine("\n\nError in ToFile:\n" + message + "\n" + ex.Message + "\n\n");
                // System.Windows.Forms.MessageBox.Show("ERROR:\n\n" + ex.Message, "Possible Permissions Issue!");
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

    public class PingStatsClass
    {
        public PingStatsClass() //Int32 ClientID)
        {
            //clientID = ClientID;
            _sw = new Stopwatch();
            PingCounter = 0;
            PingTimeTotal = 0;
            LongestPing = 0;
            LongestPingDateTimeStamp = DateTime.Now;
        }

        private Stopwatch _sw;

        public int PingCounter;

        /// <summary>
        /// Time is in milliseconds
        /// </summary>
        public long PingTimeTotal;

        /// <summary>
        /// Time is in milliseconds
        /// </summary>
        public long LongestPing;

        public DateTime LongestPingDateTimeStamp;

        /// <summary>
        /// returns the elapsed ping time in miliseconds
        /// </summary>
        /// <returns></returns>
        public long StopTheClock()
        {
            if (_sw.IsRunning)
            {
                _sw.Stop();

                PingCounter++;

                if (_sw.ElapsedMilliseconds > LongestPing)
                {
                    LongestPing = _sw.ElapsedMilliseconds;
                    LongestPingDateTimeStamp = DateTime.Now;
                }

                PingTimeTotal += _sw.ElapsedMilliseconds;
            }

            return _sw.ElapsedMilliseconds;
        }

        public void StartTheClock()
        {
            _sw.Reset();

            if (!_sw.IsRunning)
                _sw.Start();
            else
                _sw.Restart();
        }

        public long GetElapsedTime => _sw.ElapsedMilliseconds;
    }
}