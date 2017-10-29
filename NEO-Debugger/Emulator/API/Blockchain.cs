using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public static class Blockchain
    {
        [Syscall("Neo.Blockchain.GetHeight")]
        public static bool GetHeight(ExecutionEngine engine)
        {
            //returns uint
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetHeader", 0.1)]
        public static bool GetHeader(ExecutionEngine engine)
        {
            //byte[] hash
            //OR
            //uint height
            // returns Header
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetBlock", 0.2)]
        public static bool GetBlock(ExecutionEngine engine)
        {
            //uint height
            //OR
            //byte[] hash
            //returns Block 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetTransaction", 0.1)]
        public static bool GetTransaction(ExecutionEngine engine)
        {
            //byte[] hash
            //returns Transaction 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAccount", 0.1)]
        public static bool GetAccount(ExecutionEngine engine)
        {
            //byte[] script_hash
            // returns Account 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetValidators", 0.2)]
        public static bool GetValidators(ExecutionEngine engine)
        {
            //returns byte[][]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAsset", 0.1)]
        public static bool GetAsset(ExecutionEngine engine)
        {
            //byte[] asset_id
            // returns Asset
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetContract", 0.1)]
        public static bool GetContract(ExecutionEngine engine)
        {
            //byte[] script_hash
            //returns Contract
            throw new NotImplementedException();
        }
    }
}
