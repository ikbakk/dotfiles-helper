using Spectre.Console;
using DotfilesCli.Models;

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

        var stowDirs = new List<string> { "." };

        var subdirs = Directory.GetDirectories(repoPath)
            .Select(Path.GetFileName)
            .Where(d => d is not null && !d.StartsWith('.'))
            .Cast<string>();

        stowDirs.AddRange(subdirs);

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

    public static bool DeployZshConfig(string repoPath, DeviceInfo device)
    {
        var zshrcSource = device.PackageManager switch
        {
            PackageManagerKind.Pacman => Path.Combine(repoPath, ".zshrc.arch"),
            _ => Path.Combine(repoPath, ".zshrc.fedora"),
        };

        if (!File.Exists(zshrcSource))
        {
            AnsiConsole.MarkupLine($"[yellow]No distro-specific .zshrc found at {zshrcSource}; keeping existing.[/]");
            return false;
        }

        var zshrcTarget = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zshrc");

        try
        {
            File.Copy(zshrcSource, zshrcTarget, overwrite: true);
            AnsiConsole.MarkupLine($"  Deployed [cyan]{Path.GetFileName(zshrcSource)}[/] → [grey]{zshrcTarget}[/]");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deploy .zshrc:[/] {ex.Message}");
            return false;
        }
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
