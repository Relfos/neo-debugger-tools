using Neo.VM;
using Neo.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Emulator;
using System;

namespace Neo.Debugger
{
    public struct DebuggerState
    {
        public enum State
        {
            Running,
            Finished,
            Exception,
            Break
        }

        public readonly State state;
        public readonly int offset;

        public DebuggerState(State state, int offset)
        {
            this.state = state;
            this.offset = offset;
        }
    }

    public class NeoDebugger
    {
        private ExecutionEngine engine;
        private byte[] contractBytes;
        private InteropService interop;

        private HashSet<int> breakpoints = new HashSet<int>();

        public NeoDebugger(byte[] contractBytes)
        {
            this.interop = new InteropService();
            this.contractBytes = contractBytes;

            var assembly = typeof(Neo.Emulator.Helper).Assembly;
            var methods = assembly.GetTypes()
                                  .SelectMany(t => t.GetMethods())
                                  .Where(m => m.GetCustomAttributes(typeof(SyscallAttribute), false).Length > 0)
                                  .ToArray();

            foreach (var method in methods)
            {
                var attr = (SyscallAttribute) method.GetCustomAttributes(typeof(SyscallAttribute), false).FirstOrDefault();
                interop.Register(attr.Method, (engine) => { return (bool) method.Invoke(null, new object[] { engine }); });
            }

            this.Reset();
        }

        public bool Finished { get; private set; }
        private bool isReset;
        private int lastOffset = -1;

        public void Reset()
        {
            if (isReset)
            {
                return;
            }

            Finished = false;

            engine = new ExecutionEngine(null, Crypto.Default, null, interop);
            engine.LoadScript(contractBytes);

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush("symbol");
                engine.LoadScript(sb.ToArray());
            }

            foreach (var pos in breakpoints)
            {
                engine.AddBreakPoint((uint)pos);
            }

            engine.Reset();

            isReset = true;
        }

        public void SetBreakpointState(int ofs, bool enabled)
        {
            if (enabled)
            {
                breakpoints.Add(ofs);
            }
            else
            {
                breakpoints.Remove(ofs);
            }

            try
            {
                if (enabled)
                {
                    engine.AddBreakPoint((uint)ofs);
                }
                else
                {
                    engine.RemoveBreakPoint((uint)ofs);
                }
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// executes a single instruction in the current script, and returns the last script offset
        /// </summary>
        public DebuggerState Step()
        {
            if (Finished)
            {
                return new DebuggerState(DebuggerState.State.Finished, lastOffset);
            }

            isReset = false;
            engine.ExecuteSingleStep();

            try
            {
                lastOffset = engine.CurrentContext.InstructionPointer;
            }
            catch
            {
                // failed to get instruction pointer
            }

            if (engine.State.HasFlag(VMState.FAULT))
            {
                return new DebuggerState(DebuggerState.State.Exception, lastOffset);
            }

            if (engine.State.HasFlag(VMState.BREAK))
            {
                return new DebuggerState(DebuggerState.State.Break, lastOffset);
            }

            if (engine.State.HasFlag(VMState.HALT))
            {
                Finished = true;
                return new DebuggerState(DebuggerState.State.Finished, lastOffset);
            }

            return new DebuggerState(DebuggerState.State.Running, lastOffset);
        }

        /// <summary>
        /// executes the script until it finishes, fails or hits a breakpoint
        /// </summary>
        public DebuggerState Run()
        {
            DebuggerState lastState;
            do
            {
                lastState = Step();
            } while (lastState.state == DebuggerState.State.Running);

            return lastState;
        }

        public object GetResult()
        {
            var result = engine.EvaluationStack.Peek();
            return result.GetString();
        }

    }
}
