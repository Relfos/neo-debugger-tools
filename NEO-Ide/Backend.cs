using System;
using SynkServer.Core;
using System.IO;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using SynkServer.HTTP;
using SynkServer.Templates;
using System.Collections.Generic;

namespace NEO.Ide
{
    class Backend
    {
        static void Main(string[] args)
        {
            var code = @"
        public class HelloWorld : SmartContract
        {
            public static string Main()
            {
                return 'Hello World';
            }
        }
".Replace("'", "\"");


            // initialize a logger
            var log = new SynkServer.Core.Logger();

            // either parse the settings from the program args or initialize them manually
            var settings = ServerSettings.Parse(args);

            var server = new HTTPServer(log, settings);

            var templateEngine = new TemplateEngine(server, "views");

            // instantiate a new site, the second argument is the file path where the public site contents will be found
            var site = new Site(server, "public");

            site.Get("/", (request) =>
            {
                var context = new Dictionary<string, object>();
                context["code"] = code;

                return templateEngine.Render(site, context, new string[] { "index" });
            });

            server.Run();
        }

        private static void Compile(string code)
        {

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
