using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Asset
    {
        [Syscall("Neo.Asset.GetAssetId")]
        public static bool GetAssetId(ExecutionEngine engine)
        {
            // Asset
            // return byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAssetType")]
        public static bool GetAssetType(ExecutionEngine engine)
        {
            // Asset
            //return byte
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAmount")]
        public static bool GetAmount(ExecutionEngine engine)
        {
            // Asset
            //returns long
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAvailable")]
        public static bool GetAvailable(ExecutionEngine engine)
        {
            // Asset
            //returns long 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetPrecision")]
        public static bool GetPrecision(ExecutionEngine engine)
        {
            // Asset
            //return byte 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetOwner")]
        public static bool GetOwner(ExecutionEngine engine)
        {
            // Asset
            //returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAdmin")]
        public static bool GetAdmin(ExecutionEngine engine)
        {
            // Asset
            // void byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetIssuer")]
        public static bool GetIssuer(ExecutionEngine engine)
        {
            // Asset
            //return byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.Create")]
        public static bool Create(ExecutionEngine engine)
        {
            //byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer
            // retunrs Asset 
            throw new NotImplementedException();

        }

        [Syscall("Neo.Asset.Renew")]
        public static bool Renew(ExecutionEngine engine)
        {
            //byte years
            //returns uint 
            throw new NotImplementedException();

        }
    }
}
