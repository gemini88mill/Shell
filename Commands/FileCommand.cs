using Spectre.Console;

namespace Shell.Commands;

public class FileCommand : ICommand
{
    public string Name => "file";
    public string Description => "Get information about a file";

    public void Execute(string[] args)
    {
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
    }
}
