using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Header 
    {
        [Syscall("Neo.Header.GetHash")]
        public static bool GetHash(ExecutionEngine engine)
        {
            // Header
            //returns  byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetVersion")]
        public static bool GetVersion(ExecutionEngine engine)
        {
            // Header
            //returns uint 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetPrevHash")]
        public static bool GetPrevHash(ExecutionEngine engine)
    {
            // Header
            //returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetMerkleRoot")]
        public static bool GetMerkleRoot(ExecutionEngine engine)
        {
            // Header
            //returns  byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetTimestamp")]
        public static bool GetTimestamp(ExecutionEngine engine)
        {
            // Header
            //returns uint 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetConsensusData")]
        public static bool GetConsensusData(ExecutionEngine engine)
        {
            // Header
            //returns ulong 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Header.GetNextConsensus")]
        public static bool GetNextConsensus(ExecutionEngine engine)
        {
            // Header
            //returns byte[] 
            throw new NotImplementedException();
        }
    }
}
