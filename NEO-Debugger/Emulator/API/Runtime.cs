using Neo.VM;
using System.Diagnostics;

namespace Neo.Emulator.API
{
    public static class Runtime
    {
        [Syscall("Neo.Runtime.GetTrigger")]
        public static bool GetTrigger(ExecutionEngine engine)
        {
            TriggerType result = TriggerType.Application;

            engine.EvaluationStack.Push((int)result);
            return true;
        }

        [Syscall("Neo.Runtime.CheckWitness")]
        public static bool CheckWitness(ExecutionEngine engine)
        {
            //byte[] hashOrPubkey
            engine.EvaluationStack.Push(false);
            return true;
        }

        [Syscall("Neo.Runtime.Notify")]
        public static bool Notify(ExecutionEngine engine)
        {
            //params object[] state
            return false;
        }

        [Syscall("Neo.Runtime.Log")]
        public static bool Log(ExecutionEngine engine)
        {
            var msg = engine.EvaluationStack.Pop().GetString();
            Debug.WriteLine(msg);
            return true;
        }
    }
}
