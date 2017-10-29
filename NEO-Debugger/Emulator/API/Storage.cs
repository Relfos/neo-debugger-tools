using Neo.VM;
using System;
using System.Numerics;

namespace Neo.Emulator.API
{
    public static class Storage
    {
        public static Action<byte[], byte[]> OnPut;
        public static Func<byte[], byte[]> OnGet;
        public static Action<byte[]> OnDelete;

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

            var data = OnGet != null ? OnGet(key) : null;

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
            var keyItem = (VM.Types.ByteArray)engine.EvaluationStack.Pop();
            var dataItem = (VM.Types.ByteArray)engine.EvaluationStack.Pop();

            var key = keyItem.GetByteArray();
            var data = dataItem.GetByteArray();

            if (OnPut != null)
            {
                OnPut(key, data);
            }

            lastStorageLength = data != null ? data.Length : 0;

            return true;
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
