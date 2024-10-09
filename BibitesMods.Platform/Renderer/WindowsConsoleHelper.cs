using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace BibitesMods.Platform.Renderer;

public static class ConsoleHelper
{
    // https://learn.microsoft.com/en-us/windows/console/getstdhandle
    private const uint StdOutputHandle = (unchecked((uint)-11));
    private const nint DefaultStdOut = 0;

    private const uint CpUTF8 = 65001;

    public static void CreateWinOutConsole()
    {
        ThrowIfFailed(AllocConsole());
        nint newOut = GetStdHandle(StdOutputHandle);
        ThrowIfFailed(SetStdHandle(StdOutputHandle, newOut));
        //RefreshConsoleStdOut();
        ThrowIfFailed(SetConsoleCP(CpUTF8));
        ThrowIfFailed(SetConsoleOutputCP(CpUTF8));
        Console.OutputEncoding = Encoding.UTF8;
    }


    public static void CloseWinOutConsole()
    {
        ThrowIfFailed(FreeConsole());
        ThrowIfFailed(SetStdHandle(StdOutputHandle, DefaultStdOut));
        //RefreshConsoleStdOut();
    }

    private static void RefreshConsoleStdOut()
    {
        var writer = new StreamWriter(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
        Console.SetError(writer);
        Console.SetOut(writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowIfFailed(bool success)
    {
        if (!success)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }


    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCP(uint wCodePageID);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleOutputCP(uint wCodePageID);

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