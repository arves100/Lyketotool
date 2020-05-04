using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lyketo.Cryptation;

namespace Lyketo.Formats
{
    /// <summary>
    /// Implementation of LZO format (Either MIPT or MIPX)
    /// </summary>
    public class LZOFormat : IFormat
    {
        private int Stride;
        private int Elements;
        private byte[] Data;
        private uint[] Keys;
        private bool Write;
        private int Offset;
        private bool Mob;

        public LZOFormat(uint[] keys, bool mob)
        {
            Keys = keys;
            Mob = mob;
        }

        public bool Finalize()
        {
            if (!Write)
                return true;

            return false; // @todo
        }

        public bool Initialize(string fileName, bool write)
        {
            Write = write;
            if (!write)
            {
                FileStream stream = new FileStream(fileName, FileMode.Open);
                Data = MIPX.Load(stream, Keys, out Elements, out Stride);

                if (Data == null)
                    return false;

                Offset = 0;
                return true;
            }

            return false; // @todo

        }

        public void Next()
        {
            // LZO does not need any "next"
        }

        public int Count()
        {
            if (!Write)
                return Elements;

            return 0;
        }

        public string GetString(string field, int len)
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                if (Data[Offset + i] == '\0')
                    break;

                data.Add(Data[Offset + i]);
            }

            Offset += len;
            return Encoding.ASCII.GetString(data.ToArray());
        }

        public double GetDouble(string field)
        {
            Offset += 8;
            return (double)BitConverter.ToInt64(Data, Offset - 8);
        }

        public float GetFloat(string field)
        {
            Offset += 4;
            return (float)BitConverter.ToInt32(Data, Offset - 4);
        }

        public short GetInt16(string field)
        {
            Offset += 2;
            return BitConverter.ToInt16(Data, Offset - 2);
        }

        public int GetInt32(string field)
        {
            Offset += 4;
            return BitConverter.ToInt32(Data, Offset - 4);
        }

        public long GetInt64(string field)
        {
            Offset += 8;
            return BitConverter.ToInt64(Data, Offset - 8);
        }

        public char GetInt8(string field)
        {
            Offset += 1;
            return (char)Data[Offset - 1];
        }

        public ushort GetUInt16(string field)
        {
            Offset += 2;
            return BitConverter.ToUInt16(Data, Offset - 2);
        }

        public uint GetUint32(string field)
        {
            Offset += 4;
            return BitConverter.ToUInt32(Data, Offset - 4);
        }

        public ulong GetUInt64(string field)
        {
            Offset += 8;
            return BitConverter.ToUInt64(Data, Offset - 8);
        }

        public byte GetUInt8(string field)
        {
            Offset += 1;
            return Data[Offset - 1];
        }

        public void Set(string field, string value, int len)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, uint value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, int value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, short value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, ushort value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, float value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, double value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, char value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, byte value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, long value)
        {
            throw new NotImplementedException();
        }

        public void Set(string field, ulong value)
        {
            throw new NotImplementedException();
        }

        public string FilterName()
        {
            return "LZO files (item_proto, mob_proto)|item_proto;mob_proto";
        }
    }
}
