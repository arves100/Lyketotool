using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Lyketo.Unsafe
{
    /// <summary>
    /// Interface to Lyketo.Unsafe Lzo implementation
    /// </summary>
    public class Lzo
    {
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_lzo_compress", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LCompress(byte[] src, uint src_len);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_lzo_decompress", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LDecompress(byte[] src, uint src_len, uint dst_len);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_free_memory", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint FreeMem();
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_copy_buffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CopyBuffer(byte[] data, uint len);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_lzo_get_size", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetSize();

        public static byte[] Compress(byte[] src, uint src_len, out uint out_len)
        {
            out_len = 0;

            int r = LCompress(src, src_len);

            if (r != 0)
            {
                FreeMem();
                return null;
            }

            out_len = GetSize();

            byte[] data = new byte[out_len];
            CopyBuffer(data, out_len);

            FreeMem();

            return data;
        }

        public static byte[] Decompress(byte[] src, uint src_len, uint out_len)
        {
            int r = LDecompress(src, src_len, out_len);

            if (r != 0)
            {
                FreeMem();
                return null;
            }

            if (GetSize() != out_len)
            {
                FreeMem();
                return null;
            }

            byte[] data = new byte[out_len];
            CopyBuffer(data, out_len);

            FreeMem();

            return data;
        }
    }
}
