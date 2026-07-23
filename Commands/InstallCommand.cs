using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using DotfilesCli.Data;
using DotfilesCli.Infrastructure;
using DotfilesCli.Models;

namespace DotfilesCli.Commands;

public class InstallSettings : CommandSettings
{
    [CommandOption("--repo <PATH>")]
    [Description("Path to the dotfiles repository (default: ~/dotfiles)")]
    public string? RepoPath { get; set; }

    [CommandOption("--skip-stow")]
    [Description("Skip the stow step")]
    public bool SkipStow { get; set; }
}

public class InstallCommand : AsyncCommand<InstallSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, InstallSettings settings, CancellationToken cancellation)
    {
        var repoPath = settings.RepoPath
            ?? AnsiConsole.Ask<string>("[bold]Path to dotfiles repo:[/]", "~/dotfiles");

        repoPath = repoPath.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        var device = OsDetector.Detect();

        var infoRule = new Rule("[bold yellow]Environment[/]")
        {
            Style = Style.Parse("dim"),
            Justification = Justify.Left,
        };
        AnsiConsole.Write(infoRule);
        AnsiConsole.MarkupLine($"  [bold]OS:[/]  {device.Os} / {device.DistroId} {device.DistroVersion}");
        AnsiConsole.MarkupLine($"  [bold]PM:[/]  {device.PackageManager}");
        AnsiConsole.MarkupLine($"  [bold]Repo:[/] {repoPath}");
        AnsiConsole.WriteLine();

        var results = new List<(Package Package, bool Installed)>();

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan"))
            .Start("Checking installed packages...", async ctx =>
            {
                foreach (var pkg in Packages.All)
                {
                    var ok = await PackageHelper.CheckInstalledAsync(pkg);
                    results.Add((pkg, ok));
                }
            });

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(new Style(new Color(99, 102, 241)))
            .AddColumn(new TableColumn("[bold]Package[/]").Centered())
            .AddColumn(new TableColumn("[bold]Status[/]").Centered());

        var missing = new List<Package>();
        foreach (var (pkg, installed) in results)
        {
            table.AddRow(new Markup(pkg.DisplayName), PackageHelper.StatusMarkup(installed));
            if (!installed) missing.Add(pkg);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        if (missing.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold green]✓ All packages already installed![/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"  [bold yellow]{missing.Count}[/] package(s) need to be installed.");
            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("Proceed with installation?", false))
            {
                AnsiConsole.MarkupLine("[yellow]Installation cancelled.[/]");
                return 1;
            }

            await PackageHelper.InstallMissingPackagesAsync(missing, device.PackageManager, device.Os == OsKind.Linux);
        }

        if (!settings.SkipStow)
        {
            AnsiConsole.WriteLine();
            var stowRule = new Rule("[bold yellow]Stow Dotfiles[/]")
            {
                Style = Style.Parse("dim"),
                Justification = Justify.Left,
            };
            AnsiConsole.Write(stowRule);

            if (!Directory.Exists(repoPath))
            {
                AnsiConsole.MarkupLine($"[yellow]Repo directory not found at {repoPath}. Skipping stow.[/]");
            }
            else
            {
                var stowOk = StowService.StowDotfiles(repoPath);
                if (stowOk)
                    AnsiConsole.MarkupLine("[bold green]✓ Dotfiles stowed successfully![/]");
                else
                    AnsiConsole.MarkupLine("[bold red]✗ Some directories failed to stow.[/]");
            }
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Done![/] Restart your shell or run [grey dim]source ~/.zshrc[/].");
        return 0;
    }
}
