using Neo.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEO_Sign
{
    class Program
    {
        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static byte[] HexToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        static void Main(string[] args)
        {
            byte[] hashData = System.Text.Encoding.UTF8.GetBytes("Hello");

            var priv = HexToByteArray("1480ac44ae8081f67242b9aaef8349f57d25e144d1ec1609a5f0eea4bc97d014");
            var key = new KeyPair(priv);

            var pubkey = key.PublicKey.EncodePoint(false).Skip(1).ToArray();
            var signature = Crypto.Default.Sign(hashData, key.PrivateKey, pubkey);

            var verification = Crypto.Default.VerifySignature(hashData, signature, pubkey);
            Console.WriteLine(verification);
        }
    }
}
