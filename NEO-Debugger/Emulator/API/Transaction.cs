using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Transaction 
    {
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
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetReferences", 0.2)]
        public static bool GetReferences(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionOutput[]
            throw new NotImplementedException();
        }
    }
}
