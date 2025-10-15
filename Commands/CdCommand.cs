namespace Shell.Commands;

public class CdCommand : ICommand
{
    public string Name => "cd";
    public string Description => "Change directory";

    public void Execute(string[] args)
    {
        if (args.Length > 1)
        {
            try
            {
                Directory.SetCurrentDirectory(args[1]);
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
    }
}
