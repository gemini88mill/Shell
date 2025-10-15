using Spectre.Console;

namespace Shell.Commands;

public class DirCommand : ICommand
{
    public string Name => "dir";
    public string Description => "List directory contents";
    public string[] Aliases => new[] { "ls" };

    public void Execute(string[] args)
    {
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
    }
}
