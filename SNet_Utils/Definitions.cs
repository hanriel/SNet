using System;
using System.Runtime.InteropServices;

namespace SNet_Utils
{
    [Flags]
    public enum Application
    {
        None = 0,
        ClientTypeA = 1,
        ClientTypeB = 2
    }

    public enum AppLevel
    {
        None = 0,
        ClientLevel1 = 1,
        ClientLevel2 = 2
    }

    public enum PacketTypes
    {
        Ping = 1,
        PingResponse = 2,
        RequestCredentials = 3,
        MyCredentials = 4,
        Registered = 5,
        HostExiting = 6,
        ClientData = 7,
        ClientDisconnecting = 8,
        CredentialsUpdate = 9,
        Close = 10,
        Message = 11,
        MessageReceived = 12
    }

    public enum PacketTypesSubMessage
    {
        Start,
        Guts,
        End
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PacketData
    {
        /****************************************************************/
        //HEADER is 18 BYTES
        public ushort Packet_Type; //TYPE_??
        public ushort Packet_Size;
        public ushort Data_Type; // DATA_ type fields
        public ushort maskTo; // SENDTO_MY_SHUBONLY and the like.
        public uint idTo; // Used if maskTo is SENDTO_INDIVIDUAL
        public uint idFrom; // Client ID value

        public ushort nAppLevel;
        /****************************************************************/

        public uint Data1; //miscellanious information
        public uint Data2; //miscellanious information
        public uint Data3; //miscellanious information
        public uint Data4; //miscellanious information
        public uint Data5; //miscellanious information

        public int Data6; //miscellanious information
        public int Data7; //miscellanious information
        public int Data8; //miscellanious information
        public int Data9; //miscellanious information
        public int Data10; //miscellanious information

        public uint Data11; //miscellanious information
        public uint Data12; //miscellanious information
        public uint Data13; //miscellanious information
        public uint Data14; //miscellanious information
        public uint Data15; //miscellanious information

        public int Data16; //miscellanious information
        public int Data17; //miscellanious information
        public int Data18; //miscellanious information
        public int Data19; //miscellanious information
        public int Data20; //miscellanious information

        public uint Data21; //miscellanious information
        public uint Data22; //miscellanious information
        public uint Data23; //miscellanious information
        public uint Data24; //miscellanious information
        public uint Data25; //miscellanious information

        public int Data26; //miscellanious information
        public int Data27; //miscellanious information
        public int Data28; //miscellanious information
        public int Data29; //miscellanious information
        public int Data30; //miscellanious information

        public double DataDouble1;
        public double DataDouble2;
        public double DataDouble3;
        public double DataDouble4;
        public double DataDouble5;

        /// <summary>
        /// Long value1
        /// </summary>
        public long DataLong1;

        /// <summary>
        /// Long value2
        /// </summary>
        public long DataLong2;

        /// <summary>
        /// Long value3
        /// </summary>
        public long DataLong3;

        /// <summary>
        /// Long value4
        /// </summary>
        public long DataLong4;

        /// <summary>
        /// Unsigned Long value1
        /// </summary>
        public ulong DataULong1;

        /// <summary>
        /// Unsigned Long value2
        /// </summary>
        public ulong DataULong2;

        /// <summary>
        /// Unsigned Long value3
        /// </summary>
        public ulong DataULong3;

        /// <summary>
        /// Unsigned Long value4
        /// </summary>
        public ulong DataULong4;

        /// <summary>
        /// DateTime Tick value1
        /// </summary>
        public long DataTimeTick1;

        /// <summary>
        /// DateTime Tick value2
        /// </summary>
        public long DataTimeTick2;

        /// <summary>
        /// DateTime Tick value1
        /// </summary>
        public long DataTimeTick3;

        /// <summary>
        /// DateTime Tick value2
        /// </summary>
        public long DataTimeTick4;

        /// <summary>
        /// 300 Chars
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
        public readonly char[] szStringDataA = new char[300];

        /// <summary>
        /// 300 Chars
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
        public readonly char[] szStringDataB = new char[300];

        /// <summary>
        /// 150 Chars
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 150)]
        public readonly char[] szStringData150 = new char[150];

        //18 + 120 + 40 + 96 + 600 + 150 = 1024
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PacketBigString
    {
        public ushort Packet_Type; //TYPE_??
        public uint idTo;
        public uint idFrom;
        public uint StringLength;
        public uint Extra;

        //1024 - 18  = 1006 bytes are left
        //4096 - 18  = 4078 bytes are left
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1006)]
        public char[] sBigString = new char[1006];

        /* NOTE: 
         * Remember to do a search for the value '1006' in 
         * the project if you change the packet size here 
         * and replace the value to the new size
        */
    }


    public static class PacketFunctions
    {
        #region Byte Combobulator and Discombobulator

        public static byte[] StructureToByteArray(object obj)
        {
            var rawSize = Marshal.SizeOf(obj);
            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(obj, buffer, false);
            var rawData = new byte[rawSize];
            Marshal.Copy(buffer, rawData, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawData;
        }

        public static object ByteArrayToStructure(byte[] rawData, Type anytype)
        {
            var rawSize = Marshal.SizeOf(anytype);
            if (rawSize > rawData.Length)
                return null;

            var buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.Copy(rawData, 0, buffer, rawSize);
            var retObj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);
            return retObj;
        }

        #endregion
    }

    public static class EnumerationExtensions
    {
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int) (object) type & (int) (object) value) == (int) (object) value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int) (object) type == (int) (object) value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T) (object) (((int) (object) type | (int) (object) value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T) (object) (((int) (object) type & ~(int) (object) value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }
    }
}