using Neo.VM;
using System;
using System.Numerics;

namespace Neo.Emulator.API
{
    public static class Storage
    {
        [Syscall("Neo.Storage.GetContext")]
        public static bool GetCurrentContext(ExecutionEngine engine)
        {
            //returns StorageContext 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Storage.Get", 0.1)]
        public static bool Get(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key

            //returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Storage.Put", 1)]
        public static bool Put(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key, byte[] value
            //OR
            //StorageContext context, byte[] key, BigInteger value
            //OR
            //StorageContext context, byte[] key, string value
            //OR
            //StorageContext context, string key, byte[] value
            //OR
            //StorageContext context, string key, BigInteger value
            //OR
            //StorageContext context, string key, string value
            // return void
            throw new NotImplementedException();
        }

        [Syscall("Neo.Storage.Delete")]
        public static bool Delete(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key
            throw new NotImplementedException();
        }

    }
}
