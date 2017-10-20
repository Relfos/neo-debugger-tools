using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Block : Header
    {
        [Syscall("Neo.Block.GetTransactionCount")]
        public bool GetTransactionCount(ExecutionEngine engine)
        {
            // rerturns int
            throw new NotImplementedException();
        }

        [Syscall("Neo.Block.GetTransactions")]
        public bool GetTransactions(ExecutionEngine engine)
        {
            // returns Transaction[]
            throw new NotImplementedException();
        }


        [Syscall("Neo.Block.GetTransaction")]
        public bool GetTransaction(ExecutionEngine engine)
        {
            //int index
            // returns Transaction 
            throw new NotImplementedException();
        }
    }
}
