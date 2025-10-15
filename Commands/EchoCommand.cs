using Shell;
using Shell.Commands;

namespace Shell.Commands;

public class EchoCommand : ICommand
{
    public string Name => "echo";
    public string Description => "Echo the provided text";

    public void Execute(string[] args)
    {
        if (args.Length > 1)
        {
            var text = string.Join(" ", args.Skip(1));
            Logger.Info(text);
        }
        else
        {
            Logger.Warning("Usage: echo <text>");
        }
    }
}
