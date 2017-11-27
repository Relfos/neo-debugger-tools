//using Neo.SmartContract;
using System;
using System.Linq;
using System.Text;

namespace Neo.Cryptography
{
    public class KeyPair : IEquatable<KeyPair>
    {
        public readonly byte[] PrivateKey;
        public readonly ECC.ECPoint PublicKey;
        public readonly UInt160 PublicKeyHash;

        public KeyPair(byte[] privateKey)
        {
            if (privateKey.Length != 32 && privateKey.Length != 96 && privateKey.Length != 104)
                throw new ArgumentException();
            this.PrivateKey = new byte[32];
            Buffer.BlockCopy(privateKey, privateKey.Length - 32, PrivateKey, 0, 32);
            if (privateKey.Length == 32)
            {
                this.PublicKey = ECC.ECCurve.Secp256r1.G * privateKey;
            }
            else
            {
                this.PublicKey = ECC.ECPoint.FromBytes(privateKey, ECC.ECCurve.Secp256r1);
            }
            this.PublicKeyHash = Crypto.Default.ToScriptHash(PublicKey.EncodePoint(true));
        }

        public bool Equals(KeyPair other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return PublicKeyHash.Equals(other.PublicKeyHash);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyPair);
        }

        public string Export()
        {
            byte[] data = new byte[34];
            data[0] = 0x80;
            Buffer.BlockCopy(PrivateKey, 0, data, 1, 32);
            data[33] = 0x01;
            string wif = data.Base58CheckEncode();
            Array.Clear(data, 0, data.Length);
            return wif;
        }

        public override int GetHashCode()
        {
            return PublicKeyHash.GetHashCode();
        }

        private static byte[] XOR(byte[] x, byte[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException();
            return x.Zip(y, (a, b) => (byte)(a ^ b)).ToArray();
        }
    }
}
