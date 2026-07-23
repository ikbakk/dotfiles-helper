using Spectre.Console;

namespace DotfilesCli.Infrastructure;

public static class StowService
{
    public static bool StowDotfiles(string repoPath)
    {
        if (!Directory.Exists(repoPath))
        {
            AnsiConsole.MarkupLine($"[red]Directory not found:[/] {repoPath}");
            return false;
        }

        var dirs = Directory.GetDirectories(repoPath)
            .Select(Path.GetFileName)
            .Where(d => d is not null && !d.StartsWith('.'))
            .Cast<string>()
            .ToList();

        var stowDirs = dirs.Count == 0
            ? ["."]
            : dirs;

        var success = true;
        foreach (var dir in stowDirs)
        {
            AnsiConsole.MarkupLine($"  Stowing [cyan]{dir}[/]...");

            var result = RunProcess("stow", $"--adopt --no-folding --dir={repoPath} --target={Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)} {dir}", out var output);

            if (result != 0)
            {
                AnsiConsole.MarkupLine($"  [red]Failed to stow {dir}:[/] {output}");
                success = false;
            }
        }

        return success;
    }

    private static int RunProcess(string fileName, string args, out string output)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var proc = System.Diagnostics.Process.Start(psi);
        proc!.WaitForExit(30000);
        output = (proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd()).Trim();
        return proc.ExitCode;
    }
}
