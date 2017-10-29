using Neo.Tools.AVM;
using Neo.VM;
using System;
using System.Diagnostics;

namespace Neo.Emulator.API
{
    public static class Runtime
    {
        public static Action<string> OnLogMessage;

        [Syscall("Neo.Runtime.GetTrigger")]
        public static bool GetTrigger(ExecutionEngine engine)
        {
            TriggerType result = TriggerType.Application;

            engine.EvaluationStack.Push((int)result);
            return true;
        }

        [Syscall("Neo.Runtime.CheckWitness", 0.2)]
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
            var something = engine.EvaluationStack.Pop();
            if (something.IsArray)
            {
                var items = something.GetArray();
                foreach (var item in items)
                {
                    LogItem(item);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        [Syscall("Neo.Runtime.Log")]
        public static bool Log(ExecutionEngine engine)
        {
            var msg = engine.EvaluationStack.Pop();
            LogItem(msg);
            return true;
        }

        private static void LogItem(StackItem item)
        {
            var bytes = item.GetByteArray();
            var msg = FormattingUtils.OutputData(bytes, false);

            Debug.WriteLine(msg);

            if (OnLogMessage != null)
            {
                OnLogMessage(msg);
            }
        }
    }
}
