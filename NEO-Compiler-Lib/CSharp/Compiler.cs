using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Numerics;

namespace Neo.Compiler
{
    public class CSharpCompiler
    {
        private static string GenerateMainMethod(TypeDefinition t)
        {
            var code = new List<string>();
            code.Add("\tpublic static Object Main(string operation, params object[] args) {");

            code.Add("\tif (Runtime.Trigger == TriggerType.Verification) {");
            code.Add("\t\tif (Owner.Length == 20) {");            
            code.Add("\t\t\treturn Runtime.CheckWitness(Owner); // if param Owner is script hash");
            code.Add("\t\t}");
            code.Add("\telse if (Owner.Length == 33) {");            
            code.Add("\tbyte[] signature = operation.AsByteArray(); // if param Owner is public key");
            code.Add("\treturn VerifySignature(signature, Owner);");
            code.Add("\t\t}");
            code.Add("\t}");

            var methods = t.Methods;
            foreach (var method in methods)
            {
                if (!method.IsStatic)
                {
                    continue;
                }

                if (method.Name == ".cctor")
                {
                    continue;
                }

                if (method.HasParameters)
                {
                    var first = method.Parameters[0];
                    var argType = first.ParameterType;
                    if (first.Name == "value" && argType.Name.Contains("Action`"))
                    {
                        continue;
                    }
                }

                var parameters = method.Parameters;
                var parameterDescriptions = string.Join(", ", method.Parameters.Select(x => x.ParameterType + " " + x.Name).ToArray());

                Console.WriteLine("{0} {1} ({2})",
                                  method.ReturnType,
                                  method.Name,
                                  parameterDescriptions);
            }
            code.Add("}");

            var output = string.Join("\n", code.ToArray());
            return output;
        }

        public static bool Execute(string code, ILogger logger)
        {
            var MaxLanguageVersion = Enum
            .GetValues(typeof(LanguageVersion))
            .Cast<LanguageVersion>()
            .Max();

            var options = new CSharpParseOptions(kind: SourceCodeKind.Regular, languageVersion: MaxLanguageVersion);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, options);


            /*var assemblyPath = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var neededAssemblies = new[]
            {
            "mscorlib",
            "System.Runtime",
            "System.Core",
            "System.Numerics",
            "Neo.SmartContract.Framework",
            };
            
            IEnumerable<MetadataReference> references = assemblyPath
                .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

             */

            string assemblyPath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\";

            var neededAssemblies = new string[]
            {
                /*"mscorlib",
                "System",
                "System.Numerics",*/
            };

            List<MetadataReference> references = neededAssemblies.Select(x => MetadataReference.CreateFromFile(assemblyPath + x + ".dll")).ToList<MetadataReference>();

            references.Add(MetadataReference.CreateFromFile("Neo.SmartContract.Framework.dll"));

            var assemblyName = "Test.dll";

            var compileOptions = new CSharpCompilationOptions(
                           OutputKind.DynamicallyLinkedLibrary,
                           optimizationLevel: OptimizationLevel.Debug,
                           allowUnsafe: true);

            Compilation compilation = CSharpCompilation.Create(assemblyName, options: compileOptions, references: references, syntaxTrees: new[] { syntaxTree });

            var stream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(stream, pdbStream);

            if (emitResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                AssemblyDefinition assemblyInfo = AssemblyDefinition.ReadAssembly(stream);

                var mod = assemblyInfo.Modules.FirstOrDefault();
                foreach (var t in mod.Types)
                {
                    if (!t.Name.StartsWith("<"))
                    {
                        GenerateMainMethod(t);
                    }
                }

                return true;
            }
            else
            {
                foreach (var d in emitResult.Diagnostics)
                {
                    var lineSpan = d.Location.GetLineSpan();
                    var startLine = lineSpan.StartLinePosition.Line;
                    logger.Log(string.Format("Line {0}: {1}", startLine, d.GetMessage()));
                }
                return false;
            }
        }

    }
}
