using System.Reflection;
using Doorstop;
using HarmonyLib;
using ManagementScripts;

namespace BibitesMods.Bootstrap;

[HarmonyPatch(typeof(AppInitializer), nameof(AppInitializer.Awake))]
internal static class AppInitializerPatche
{
    const string TargetAssembly = "BibitesMods.Platform";
    const string TargetType = "BibitesMods.Platform.BibitesModsStandalonePlatform";
    const string TargetMethod = "Main";

    const string LogFile = "BibitesMods.log";

    const string NoStartupArgument = "-noBibitesMods";

    [HarmonyPrefix]
    public static void StartNeurone()
    {
        ClearOldLog();
        try
        {
            Entrypoint.ClearHook();
            var startupArgument = Environment.GetCommandLineArgs();

            if (startupArgument.Any(x => x.Equals(NoStartupArgument, StringComparison.OrdinalIgnoreCase)))
            {
                Log($"Started with '{NoStartupArgument}' argument! BibitesModsPlatform will not be loaded");
                return;
            }

            Log("Bootstrapping BibitesModsPlatform via reflections");

            var assemblies = new List<Assembly>();
            var domain = AppDomain.CurrentDomain;
            var currentUri = new Uri(Directory.GetCurrentDirectory());

            foreach (var file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "BibitesMods", "Managed"), "*.dll"))
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

            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == TargetAssembly);
            var entrypoint = coreAssembly.GetType(TargetType);
            if (entrypoint == null) throw new Exception($"{TargetType} not found");
            var main = entrypoint.GetMethod(TargetMethod, BindingFlags.Public | BindingFlags.Static);
            if (main == null) throw new Exception($"{TargetType}.{TargetMethod}() is null");
            main.Invoke(null, Array.Empty<object>());
        }
        catch (Exception e)
        {
            Log($"Failed to load ThePlatform: {e}");
        }
    }

    private static void ClearOldLog()
    {
        File.WriteAllText(LogFile, string.Empty);
    }

    private static void Log(string message)
    {
        File.AppendAllText(LogFile, message + Environment.NewLine);
    }
}
