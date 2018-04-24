using System;
using System.Linq;

namespace libfdt
{
    public static class Utils
    {
        public static UInt32 PopUInt32(this byte[] array, ref UInt32 offset)
        {
            UInt32 parsedValue = ReadUInt32(array, offset);

            offset += 4;

            return parsedValue;
        }

        public static UInt32 ReadUInt32(this byte[] array, UInt32 offset)
        {
            byte[] bytes = array.Skip((int)offset).Take(4).Reverse().ToArray();

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static string PopString(this byte[] array, ref UInt32 offset)
        {
            String parsedString = ReadString(array, offset);

            offset += (UInt32)parsedString.Length + 1;

            return parsedString;
        }

        public static string ReadString(this byte[] array, UInt32 offset)
        {
            UInt32 end = offset;
            while (end < array.Length && array[end] != 0)
            {
                end++;
            }

            unsafe
            {
                fixed (byte* pAscii = array)
                {
                    return new String((sbyte*)pAscii, (int)offset, (int)(end - offset));
                }
            }
        }
    }
}
