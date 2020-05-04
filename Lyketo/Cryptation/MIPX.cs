using System;
using System.IO;

namespace Lyketo.Cryptation
{
    public class MIPX
    {
        public static byte[] Load(Stream stream, uint[] keys, out int elements, out int stride)
        {
            byte[] fcc = new byte[4];
            stream.Read(fcc, 0, 4);

            elements = 0;
            stride = 0;

            if (fcc[0] != 'M' || fcc[1] != 'I' || fcc[2] != 'P' || fcc[3] != 'X')
                return null;

            int ver;
            stream.Read(fcc, 0, 4);
            ver = BitConverter.ToInt32(fcc, 0);

            if (ver != 1)
                return null;

            stream.Read(fcc, 0, 4);
            stride = BitConverter.ToInt32(fcc, 0);

            stream.Read(fcc, 0, 4);
            elements = BitConverter.ToInt32(fcc, 0);

            stream.Read(fcc, 0, 4);
            int szo = BitConverter.ToInt32(fcc, 0);

            byte[] data = new byte[szo];
            stream.Read(data, 0, szo);

            return CryptedObject.Load(new MemoryStream(data), keys);
        }
    }
}
