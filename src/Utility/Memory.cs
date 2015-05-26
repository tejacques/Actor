using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility
{

    public class Offsets
    {

        public const int MouseX = 0x114D1B0;
        public const int MouseY = 0x114D1B4;

        public static readonly int[] CursorCoords = new[] {
            0x00FB02F0,
            0x248,
            0x28,
            0x1FC,
            0x14,
            0xC64,
        };

        public const int CursorX = 0x0;
        public const int CursorY = 0x2;

        public static readonly int[] BoardCoords = new[] {
            0x00FB02F0,
            0x14,
            0x58,
            0x714,
            0x120,
        };

        public const int BoardX = 0x0;
        public const int BoardY = 0x2;

        public static readonly int[] MenuRules = new[] {
            0x01133420,
            0x7F4,
            0x2B0,
            0x0,
            0x1F0,
            0x3C,
        };

        public static readonly int[] BoardRules = new[] {
            0x00FB02F0,
            0x1C,
            0x18,
            0x2A8,
        };

        public static readonly int[] BoardState = new[] {
            0x00FB02F0,
            0x1C,
            0x10,
            0x130,
            0x1C,
            0x0,
        };

        public static readonly int[] MenuSelection = new[] {
            0x00FB02F0,
            0x8,
            0xB8,
            0x1B0,
            0xC0,
        };

        public const int ActiveSelection = 0x0;
        public const int PassiveSelection = 0x2;

        public enum BoardOffsets
        {
            Player0Hand = 0x0,
            Player1Hand = 0x78,
            Board = 0xF0,
            Timer = 0x1D4,
            Turn = 0x1DC,
            BoardPositionOffset = 0x18,
        }

        public enum BoardPositions
        {
            PlayerId = 0x4,
            CardId = 0x8,
        }

    }

    public class MemoryAPI
    {
        [Flags]
        public enum ProcessAccessType
        {
            PROCESS_TERMINATE = (0x0001),
            PROCESS_CREATE_THREAD = (0x0002),
            PROCESS_SET_SESSIONID = (0x0004),
            PROCESS_VM_OPERATION = (0x0008),
            PROCESS_VM_READ = (0x0010),
            PROCESS_VM_WRITE = (0x0020),
            PROCESS_DUP_HANDLE = (0x0040),
            PROCESS_CREATE_PROCESS = (0x0080),
            PROCESS_SET_QUOTA = (0x0100),
            PROCESS_SET_INFORMATION = (0x0200),
            PROCESS_QUERY_INFORMATION = (0x0400),
            All = 0x001F0FFF,
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessType dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            UInt32 dwProcessId);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size,
            out IntPtr lpNumberOfBytesRead);

        public bool ReadProcessMemoryHelper(
            long lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size,
            out int lpNumberOfBytesRead)
        {
            IntPtr lpNumBytesRead;
            var res = ReadProcessMemory(
                m_hProcess,
                new IntPtr(lpBaseAddress),
                buffer,
                size,
                out lpNumBytesRead);

            lpNumberOfBytesRead = lpNumBytesRead.ToInt32();

            return res != 0;
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WriteProcessMemory(
            IntPtr hProcess,
            int lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size, 
            int lpNumberOfBytesWritten); 

        public string ProcessName { get; private set; }
        private int _baseAddr;
        private Process _process;
        private IntPtr m_hProcess = IntPtr.Zero;
        public Process Process
        {
            get
            {
                return this._process;
            } 
        }

        public MemoryAPI(string processName)
        {
            this.ProcessName = processName;
            var processes = Process.GetProcesses();

            this._process = processes
                .FirstOrDefault(process => 
                    process.ProcessName.StartsWith(processName));
            //var processes = Process.GetProcessesByName(ProcessName);
            //this._process = processes.FirstOrDefault();

            this._baseAddr = BaseAddress();

            MemoryAPI.ProcessAccessType access = MemoryAPI.ProcessAccessType.All; 
            m_hProcess = MemoryAPI.OpenProcess(access, true, (UInt32)this._process.Id);
        }
        private int BaseAddress()
        {
            return this
                ._process
                .MainModule
                .BaseAddress
                .ToInt32();
        }

        public long Pointer(params int[] Offsets)
        {
            long pointerAddress = _baseAddr;

            if (Offsets.Length > 1)
            {
                byte[] buff = new byte[4];
                for (int i = 0; i < Offsets.Length - 1; i++)
                {
                    int bytesRead;
                    var cur = pointerAddress;
                    var offset = Offsets[i];
                    var next = cur + offset;
                    //Console.WriteLine("[{0}+{1}], {2}", cur.ToString("X"), offset.ToString("X"), next.ToString("X"));

                    if (0 == cur)
                    {
                        return 0;
                    }

                    var readProcess = ReadProcessMemoryHelper(
                        next,
                        buff,
                        4,
                        out bytesRead);
                    if (readProcess)
                    {
                        pointerAddress = BitConverter.ToUInt32(buff, 0);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            if (0 == pointerAddress)
            {
                return 0;
            }

            var of = Offsets[Offsets.Length - 1];
            //Console.WriteLine("[{0}+{1}], {2}", pointerAddress.ToString("X"), of.ToString("X"), (pointerAddress+of).ToString("X"));
            return pointerAddress + of;
        }

        public byte[] Read(long address, uint bytesToRead, out int bytesRead)
        {
            byte[] buffer = new byte[bytesToRead];

            try
            {

                //Console.WriteLine("Reading " + bytesToRead + " bytes from " + address);

                if (0 == address)
                {
                    bytesRead = 0;
                    return buffer;
                }

                if (!ReadProcessMemoryHelper(address, buffer, bytesToRead, out bytesRead))
                {
                    throw new System.ComponentModel.Win32Exception();
                }
                //Console.WriteLine("BytesToRead: " + bytesToRead + ", Bytes Read: " + bytesRead);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                bytesRead = 0;
                Console.WriteLine("Error encountered during read: {0}", e.Message);
            }

            return buffer;
        }

        public int ReadInt(long pointer)
        {
            int bytesRead;
            var bytes = Read(pointer, 4, out bytesRead);
            return BitConverter.ToInt32(bytes, 0);
        }

        public Int16 ReadInt16(long pointer)
        {
            int bytesRead;
            var bytes = Read(pointer, 2, out bytesRead);
            return BitConverter.ToInt16(bytes, 0);
        }

        public byte ReadByte(long pointer)
        {
            int bytesRead;
            var bytes = Read(pointer, 1, out bytesRead);
            return bytes[0];
        }
    }
}
