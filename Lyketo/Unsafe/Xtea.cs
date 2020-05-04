using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Lyketo.Unsafe
{
    /// <summary>
    /// Connector to unsafe Xtea in Lyketo.Unsafe
    /// </summary>
    public class Xtea
    {
        #region DllImports
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_xtea_crypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool LXteaCrypt(uint[] data, uint len, uint[] key);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_xtea_decrypt", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool LXteaDecrypt(uint[] data, uint len, uint[] key);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_copy_buffer", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CopyBuffer(byte[] data, uint len);
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_get_buffer_size", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint BufferSize();
        [DllImport("Lyketo.Unsafe.dll", EntryPoint = "l_free_memory", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint FreeMem();
        
        #endregion

        public static byte[] XteaCrypt(byte[] data, uint[] key, out uint len)
        {
            len = 0;

            var n = ToUintArray(data);
            if (!LXteaCrypt(n, (uint)data.Length, key))
            {
                FreeMem();
                return null;
            }

            len = BufferSize();

            byte[] r = new byte[len];
            CopyBuffer(r, len);

            FreeMem();

            return r;
        }

        public static byte[] XteaDecrypt(byte[] data, uint len, uint[] key)
        {
            var n = ToUintArray(data);

            if (!LXteaDecrypt(n, (uint)data.Length, key))
            {
                FreeMem();
                return null;
            }

            byte[] r = new byte[len];
            CopyBuffer(r, len);

            FreeMem();

            return r;
        }

        /// <summary>
        /// Ref: metin2oldexperience PacketCrypt.cs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static uint[] ToUintArray(byte[] input)
        {
            List<uint> array = new List<uint>();

            int cb = 0;
            byte[] n = new byte[4];

            for (int i = 0; i < input.Length; i++)
            {
                n[cb] = input[i];

                if (cb == 3)
                {
                    array.Add(BitConverter.ToUInt32(n, 0));

                    cb = 0;

                    n[0] = 0;
                    n[1] = 0;
                    n[2] = 0;
                    n[3] = 0;
                }
                else
                    cb++;
            }

            if (cb != 0)
            {
                array.Add(BitConverter.ToUInt32(n, 0));
            }

            return array.ToArray();
        }
    }
}
