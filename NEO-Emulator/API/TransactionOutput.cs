using LunarParser;
using Neo.Emulator.Utils;
using Neo.VM;
using System.Numerics;

namespace Neo.Emulator.API
{
    public class TransactionOutput : IApiInterface, IInteropInterface
    {
        public byte[] id;
        public BigInteger ammount;
        public byte[] hash;

        internal void Load(DataNode root)
        {
            var hex = root.GetString("id");
            this.id = hex.HexToByte();

            hex = root.GetString("hash");
            this.hash = hex.HexToByte();

            var amm = root.GetString("ammount");
            this.ammount = BigInteger.Parse(amm);
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("output");
            result.AddField("id", this.id.ByteToHex());
            result.AddField("hash", this.hash.ByteToHex());
            result.AddField("ammount", this.ammount.ToString());
            return result;
        }

        [Syscall("Neo.Output.GetAssetId")]
        public static bool GetAssetId(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if  (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();

            engine.EvaluationStack.Push(tx.id);
            return true;
        }

        [Syscall("Neo.Output.GetValue")]
        public static bool GetValue(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();

            engine.EvaluationStack.Push(tx.ammount);
            return true;
        }

        [Syscall("Neo.Output.GetScriptHash")]
        public static bool GetScriptHash(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();
            engine.EvaluationStack.Push(tx.hash);

            /*var debugger = engine.ScriptContainer as NeoDebugger;

            if (debugger == null)
            {
                return false;
            }

            engine.EvaluationStack.Push(engine.CurrentContext.ScriptHash);
            */

            return true;
        }
    }
}
