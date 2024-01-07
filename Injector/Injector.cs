using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using FieldAttributes = dnlib.DotNet.FieldAttributes;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using TypeAttributes = dnlib.DotNet.TypeAttributes;


namespace Injector
{
    internal class Injector
    {
        private readonly bool _writeToDisk;
        private readonly string _outputPath;

        public Injector(bool writeToDisk = true, string outputPath = "")
        {
            _writeToDisk = writeToDisk;
            if (string.IsNullOrEmpty(outputPath))
                _outputPath = Path.Combine(Environment.CurrentDirectory + "/Delivery");
            else
                _outputPath = outputPath;
        }


        internal void Start(ModuleDefMD loadModule)
        {
            var sourceModule = ModuleDefMD.Load(Assembly.GetExecutingAssembly().Location);

            var loaderSource = sourceModule.Types.First(p => p.Name == nameof(BibitesModsLoaderVector));
            loaderSource.DeclaringType = null;
            var loader = loaderSource.Methods.First(p => p.Name == nameof(BibitesModsLoaderVector.Execute));

            SwapTypes(sourceModule, loadModule, loaderSource);

            InjectLoader(loadModule, loader);
            StoreModule(loadModule);
            Publicise(loadModule);
        }

        private void Publicise(ModuleDef md)
        {
            var types = md.Assembly.ManifestModule.Types.ToList();
            var nested = new List<TypeDef>();
            foreach (var type in types)
            {
                if (!type.IsPublic)
                {
                    var isInter = type.IsInterface;
                    var isAbstr = type.IsAbstract;

                    type.Attributes = type.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;

                    if (isInter)
                        type.IsInterface = true;
                    if (isAbstr)
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
                
                md.Name = md.Name.Replace(".dll", "-Publicized.dll");
                md.Write(Path.Combine(_outputPath, md.Name));
                Console.WriteLine($"Wrote {md.Name} to Delivery directory");
            }
        }

        private void StoreModule(ModuleDef def)
        {
            if (!_writeToDisk)
                return;

            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);
            
            def.Write(Path.Combine(_outputPath, def.Name));
            Console.WriteLine($"Wrote {def.Name} to Delivery directory");
        }

        private void SwapTypes(ModuleDef from, ModuleDef to, TypeDef type)
        {
            from.Types.Remove(type);
            to.Types.Add(type);
        }

        const string TargetType = "AppInitializer";
        const string TargetMethod = "Awake";

        private void InjectLoader(ModuleDef moduleDef, MethodDef callable)
        {
            foreach (var type in moduleDef.Assembly.Modules.SelectMany(p => p.Types))
            {
                if (type.Name == TargetType)
                {
                    MethodDef startMethod = type.Methods.First(t => t.Name == TargetMethod);
                    startMethod.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(callable));
                    return;
                }
            }
        }

    }
}
