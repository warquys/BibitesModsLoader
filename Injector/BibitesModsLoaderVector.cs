using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Injector;

internal class BibitesModsLoaderVector
{
    private static string LogPath = "";

    public static void Execute()
    {
        try
        {
            var curentDirectory = Directory.GetCurrentDirectory();
            LogPath = Path.Combine(curentDirectory, "ModdingBibiteLog.txt");
            File.WriteAllText(LogPath, string.Empty);

            var startupArgs = Environment.GetCommandLineArgs();
            if (startupArgs.Any(x => x.Equals("-nomods", StringComparison.OrdinalIgnoreCase)))
            {
                Log("Started with \"-nomods\" argument! no mods will not be loaded");
                return;
            }

            Log("Bootstrapping via reflections");

            var assemblies = new List<Assembly>();
            var domain = AppDomain.CurrentDomain;
            var currentUri = new Uri(curentDirectory);
            var files = Directory.GetFiles(Path.Combine(curentDirectory, "mods", "depedency"), "*.dll");

            foreach (var file in files)
            {
                try
                {
                    var fileUri = new Uri(file);
                    var targetUri = currentUri.MakeRelativeUri(fileUri);
                    Log($"Loading assembly at {Uri.UnescapeDataString(targetUri.ToString())}");
                    var assembly = domain.Load(File.ReadAllBytes(file));
                    Log($"Loaded assembly {assembly.FullName}");
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Log("Failed to load Assembly\n" + file + "\n" + ex);
                }
            }

            domain.AssemblyResolve += delegate (object sender, ResolveEventArgs eventArgs)
            {
                return assemblies.First(x => x.FullName == eventArgs.Name);
            };

            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == BibitesModsLoader.ModInfo.MOD_NAME);
            var entrypoint = coreAssembly.GetType("BibitesModsLoader.BibitesModsLoader");
            if (entrypoint == null) throw new EntryPointNotFoundException("BibitesModsLoader found");
            var main = entrypoint.GetMethod(
                nameof(BibitesModsLoader.BibitesModsLoader.Main),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                new Type[] { typeof(string) }, 
                modifiers: null);
            if (main == null) throw new EntryPointNotFoundException("BibitesModsLoader.Main() is null");
            main.Invoke(null, new string[] { LogPath });
        }
        catch (Exception e)
        {
            Log($"Failed to load: {e}");
        }
    }

    private static void Log(string msg)
    {
        if (string.IsNullOrEmpty(LogPath)) return;
        File.AppendAllText(LogPath, msg + Environment.NewLine);
    }
}
