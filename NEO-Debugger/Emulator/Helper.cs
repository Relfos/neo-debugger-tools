using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Emulator
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return key.Sum(b => b);
        }
    }
    public static class Helper
    {
        [Nonemit]
        public static BigInteger AsBigInteger(this byte[] source)
        {
            return new BigInteger(source);
        }

        [Nonemit]
        public static byte[] AsByteArray(this BigInteger source)
        {
            return source.ToByteArray();
        }

        [Nonemit]
        public static byte[] AsByteArray(this string source)
        {
            return System.Text.Encoding.ASCII.GetBytes(source);
        }

        [Nonemit]
        public static string AsString(this byte[] source)
        {
            return System.Text.Encoding.ASCII.GetString(source);
        }

        [OpCode(OpCode.CAT)]
        public static byte[] Concat(this byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        [OpCode(OpCode.SUBSTR)]
        public static byte[] Range(this byte[] source, int index, int count)
        {
            byte[] ret = new byte[count];
            Buffer.BlockCopy(source, index, ret, 0, count);
            return ret;
        }

        [OpCode(OpCode.LEFT)]
        public static byte[] Take(this byte[] source, int count)
        {
            byte[] ret = new byte[count];
            Buffer.BlockCopy(source, 0, ret, 0, count);
            return ret;
        }

    }
}
