using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Emulator.API
{
    public static class Storage
    {
        public static Dictionary<byte[], byte[]> storage = new Dictionary<byte[], byte[]>(new ByteArrayComparer());

        public static int lastStorageLength;

        [Syscall("Neo.Storage.GetContext")]
        public static bool GetCurrentContext(ExecutionEngine engine)
        {
            var context = new Neo.VM.Types.Integer(0);
            engine.EvaluationStack.Push(context);
            //returns StorageContext 
            return true;
        }

        [Syscall("Neo.Storage.Get", 0.1)]
        public static bool Get(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key

            //returns byte[]

            var context = engine.EvaluationStack.Pop();
            var item = (VM.Types.ByteArray) engine.EvaluationStack.Pop();

            var key = item.GetByteArray();

            byte[] data = null;
            if (storage.ContainsKey(key))
            {
                data = storage[key];
            }

            if (data == null)
            {
                data = new byte[0];
            }

            var result = new VM.Types.ByteArray(data);
            engine.EvaluationStack.Push(result);

            return true;
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

            var context = engine.EvaluationStack.Pop();
            var keyItem = engine.EvaluationStack.Pop();
            var dataItem = engine.EvaluationStack.Pop();

            var key = keyItem.GetByteArray();
            var data = dataItem.GetByteArray();

            storage[key] = data;

            lastStorageLength = data != null ? data.Length : 0;

            return true;
        }

        [Syscall("Neo.Storage.Delete")]
        public static bool Delete(ExecutionEngine engine)
        {
            //StorageContext context, byte[] key
            //OR
            //StorageContext context, string key
            //data.Remove(key);
            throw new NotImplementedException();
        }

    }
}
