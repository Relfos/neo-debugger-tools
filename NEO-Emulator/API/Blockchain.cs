using LunarParser;
using LunarParser.JSON;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Emulator.API
{
    public static class Blockchain
    {
        public static uint currentHeight { get { return (uint)blocks.Count; } }
        public static Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();

        public static bool Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            var json = File.ReadAllText(fileName);
            var root = JSONReader.ReadFromString(json);

            blocks.Clear();
            foreach (var child in root.Children)
            {
                if (child.Name.Equals("block"))
                {
                    var block = new Block();
                    if (block.Load(child))
                    {
                        uint index = (uint)(blocks.Count + 1);
                        blocks[index] = block;
                    }
                }
            }

            return true;
        }

        public static void Save(string fileName)
        {
            var result = DataNode.CreateObject("blockchain");
            for (uint i=1; i<=blocks.Count; i++)
            {
                var block = blocks[i];
                result.AddNode(block.Save());
            }

            var json = JSONWriter.WriteToString(result);
            File.WriteAllText(fileName, json);
        }

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
