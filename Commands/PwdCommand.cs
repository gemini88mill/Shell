namespace Shell.Commands;

public class PwdCommand : ICommand
{
    public string Name => "pwd";
    public string Description => "Show current directory";

    public void Execute(string[] args)
    {
        Logger.Info($"Current directory: {Directory.GetCurrentDirectory()}");
    }
}
