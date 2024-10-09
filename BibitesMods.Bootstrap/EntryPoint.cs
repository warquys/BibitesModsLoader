using System.Reflection;
using SimulationScripts.BibiteScripts;

// Do not rename this class or the namespace !
namespace Doorstop;

public static class Entrypoint
{
    public const string LogFile = "BibitesMods.log";

    const string TargetFile_Mods = "Mods";
    const string TargetFile_Managed = "Managed";

    const string TargetAssembly = "BibitesMods.Platform";
    const string TargetType = "BibitesMods.Platform.BibitesModsStandalonePlatform";
    const string TargetMethod = "Main";

    public static void Start()
    {
        ClearOldLog();

        Log("Bootstrapping BibitesModsPlatform via reflections");

        var assemblies = new List<Assembly>();
        var domain = AppDomain.CurrentDomain;
        var currentUri = new Uri(Directory.GetCurrentDirectory());
        var targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), TargetFile_Mods, TargetFile_Managed);

        if (!Directory.Exists(targetDirectory))
        {
            Log($"The {targetDirectory} do not exist check your installation");
            return;
        }

        foreach (var file in Directory.GetFiles(targetDirectory, "*.dll"))
        {
            try
            {
                var fileUri = new Uri(file);
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

        try
        {
            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == TargetAssembly);
            var entryPoint = coreAssembly.GetType(TargetType);
            if (entryPoint == null) throw new Exception($"{TargetType} not found");
            var main = entryPoint.GetMethod(TargetMethod, BindingFlags.Public | BindingFlags.Static);
            if (main == null) throw new Exception($"{TargetType}.{TargetMethod}() is null");
            main.Invoke(null, Array.Empty<object>());
        }
        catch (Exception e)
        {
            Log($"Failed to load ThePlatform: {e}");
        }
    }

    public static void ClearOldLog()
    {
        File.WriteAllText(LogFile, string.Empty);
    }

    public static void Log(string message)
    {
        File.AppendAllText(LogFile, message + Environment.NewLine);
    }
}
