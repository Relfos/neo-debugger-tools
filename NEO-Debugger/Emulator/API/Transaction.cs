using Neo.Debugger;
using Neo.VM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Emulator.API
{
    public class Transaction 
    {
        public TransactionInput[] inputs;
        public TransactionOutput[] outputs;

        [Syscall("Neo.Transaction.GetHash")]
        public static bool GetHash(ExecutionEngine engine)
        {
            //Transaction
            // returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetType")]
        public static bool GetType(ExecutionEngine engine)
        {
            //Transaction
            // returns byte 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetAttributes")]
        public static bool GetAttributes(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionAttribute[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetInputs")]
        public static bool GetInputs(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionInput[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetOutputs")]
        public static bool GetOutputs(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionOutput[]

            return GetReferences(engine);
        }

        [Syscall("Neo.Transaction.GetReferences", 0.2)]
        public static bool GetReferences(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var debugger = obj.GetInterface<NeoDebugger>();

            var transactions = new List<StackItem>();

            foreach (var entry in debugger.transaction)
            {
                var tx = new TransactionOutput() { ammount = entry.ammount, id = entry.id };
                transactions.Add(new VM.Types.InteropInterface(tx));
            }

            var outputs = new VM.Types.Array(transactions.ToArray<StackItem>());

            engine.EvaluationStack.Push(outputs);

            return true;
        }
    }
}
