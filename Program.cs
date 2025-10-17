using Spectre.Console;
using Shell;
using Shell.Commands;
using Shell.Utils;
using Shell.Parsers;
using System.Text;

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
        if (ReplHelpers.HandleReplCommand(input, ref isRunning, commandHistory))
            continue;

        // Parse and execute the command
        var parsedCommand = InputParser.ParseInputWithRedirection(input);
        ExecuteCommand(parsedCommand);
    }
    catch (Exception ex)
    {
        Logger.Exception(ex, "Main program loop");
    }

    Logger.NewLine();
}

Logger.Info("Goodbye!");

void ExecuteCommand(ParsedCommand parsedCommand)
{
    var args = parsedCommand.CommandArgs;
    if (args.Length == 0) return;

    var commandKey = args[0];
    if (commands.TryGetValue(commandKey, out var cmd))
    {
        // Start capturing output if redirection is specified
        if (parsedCommand.OutputFile != null)
        {
            Logger.StartCapture();
        }

        try
        {
            cmd.Execute(args);
        }
        finally
        {
            // If we were capturing, save to file
            if (parsedCommand.OutputFile != null)
            {
                try
                {
                    var capturedOutput = Logger.StopCapture();
                    File.AppendAllText(parsedCommand.OutputFile, capturedOutput, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to write to file '{parsedCommand.OutputFile}': {ex.Message}");
                }
            }
        }
        return;
    }

    Logger.Error($"Unknown command: {commandKey}");
    Logger.Warning("Type 'help' for available commands");
}
