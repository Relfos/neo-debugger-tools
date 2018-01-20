using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEO.Ide
{
    public interface ILanguageService
    {
        SyntaxTree ParseText(string code, SourceCodeKind kind);

        Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations);
    }
}
