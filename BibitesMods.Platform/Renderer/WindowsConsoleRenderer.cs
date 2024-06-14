using System;
using System.Text;
using Neuron.Core.Logging;
using Neuron.Core.Logging.Processing;

namespace BibitesMods.Platform.Renderer;

public sealed class WindowsConsoleRenderer : ILogRender, IDisposable
{
    private bool disposed = false;

    public WindowsConsoleRenderer()
    {
        ConsoleHelper.CreateWinOutConsole();
    }

    ~WindowsConsoleRenderer()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            ConsoleHelper.CloseWinOutConsole();
            disposed = true;
        }
    }

    public void Render(LogOutput output)
    {
        var buffer = new StringBuilder();
        var color = output.Level switch
        {
            LogLevel.Verbose => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.Cyan,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.DarkRed,
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var token in output.Tokens)
        {
            buffer.Append(token.Message);
        }

        Console.ForegroundColor = color;
        Console.WriteLine($"[{output.Level}] [{output.Caller}] " + buffer);
        Console.ResetColor();
    }
}
