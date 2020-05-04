using Lyketo.Unsafe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Lyketo.Cryptation
{
    public class CryptedObject
    {
        public static byte[] Load(Stream stream, uint[] keys)
        {
            byte[] int32t = new byte[4], fourcc = new byte[4];

            stream.Read(fourcc, 0, 4);

            if (fourcc[0] == 'M' && fourcc[1] == 'C' && fourcc[2] == 'O' && fourcc[3] == 'Z')
            {

            }
            // @todo: Snappy
            else
                return null;

            stream.Read(int32t, 0, 4);
            uint cryptedSize = BitConverter.ToUInt32(int32t, 0);

            stream.Read(int32t, 0, 4);
            uint compressSize = BitConverter.ToUInt32(int32t, 0);

            stream.Read(int32t, 0, 4);
            uint realSize = BitConverter.ToUInt32(int32t, 0);

            byte[] cdata;

            if (cryptedSize == 0)
            {
                if (stream.Length < compressSize)
                {
                    return null;
                }

                cdata = new byte[compressSize];
                stream.Read(cdata, 0, (int)compressSize);

                if (cdata[0] != fourcc[0] || cdata[1] != fourcc[1] || cdata[2] != fourcc[2] || cdata[3] != fourcc[3])
                {
                    return null;
                }
            }
            else
            {
                if (stream.Length < cryptedSize)
                {
                    return null;
                }

                byte[] data = new byte[cryptedSize];
                stream.Read(data, 0, (int)cryptedSize);

                cdata = Xtea.XteaDecrypt(data, cryptedSize, keys);

                if (cdata[0] != fourcc[0] || cdata[1] != fourcc[1] || cdata[2] != fourcc[2] || cdata[3] != fourcc[3])
                {
                    return null;
                }
            }

            return Lzo.Decompress(cdata, compressSize, realSize);
        }
    }
}
