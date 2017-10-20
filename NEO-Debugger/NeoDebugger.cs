using Neo.VM;
using Neo.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Emulator;
using System;

namespace Neo.Debugger
{
    public struct DebugMapEntry
    {
        public string url;
        public int line;

        public int startOfs;
        public int endOfs;
    }

    public class NeoDebugger
    {
        private ExecutionEngine engine;

        public NeoDebugger(byte[] contractBytes)
        {
            var interop = new InteropService();

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

            engine = new ExecutionEngine(null, Crypto.Default, null, interop);
            engine.LoadScript(contractBytes);

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush("symbol");
                engine.LoadScript(sb.ToArray());
            }

            //engine.Execute(); // start execution


            this.Reset();
        }

        public bool Finished { get; private set; }

        public void Reset()
        {
            Finished = false;
            engine.Reset();
        }

        /// <summary>
        /// returns current script offset
        /// </summary>
        public int Step()
        {
            Finished = !engine.ExecuteSingleStep();
            return Finished ? 0 : engine.CurrentContext.InstructionPointer;
        }

        public static List<DebugMapEntry> LoadMapFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
                    
            var lines = File.ReadAllLines(path);
            var result = new List<DebugMapEntry>();
            foreach (var line in lines)
            {
                var temp = line.Split(new char[] { ',' }, 4);

                var entry = new DebugMapEntry();
                int.TryParse(temp[0], out entry.startOfs);
                int.TryParse(temp[1], out entry.endOfs);
                int.TryParse(temp[2], out entry.line);
                entry.url = temp[3];
                result.Add(entry);
            }
            return result;
        }

        public object GetResult()
        {
            var result = engine.EvaluationStack.Peek();
            return result.GetString();
        }
    }
}
