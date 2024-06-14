using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BibitesMods.Platform.Renderer;

public static class ConsoleHelper
{
    // https://learn.microsoft.com/en-us/windows/console/getstdhandle
    private const uint StdOutputHandle = (unchecked((uint)-11));
    private const nint DefaultStdOut = 7;

    public static void CreateWinOutConsole()
    {
        ThrowIfFailed(AllocConsole());
        nint newOut = GetStdHandle(StdOutputHandle);
        ThrowIfFailed(SetStdHandle(StdOutputHandle, newOut));
        RefreshConsoleStdOut();
    }


    public static void CloseWinOutConsole()
    {
        ThrowIfFailed(FreeConsole());
        ThrowIfFailed(SetStdHandle(StdOutputHandle, DefaultStdOut));
        RefreshConsoleStdOut();
    }

    private static void RefreshConsoleStdOut()
    {
        var writer = new StreamWriter(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
        Console.SetOut(writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowIfFailed(bool success)
    {
        if (!success)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }


    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern nint GetStdHandle(uint nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetStdHandle(uint nStdHandle, nint handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeConsole();
}