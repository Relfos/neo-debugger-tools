using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Mono.Cecil;

namespace NEO.Ide
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = @"
        using System;
        public class ExampleClass {
            
            private readonly string _message;

            public ExampleClass()
            {
                _message = ""Hello World"";
            }

            public string getMessage()
            {
                return _message;
            }

        }";

            CreateAssemblyDefinition(code);
        }


        public static void CreateAssemblyDefinition(string code)
        {

            var sourceLanguage = new CSharpLanguage();
            SyntaxTree syntaxTree = sourceLanguage.ParseText(code, SourceCodeKind.Regular);
            
            Compilation compilation = sourceLanguage
                  .CreateLibraryCompilation(assemblyName: "InMemoryAssembly", enableOptimisations: false)
              //    .AddReferences(_references)
                  .AddSyntaxTrees(syntaxTree);

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stream);
            }
        }

    }
}
