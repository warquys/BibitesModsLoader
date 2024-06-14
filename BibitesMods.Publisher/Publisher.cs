using dnlib.DotNet;
using System.Reflection;
using System.Runtime.CompilerServices;
using FieldAttributes = dnlib.DotNet.FieldAttributes;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using TypeAttributes = dnlib.DotNet.TypeAttributes;

namespace BibitesMods.Publisher;

public class Publisher
{
    private readonly bool _writeToDisk;
    private readonly string _outputPath;

    public Publisher(bool writeToDisk = true, string outputPath = "")
    {
        _writeToDisk = writeToDisk;
        if (string.IsNullOrEmpty(outputPath))
            _outputPath = Path.Combine(Environment.CurrentDirectory + "/Delivery");
        else
            _outputPath = outputPath;
    }


    public void Publicise(ModuleDefMD loadModule)
    {
        var types = loadModule.Assembly.ManifestModule.Types.ToList();
        var nested = new List<TypeDef>();
        foreach (var type in types)
        {
            if (!type.IsPublic)
            {
                var isInterface = type.IsInterface;
                var isAbstract = type.IsAbstract;

                type.Attributes = type.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;

                if (isInterface)
                    type.IsInterface = true;
                if (isAbstract)
                    type.IsAbstract = true;
            }
            if (type.CustomAttributes.Find(typeof(CompilerGeneratedAttribute).FullName) != null)
                continue;
            nested.AddRange(type.NestedTypes.ToList());
        }

        foreach (var def in nested)
        {
            if (def.CustomAttributes.Find(typeof(CompilerGeneratedAttribute).FullName) != null)
                continue;
            def.Attributes = def.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
        }

        types.AddRange(nested);

        foreach (var def in types.SelectMany(p => p.Methods).Where(p => p != null && !p.IsPublic))
            def.Access = MethodAttributes.Public;

        foreach (var type in types)
        {
            var events = type.Events.Select(p => p.Name).ToArray();
            foreach (var field in type.Fields)
            {
                var isEventBackingField = events.Any(p =>
                        String.Equals(p, field.Name, StringComparison.InvariantCultureIgnoreCase));
                if (field != null && !field.IsPublic && !isEventBackingField)
                    field.Access = FieldAttributes.Public;
            }
        }

        if (_writeToDisk)
        {
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            loadModule.Name = loadModule.Name.Replace(".dll", "-Publicized.dll");
            loadModule.Write(Path.Combine(_outputPath, loadModule.Name));
            Console.WriteLine($"Wrote {loadModule.Name} to Delivery directory");
        }
    }

}
