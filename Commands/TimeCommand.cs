namespace Shell.Commands;

public class TimeCommand : ICommand
{
    public string Name => "time";
    public string Description => "Show current time";

    public void Execute(string[] args)
    {
        var now = DateTime.Now;
        Logger.Info($"Current time: {now:yyyy-MM-dd HH:mm:ss}");
    }
}
