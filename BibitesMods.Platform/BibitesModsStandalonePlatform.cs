using System.Runtime.InteropServices;
using Neuron.Core;
using Neuron.Core.Platform;
using Neuron.Core.Scheduling;
using BibitesMods.Platform.Renderer;
using UnityEngine;
using BibitesMods.Bootstrap;

// Do not rename the namespace, class, or Main method!
// They are use by BibitesMods.Bootstrap

namespace BibitesMods.Platform;

public class BibitesModsStandalonePlatform : IPlatform
{
    public const string LogInFileArgumente = "-filelog";

    public bool HandleInput { get; set; }

    public static void Main()
    {
        AppInitializerPatch.SetHook();
    }

    public PlatformConfiguration Configuration { get; set; } = new();
    public ActionCoroutineReactor CoroutineReactor = new();
    public NeuronBase NeuronBase { get; set; }

    public void Load()
    {
        //Configuration.ConsoleWidth = 85;
        Configuration.BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Mods");
        Configuration.FileIo = true;
        Configuration.CoroutineReactor = CoroutineReactor;
        Configuration.OverrideConsoleEncoding = false;
        Configuration.EnableConsoleLogging = false;

        var startupArgument = Environment.GetCommandLineArgs();

        if (startupArgument.Any(x => x.Equals(LogInFileArgumente, StringComparison.OrdinalIgnoreCase)))
        {
            Configuration.ConsoleWidth = 100;
            Configuration.LogEventSink = new TextFileRenderer();
            
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Configuration.LogEventSink = new WindowsConsoleRenderer();
            HandleInput = true;
            Console.WriteLine();
            Console.WriteLine($"/!\\ Closing this console result to also closing the projet, start the exe with '{BibitesModsStandalonePlatform.LogInFileArgumente}' to remove the console. /!\\");
        }
    }

    public void Enable()
    {
        new GameObject("CoroutineHandler", new Type[] { typeof(CoroutineHandler) });
    }

    public void Continue()
    {

    }

    public void Disable()
    {
        HandleInput = false;
        if (Configuration.LogEventSink is IDisposable disposable)
            disposable.Dispose();
    }
}