using BibitesMods;
using Neuron.Core.Modules;

namespace ExampleModule
{
    [Module(
        Name = "Example Module",
        Description = "Example Description",
        Dependencies = new[]
        {
            typeof(BibitesModsModule)
        }
    )]
    public class ExampleModule
    {

    }
}