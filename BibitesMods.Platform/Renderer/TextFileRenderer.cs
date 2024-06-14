using System.Text;
using Neuron.Core.Logging.Processing;

namespace BibitesMods.Platform.Renderer;

public sealed class TextFileRenderer : ILogRender
{
    // Same as AppInitializerPatche.cs of BibitesMods.Bootstrap
    const string LogFile = "BibitesMods.log";

    public void Render(LogOutput output)
    {
        var buffer = new StringBuilder();

        foreach (var token in output.Tokens)
        {
            buffer.Append(token.Message);
        }

        File.AppendAllText(LogFile, $"[{output.Level}] [{output.Caller}] " + buffer + Environment.NewLine);
    }
}
