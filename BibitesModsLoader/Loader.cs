using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BibitesModsLoader;

public static class Loader
{
    private static bool _enabled = false;

    public static Version Version { get; } = new Version(ModInfo.MOD_VERSION);
    public static SortedList<ModAttribute, IMod> Mods { get; } = [];
    public static string LogPath { get; private set; }

    public static void Main(string logPath)
    {
        if (_enabled) return;
        _enabled = true;
        LogPath = logPath;
        // TODO: add code to avoid sending report to unity
        LoadMods();
    }

    public static void Log(string msg)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var message = $"[{callingAssembly.GetName().Name}] {msg}{Environment.NewLine}";
        File.AppendAllText(LogPath, message);
    }

    public static void Log(object obj)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var message = $"[{callingAssembly.GetName().Name}] {obj}{Environment.NewLine}";
        File.AppendAllText(LogPath, message);
    }

    private static void LoadMods()
    {
        var assemblies = new List<Assembly>();
        var domain = AppDomain.CurrentDomain;
        var currentUri = new Uri(Directory.GetCurrentDirectory());
        var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "mods"), "*.dll");
        
        foreach (var file in files)
        {
            try
            {
                var fileUri = new Uri(file);
                var targetUri = currentUri.MakeRelativeUri(fileUri);
                var assembly = domain.Load(File.ReadAllBytes(file));
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

        foreach (var assembly in assemblies)
        {
            if (TryLoadPlugins(assembly, out var modInfo, out var mainClass))
                Mods.Add(modInfo, mainClass);
        }

        foreach (var mod in Mods)
        {
            try
            {
                mod.Value.Enable();
                Log($"{mod.Key.Name} Loaded...");
            }
            catch (Exception e)
            {
                Log($"Expetion when trying to enable {mod.Key.Name}! {Environment.NewLine}{e}{Environment.NewLine} skiped.");
            }
        }
        Log($"Finish to load all mods.");
    }

    public static bool TryLoadPlugins(Assembly assembly, out ModAttribute modInfo, out IMod mainClass)
    {
        var modsTypes = assembly.GetTypes().Where(p => typeof(IMod).IsAssignableFrom(p));
        var count = modsTypes.Count();

        if (count != 1)
        {
            Log($"There is {count} mod class in the assembly call {assembly.FullName}, skiped. (Only one allowed)");
            modInfo = null;
            mainClass = null;
            return false;
        }

        var modtype = modsTypes.First();
        modInfo = modtype.GetCustomAttribute<ModAttribute>();
        if (modInfo == null)
        {
            Log($"There is no ModAttribute in the assembly call {assembly.FullName}, skiped.");
            mainClass = null;
            return false;
        }

        try
        {
            mainClass = Activator.CreateInstance(modtype) as IMod;
        }
        catch (Exception e)
        {
            Log($"Expetion when trying to load {modInfo.Name}! {Environment.NewLine}{e}{Environment.NewLine} skiped.");
            mainClass = null; 
            return false;
        }
        return true;
    }
}