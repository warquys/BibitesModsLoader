using System.Runtime.InteropServices;
using Neuron.Core;
using Neuron.Core.Platform;
using Neuron.Core.Scheduling;
using BibitesMods.Platform.Renderer;
using UnityEngine;

// Do not rename the namespace, class, or Main method!
// They are use by BibitesMods.Bootstrap

namespace BibitesMods.Platform;

public class BibitesModsStandalonePlatform : IPlatform
{
    const string LogInFileArgumente = "-filelog";

    public static void Main()
    {
        var entrypoint = new BibitesModsStandalonePlatform();
        entrypoint.Boostrap();
    }

    public PlatformConfiguration Configuration { get; set; } = new();
    public ActionCoroutineReactor CoroutineReactor = new();
    public NeuronBase NeuronBase { get; set; }

    public void Load()
    {
        Configuration.ConsoleWidth = 85;
        Configuration.BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BibitesMods");
        Configuration.FileIo = true;
        Configuration.CoroutineReactor = CoroutineReactor;
        Configuration.OverrideConsoleEncoding = false;
        Configuration.EnableConsoleLogging = false;

        var startupArgument = Environment.GetCommandLineArgs();
        if (startupArgument.Any(x => x.Equals(LogInFileArgumente, StringComparison.OrdinalIgnoreCase)))
        {
            Configuration.LogEventSink = new WindowsConsoleRenderer();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Configuration.LogEventSink = new TextFileRenderer();
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

    }
}