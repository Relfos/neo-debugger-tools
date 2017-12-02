using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Emulator.API
{
    public class Blockchain
    {
        public static uint currentHeight = 1;
        public static Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();

        [Syscall("Neo.Blockchain.GetHeight")]
        public static bool GetHeight(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(currentHeight);

            return true;
        }

        [Syscall("Neo.Blockchain.GetHeader", 0.1)]
        public static bool GetHeader(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();

            Block block = null;

            var hash = obj.GetByteArray();

            if (hash.Length>1)
            {
                throw new NotImplementedException();
            }

            if (hash.Length == 1)
            { 
                var temp = obj.GetBigInteger();

                var height = (uint)temp;

                if (blocks.ContainsKey(height))
                {
                    block = blocks[height];
                }
                else
                if (height<=currentHeight)
                {
                    block = new Block();
                    block.timestamp = 1506787300;
                    blocks[height] = block;
                }
            }

            if (block == null)
            {
            }

            engine.EvaluationStack.Push(new VM.Types.InteropInterface(block));
            return true;
            // returns Header
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
