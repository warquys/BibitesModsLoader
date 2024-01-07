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

                if (args.Length == 0)
                    return;

                var loadModule = ModuleDefMD.Load(args[0], new ModuleCreationOptions());

                injector.Start(loadModule);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}