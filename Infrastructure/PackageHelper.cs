using Spectre.Console;
using DotfilesCli.Models;

namespace DotfilesCli.Infrastructure;

public static class PackageHelper
{
    public static async Task<bool> CheckInstalledAsync(Package pkg)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{pkg.CheckCommand}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            if (proc is null) return false;

            await proc.WaitForExitAsync().ConfigureAwait(false);
            return proc.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<int> RunInstallCommandAsync(string command)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            if (proc is null) return -1;

            await proc.WaitForExitAsync().ConfigureAwait(false);
            return proc.ExitCode;
        }
        catch
        {
            return -1;
        }
    }

    public static Markup StatusMarkup(bool installed) => installed
        ? new Markup("[green]✓ Installed[/]")
        : new Markup("[red bold]✗ Missing[/]");

    public static Markup CategoryMarkup(string category) => category switch
    {
        "system" => new Markup("[blue]system[/]"),
        "runtime" => new Markup("[green]runtime[/]"),
        "tool" => new Markup("[yellow]tool[/]"),
        _ => new Markup(category),
    };

    public static async Task InstallMissingPackagesAsync(
        List<Package> missing,
        PackageManagerKind pm,
        bool isLinux)
    {
        if (missing.Count == 0) return;

        var needSudo = missing.Any(p => p.NeedsSudo);

        if (needSudo && isLinux)
        {
            if (!SudoHandler.EnsureSudo())
            {
                var sudoCmds = missing
                    .Where(p => p.NeedsSudo)
                    .SelectMany(p => p.InstallCommands.GetValueOrDefault(pm, []));

                SudoHandler.PrintCommands(sudoCmds);
                AnsiConsole.MarkupLine("[yellow]Run the commands above, then re-run this tool to continue.[/]");
                return;
            }
        }

        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Installing packages", maxValue: missing.Count);

                foreach (var pkg in missing)
                {
                    task.Description = $"[cyan]{pkg.DisplayName}[/]";
                    var commands = pkg.InstallCommands.GetValueOrDefault(pm, []);

                    foreach (var cmd in commands)
                    {
                        var exitCode = await RunInstallCommandAsync(cmd);
                        if (exitCode != 0)
                            AnsiConsole.MarkupLine($"  [red]Failed:[/] {cmd}");
                    }

                    if (!string.IsNullOrEmpty(pkg.PostInstallMessage))
                        AnsiConsole.MarkupLine($"  [grey]→ {pkg.PostInstallMessage}[/]");

                    task.Increment(1);
                }
            });

        AnsiConsole.MarkupLine("\n[bold green]All packages installed![/]\n");
    }
}
