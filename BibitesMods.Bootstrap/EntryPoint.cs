using System.Reflection;
using HarmonyLib;

// Do not rename this class or change the namespace !
namespace Doorstop;

internal class Entrypoint
{
    const string BibitesAssembly = nameof(BibitesAssembly);

    public static Harmony? Harmony { get; set; }

    public static void Start()
    {
        AppDomain.CurrentDomain.AssemblyLoad += BibitesAssemblyCatcher;
        Assembly.LoadFrom("Mods/0Harmony.dll");
    }

    private static void BibitesAssemblyCatcher(object sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;
        if (assembly.GetName().Name == BibitesAssembly)
        {
            SetupHarmony();
        }
    }

    public static void SetupHarmony()
    {
        Harmony = new Harmony("BibitesMods.Bootstrap");
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static void ClearHook()
    {
        if (Harmony != null)
        {
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
        }
        AppDomain.CurrentDomain.AssemblyLoad -= BibitesAssemblyCatcher;
    }

}
