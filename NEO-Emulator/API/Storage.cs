using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Emulator.API
{
    public static class Storage
    {
        public static Dictionary<byte[], byte[]> entries = new Dictionary<byte[], byte[]>(new ByteArrayComparer());
        public static int sizeInBytes { get; private set; }

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
            if (entries.ContainsKey(key))
            {
                data = entries[key];
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

            if (entries.ContainsKey(key))
            {
                var oldEntry = entries[key];
                if (oldEntry != null)
                {
                    sizeInBytes -= oldEntry.Length;
                }
            }

            entries[key] = data;

            if (data != null)
            {
                sizeInBytes += data.Length;
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

            var context = engine.EvaluationStack.Pop();
            var keyItem = engine.EvaluationStack.Pop();

            var key = keyItem.GetByteArray();

            if (entries.ContainsKey(key))
            {
                var oldEntry = entries[key];
                if (oldEntry != null)
                {
                    sizeInBytes -= oldEntry.Length;
                }

                entries.Remove(key);
            }

            return true;
        }

        public static void Load(string path)
        {
            var lines = File.ReadAllLines(path);
            sizeInBytes = 0;
            entries.Clear();
            foreach (var line in lines)
            {
                var temp = line.Split(',');
                var key = Convert.FromBase64String(temp[0]);
                var data = Convert.FromBase64String(temp[1]);

                sizeInBytes += data.Length;

                entries[key] = data;
            }
        }

        public static void Save(string path)
        {
            var sb = new StringBuilder();
            foreach (var entry in entries)
            {
                var key = Convert.ToBase64String(entry.Key);
                var data = Convert.ToBase64String(entry.Value);
                sb.Append(key);
                sb.Append(',');
                sb.AppendLine(data);
            }
            File.WriteAllText(path, sb.ToString());
        }
    }
}
