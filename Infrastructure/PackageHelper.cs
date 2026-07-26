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
        DeviceInfo device)
    {
        if (missing.Count == 0) return;

        var isLinux = device.Os == OsKind.Linux;
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

        var aurHelper = await ResolveAurHelperAsync(device);

        var total = missing.Count;

        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Starting...", maxValue: total);

                for (var i = 0; i < total; i++)
                {
                    var pkg = missing[i];
                    task.Description = $"Installing [cyan]{pkg.DisplayName}[/] ({i + 1}/{total})";

                    var commands = pkg.InstallCommands.GetValueOrDefault(pm, []);

                    foreach (var cmd in commands)
                    {
                        var exitCode = await RunInstallCommandAsync(cmd);
                        if (exitCode != 0)
                        {
                            if (aurHelper is not null)
                            {
                                var aurCmd = cmd.Replace("pacman", aurHelper);
                                AnsiConsole.MarkupLine($"  [yellow]Retrying with {aurHelper}...[/]");
                                exitCode = await RunInstallCommandAsync(aurCmd);
                            }

                            if (exitCode != 0)
                                AnsiConsole.MarkupLine($"  [red]Failed:[/] {cmd}");
                        }
                    }

                    if (!string.IsNullOrEmpty(pkg.PostInstallMessage))
                        AnsiConsole.MarkupLine($"  [grey]→ {pkg.PostInstallMessage}[/]");

                    task.Increment(1);
                }
            });

        AnsiConsole.MarkupLine("\n[bold green]All packages installed![/]\n");
    }

    private static Task<string?> ResolveAurHelperAsync(DeviceInfo device)
    {
        if (device.PackageManager != PackageManagerKind.Pacman)
            return Task.FromResult<string?>(null);

        var helpers = device.AvailableAurHelpers;
        if (helpers.Count == 0)
            return Task.FromResult<string?>(null);

        if (helpers.Count == 1)
        {
            AnsiConsole.MarkupLine($"  [grey]AUR helper detected:[/] [cyan]{helpers[0]}[/]");
            return Task.FromResult<string?>(helpers[0]);
        }

        return Task.FromResult<string?>(AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Multiple AUR helpers detected. Which one should I use?")
                .AddChoices(helpers)));
    }
}
