using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NEO.Ide
{
    public class CSharpLanguage : ILanguageService
    {
        private static readonly LanguageVersion MaxLanguageVersion = Enum
            .GetValues(typeof(LanguageVersion))
            .Cast<LanguageVersion>()
            .Max();

        private readonly IReadOnlyCollection<MetadataReference> _references = new[] {
          MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location)
        };

        public SyntaxTree ParseText(string sourceCode, SourceCodeKind kind)
        {
            var options = new CSharpParseOptions(kind: kind, languageVersion: MaxLanguageVersion);

            // Return a syntax tree of our source code
            return CSharpSyntaxTree.ParseText(sourceCode, options);
        }

        public Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations)
        {
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: enableOptimisations ? OptimizationLevel.Release : OptimizationLevel.Debug,
                allowUnsafe: true);

            return CSharpCompilation.Create(assemblyName, options: options, references: _references);
        }

    }
}
