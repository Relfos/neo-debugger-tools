using Neo.Debugger;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.Emulator.API
{
    public class TransactionOutput : IApiInterface, IInteropInterface
    {
        public byte[] id;
        public BigInteger ammount;

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

            var debugger = engine.ScriptContainer as NeoDebugger;

            if (debugger == null)
            {
                return false;
            }

            // returns byte[] 
            engine.EvaluationStack.Push(engine.CurrentContext.ScriptHash);

            return true;
        }
    }
}
