using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class TransactionOutput : IApiInterface
    {
        [Syscall("Neo.Output.GetAssetId")]
        public static bool GetAssetId(ExecutionEngine engine)
        {
            //TransactionOutput
            // returns byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Output.GetValue")]
        public static bool GetValue(ExecutionEngine engine)
        {
            //TransactionOutput
            // returns long
            throw new NotImplementedException();
        }

        [Syscall("Neo.Output.GetScriptHash")]
        public static bool GetScriptHash(ExecutionEngine engine)
        {
            //TransactionOutput
            // returns byte[] 
            throw new NotImplementedException();
        }
    }
}
