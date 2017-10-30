using Neo.Tools.AVM;
using Neo.VM;
using System;
using System.Diagnostics;
using System.Text;

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
                var sb = new StringBuilder();

                var items = something.GetArray();

                int index = 0;

                foreach (var item in items)
                {
                    if (index > 0)
                    {
                        sb.Append(" / ");
                    }

                    sb.Append(FormattingUtils.StackItemAsString(item));
                    index++;
                }

                DoLog(sb.ToString());
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
            DoLog(FormattingUtils.StackItemAsString(msg));
            return true;
        }

        private static void DoLog(string msg)
        {
            Debug.WriteLine(msg);

            if (OnLogMessage != null)
            {
                OnLogMessage(msg);
            }
        }
    }
}
