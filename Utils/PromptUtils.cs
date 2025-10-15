using LibGit2Sharp;
using Spectre.Console;

namespace Shell.Utils;

public static class PromptUtils
{
    public static string FormatPrompt()
    {
        var currentDirectory = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);

        var isRoot = Directory.GetCurrentDirectory() == Path.GetPathRoot(Directory.GetCurrentDirectory());
        var pathPiece = isRoot ? $"root[[{Directory.GetCurrentDirectory()}]]" : $"{currentDirectory[^1]}";

        return $"▶ [white]{pathPiece}[/] [purple]{GetGitBranch()}[/]";
    }

    private static string? GetGitBranch()
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var repoPath = Repository.Discover(currentDirectory);

            if (repoPath == null)
            {
                return null;
            }

            var repo = new Repository(repoPath);
            return $"({repo.Head.FriendlyName})";
        }
        catch
        {
            return null;
        }
        
    }
}