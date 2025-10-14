using Spectre.Console;

namespace Shell
{
  /// <summary>
  /// Centralized logging and I/O management for the Shell application.
  /// Handles all console output, input, and exception management.
  /// </summary>
  public static class Logger
  {
    /// <summary>
    /// Outputs basic informational messages to the console.
    /// </summary>
    /// <param name="message">The message to display</param>
    public static void Info(string message)
    {
      AnsiConsole.MarkupLine($"[green]{message}[/]");
    }

    /// <summary>
    /// Outputs warning messages to the console.
    /// </summary>
    /// <param name="message">The warning message to display</param>
    public static void Warning(string message)
    {
      AnsiConsole.MarkupLine($"[yellow]Warning: {message}[/]");
    }

    /// <summary>
    /// Outputs error messages to the console.
    /// </summary>
    /// <param name="message">The error message to display</param>
    public static void Error(string message)
    {
      AnsiConsole.MarkupLine($"[red]Error: {message}[/]");
    }

    /// <summary>
    /// Prompts the user for input and returns their response.
    /// </summary>
    /// <param name="prompt">The prompt message to display to the user</param>
    /// <returns>The user's input as a string, or empty string if no input available</returns>
    public static string Ask(string prompt)
    {
      AnsiConsole.Markup($"[bold blue]{prompt}[/] ");
      return Console.ReadLine()?.Trim() ?? "";
    }

    /// <summary>
    /// Handles exceptions that break the program execution.
    /// Displays detailed exception information and optionally logs to file.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="context">Optional context information about where the exception occurred</param>
    public static void Exception(Exception exception, string? context = null)
    {
      AnsiConsole.WriteLine();
      AnsiConsole.MarkupLine("[red]An exception occurred:[/]");

      if (!string.IsNullOrEmpty(context))
      {
        AnsiConsole.MarkupLine($"[yellow]Context: {context}[/]");
      }

      AnsiConsole.WriteException(exception);
      AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Outputs a success message to the console.
    /// </summary>
    /// <param name="message">The success message to display</param>
    public static void Success(string message)
    {
      AnsiConsole.MarkupLine($"[green]✓ {message}[/]");
    }

    /// <summary>
    /// Outputs a debug message to the console (only in debug builds).
    /// </summary>
    /// <param name="message">The debug message to display</param>
    public static void Debug(string message)
    {
#if DEBUG
      AnsiConsole.MarkupLine($"[dim]DEBUG: {message}[/]");
#endif
    }

    /// <summary>
    /// Clears the console screen.
    /// </summary>
    public static void Clear()
    {
      AnsiConsole.Clear();
    }

    /// <summary>
    /// Outputs a newline to the console.
    /// </summary>
    public static void NewLine()
    {
      AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Outputs a separator line to the console.
    /// </summary>
    /// <param name="character">The character to use for the separator (default: '-')</param>
    /// <param name="length">The length of the separator line (default: 50)</param>
    public static void Separator(char character = '-', int length = 50)
    {
      AnsiConsole.MarkupLine($"[dim]{new string(character, length)}[/]");
    }
  }
}
