using Spectre.Console;
using System.Text;

namespace Shell
{
  /// <summary>
  /// Centralized logging and I/O management for the Shell application.
  /// Handles all console output, input, and exception management.
  /// </summary>
  public static class Logger
  {
    private static StringBuilder? _captureBuffer;
    /// <summary>
    /// Outputs basic informational messages to the console.
    /// </summary>
    /// <param name="message">The message to display</param>
    public static void Info(string message)
    {
      var formattedMessage = $"[green]{message}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
    }

    /// <summary>
    /// Outputs warning messages to the console.
    /// </summary>
    /// <param name="message">The warning message to display</param>
    public static void Warning(string message)
    {
      var formattedMessage = $"[yellow]Warning: {message}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
    }

    /// <summary>
    /// Outputs error messages to the console.
    /// </summary>
    /// <param name="message">The error message to display</param>
    public static void Error(string message)
    {
      var formattedMessage = $"[red]Error: {message}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
    }

    /// <summary>
    /// Prompts the user for input and returns their response.
    /// </summary>
    /// <param name="prompt">The prompt message to display to the user</param>
    /// <returns>The user's input as a string, or empty string if no input available</returns>
    public static string Ask(string prompt)
    {

      return AnsiConsole.Prompt(new TextPrompt<string>($"{prompt}"));
    }

    /// <summary>
    /// Handles exceptions that break the program execution.
    /// Displays detailed exception information and optionally logs to file.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="context">Optional context information about where the exception occurred</param>
    public static void Exception(Exception exception, string? context = null)
    {
      // Exceptions should always be displayed to console, even during redirection
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
      var formattedMessage = $"[green]✓ {message}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
    }

    /// <summary>
    /// Outputs a debug message to the console (only in debug builds).
    /// </summary>
    /// <param name="message">The debug message to display</param>
    public static void Debug(string message)
    {
#if DEBUG
      var formattedMessage = $"[dim]DEBUG: {message}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
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
      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        _captureBuffer.AppendLine();
      }
      else
      {
        // Normal console output
        AnsiConsole.WriteLine();
      }
    }

    /// <summary>
    /// Outputs a separator line to the console.
    /// </summary>
    /// <param name="character">The character to use for the separator (default: '-')</param>
    /// <param name="length">The length of the separator line (default: 50)</param>
    public static void Separator(char character = '-', int length = 50)
    {
      var formattedMessage = $"[dim]{new string(character, length)}[/]";

      if (_captureBuffer != null)
      {
        // Only capture to file, don't display to console
        var plainText = Markup.Remove(formattedMessage);
        _captureBuffer.AppendLine(plainText);
      }
      else
      {
        // Normal console output
        AnsiConsole.MarkupLine(formattedMessage);
      }
    }

    /// <summary>
    /// Starts capturing output to a buffer for file redirection.
    /// </summary>
    public static void StartCapture()
    {
      _captureBuffer = new StringBuilder();
    }

    /// <summary>
    /// Stops capturing output and returns the captured text.
    /// </summary>
    /// <returns>The captured plain text output</returns>
    public static string StopCapture()
    {
      if (_captureBuffer == null)
        return string.Empty;

      var capturedText = _captureBuffer.ToString();
      _captureBuffer = null;
      return capturedText;
    }
  }
}
