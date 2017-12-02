using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class TransactionInput : IApiInterface
    {
        [Syscall("Neo.Input.GetHash")]
        public bool PrevHash()
        {
            //TransactionInput
            // returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Input.GetIndex")]
        public static bool GetPrevIndex()
        {
            //TransactionInput
            // returns ushort 
            throw new NotImplementedException();
        }
    }
}
