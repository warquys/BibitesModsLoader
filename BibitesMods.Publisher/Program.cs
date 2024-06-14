// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using dnlib.DotNet;
using BibitesMods.Publisher;



var publisher = new Publisher();

try
{
    string path;
    if (args.Length == 0)
    {
        Console.WriteLine("Location of the assembly:");
        path = Console.ReadLine()!;
    }
    else
    {
        path = args[0];
    }

    var loadModule = ModuleDefMD.Load(path, new ModuleCreationOptions());
    publisher.Publicise(loadModule);
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.ReadKey();
}