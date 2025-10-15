namespace Shell.Commands;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    // Optional aliases that trigger the same command (e.g., "ls" for "dir")
    string[] Aliases => System.Array.Empty<string>();
    void Execute(string[] args);
}
