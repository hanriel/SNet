using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SNet_Utils;

namespace SNet_Client
{
    public partial class Form1 : Form
    {
        /*******************************************************/
        private Client _client;

        private MotherOfRawPackets _hostServerRawPackets;
        static AutoResetEvent _autoEventHostServer; //mutex
        private static AutoResetEvent _autoEvent2; //mutex
        private Thread _dataProcessHostServerThread;
        private Thread FullPacketDataProcessThread;

        private Queue<FullPacket> _fullHostServerPacketList;
        /*******************************************************/

        private bool _appIsExiting;
        public bool ServerConnected;
        private int _myHostServerId;
        private long _serverTime = DateTime.Now.Ticks;

        private System.Windows.Forms.Timer _generalTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /**********************************************/
            //Create a directory we can write stuff too
            CheckOnApplicationDirectory();
            /**********************************************/

            textBoxText.Text = @"TAMPA, Fla. – Tampa police say a witness was able to provide a description of a suspect who is believed to have shot a man from behind";
            
            
            var p = Environment.MachineName;
            var versionNumber = Assembly.GetEntryAssembly().GetName().Version.Major + "." +
                                Assembly.GetEntryAssembly().GetName().Version.Minor + "." +
                                Assembly.GetEntryAssembly().GetName().Version.Build;


            la_info.Text = $@"Machine Name: {p}" + Environment.NewLine + $@"Program ver: {versionNumber}";

            Text += versionNumber;
            ConnectButton();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoServerDisconnect();
            _appIsExiting = true;
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            ConnectButton();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            TellServerImDisconnecting();
            DoServerDisconnect();
            buttonDisconnect.Enabled = false;
            buttonSendDataToServer.Enabled = false;
            ServerConnected = false;
            labelStatusInfo.Text = "Disconnected";
            labelStatusInfo.ForeColor = System.Drawing.Color.Red;
            buttonConnectToServer.Enabled = true;
            SetSomeLabelInfoFromThread("...");
        }

        private void buttonSendDataToServer_Click(object sender, EventArgs e)
        {
            PacketData xdata = new PacketData();

            /****************************************************************/
            //prepair the start packet
            xdata.Packet_Type = (ushort) PacketTypes.Message;
            xdata.Data_Type = (ushort) PacketTypesSubMessage.Start;
            xdata.Packet_Size = 16;
            xdata.maskTo = 0;
            xdata.idTo = 0;
            xdata.idFrom = 0;

            //Before we send the text, lets stuff those Number values in the first data packet!
            int num1 = 0;
            int.TryParse(textBoxNum1.Text, out num1);
            xdata.Data16 = num1;
            int num2 = 0;
            int.TryParse(textBoxNum2.Text, out num2);
            xdata.Data17 = num2;

            int pos = 0;
            int chunkSize = xdata.szStringDataA.Length; //300 bytes

            if (textBoxText.Text.Length <= xdata.szStringDataA.Length)
            {
                textBoxText.Text.CopyTo(0, xdata.szStringDataA, 0, textBoxText.Text.Length);
                chunkSize = textBoxText.Text.Length;
            }
            else
                textBoxText.Text.CopyTo(0, xdata.szStringDataA, 0, xdata.szStringDataA.Length);

            xdata.Data1 = (uint) chunkSize;

            byte[] byData = PacketFunctions.StructureToByteArray(xdata);

            SendMessageToServer(byData);

            /**************************************************/
            //Send the message body(if there is any)
            xdata.Data_Type = (ushort) PacketTypesSubMessage.Guts;
            pos = chunkSize; //set position
            while (true)
            {
                int PosFromEnd = textBoxText.Text.Length - pos;

                if (PosFromEnd <= 0)
                    break;

                Array.Clear(xdata.szStringDataA, 0,
                    xdata.szStringDataA.Length); //Clear this field before putting more data in it

                if (PosFromEnd < xdata.szStringDataA.Length)
                    chunkSize = textBoxText.Text.Length - pos;
                else
                    chunkSize = xdata.szStringDataA.Length;

                textBoxText.Text.CopyTo(pos, xdata.szStringDataA, 0, chunkSize);
                xdata.Data1 = (uint) chunkSize;
                pos += chunkSize; //set new position

                byData = PacketFunctions.StructureToByteArray(xdata);
                SendMessageToServer(byData);
            }

            /**************************************************/
            //Send an EndMessage
            xdata.Data_Type = (ushort) PacketTypesSubMessage.End;
            xdata.Data1 = (uint) pos; //send the total which should be the 'pos' value
            byData = PacketFunctions.StructureToByteArray(xdata);
            SendMessageToServer(byData);
        }

        private void ConnectButton()
        {
            ServerConnected = true; //Set this before initializing the connection loops
            InitializeServerConnection();
            if (ConnectToHostServer())
            {
                ServerConnected = true;
                buttonConnectToServer.Enabled = false;
                buttonDisconnect.Enabled = true;
                buttonSendDataToServer.Enabled = true;
                labelStatusInfo.Text = "Connected!!";
                labelStatusInfo.ForeColor = Color.Green;
                BeginGeneralTimer();
            }
            else
            {
                ServerConnected = false;
                labelStatusInfo.Text = "Can't connect";
                labelStatusInfo.ForeColor = Color.Red;
            }
        }
        private bool ConnectToHostServer()
        {
            try
            {
                if (_client == null)
                {
                    _client = new Client();
                    _client.OnDisconnected += OnDisconnect;
                    _client.OnReceiveData += OnDataReceive;
                }
                else
                {
                    //if we get here then we already have a client object so see if we are already connected
                    if (_client.Connected)
                        return true;
                }

                string szIPstr = GetSHubAddress();
                if (szIPstr.Length == 0)
                {
                    return false;
                }

                int port = 0;
                if (!int.TryParse(textBoxServerListeningPort.Text, out port))
                    port = 9999;

                IPAddress ipAdd = IPAddress.Parse(szIPstr);
                _client.Connect(ipAdd, port); //(int)GeneralSettings.HostPort);

                if (_client.Connected)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: ConnectToHostServer - {exceptionMessage}");
            }

            return false;
        }

        bool ImDisconnecting;

        public void DoServerDisconnect()
        {
            var Line = 0;
            if (ImDisconnecting)
                return;

            ImDisconnecting = true;

            Console.WriteLine("\nIN DoServerDisconnect\n");
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(DoServerDisconnect));
                    return;
                }


                int i = 0;
                Line = 1;


                if (_client != null)
                {
                    TellServerImDisconnecting();
                    Thread.Sleep(75); // this is needed!
                }

                Line = 4;

                ServerConnected = false;

                DestroyGeneralTimer();

                Line = 5;


                /***************************************************/
                try
                {
                    //bust out of the data loops
                    if (_autoEventHostServer != null)
                    {
                        _autoEventHostServer.Set();

                        i = 0;
                        while (_dataProcessHostServerThread.IsAlive)
                        {
                            Thread.Sleep(1);
                            if (i++ > 200)
                            {
                                _dataProcessHostServerThread.Abort();
                                //Debug.WriteLine("\nHAD TO ABORT PACKET THREAD\n");
                                break;
                            }
                        }

                        _autoEventHostServer.Close();
                        _autoEventHostServer.Dispose();
                        _autoEventHostServer = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DoServerDisconnectA: {ex.Message}");
                }

                Line = 8;
                if (_autoEvent2 != null)
                {
                    _autoEvent2.Set();

                    _autoEvent2.Close();
                    _autoEvent2.Dispose();
                    _autoEvent2 = null;
                }
                /***************************************************/

                Line = 9;
                //Debug.WriteLine("AppIsExiting = " + AppIsExiting.ToString());
                if (_client != null)
                {
                    if (_client.OnReceiveData != null)
                        _client.OnReceiveData -= OnDataReceive;
                    if (_client.OnDisconnected != null)
                        _client.OnDisconnected -= OnDisconnect;

                    _client.Disconnect();
                    _client = null;
                }

                Line = 10;

                try
                {
                    Line = 13;
                    //buttonConnect.Text = "Connect";
                    labelStatusInfo.Text = "NOT Connected";
                    Line = 14;
                    labelStatusInfo.ForeColor = System.Drawing.Color.Red;
                }
                catch
                {
                }

                Line = 15;

                buttonConnectToServer.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DoServerDisconnectB: {ex.Message}");
            }
            finally
            {
                ImDisconnecting = false;
            }

            return;
        }

        private void InitializeServerConnection()
        {
            try
            {
                /**** Packet processor mutex, loop and other support variables *************************/
                _autoEventHostServer = new AutoResetEvent(false); //the data mutex
                _autoEvent2 = new AutoResetEvent(false); //the FullPacket data mutex
                FullPacketDataProcessThread = new Thread(new ThreadStart(ProcessRecievedServerData));
                _dataProcessHostServerThread = new Thread(new ThreadStart(NormalizeServerRawPackets));


                if (_hostServerRawPackets == null)
                    _hostServerRawPackets = new MotherOfRawPackets(0);
                else
                {
                    _hostServerRawPackets.ClearList();
                }

                if (_fullHostServerPacketList == null)
                    _fullHostServerPacketList = new Queue<FullPacket>();
                else
                {
                    lock (_fullHostServerPacketList)
                        _fullHostServerPacketList.Clear();
                }
                /***************************************************************************************/


                FullPacketDataProcessThread.Start();
                _dataProcessHostServerThread.Start();

                labelStatusInfo.Text = "Connecting...";
                labelStatusInfo.ForeColor = System.Drawing.Color.Navy;
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: InitializeServerConnection - {exceptionMessage}");
            }
        }

        #region Callbacks from the TCPIP client layer

        /// <summary>
        /// Data coming in from the TCPIP server
        /// </summary>
        private void OnDataReceive(byte[] message, int messageSize)
        {
            if (_appIsExiting)
                return;
            //Console.WriteLine($"Raw Data From: Host Server, Size of Packet: {messageSize}");
            _hostServerRawPackets.AddToList(message, messageSize);
            if (_autoEventHostServer != null)
                _autoEventHostServer.Set(); //Fire in the hole
        }

        /// <summary>
        /// Server disconnected
        /// </summary>
        private void OnDisconnect()
        {
            //Debug.WriteLine("Something Disconnected!! - OnDisconnect()");
            DoServerDisconnect();
        }

        #endregion

        internal void SendMessageToServer(byte[] byData)
        {
            //TimeSpan ts = client.LastDataFromServer

            if (_client.Connected)
                _client.SendMessage(byData);
        }

        #region Packet factory Processing from server

        private void NormalizeServerRawPackets()
        {
            try
            {
                Console.WriteLine($"NormalizeServerRawPackets ThreadID = {Thread.CurrentThread.ManagedThreadId}");

                while (ServerConnected)
                {
                    //ods.DebugOut("Before AutoEvent");
                    _autoEventHostServer.WaitOne(10000); //wait at mutex until signal
                    //autoEventHostServer.WaitOne();//wait at mutex until signal
                    //ods.DebugOut("After AutoEvent");

                    if (_appIsExiting || this.IsDisposed)
                        break;

                    /**********************************************/

                    if (_hostServerRawPackets.GetItemCount == 0)
                        continue;

                    //byte[] packetplayground = new byte[45056];//good for 10 full packets(40960) + 1 remainder(4096)
                    byte[] packetplayground = new byte[11264]; //good for 10 full packets(10240) + 1 remainder(1024)
                    RawPackets rp;

                    int actualPackets = 0;

                    while (true)
                    {
                        if (_hostServerRawPackets.GetItemCount == 0)
                            break;

                        int holdLen = 0;

                        if (_hostServerRawPackets.BytesRemaining > 0)
                            Copy(_hostServerRawPackets.Remainder, 0, packetplayground, 0,
                                _hostServerRawPackets.BytesRemaining);

                        holdLen = _hostServerRawPackets.BytesRemaining;

                        for (int i = 0;
                            i < 10;
                            i++) //only go through a max of 10 times so there will be room for any remainder
                        {
                            rp = _hostServerRawPackets.GetTopItem;

                            Copy(rp.DataChunk, 0, packetplayground, holdLen, rp.ChunkLen);

                            holdLen += rp.ChunkLen;

                            if (_hostServerRawPackets.GetItemCount == 0
                            ) //make sure there is more in the list befor continuing
                                break;
                        }

                        actualPackets = 0;

                        #region new PACKET_SIZE 1024

                        if (holdLen >= 1024) //make sure we have at least one packet in there
                        {
                            actualPackets = holdLen / 1024;
                            _hostServerRawPackets.BytesRemaining = holdLen - (actualPackets * 1024);

                            for (int i = 0; i < actualPackets; i++)
                            {
                                byte[] tmpByteArr = new byte[1024];
                                Copy(packetplayground, i * 1024, tmpByteArr, 0, 1024);
                                lock (_fullHostServerPacketList)
                                    _fullHostServerPacketList.Enqueue(new FullPacket(_hostServerRawPackets.ListClientId,
                                        tmpByteArr));
                            }
                        }
                        else
                        {
                            _hostServerRawPackets.BytesRemaining = holdLen;
                        }

                        //hang onto the remainder
                        Copy(packetplayground, actualPackets * 1024, _hostServerRawPackets.Remainder, 0,
                            _hostServerRawPackets.BytesRemaining);

                        #endregion


                        if (_fullHostServerPacketList.Count > 0)
                            _autoEvent2.Set();
                    } //end of while(true)

                    /**********************************************/
                }

                Console.WriteLine("Exiting the packet normalizer");
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: NormalizeServerRawPackets - {exceptionMessage}");
            }
        }

        private void ProcessRecievedServerData()
        {
            try
            {
                Console.WriteLine($"ProcessRecievedHostServerData ThreadID = {Thread.CurrentThread.ManagedThreadId}");
                while (ServerConnected)
                {
                    //ods.DebugOut("Before AutoEvent2");
                    _autoEvent2.WaitOne(10000); //wait at mutex until signal
                    //autoEvent2.WaitOne();
                    //ods.DebugOut("After AutoEvent2");
                    if (_appIsExiting || !ServerConnected || this.IsDisposed)
                        break;

                    while (_fullHostServerPacketList.Count > 0)
                    {
                        try
                        {
                            FullPacket fp;
                            lock (_fullHostServerPacketList)
                                fp = _fullHostServerPacketList.Dequeue();

                            var type = (ushort) (fp.ThePacket[1] << 8 | fp.ThePacket[0]);
                            //Debug.WriteLine("Got Server data... Packet type: " + ((PacketTypes)type).ToString());
                            switch (type) //Interrogate the first 2 Bytes to see what the packet TYPE is
                            {
                                case (byte) PacketTypes.RequestCredentials:
                                {
                                    ReplyToHostCredentialRequest(fp.ThePacket);
                                    //(new Thread(() => ReplyToHostCredentialRequest(fp.ThePacket))).Start();//
                                }
                                    break;
                                case (byte) PacketTypes.Ping:
                                {
                                    ReplyToHostPing(fp.ThePacket);
                                    Console.WriteLine($@"Received Ping: {GeneralFunctions.GetDateTimeFormatted}");
                                }
                                    break;
                                case (byte) PacketTypes.HostExiting:
                                    HostCommunicationsHasQuit(true);
                                    break;
                                case (byte) PacketTypes.Registered:
                                {
                                    SetConnectionsStatus();
                                }
                                    break;
                                case (byte) PacketTypes.MessageReceived:
                                    break;
                            }

                            if (_client != null)
                                _client.LastDataFromServer = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                            Console.WriteLine($"EXCEPTION IN: ProcessRecievedServerData A - {exceptionMessage}");
                        }
                    } //end while
                } //end while serverconnected

                //ods.DebugOut("Exiting the ProcessRecievedHostServerData() thread");
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($@"EXCEPTION IN: ProcessRecievedServerData B - {exceptionMessage}");
            }
        }

        #endregion

        private void SetConnectionsStatus()
        {
            try
            {
                if (!InvokeRequired) return;
                Invoke(new MethodInvoker(SetConnectionsStatus));
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($@"EXCEPTION IN: SetConnectionsStatus - {exceptionMessage}");
            }
        }

        #region Packets

        private void ReplyToHostPing(byte[] message)
        {
            try
            {
                PacketData IncomingData = new PacketData();
                IncomingData = (PacketData) PacketFunctions.ByteArrayToStructure(message, typeof(PacketData));

                /****************************************************************************************/
                //calculate how long that ping took to get here
                TimeSpan ts = (new DateTime(IncomingData.DataLong1)) - (new DateTime(_serverTime));
                Console.WriteLine(
                    $"{GeneralFunctions.GetDateTimeFormatted}: {string.Format("Ping From Server to client: {0:0.##}ms", ts.TotalMilliseconds)}");
                /****************************************************************************************/

                _serverTime = IncomingData.DataLong1; // Server computer's current time!

                PacketData xdata = new PacketData();

                xdata.Packet_Type = (ushort) PacketTypes.PingResponse;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                xdata.DataLong1 = IncomingData.DataLong1;

                byte[] byData = PacketFunctions.StructureToByteArray(xdata);

                SendMessageToServer(byData);

                CheckThisComputersTimeAgainstServerTime();
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: ReplyToHostPing - {exceptionMessage}");
            }
        }

        private void CheckThisComputersTimeAgainstServerTime()
        {
            long timeDiff = DateTime.UtcNow.Ticks - _serverTime;
            TimeSpan ts = TimeSpan.FromTicks(Math.Abs(timeDiff));
            Console.WriteLine($"Server diff in secs: {ts.TotalSeconds}");

            if (ts.TotalMinutes > 15)
            {
                string msg = string.Format("Computer Time Discrepancy!! " +
                                           "The time on this computer differs greatly " +
                                           "compared to the time on the Realtrac Server " +
                                           "computer. Check this PC's time.");

                Console.WriteLine(msg);
            }
        }

        public void ReplyToHostCredentialRequest(byte[] message)
        {
            if (_client == null)
                return;

            Console.WriteLine($@"ReplyToHostCredentialRequest ThreadID = {Thread.CurrentThread.ManagedThreadId}");
            var loc = 0;
            try
            {
                //We will assume to tell the host this is just an update of the
                //credentials we first sent during the application start. This
                //will be true if the 'message' argument is null, otherwise we
                //will change the packet type below to the 'TYPE_MyCredentials'.
                var paketType = (ushort) PacketTypes.CredentialsUpdate;

                if (message != null)
                {
                    var myOldServerID = 0;
                    //The host server has past my ID.
                    var incomingData = (PacketData) PacketFunctions.ByteArrayToStructure(message, typeof(PacketData));
                    loc = 10;
                    if (_myHostServerId > 0)
                        myOldServerID = _myHostServerId;
                    loc = 20;
                    _myHostServerId = (int) incomingData.idTo; //Hang onto this value
                    loc = 25;

                    Console.WriteLine($@"My Host Server ID is {_myHostServerId}");

                    var myAddressAsSeenByTheHost =
                        new string(incomingData.szStringDataA).TrimEnd('\0'); //My computer address
                    SetSomeLabelInfoFromThread(
                        $"My Address As Seen By The Server: {myAddressAsSeenByTheHost}, and my ID given by the server is: {_myHostServerId}");

                    _serverTime = incomingData.DataLong1;

                    paketType = (ushort) PacketTypes.MyCredentials;
                }

                var xdata = new PacketData
                {
                    Packet_Type = paketType,
                    Data_Type = 0,
                    Packet_Size = (ushort) Marshal.SizeOf(typeof(PacketData)),
                    maskTo = 0,
                    idTo = 0,
                    idFrom = 0
                };


                //Station Name
                var p = Environment.MachineName;
                if (p.Length > (xdata.szStringDataA.Length - 1))
                    p.CopyTo(0, xdata.szStringDataA, 0, (xdata.szStringDataA.Length - 1));
                else
                    p.CopyTo(0, xdata.szStringDataA, 0, p.Length);
                xdata.szStringDataA[(xdata.szStringDataA.Length - 1)] = '\0'; //cap it off just incase

                //App and DLL Version

                var versionNumber = Assembly.GetEntryAssembly().GetName().Version.Major + "." +
                                       Assembly.GetEntryAssembly().GetName().Version.Minor + "." +
                                       Assembly.GetEntryAssembly().GetName().Version.Build;

                loc = 30;

                versionNumber.CopyTo(0, xdata.szStringDataB, 0, versionNumber.Length);
                loc = 40;
                //Station Name
                var l = textBoxClientName.Text;
                if (l.Length > (xdata.szStringData150.Length - 1))
                    l.CopyTo(0, xdata.szStringData150, 0, (xdata.szStringData150.Length - 1));
                else
                    l.CopyTo(0, xdata.szStringData150, 0, l.Length);
                xdata.szStringData150[(xdata.szStringData150.Length - 1)] = '\0'; //cap it off just incase

                loc = 50;

                //Application type
                xdata.nAppLevel = (ushort) AppLevel.None;
                
                var byData = PacketFunctions.StructureToByteArray(xdata);
                loc = 60;
                SendMessageToServer(byData);
                loc = 70;
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(
                    $@"EXCEPTION at location {loc}, IN: ReplyToHostCredentialRequest - {exceptionMessage}");
            }
        }

        private delegate void SetSomeLabelInfoDelegate(string info);

        private void SetSomeLabelInfoFromThread(string info)
        {
            if (InvokeRequired)
            {
                this.Invoke(new SetSomeLabelInfoDelegate(SetSomeLabelInfoFromThread), new object[] {info});
                return;
            }

            labelConnectionStuff.Text = info;
        }

        private delegate void HostCommunicationsHasQuitDelegate(bool FromHost);

        private void HostCommunicationsHasQuit(bool FromHost)
        {
            if (InvokeRequired)
            {
                this.Invoke(new HostCommunicationsHasQuitDelegate(HostCommunicationsHasQuit), new object[] {FromHost});
                return;
            }

            if (_client != null)
            {
                int c = 100;
                do
                {
                    c--;
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(10);
                } while (c > 0);

                DoServerDisconnect();

                labelStatusInfo.Text = FromHost
                    ? "The Server has exited"
                    : "App has lost communication with the server (network issue).";

                labelStatusInfo.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void TellServerImDisconnecting()
        {
            try
            {
                PacketData xdata = new PacketData();

                xdata.Packet_Type = (ushort) PacketTypes.Close;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                byte[] byData = PacketFunctions.StructureToByteArray(xdata);

                SendMessageToServer(byData);
            }
            catch (Exception ex)
            {
                string exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"EXCEPTION IN: TellServerImDisconnecting - {exceptionMessage}");
            }
        }

        #endregion

        #region General Timer

        /// <summary>
        /// This will watch the TCPIP communication, after 5 minutes of no communications with the 
        /// Server we will assume the connections has been severed
        /// </summary>
        private void BeginGeneralTimer()
        {
            //create the general timer but skip over it if its already running
            if (_generalTimer == null)
            {
                _generalTimer = new System.Windows.Forms.Timer();
                _generalTimer.Tick += new EventHandler(GeneralTimer_Tick);
                _generalTimer.Interval = 5000;
                _generalTimer.Enabled = true;
            }
        }

        private void GeneralTimer_Tick(object sender, EventArgs e)
        {
            if (_client != null)
            {
                TimeSpan ts = DateTime.Now - _client.LastDataFromServer;

                //If we dont hear from the server for more than 5 minutes then there is a problem so disconnect
                if (ts.TotalMinutes > 5)
                {
                    DestroyGeneralTimer();
                    HostCommunicationsHasQuit(false);
                }
            }

            // Add 5 seconds worth of Ticks to the server time
            _serverTime += (TimeSpan.TicksPerSecond * 5);
            //Console.WriteLine("SERVER TIME: " + (new DateTime(GeneralFunction.ServerTime)).ToLocalTime().TimeOfDay.ToString());
        }

        private void DestroyGeneralTimer()
        {
            if (_generalTimer != null)
            {
                if (_generalTimer.Enabled == true)
                    _generalTimer.Enabled = false;

                try
                {
                    _generalTimer.Tick -= GeneralTimer_Tick;
                }
                catch (Exception)
                {
                    //just incase there was no event to remove
                }

                _generalTimer.Dispose();
                _generalTimer = null;
            }
        }

        #endregion//General Timer section

        private string GetSHubAddress() //translates a named IP to an address
        {
            var sHubServer = textBoxServer.Text; //GeneralSettings.HostIP.Trim();

            if (sHubServer.Length < 1)
                return string.Empty;

            try
            {
                var qaudNums = sHubServer.Split('.');

                // See if its not a straightup IP address.. 
                //if not then we have to resolve the computer name
                if (qaudNums.Length != 4)
                {
                    //Must be a name so see if we can resolve it
                    var hostEntry = Dns.GetHostEntry(sHubServer);

                    foreach (var a in hostEntry.AddressList)
                    {
                        if (a.AddressFamily != AddressFamily.InterNetwork) continue;
                        sHubServer = a.ToString();
                        break;
                    }

                    //SHubServer = hostEntry.AddressList[0].ToString();
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($@"Exception: {se.Message}");
                //statusStrip1.Items[1].Text = se.Message + " for " + Properties.Settings.Default.HostIP;
                sHubServer = string.Empty;
            }

            return sHubServer;
        }

        private static void CheckOnApplicationDirectory()
        {
            try
            {
                var appPath = GeneralFunctions.GetAppPath;

                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
            }
            catch
            {
                Console.WriteLine("ISSUE CREATING A DIRECTORY");
            }
        }

        #region UNSAFE CODE

        // The unsafe keyword allows pointers to be used within the following method:
        static unsafe void Copy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            try
            {
                if (src == null || srcIndex < 0 || dst == null || dstIndex < 0 || count < 0)
                {
                    Console.WriteLine("Serious Error in the Copy function 1");
                    throw new System.ArgumentException();
                }

                int srcLen = src.Length;
                int dstLen = dst.Length;
                if (srcLen - srcIndex < count || dstLen - dstIndex < count)
                {
                    Console.WriteLine("Serious Error in the Copy function 2");
                    throw new System.ArgumentException();
                }

                // The following fixed statement pins the location of the src and dst objects
                // in memory so that they will not be moved by garbage collection.
                fixed (byte* pSrc = src, pDst = dst)
                {
                    byte* ps = pSrc + srcIndex;
                    byte* pd = pDst + dstIndex;

                    // Loop over the count in blocks of 4 bytes, copying an integer (4 bytes) at a time:
                    for (int i = 0; i < count / 4; i++)
                    {
                        *((int*) pd) = *((int*) ps);
                        pd += 4;
                        ps += 4;
                    }

                    // Complete the copy by moving any bytes that weren't moved in blocks of 4:
                    for (int i = 0; i < count % 4; i++)
                    {
                        *pd = *ps;
                        pd++;
                        ps++;
                    }
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine("EXCEPTION IN: Copy - " + exceptionMessage);
            }
        }

        #endregion
    }
}