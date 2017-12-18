using LunarParser;
using Neo.VM;
using System;
using Neo.Debugger.Utils;

namespace Neo.Emulator.API
{
    public class TransactionInput : IInteropInterface
    {
        public int prevIndex;
        public byte[] prevHash;

        internal void Load(DataNode root)
        {
            var hex = root.GetString("hash");
            this.prevHash = hex.HexToByte();
            this.prevIndex = root.GetInt32("index");
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("input");
            result.AddField("hash", this.prevHash.ByteToHex());
            result.AddField("index", this.prevIndex.ToString());
            return result;
        }

        [Syscall("Neo.Input.GetHash")]
        public bool GetPrevHash(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var input = obj.GetInterface<TransactionInput>();

            if (input == null)
                return false;

            engine.EvaluationStack.Push(input.prevHash);
            return true;
        }

        [Syscall("Neo.Input.GetIndex")]
        public static bool GetPrevIndex(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var input = obj.GetInterface<TransactionInput>();

            if (input == null)
                return false;

            engine.EvaluationStack.Push(input.prevIndex);
            return true;
        }
    }
}
