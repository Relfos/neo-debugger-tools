using System;

namespace NEO_DevShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-NEO Developer Shell- version 0.1");
            var shell = new Shell();

            while (true)
            {
                Console.Write(">");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (!shell.Execute(input))
                {
                    Console.WriteLine("Unknown command. Type HELP for list of commands.");
                }
            }
        }
    }
}
