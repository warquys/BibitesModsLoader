using System;
using System.Threading.Tasks;

internal class Program
{
    public static Task<string> CommandInPut;

    // Not really async just to fake the work
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        CommandInPut = Task.Run(RunReader);

        // Fake work and log from mods
        for (int i = 0; i < 100; i++)
        {
            // game branch
            await Task.Delay(100);
            Console.WriteLine($"Bibite work on {i}...");
            // Command Handler
            HandleCommand();
        }

        Console.ReadLine();
    }

    public static void HandleCommand()
    {
        if (!CommandInPut.IsCompleted)
            return;
        
        string command;
        try
        {
            command = CommandInPut.Result;
            switch (CommandInPut.Result)
            {
                case "q":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine(command);
                    break;
            }
        }
        catch (AggregateException e)
        {
            foreach (var exception in e.InnerExceptions)
            {
                Console.Write(exception);
            }
        }
        // Find an easy solution to avoid overlaping
        CommandInPut = Task.Run(RunReader);
    }
    
    public static Task<string> RunReader() => Task.FromResult(Console.ReadLine());
}
