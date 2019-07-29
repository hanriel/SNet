using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SNet_Utils;
using Application = System.Windows.Forms.Application;

namespace SNet_Server
{
    public partial class Form1 : Form
    {
        private enum Ink
        {
            ClrBlack = 0,
            ClrRed = 1,
            ClrBlue = 2,
            ClrGreen = 3,
            ClrPurple = 4
        }

        private bool _validBrowser;
        private bool _displayReady;

        bool _serverIsExiting;
        private const int MyPort = 5678;

        /*******************************************************/
        /// <summary>
        /// TCPiP server
        /// </summary>
        Server _svr;

        private Dictionary<int, MotherOfRawPackets> _dClientRawPacketList;
        private Queue<FullPacket> _fullPacketList;
        static AutoResetEvent _autoEvent;
        static AutoResetEvent _autoEvent2;
        private Thread _dataProcessThread;

        private Thread _fullPacketDataProcessThread;
        /*******************************************************/

        System.Timers.Timer _timerGarbagePatrol;
        System.Timers.Timer _timerPing;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /**********************************************/
            // Init the communications window so we can whats going on
            _validBrowser = BrowserVersion();
            // Setup data monitor
            CommunicationsDisplay.Navigate("about:blank");

            OnCommunications($"Loading... {GeneralFunctions.GetDateTimeFormatted}", Ink.ClrBlue);
            /**********************************************/

            /**********************************************/
            //Create a directory we can write stuff too
            CheckOnApplicationDirectory();
            /**********************************************/

            /**********************************************/
            //Start listening for TCP-IP client connections
            StartPacketCommunicationsServiceThread();
            /**********************************************/

            /********************************************************/
            // Create some timers for maintenance
            // 4 minute ping timer
            _timerPing = new System.Timers.Timer {Interval = 240000, Enabled = true};
            _timerPing.Elapsed += timerPing_Elapsed;

            // 5 minute connection integrity patrol
            _timerGarbagePatrol = new System.Timers.Timer {Interval = 600000, Enabled = true};
            _timerGarbagePatrol.Elapsed += timerGarbagePatrol_Elapsed;
            /********************************************************/

            // enumerate my IPs
            SetHostNameAndAddress();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_svr != null)
            {
                var xdata = new PacketData
                {
                    Packet_Type = (ushort) PacketTypes.HostExiting,
                    Data_Type = 0,
                    Packet_Size = 16,
                    maskTo = 0,
                    idTo = 0,
                    idFrom = 0
                };

                var byData = PacketFunctions.StructureToByteArray(xdata);

                _svr.SendMessage(byData);

                Thread.Sleep(250);
            }

            _serverIsExiting = true;
            try
            {
                if (_timerGarbagePatrol != null)
                {
                    _timerGarbagePatrol.Stop();
                    _timerGarbagePatrol.Elapsed -= timerGarbagePatrol_Elapsed;
                    _timerGarbagePatrol.Dispose();
                    _timerGarbagePatrol = null;
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                if (_timerPing != null)
                {
                    _timerPing.Stop();
                    _timerPing.Elapsed -= timerPing_Elapsed;
                    _timerPing.Dispose();
                    _timerPing = null;
                }
            }
            catch
            {
                // ignored
            }

            KillTheServer();
        }


        private void StartPacketCommunicationsServiceThread()
        {
            try
            {
                //Packet processor mutex and loop
                _autoEvent = new AutoResetEvent(false); //the RawPacket data mutex
                _autoEvent2 = new AutoResetEvent(false); //the FullPacket data mutex
                _dataProcessThread = new Thread(NormalizeThePackets);
                _fullPacketDataProcessThread = new Thread(ProcessReceivedData);

                //Lists
                _dClientRawPacketList = new Dictionary<int, MotherOfRawPackets>();
                _fullPacketList = new Queue<FullPacket>();

                //Create HostServer
                _svr = new Server();

                _svr.Listen(MyPort); //MySettings.HostPort);
                _svr.OnReceiveData += OnDataReceived;
                _svr.OnClientConnect += NewClientConnected;
                _svr.OnClientDisconnect += ClientDisconnect;

                _dataProcessThread.Start();
                _fullPacketDataProcessThread.Start();

                OnCommunications($"TCPiP Server is listening on port {MyPort}", Ink.ClrGreen);
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                //Debug.WriteLine($"EXCEPTION IN: StartPacketCommunicationsServiceThread - {exceptionMessage}");
                OnCommunications($"EXCEPTION: TCPiP FAILED TO START, exception: {exceptionMessage}", Ink.ClrRed);
            }
        }

        private void KillTheServer()
        {
            try
            {
                _svr?.Stop();
            }
            catch
            {
                // ignored
            }

            try
            {
                if (_autoEvent != null)
                {
                    _autoEvent.Set();

                    Thread.Sleep(30);
                    _autoEvent.Close();
                    _autoEvent.Dispose();
                    _autoEvent = null;
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                if (_autoEvent2 != null)
                {
                    _autoEvent2.Set();

                    Thread.Sleep(30);
                    _autoEvent2.Close();
                    _autoEvent2.Dispose();
                    _autoEvent2 = null;
                }
            }
            catch
            {
                // ignored
            }

            Thread.Sleep(15);

            try
            {
                if (_dClientRawPacketList != null)
                {
                    _dClientRawPacketList.Clear();
                    _dClientRawPacketList = null;
                }
            }
            catch
            {
                // ignored
            }

            _svr = null;
        }

        #region TIMERS

        /// <summary>
        /// Fires every 4 minutes
        /// </summary>
        void timerPing_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PingTheConnections();
        }

        void PingTheConnections()
        {
            if (_svr == null)
                return;

            try
            {
                var xdata = new PacketData();

                xdata.Packet_Type = (ushort) PacketTypes.Ping;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                xdata.DataLong1 = DateTime.UtcNow.Ticks;

                var byData = PacketFunctions.StructureToByteArray(xdata);

                //Stopwatch sw = new Stopwatch();

                //sw.Start();
                lock (_svr.WorkerSockets)
                {
                    foreach (var s in _svr.WorkerSockets.Values)
                    {
                        //Console.WriteLine("Ping id - " + s.iClientID.ToString());
                        //Thread.Sleep(25);//allow a slight moment so all the replies dont happen at the same time
                        s.PingStatClass.StartTheClock();

                        try
                        {
                            _svr.SendMessage(s.ClientId, byData);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                //sw.Stop();
                //Debug.WriteLine("TimeAfterSend: " + sw.ElapsedMilliseconds.ToString() + "ms");
            }
            catch
            {
                // ignored
            }
        }
        /**********************************************************************************************************************/

        //private void GarbagePatrol_Tick(object sender, EventArgs e)
        private void timerGarbagePatrol_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                CheckConnectionTimersGarbagePatrol();
            }
            catch
            {
                // ignored
            }
        }

        private void CheckConnectionTimersGarbagePatrol()
        {
            var ClientIDsToClear = new List<int>();

            Debug.WriteLine($"{_svr.WorkerSockets.Values.Count} - List Count: {_svr.WorkerSockets.Values.Count}");

            lock (_svr.WorkerSockets)
            {
                foreach (var s in _svr.WorkerSockets.Values)
                {
                    var diff = DateTime.Now - s.DTimer;
                    //Debug.WriteLine("iClientID: " + s.iClientID + " - " + "Time: " + diff.TotalSeconds.ToString());

                    if (diff.TotalSeconds >= 600 || s.UserSocket.Connected == false) //10 minutes
                    {
                        //Punt the ListVeiw item here but we must make a list of
                        //clients that we have lost connection with, its not good to remove
                        //the Servers internal client item while inside its foreach loop;
                        //listView1.Items.RemoveByKey(s.iClientID.ToString());
                        ClientIDsToClear.Add(s.ClientId);
                    }
                }
            }

            Debug.WriteLine(
                $"{DateTime.Now.ToLongTimeString()} - Garbage Patrol num of IDs to remove: {ClientIDsToClear.Count}");

            //Ok remove any internal data items we may have
            if (ClientIDsToClear.Count > 0)
            {
                foreach (var cID in ClientIDsToClear)
                {
                    SendMessageOfClientDisconnect(cID);

                    CleanupDeadClient(cID);
                    Thread.Sleep(5);
                }
            }
        }

        private delegate void CleanupDeadClientDelegate(int clientNumber);

        private void CleanupDeadClient(int clientNumber)
        {
            if (InvokeRequired)
            {
                Invoke(new CleanupDeadClientDelegate(CleanupDeadClient), new object[] {clientNumber});
                return;
            }

            try
            {
                lock (_dClientRawPacketList) //http://www.albahari.com/threading/part2.aspx#_Locking
                {
                    if (_dClientRawPacketList.ContainsKey(clientNumber))
                    {
                        _dClientRawPacketList[clientNumber].ClearList();
                        _dClientRawPacketList.Remove(clientNumber);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                lock (_svr.WorkerSockets)
                {
                    if (_svr.WorkerSockets.ContainsKey(clientNumber))
                    {
                        _svr.WorkerSockets[clientNumber].UserSocket.Close();
                        _svr.WorkerSockets.Remove(clientNumber);
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (listView1.Items.ContainsKey(clientNumber.ToString()))
                {
                    listView1.Items.RemoveByKey(clientNumber.ToString());
                }
            }
            catch
            {
            }
        }

        #endregion

        #region TCPIP Layer incoming data

        private void OnDataReceived(int clientNumber, byte[] message, int messageSize)
        {
            if (_dClientRawPacketList.ContainsKey(clientNumber))
            {
                _dClientRawPacketList[clientNumber].AddToList(message, messageSize);

                //Debug.WriteLine("Raw Data From: " + clientNumber.ToString() + ", Size of Packet: " + messageSize.ToString());
                _autoEvent.Set(); //Fire in the hole
            }
        }

        #endregion

        #region CLIENT CONNECTION PROCESS

        private void NewClientConnected(int ConnectionID)
        {
            try
            {
                Debug.WriteLine($"(RT Client)NewClientConnected: {ConnectionID}");
                OnCommunications($"{GeneralFunctions.GetDateTimeFormatted} Incoming Connection {ConnectionID}",
                    Ink.ClrPurple);
                if (_svr.WorkerSockets.ContainsKey(ConnectionID))
                {
                    lock (_dClientRawPacketList)
                    {
                        //Add the raw Packet collector
                        if (!_dClientRawPacketList.ContainsKey(ConnectionID))
                        {
                            _dClientRawPacketList.Add(ConnectionID, new MotherOfRawPackets(ConnectionID));
                        }
                    }

                    SetNewConnectionData_FromThread(ConnectionID);
                }
                else
                {
                    Debug.WriteLine("UNKNOWN CONNECTIONID" + ConnectionID);
                }
            }
            catch (Exception ex)
            {
                OnCommunications($"EXCEPTION: NewClientConnected on client {ConnectionID}, exception: {ex.Message}",
                    Ink.ClrRed);
            }
        }

        private delegate void SetNewConnectionDataDelegate(int clientNumber);

        private void SetNewConnectionData_FromThread(int clientNumber)
        {
            if (InvokeRequired)
            {
                Invoke(new SetNewConnectionDataDelegate(SetNewConnectionData_FromThread), clientNumber);
                return;
            }

            try
            {
                lock (_svr.WorkerSockets)
                {
                    /*********************************   Add the data to the Listview  *************************************/
                    var li =
                        new ListViewItem(_svr.WorkerSockets[clientNumber].UserSocket.RemoteEndPoint.ToString());
                    li.Name = clientNumber.ToString(); //Set the Key as a unique identifier
                    li.Tag = clientNumber;

                    listView1.Items.Add(li); //index 0 Clients IP address
                    li.SubItems.Add("Receiving..."); //index 1 Computer name
                    li.SubItems.Add("Receiving..."); //index 2 version
                    li.SubItems.Add(clientNumber.ToString()); //index 3 //Client's ID
                    li.SubItems.Add("Receiving..."); //index 4 Clients Name
                    li.SubItems.Add("..."); //index 5 Ping time
                    /*******************************************************************************************************/
                }

                if (_svr.WorkerSockets[clientNumber].UserSocket.Connected)
                {
                    OnCommunications($"RequestNewConnectionCredentials from: {clientNumber}", Ink.ClrPurple);
                    RequestNewConnectionCredentials(clientNumber);
                }
                else
                {
                    Debug.WriteLine(
                        $"ISSUE!!!(RequestNewConnectionCredentials) UserSocket.Connected is FALSE from: {clientNumber}");
                }
            }
            catch (Exception ex)
            {
                OnCommunications(
                    $"EXCEPTION: SetNewConnectionData_FromThread on client {clientNumber}, exception: {ex.Message}",
                    Ink.ClrRed);
            }
        }


        private delegate void PostUserCredentialsDelegate(int clientNumber, byte[] message);

        /// <summary>
        /// return bool, TRUE if its a FullClient Connection
        /// </summary>
        /// <param name="clientNumber"></param>
        /// <param name="message"></param>
        private void PostUserCredentials(int clientNumber, byte[] message)
        {
            if (InvokeRequired)
            {
                Invoke(new PostUserCredentialsDelegate(PostUserCredentials), clientNumber, message);
                return;
            }

            try
            {
                var IncomingData = (PacketData) PacketFunctions.ByteArrayToStructure(message, typeof(PacketData));

                lock (_svr.WorkerSockets)
                {
                    var computerName =
                        new string(IncomingData.szStringDataA).TrimEnd('\0'); //Station/Computer's name
                    var versionStr = new string(IncomingData.szStringDataB).TrimEnd('\0'); //app version
                    var clientsName = new string(IncomingData.szStringData150).TrimEnd('\0'); //Client's Name

                    listView1.Items[clientNumber.ToString()].SubItems[1].Text = computerName;
                    listView1.Items[clientNumber.ToString()].SubItems[2].Text = versionStr;
                    listView1.Items[clientNumber.ToString()].SubItems[4].Text = clientsName;

                    if (_svr.WorkerSockets.ContainsKey(clientNumber))
                    {
                        _svr.WorkerSockets[clientNumber].SzStationName = computerName;

                        _svr.WorkerSockets[clientNumber].SzClientName = clientsName;

                        OnCommunications(
                            $"{GeneralFunctions.GetDateTimeFormatted} Registered Connection ({clientNumber}) for '{clientsName}' on PC: {computerName}",
                            Ink.ClrGreen);
                    }
                } //end lock
            }
            catch (Exception ex)
            {
                OnCommunications($"EXCEPTION: PostUserCredentials on client {clientNumber}, exception: {ex.Message}",
                    Ink.ClrRed);
            }
        }


        private void ClientDisconnect(int clientNumber)
        {
            if (_serverIsExiting)
                return;

            /*******************************************************/
            lock (_dClientRawPacketList) //Make sure we don't do this twice
            {
                if (!_dClientRawPacketList.ContainsKey(clientNumber))
                {
                    lock (_svr.WorkerSockets)
                    {
                        if (!_svr.WorkerSockets.ContainsKey(clientNumber))
                        {
                            return;
                        }
                    }
                }
            }
            /*******************************************************/

            try
            {
                RemoveClient_FromThread(clientNumber);
            }
            catch (Exception ex)
            {
                OnCommunications($"EXCEPTION: ClientDisconnect on client {clientNumber}, exception: {ex.Message}",
                    Ink.ClrRed);
            }

            CleanupDeadClient(clientNumber);


            Thread.Sleep(10);
        }

        private void RemoveClient_FromThread(int clientNumber)
        {
            try
            {
                SendMessageOfClientDisconnect(clientNumber);
                OnCommunications(
                    $"{GeneralFunctions.GetDateTimeFormatted} - {clientNumber} has disconnected",
                    Ink.ClrBlue);
            }
            catch (Exception ex)
            {
                OnCommunications(
                    $"EXCEPTION: RemoveClient_FromThread on client {clientNumber}, Exception: {ex.Message}",
                    Ink.ClrRed);
            }
        }

        #endregion

        #region Packet factory Processing from clients

        private void NormalizeThePackets()
        {
            if (_svr == null)
                return;

            while (_svr.IsListening)
            {
                //Debug.WriteLine("Before AutoEvent");
                _autoEvent.WaitOne(
                    10000); //wait at mutex until signal, and drop through every 10 seconds if something strange happens
                //Debug.WriteLine("After AutoEvent");

                /**********************************************/
                lock (_dClientRawPacketList) //http://www.albahari.com/threading/part2.aspx#_Locking
                {
                    foreach (var mrp in _dClientRawPacketList.Values)
                    {
                        if (mrp.GetItemCount.Equals(0))
                            continue;
                        try
                        {
                            var packetPlayGround =
                                new byte[11264]; //good for 10 full packets(10240) + 1 remainder(1024)
                            RawPackets rp;

                            while (true)
                            {
                                if (mrp.GetItemCount == 0)
                                    break;

                                var holdLen = 0;

                                if (mrp.BytesRemaining > 0)
                                    Copy(mrp.Remainder, 0, packetPlayGround, 0, mrp.BytesRemaining);

                                holdLen = mrp.BytesRemaining;

                                for (var i = 0;
                                    i < 10;
                                    i++) //only go through a max of 10 times so there will be room for any remainder
                                {
                                    rp = mrp.GetTopItem; //dequeue

                                    Copy(rp.DataChunk, 0, packetPlayGround, holdLen, rp.ChunkLen);

                                    holdLen += rp.ChunkLen;

                                    if (mrp.GetItemCount.Equals(0)
                                    ) //make sure there is more in the list befor continuing
                                        break;
                                }

                                var actualPackets = 0;

                                #region PACKET_SIZE 1024

                                if (holdLen >= 1024) //make sure we have at least one packet in there
                                {
                                    actualPackets = holdLen / 1024;
                                    mrp.BytesRemaining = holdLen - (actualPackets * 1024);

                                    for (var i = 0; i < actualPackets; i++)
                                    {
                                        var tmpByteArr = new byte[1024];
                                        Copy(packetPlayGround, i * 1024, tmpByteArr, 0, 1024);
                                        lock (_fullPacketList)
                                            _fullPacketList.Enqueue(new FullPacket(mrp.ListClientId, tmpByteArr));
                                    }
                                }
                                else
                                {
                                    mrp.BytesRemaining = holdLen;
                                }

                                //hang onto the remainder
                                Copy(packetPlayGround, actualPackets * 1024, mrp.Remainder, 0, mrp.BytesRemaining);

                                #endregion

                                if (_fullPacketList.Count > 0)
                                    _autoEvent2.Set();
                                //Call_ProcessRecievedData_FromThread();
                            } //end of while(true)
                        }
                        catch (Exception ex)
                        {
                            mrp.ClearList(); //pe 03-20-2013
                            var msg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;

                            OnCommunications("EXCEPTION in  NormalizeThePackets - " + msg, Ink.ClrRed);
                        }
                    } //end of foreach (dClientRawPacketList)
                } //end of lock

                /**********************************************/
                if (_serverIsExiting)
                    break;
            } //EndOf of while(svr.IsListening)

            Debug.WriteLine("Exiting the packet normalizer");
            OnCommunications("Exiting the packet normalizer", Ink.ClrRed);
        }

        private void ProcessReceivedData()
        {
            if (_svr == null)
                return;

            while (_svr.IsListening)
            {
                //Debug.WriteLine("Before AutoEvent");
                _autoEvent2.WaitOne(); //wait at mutex until signal
                //Debug.WriteLine("After AutoEvent");

                try
                {
                    while (_fullPacketList.Count > 0)
                    {
                        FullPacket fp;
                        lock (_fullPacketList)
                            fp = _fullPacketList.Dequeue();
                        //Console.WriteLine(GetDateTimeFormatted +" - Full packet fromID: " + fp.iFromClient.ToString() + ", Type: " + ((PACKETTYPES)fp.ThePacket[0]).ToString());
                        var type = (ushort) (fp.ThePacket[1] << 8 | fp.ThePacket[0]);
                        switch (type) //Interrigate the first 2 Bytes to see what the packet TYPE is
                        {
                            case (ushort) PacketTypes.MyCredentials:
                            {
                                PostUserCredentials(fp.FromClient, fp.ThePacket);
                                SendRegisteredMessage(fp.FromClient);
                            }
                                break;
                            case (ushort) PacketTypes.CredentialsUpdate:
                                break;
                            case (ushort) PacketTypes.PingResponse:
                                //Debug.WriteLine(DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToLongTimeString() + " - Received Ping from: " + fp.iFromClient.ToString() + ", on " + DateTime.Now.ToShortDateString() + ", at: " + DateTime.Now.ToLongTimeString());
                                UpdateTheConnectionTimers(fp.FromClient);
                                break;
                            case (ushort) PacketTypes.Close:
                                ClientDisconnect(fp.FromClient);
                                break;
                            case (ushort) PacketTypes.Message:
                            {
                                AssembleMessage(fp.FromClient, fp.ThePacket);
                            }
                                break;
                            default:
                                PassDataThru(type, fp.FromClient, fp.ThePacket);
                                break;
                        }
                    } //END  while (FullPacketList.Count > 0)
                } //try
                catch (Exception ex)
                {
                    try
                    {
                        var msg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                        OnCommunications($"EXCEPTION in  ProcessReceivedData - {msg}", Ink.ClrRed);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (_serverIsExiting)
                    break;
            } //End while (svr.IsListening)

            var info2 = $"AppIsExiting = {_serverIsExiting.ToString()}";
            var info3 = "Past the ProcessReceivedData loop";

            Debug.WriteLine(info2);
            Debug.WriteLine(info3);

            try
            {
                OnCommunications(info3,
                    Ink.ClrRed); // "Past the ProcessReceivedData loop" also is logged to InfoLog.log
            }
            catch
            {
                // ignored
            }

            if (!_serverIsExiting)
            {
                //if we got here then something went wrong, we need to shut down the service
                OnCommunications("SOMETHING CRASHED", Ink.ClrRed);
            }
        }

        private void PassDataThru(ushort type, int MessageFrom, byte[] message)
        {
            try
            {
                var ForwardTo = message[11] << 24 | message[10] << 16 | message[9] << 8 | message[8];

                //Stuff in who this packet is from so we know who sent it
                var x = BitConverter.GetBytes(MessageFrom);
                message[12] = (byte) x[0]; //idFrom
                message[13] = (byte) x[1]; //idFrom
                message[14] = (byte) x[2]; //idFrom
                message[15] = (byte) x[3]; //idFrom

                if (ForwardTo > 0)
                    _svr.SendMessage(ForwardTo, message);
                else
                    _svr.SendMessage(message);
            }
            catch (Exception ex)
            {
                var msg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                OnCommunications($"EXCEPTION in  PassDataThru - {msg}", Ink.ClrRed);
            }
        }

        #endregion

        StringBuilder _sb = null;

        private void AssembleMessage(int clientId, byte[] message)
        {
            try
            {
                var incomingData = (PacketData) PacketFunctions.ByteArrayToStructure(message, typeof(PacketData));

                switch (incomingData.Data_Type)
                {
                    case (ushort) PacketTypesSubMessage.Start:
                    {
                        if (_svr.WorkerSockets.ContainsKey(clientId))
                        {
                            OnCommunications(
                                $"Client '{_svr.WorkerSockets[clientId].SzClientName}' sent some numbers and some text... num1= {incomingData.Data16} and num2= {incomingData.Data17}:",
                                Ink.ClrBlue);
                            OnCommunications($"Client also said:", Ink.ClrBlue);
                            OnCommunications(new string(incomingData.szStringDataA).TrimEnd('\0'), Ink.ClrGreen);
                        }
                    }
                        break;
                    case (ushort) PacketTypesSubMessage.Guts:
                    {
                        OnCommunications(new string(incomingData.szStringDataA).TrimEnd('\0'), Ink.ClrGreen);
                    }
                        break;
                    case (ushort) PacketTypesSubMessage.End:
                    {
                        OnCommunications("FINISHED GETTING MESSAGE", Ink.ClrBlue);

                        /****************************************************************/
                        //Now tell the client teh message was received!
                        var xdata = new PacketData {Packet_Type = (ushort) PacketTypes.MessageReceived};


                        var byData = PacketFunctions.StructureToByteArray(xdata);

                        _svr.SendMessage(clientId, byData);
                    }
                        break;
                }
            }
            catch
            {
                Console.WriteLine(@"ERROR Assembling message");
            }
        }

        private void UpdateTheConnectionTimers(int clientNumber)
        {
            lock (_svr.WorkerSockets)
            {
                try
                {
                    if (_svr.WorkerSockets.ContainsKey(clientNumber))
                    {
                        _svr.WorkerSockets[clientNumber].DTimer = DateTime.Now;
                        var elapsedTime = _svr.WorkerSockets[clientNumber].PingStatClass.StopTheClock();

                        Console.WriteLine(
                            $@"{GeneralFunctions.GetDateTimeFormatted}: Ping From Server to client: {elapsedTime}ms");

                        UpdateThePingTimeFromThread(clientNumber, elapsedTime);
                        /****************************************************************************************/
                    }
                }
                catch (Exception ex)
                {
                    var msg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                    OnCommunications($"EXCEPTION in UpdateTheConnectionTimers - {msg}", Ink.ClrRed);
                }
            }
        }

        private delegate void UpdateThePingTimeFromThreadDelegate(int clientNumber, long elapsedTimeInMilliseconds);

        private void UpdateThePingTimeFromThread(int clientNumber, long elapsedTimeInMilliseconds)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateThePingTimeFromThreadDelegate(UpdateThePingTimeFromThread), clientNumber,
                    elapsedTimeInMilliseconds);
                return;
            }

            listView1.Items[clientNumber.ToString()].SubItems[5].Text = $@"{elapsedTimeInMilliseconds:0.##}ms";
        }

        private void SetHostNameAndAddress()
        {
            var strHostName = Dns.GetHostName();

            labelMyIP.Text = $@"Host Name: {strHostName}, Listening on Port: {MyPort}";

            var ips = Dns.GetHostAddresses(strHostName);

            foreach (var ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    listBox1.Items.Add(ip.ToString());
                else
                    listBoxUnusedIPs.Items.Add(ip.ToString());
            }
        }

        private void CheckOnApplicationDirectory()
        {
            try
            {
                var appPath = GeneralFunctions.GetAppPath;

                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
            }
            catch (Exception ex)
            {
                var msg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                OnCommunications($"EXCEPTION: ISSUE CREATING A DIRECTORY - {msg}", Ink.ClrRed);
            }
        }

        #region PACKET MESSAGES

        private void RequestNewConnectionCredentials(int clientId)
        {
            try
            {
                var xdata = new PacketData
                {
                    Packet_Type = (ushort) PacketTypes.RequestCredentials,
                    Data_Type = 0,
                    Packet_Size = 16,
                    maskTo = 0,
                    idTo = (ushort) clientId,
                    idFrom = 0,
                    DataLong1 = DateTime.UtcNow.Ticks
                };


                if (!_svr.WorkerSockets.ContainsKey(clientId))
                    return;

                lock (_svr.WorkerSockets)
                {
                    //ship back their address for reference to the client
                    var clientAddr = ((IPEndPoint) _svr.WorkerSockets[clientId].UserSocket.RemoteEndPoint).Address
                        .ToString();
                    clientAddr.CopyTo(0, xdata.szStringDataA, 0, clientAddr.Length);

                    var byData = PacketFunctions.StructureToByteArray(xdata);

                    if (!_svr.WorkerSockets[clientId].UserSocket.Connected) return;

                    _svr.SendMessage(clientId, byData);
                    Debug.WriteLine(DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToLongTimeString() +
                                    " - from " + clientId);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void SendMessageOfClientDisconnect(int clientId)
        {
            try
            {
                var xdata = new PacketData
                {
                    Packet_Type = (ushort) PacketTypes.ClientDisconnecting,
                    Data_Type = 0,
                    Packet_Size = (ushort) Marshal.SizeOf(typeof(PacketData)),
                    maskTo = 0,
                    idTo = 0,
                    idFrom = (uint) clientId
                };

                var byData = PacketFunctions.StructureToByteArray(xdata);
                _svr.SendMessage(byData);
            }
            catch
            {
                // ignored
            }
        }

        private void SendRegisteredMessage(int clientId)
        {
            try
            {
                var xdata = new PacketData
                {
                    Packet_Type = (ushort) PacketTypes.Registered,
                    Data_Type = 0,
                    Packet_Size = (ushort) Marshal.SizeOf(typeof(PacketData)),
                    maskTo = 0,
                    idTo = 0,
                    idFrom = (uint) clientId,
                    Data6 = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major,
                    Data7 = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor,
                    Data8 = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Build
                };

                var byData = PacketFunctions.StructureToByteArray(xdata);
                _svr.SendMessage(byData);
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region WEB CONTROL

        private bool BrowserVersion()
        {
            Debug.WriteLine("CommunicationsDisplay.Version: " + CommunicationsDisplay.Version);

            if (CommunicationsDisplay.Version.Major >= 9) return true;

            MessageBox.Show(this,
                @"You must update your web browser to Internet Explorer 9 or greater to see the service output information!",
                @"Message", MessageBoxButtons.OK);
            return false;
        }

        private delegate void OnCommunicationsDelegate(string str, Ink ink);

        private void OnCommunications(string str, Ink ink)
        {
            if (_validBrowser == false)
            {
                Debug.WriteLine("INVALID BROWSER, must update Internet Explorer to version 8 or better!!");
                return;
            }

            try
            {
                if (InvokeRequired)
                {
                    Invoke(new OnCommunicationsDelegate(OnCommunications), str, ink);
                    return;
                }

                var doc = CommunicationsDisplay.Document;

                string style;
                switch (ink)
                {
                    case Ink.ClrGreen:
                        style = Properties.Settings.Default.StyleGreen;
                        break;
                    case Ink.ClrBlue:
                        style = Properties.Settings.Default.StyleBlue;
                        break;
                    case Ink.ClrRed:
                        style = Properties.Settings.Default.StyleRed;
                        break;
                    case Ink.ClrPurple:
                        style = Properties.Settings.Default.StylePurple;
                        break;
                    default:
                        style = Properties.Settings.Default.StyleBlack;
                        break;
                }

                if (doc != null) doc.Write($"<div style=\"{style}\">{str}</div>");
                ScrollMessageIntoView();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EXCEPTION IN OnCommunications @ {ex.Message}");
            }
        }

        /// <summary>
        /// force the web control to the last item in the window... set to the bottom for the latest activity
        /// </summary>
        private void ScrollMessageIntoView()
        {
            // MOST IMP : processes all windows messages queue
            Application.DoEvents();

            if (CommunicationsDisplay.Document != null)
            {
                CommunicationsDisplay.Document.Window.ScrollTo(0,
                    CommunicationsDisplay.Document.Body.ScrollRectangle.Height);
            }
        }

        private void ClearEventAndStatusDisplays()
        {
            // Clear communications
            _displayReady = false;
            CommunicationsDisplay.Navigate("about:blank");
            while (!_displayReady)
            {
                Application.DoEvents();
            }
        }

        private void CommunicationsDisplay_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //Debug.WriteLine("CommunicationsDisplay_Navigated");
            //OnCommunications("........", INK.CLR_BLACK);
            _displayReady = true;
        }

        #endregion

        #region UNSAFE CODE

        // The unsafe keyword allows pointers to be used within the following method:
        static unsafe void Copy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            try
            {
                if (src == null || srcIndex < 0 || dst == null || dstIndex < 0 || count < 0)
                {
                    Console.WriteLine("Serious Error in the Copy function 1");
                    throw new ArgumentException();
                }

                var srcLen = src.Length;
                var dstLen = dst.Length;
                if (srcLen - srcIndex < count || dstLen - dstIndex < count)
                {
                    Console.WriteLine("Serious Error in the Copy function 2");
                    throw new ArgumentException();
                }

                // The following fixed statement pins the location of the src and dst objects
                // in memory so that they will not be moved by garbage collection.
                fixed (byte* pSrc = src, pDst = dst)
                {
                    var ps = pSrc + srcIndex;
                    var pd = pDst + dstIndex;

                    // Loop over the count in blocks of 4 bytes, copying an integer (4 bytes) at a time:
                    for (var i = 0; i < count / 4; i++)
                    {
                        *((int*) pd) = *((int*) ps);
                        pd += 4;
                        ps += 4;
                    }

                    // Complete the copy by moving any bytes that weren't moved in blocks of 4:
                    for (var i = 0; i < count % 4; i++)
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
                Debug.WriteLine("EXCEPTION IN: Copy - " + exceptionMessage);
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_svr.WorkerSockets.Count.ToString());
        }
    }
}