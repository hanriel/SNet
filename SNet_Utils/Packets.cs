using System.Collections.Generic;

namespace SNet_Utils
{
    public class MotherOfRawPackets
    {
        public MotherOfRawPackets(int listClientId)
        {
            ListClientId = listClientId;
            _rawPacketsList = new Queue<RawPackets>();
            Remainder = new byte[1024];

            BytesRemaining = 0;
        }

        public int ListClientId { get; }

        public int BytesRemaining { get; set; }

        public byte[] Remainder { get; set; }


        /***************** List operations ********************************************/
        public void AddToList(byte[] data, int sizeOfChunk)
        {
            lock (_rawPacketsList)
                _rawPacketsList.Enqueue(new RawPackets(ListClientId, data, sizeOfChunk));
        }

        public void ClearList()
        {
            //_RawPacketsList.TrimExcess();
            lock (_rawPacketsList)
                _rawPacketsList.Clear();
        }

        public RawPackets GetTopItem
        {
            get
            {
                RawPackets rp;
                lock (_rawPacketsList)
                    rp = _rawPacketsList.Dequeue();
                return rp;
            }
        }

        public int GetItemCount
        {
            get
            {
                return _rawPacketsList?.Count ?? 0;
            }
        }

        public void TrimTheFat() //Not sure if this helps anything
        {
            //_RawPacketsList.TrimExcess();
        }
        /******************************************************************************/

        //Private variables
        private readonly Queue<RawPackets> _rawPacketsList;
    }

    public class RawPackets
    {
        public RawPackets(int iClientId, byte[] theChunk, int sizeOfChunk)
        {
            DataChunk = new byte[sizeOfChunk];
            DataChunk = theChunk;
            ClientId = iClientId;
            ChunkLen = sizeOfChunk;
        }

        public byte[] DataChunk { get; }

        public int ClientId { get; }

        public int ChunkLen { get; }
    }

    public class FullPacket
    {
        public FullPacket(int iFromClient, byte[] thePacket)
        {
            ThePacket = new byte[1024];
            ThePacket = thePacket;
            FromClient = iFromClient;
        }

        public byte[] ThePacket { get; set; }

        public int FromClient { get; set; }
    }
}