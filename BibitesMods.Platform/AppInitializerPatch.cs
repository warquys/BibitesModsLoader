using System.Reflection;
using BibitesMods.Platform;
using Doorstop;
using HarmonyLib;
using ManagementScripts;
using Neuron.Core.Platform;

namespace BibitesMods.Bootstrap;

[HarmonyPatch(typeof(AppInitializer), nameof(AppInitializer.Awake))]
internal static class AppInitializerPatch
{
    const string BibitesAssembly = nameof(BibitesAssembly);

    const string NoStartupArgument = "-noBibitesMods";

    public static Harmony Harmony { get; set; }

    [HarmonyPrefix]
    public static void StartNeurone()
    {
        ClearHook();
        try
        {
            var startupArgument = Environment.GetCommandLineArgs();

            if (startupArgument.Any(x => x.Equals(NoStartupArgument, StringComparison.OrdinalIgnoreCase)))
            {
                Entrypoint.Log($"Started with '{NoStartupArgument}' argument! BibitesModsPlatform will not be loaded");
                return;
            }

            var entryPoint = new BibitesModsStandalonePlatform();
            entryPoint.Boostrap();
        }
        catch (Exception e)
        {
            Entrypoint.Log(e.ToString());
        }   
    }

    public static void SetHook()
    {
        AppDomain.CurrentDomain.AssemblyLoad += BibitesAssemblyCatcher;
    }

    private static void BibitesAssemblyCatcher(object sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;
        if (assembly.GetName().Name == BibitesAssembly)
        {
            AppInitializerPatch.SetupHarmony();
        }
    }

    private static void ClearHook()
    {
        if (Harmony != null)
        {
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
        }
        AppDomain.CurrentDomain.AssemblyLoad -= BibitesAssemblyCatcher;
    }

    private static void SetupHarmony()
    {
        Harmony = new Harmony("BibitesMods.Bootstrap");
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
