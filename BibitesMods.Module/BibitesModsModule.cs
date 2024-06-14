using Neuron.Core.Modules;
using Neuron.Modules.Commands;
using Neuron.Modules.Patcher;
using Ninject;

namespace BibitesMods;

[Module(
    Name = ModeInfo.NAME,
    Description = ModeInfo.DESCRIPTION,
    Dependencies = new[]
    {
        typeof(PatcherModule),
        typeof(CommandsModule)
    }
)]
public partial class BibitesModsModule : Module
{
    [Inject]
    public PatcherService Patcher { get; set; }

    [Inject]
    public CommandService Commands { get; set; }
    
    private IKernel _kernel;


    public override void Load(IKernel kernel)
    {
        Logger.Info("BibitesMods.Module is loading");

        _kernel = kernel;

        // Utility.Version is form bibites project
        if (BasedGameVersion != Utility.Version.Present)
            Logger.Warn($"Version: This Version of BibitesMods is build for Version {BasedGameVersion}, Game Current Version{Utility.Version.Present}\nBugs may occurs");
    
        Logger.Info("BibitesMods.Module is loaded");
    }
}
