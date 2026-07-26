using System.Text.RegularExpressions;
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

        var targetDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var ignorePatterns = LoadStowIgnore(Path.Combine(repoPath, ".stow-local-ignore"));
        var backupDir = default(string?);

        foreach (var dir in stowDirs)
        {
            var conflicts = FindConflicts(Path.Combine(repoPath, dir), targetDir, ignorePatterns, dir == ".");

            if (conflicts.Count > 0)
            {
                backupDir ??= CreateBackupDir();
                foreach (var conflict in conflicts)
                {
                    var dest = Path.Combine(targetDir, conflict);
                    var backupPath = Path.Combine(backupDir, conflict);
                    var backupParent = Path.GetDirectoryName(backupPath)!;
                    Directory.CreateDirectory(backupParent);
                    File.Copy(dest, backupPath, overwrite: true);
                    AnsiConsole.MarkupLine($"  [yellow]Backed up[/] [grey]{conflict}[/]");
                    File.Delete(dest);
                }
            }
        }

        var success = true;
        foreach (var dir in stowDirs)
        {
            AnsiConsole.MarkupLine($"  Stowing [cyan]{dir}[/]...");

            var result = RunProcess("stow", $"--no-folding --dir={repoPath} --target={targetDir} {dir}", out var output);

            if (result != 0)
            {
                AnsiConsole.MarkupLine($"  [red]Failed to stow {dir}:[/] {output}");
                success = false;
            }
        }

        if (backupDir is not null)
            AnsiConsole.MarkupLine($"  [grey]Backups saved to[/] [cyan]{backupDir}[/]");

        return success;
    }

    private static List<string> FindConflicts(string sourceDir, string targetDir, List<Regex> ignorePatterns, bool isRoot)
    {
        var conflicts = new List<string>();
        if (!Directory.Exists(sourceDir))
            return conflicts;

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDir, file);
            if (ignorePatterns.Any(p => p.IsMatch(relative)))
                continue;

            var dest = Path.Combine(targetDir, relative);

            if (File.Exists(dest))
            {
                var linkTarget = default(string?);
                try { linkTarget = System.IO.File.ResolveLinkTarget(dest, false)?.FullName; } catch { }
                if (linkTarget != file)
                    conflicts.Add(relative);
            }
        }
        return conflicts;
    }

    private static List<Regex> LoadStowIgnore(string ignoreFilePath)
    {
        var patterns = new List<Regex>();
        if (!File.Exists(ignoreFilePath))
            return patterns;

        foreach (var line in File.ReadAllLines(ignoreFilePath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                continue;
            try { patterns.Add(new Regex(trimmed, RegexOptions.Compiled)); }
            catch { }
        }
        return patterns;
    }

    private static string CreateBackupDir()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var dir = Path.Combine(home, ".dotfiles-backup", timestamp);
        Directory.CreateDirectory(dir);
        return dir;
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
            if (File.Exists(zshrcTarget))
            {
                var linkTarget = default(string?);
                try { linkTarget = System.IO.File.ResolveLinkTarget(zshrcTarget, false)?.FullName; } catch { }
                if (linkTarget is not null)
                    File.Delete(zshrcTarget);
            }
            File.Copy(zshrcSource, zshrcTarget);
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
