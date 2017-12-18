using Neo.Debugger;
using Neo.VM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using LunarParser;

namespace Neo.Emulator.API
{
    public class Transaction : IInteropInterface
    {
        public List<TransactionInput> inputs = new List<TransactionInput>();
        public List<TransactionOutput> outputs = new List<TransactionOutput>();

        [Syscall("Neo.Transaction.GetHash")]
        public static bool GetHash(ExecutionEngine engine)
        {
            //Transaction
            // returns byte[]
            throw new NotImplementedException();
        }

        internal bool Load(DataNode root)
        {
            inputs.Clear();
            outputs.Clear();

            foreach (var child in root.Children)
            {
                if (child.Name == "input")
                {
                    var input = new TransactionInput();
                    input.Load(child);
                    inputs.Add(input);
                }

                if (child.Name == "output")
                {
                    var output = new TransactionOutput();
                    output.Load(child);
                    outputs.Add(output);
                }
            }

            return true;
        }


        public DataNode Save()
        {
            var result = DataNode.CreateObject("transaction");
            foreach (var input in inputs)
            {
                result.AddNode(input.Save());
            }
            foreach (var output in outputs)
            {
                result.AddNode(output.Save());
            }
            return result;
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
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<Transaction>();

            var transactions = new List<StackItem>();

            foreach (var entry in tx.inputs)
            {
                transactions.Add(new VM.Types.InteropInterface(entry));
            }

            var outputs = new VM.Types.Array(transactions.ToArray<StackItem>());

            engine.EvaluationStack.Push(outputs);

            return true;
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

            var tx = obj.GetInterface<Transaction>();

            var transactions = new List<StackItem>();

            foreach (var entry in tx.outputs)
            {
                transactions.Add(new VM.Types.InteropInterface(entry));
            }

            var outputs = new VM.Types.Array(transactions.ToArray<StackItem>());

            engine.EvaluationStack.Push(outputs);

            return true;
        }
    }
}
