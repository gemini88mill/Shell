using Spectre.Console;
using Shell;

bool isRunning = true;
var commandHistory = new List<string>();

AnsiConsole.Write(new FigletText("rsh").Color(Color.Blue));
Logger.Info("Welcome to the Shell REPL! Type help for available commands or exit to quit.");
Logger.NewLine();

while (isRunning)
{
    try
    {
        // Get user input with a styled prompt
        var input = Logger.Ask($"rsh {Directory.GetCurrentDirectory()}>");

        if (string.IsNullOrEmpty(input))
        {
            // If we can't read input (non-interactive mode), exit gracefully
            if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
            {
                continue;
            }
            else
            {
                Logger.Warning("No input available. Exiting REPL.");
                break;
            }
        }

        // Add to history
        commandHistory.Add(input);

        // Handle special REPL commands
        if (HandleReplCommand(input))
            continue;

        // Parse and execute the command
        var commandArgs = ParseInput(input);
        ExecuteCommand(commandArgs);
    }
    catch (Exception ex)
    {
        Logger.Exception(ex, "Main program loop");
    }

    Logger.NewLine();
}

Logger.Info("Goodbye!");

void ExecuteCommand(string[] args)
{
    if (args.Length == 0) return;

    var command = args[0].ToLowerInvariant();

    switch (command)
    {
        case "echo":
            if (args.Length > 1)
            {
                var text = string.Join(" ", args.Skip(1));
                Logger.Info(text);
            }
            else
            {
                Logger.Warning("Usage: echo <text>");
            }
            break;

        case "time":
            var now = DateTime.Now;
            Logger.Info($"Current time: {now:yyyy-MM-dd HH:mm:ss}");
            break;

        case "calc":
            if (args.Length > 1)
            {
                var expression = string.Join(" ", args.Skip(1));
                try
                {
                    var result = EvaluateExpression(expression);
                    Logger.Success($"{expression} = {result}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error evaluating expression: {ex.Message}");
                }
            }
            else
            {
                Logger.Warning("Usage: calc <expression>");
            }
            break;

        case "file":
            if (args.Length > 1)
            {
                var filePath = args[1];
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Exists)
                    {
                        var table = new Table();
                        table.AddColumn("Property");
                        table.AddColumn("Value");

                        table.AddRow("Name", fileInfo.Name);
                        table.AddRow("Full Path", fileInfo.FullName);
                        table.AddRow("Size", $"{fileInfo.Length:N0} bytes");
                        table.AddRow("Created", fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        table.AddRow("Modified", fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));

                        AnsiConsole.Write(table);
                    }
                    else
                    {
                        Logger.Error($"File not found: {fileInfo.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error accessing file: {ex.Message}");
                }
            }
            else
            {
                Logger.Warning("Usage: file <path>");
            }
            break;

        case "dir":
            var targetPath = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();
            try
            {
                var dirInfo = new DirectoryInfo(targetPath);
                if (dirInfo.Exists)
                {
                    var table = new Table();
                    table.AddColumn("Name");
                    table.AddColumn("Type");
                    table.AddColumn("Size");
                    table.AddColumn("Modified");

                    foreach (var item in dirInfo.GetFileSystemInfos().OrderBy(x => x.Name))
                    {
                        var type = item is DirectoryInfo ? "[blue]DIR[/]" : "[green]FILE[/]";
                        var size = item is FileInfo file ? $"{file.Length:N0} bytes" : "";
                        var modified = item.LastWriteTime.ToString("yyyy-MM-dd HH:mm");

                        table.AddRow(item.Name, type, size, modified);
                    }

                    AnsiConsole.Write(table);
                }
                else
                {
                    Logger.Error($"Directory not found: {dirInfo.FullName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error accessing directory: {ex.Message}");
            }
            break;

        case "pwd":
            Logger.Info($"Current directory: {Directory.GetCurrentDirectory()}");
            break;

        case "cd":
            if (args.Length > 1)
            {
                try
                {
                    Directory.SetCurrentDirectory(args[1]);
                    Logger.Success($"Changed to: {Directory.GetCurrentDirectory()}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error changing directory: {ex.Message}");
                }
            }
            else
            {
                Logger.Info($"Current directory: {Directory.GetCurrentDirectory()}");
            }
            break;

        default:
            Logger.Error($"Unknown command: {command}");
            Logger.Warning("Type 'help' for available commands");
            break;
    }
}

bool HandleReplCommand(string input)
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
            ShowCommandHistory();
            return true;
    }

    return false;
}

void ShowCommandHistory()
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

void ShowReplHelp()
{
    var table = new Table();
    table.AddColumn("Command");
    table.AddColumn("Description");

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

string[] ParseInput(string input)
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

double EvaluateExpression(string expression)
{
    // Simple expression evaluator - handles basic arithmetic
    expression = expression.Replace(" ", "");

    // Handle parentheses
    while (expression.Contains('('))
    {
        var start = expression.LastIndexOf('(');
        var end = expression.IndexOf(')', start);
        if (end == -1) throw new ArgumentException("Mismatched parentheses");

        var subExpression = expression.Substring(start + 1, end - start - 1);
        var result = EvaluateSimpleExpression(subExpression);
        expression = expression.Substring(0, start) + result + expression.Substring(end + 1);
    }

    return EvaluateSimpleExpression(expression);
}

double EvaluateSimpleExpression(string expression)
{
    // Handle multiplication and division first
    var parts = expression.Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 1)
    {
        return EvaluateMultiplicationDivision(expression);
    }

    var result = EvaluateMultiplicationDivision(parts[0]);
    var currentIndex = parts[0].Length;

    for (int i = 1; i < parts.Length; i++)
    {
        var operatorIndex = currentIndex;
        while (operatorIndex < expression.Length && expression[operatorIndex] != '+' && expression[operatorIndex] != '-')
            operatorIndex++;

        if (operatorIndex >= expression.Length) break;

        var op = expression[operatorIndex];
        var value = EvaluateMultiplicationDivision(parts[i]);

        if (op == '+')
            result += value;
        else
            result -= value;

        currentIndex = operatorIndex + 1 + parts[i].Length;
    }

    return result;
}

double EvaluateMultiplicationDivision(string expression)
{
    var parts = expression.Split(new[] { '*', '/' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 1)
    {
        if (double.TryParse(parts[0], out var value))
            return value;
        throw new ArgumentException($"Invalid number: {parts[0]}");
    }

    var result = double.Parse(parts[0]);
    var currentIndex = parts[0].Length;

    for (int i = 1; i < parts.Length; i++)
    {
        var operatorIndex = currentIndex;
        while (operatorIndex < expression.Length && expression[operatorIndex] != '*' && expression[operatorIndex] != '/')
            operatorIndex++;

        if (operatorIndex >= expression.Length) break;

        var op = expression[operatorIndex];
        var value = double.Parse(parts[i]);

        if (op == '*')
            result *= value;
        else
        {
            if (value == 0) throw new ArgumentException("Division by zero");
            result /= value;
        }

        currentIndex = operatorIndex + 1 + parts[i].Length;
    }

    return result;
}