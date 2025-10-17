using System;
using System.Collections.Generic;

namespace Shell.Parsers
{
  public record ParsedCommand(string[] CommandArgs, string? OutputFile);

  public static class InputParser
  {
    public static string[] ParseInput(string input)
    {
      // Simple command line parsing - handles quoted strings
      var args = new List<string>();
      var current = "";
      var inQuotes = false;
      var quoteChar = '"';

      for (int i = 0; i < input.Length; i++)
      {
        var c = input[i];

        if (c == '"' || c == '\'')
        {
          if (!inQuotes)
          {
            inQuotes = true;
            quoteChar = c;
          }
          else if (c == quoteChar)
          {
            inQuotes = false;
          }
          else
          {
            current += c;
          }
        }
        else if (c == ' ' && !inQuotes)
        {
          if (!string.IsNullOrEmpty(current))
          {
            args.Add(current);
            current = "";
          }
        }
        else
        {
          current += c;
        }
      }

      if (!string.IsNullOrEmpty(current))
      {
        args.Add(current);
      }

      return args.ToArray();
    }

    public static ParsedCommand ParseInputWithRedirection(string input)
    {
      // Find >> token outside of quotes
      var inQuotes = false;
      var quoteChar = '"';
      var redirectionIndex = -1;

      for (int i = 0; i < input.Length - 1; i++)
      {
        var c = input[i];

        if (c == '"' || c == '\'')
        {
          if (!inQuotes)
          {
            inQuotes = true;
            quoteChar = c;
          }
          else if (c == quoteChar)
          {
            inQuotes = false;
          }
        }
        else if (!inQuotes && c == '>' && input[i + 1] == '>')
        {
          redirectionIndex = i;
          break;
        }
      }

      if (redirectionIndex == -1)
      {
        // No redirection found, use existing parser
        return new ParsedCommand(ParseInput(input), null);
      }

      // Split input into command part and file part
      var commandPart = input.Substring(0, redirectionIndex).Trim();
      var filePart = input.Substring(redirectionIndex + 2).Trim();

      if (string.IsNullOrEmpty(filePart))
      {
        // >> found but no filename, treat as literal text
        return new ParsedCommand(ParseInput(input), null);
      }

      // Parse command part using existing logic
      var commandArgs = ParseInput(commandPart);

      return new ParsedCommand(commandArgs, filePart);
    }
  }
}
