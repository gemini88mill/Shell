using Spectre.Console;
using Shell;
using Shell.Commands;
using Shell.Utils;

bool isRunning = true;
var commandHistory = new List<string>();

// Register commands
var commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
{
    ["echo"] = new EchoCommand(),
    ["time"] = new TimeCommand(),
    ["calc"] = new CalcCommand(),
    ["file"] = new FileCommand(),
    ["dir"] = new DirCommand(),
    ["pwd"] = new PwdCommand(),
    ["cd"] = new CdCommand(),
};

// Register aliases for all commands
foreach (var cmd in commands.Values.ToList())
{
    if (cmd.Aliases is null) continue;
    foreach (var alias in cmd.Aliases)
    {
        if (!string.IsNullOrWhiteSpace(alias) && !commands.ContainsKey(alias))
        {
            commands[alias] = cmd;
        }
    }
}

AnsiConsole.Write(new FigletText("rsh").Color(Color.Blue));
Logger.Info("Welcome to the Shell REPL! Type help for available commands or exit to quit.");
Logger.NewLine();

while (isRunning)
{
    try
    {
        // Get user input with a styled prompt
        var input = Logger.Ask($"{PromptUtils.FormatPrompt()}");

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

    var commandKey = args[0];
    if (commands.TryGetValue(commandKey, out var cmd))
    {
        cmd.Execute(args);
        return;
    }

    Logger.Error($"Unknown command: {commandKey}");
    Logger.Warning("Type 'help' for available commands");
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