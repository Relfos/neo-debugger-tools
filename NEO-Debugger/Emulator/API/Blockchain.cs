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

        [Syscall("Neo.Blockchain.GetHeader")]
        public static bool GetHeader(ExecutionEngine engine)
        {
            //byte[] hash
            //OR
            //uint height
            // returns Header
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetBlock")]
        public static bool GetBlock(ExecutionEngine engine)
        {
            //uint height
            //OR
            //byte[] hash
            //returns Block 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetTransaction")]
        public static bool GetTransaction(ExecutionEngine engine)
        {
            //byte[] hash
            //returns Transaction 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAccount")]
        public static bool GetAccount(ExecutionEngine engine)
        {
            //byte[] script_hash
            // returns Account 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetValidators")]
        public static bool GetValidators(ExecutionEngine engine)
        {
            //returns byte[][]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAsset")]
        public static bool GetAsset(ExecutionEngine engine)
        {
            //byte[] asset_id
            // returns Asset
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetContract")]
        public static bool GetContract(ExecutionEngine engine)
        {
            //byte[] script_hash
            //returns Contract
            throw new NotImplementedException();
        }
    }
}
