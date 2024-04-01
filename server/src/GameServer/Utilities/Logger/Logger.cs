using System.Diagnostics;

namespace GameServer.Utilities.Logger;

/// <summary>
/// Logger class provides logging functionality.
/// </summary>
public class Logger : ILogger
{
    #region Fields and properties
    private readonly string _loggingNamespace;
    #endregion

    #region Constructors and finalizers
    /// <summary>
    /// Creates a new Logger instance.
    /// </summary>
    public Logger(string loggingNamespace)
    {
        _loggingNamespace = loggingNamespace;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Logs an debug message.
    /// </summary>
    /// <param name="message">The message</param>
    public void Debug(string message)
    {
        lock (Console.Out)
        {
            PrintDebug(message);
        }
    }

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message</param>
    public void Info(string message)
    {
        lock (Console.Out)
        {
            Print($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
            Print($"INFO  ", ConsoleColor.Blue);
            Print($"[{_loggingNamespace}] {message}", ConsoleColor.White);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Logs an warning message.
    /// </summary>
    /// <param name="message">The message</param>
    public void Warning(string message)
    {
        lock (Console.Out)
        {
            Print($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
            Print($"WARN  ", ConsoleColor.Yellow);
            Print($"[{_loggingNamespace}] {message}", ConsoleColor.Yellow);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message</param>
    public void Error(string message)
    {
        lock (Console.Out)
        {
            Print($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
            Print($"ERROR ", ConsoleColor.Red);
            Print($"[{_loggingNamespace}] {message}", ConsoleColor.Red);
            Console.WriteLine();
        }
    }

    [Conditional("DEBUG")]
    private void PrintDebug(string message)
    {
        Print($"{GetCurrentTimeString()} ", ConsoleColor.Cyan);
        Print($"DEBUG ", ConsoleColor.Gray);
        Print($"[{_loggingNamespace}] {message}", ConsoleColor.Gray);
        Console.WriteLine();
    }

    /// <summary>
    /// Gets the current time string.
    /// </summary>
    /// <returns>The current time string</returns>
    private static string GetCurrentTimeString()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Prints a message.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="messageColor">The message color</param>
    private static void Print(string message, ConsoleColor messageColor)
    {
        Console.ForegroundColor = messageColor;
        Console.Write(message);
        Console.ResetColor();
    }
    #endregion
}
