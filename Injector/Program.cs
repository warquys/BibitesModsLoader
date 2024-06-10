using System;
using dnlib.DotNet;

// Origanl Code from https://github.com/SynapseSL/Synapse Synapse3.Injector

namespace Injector
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var injector = new Injector();
            try
            {
                string path;
                if (args.Length == 0)
                {
                    Console.WriteLine("Location of the assembly:");
                    path = Console.ReadLine();
                }
                else
                {
                    path = args[0];
                }

                var loadModule = ModuleDefMD.Load(path, new ModuleCreationOptions());

                injector.Start(loadModule);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}