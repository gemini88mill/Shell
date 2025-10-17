using System;
using System.Collections.Generic;
using Spectre.Console;

namespace Shell.Utils
{
  public static class ReplHelpers
  {
    public static bool HandleReplCommand(string input, ref bool isRunning, List<string> commandHistory)
    {
      var command = input.ToLowerInvariant().Split(' ')[0];

      switch (command)
      {
        case "exit":
        case "quit":
          isRunning = false;
          return true;

        case "clear":
          Logger.Clear();
          return true;

        case "help":
          ShowReplHelp();
          return true;

        case "version":
          Logger.Info("Shell REPL v1.0.0");
          return true;

        case "history":
          ShowCommandHistory(commandHistory);
          return true;
      }

      return false;
    }

    public static void ShowCommandHistory(List<string> commandHistory)
    {
      if (commandHistory.Count == 0)
      {
        Logger.Warning("No commands in history");
        return;
      }

      var table = new Table();
      table.AddColumn("Index");
      table.AddColumn("Command");

      for (int i = 0; i < commandHistory.Count; i++)
      {
        table.AddRow((i + 1).ToString(), commandHistory[i]);
      }

      AnsiConsole.Write(table);
    }

    public static void ShowReplHelp()
    {
      var table = new Grid();
      table.AddColumn();
      table.AddColumn();

      table.AddRow("Command", "Description");

      table.AddRow("help", "Show this help message");
      table.AddRow("exit/quit", "Exit the REPL");
      table.AddRow("clear", "Clear the screen");
      table.AddRow("version", "Show version information");
      table.AddRow("history", "Show command history");
      table.AddRow("echo [dim]<text>[/]", "Echo the provided text");
      table.AddRow("time", "Show current time");
      table.AddRow("calc [dim]<expression>[/]", "Evaluate a simple math expression");
      table.AddRow("file [dim]<path>[/]", "Get information about a file");
      table.AddRow("dir [dim][[path]][/]", "List directory contents");
      table.AddRow("pwd", "Show current directory");
      table.AddRow("cd [dim][[path]][/]", "Change directory");

      AnsiConsole.Write(table);
      Logger.NewLine();
      Logger.Info("This is a simple REPL built with Spectre.Console for beautiful terminal output.");
    }
  }
}
