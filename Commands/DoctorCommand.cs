using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using DotfilesCli.Data;
using DotfilesCli.Infrastructure;
using DotfilesCli.Models;

namespace DotfilesCli.Commands;

public class DoctorSettings : CommandSettings
{
    [CommandOption("--repo <PATH>")]
    [Description("Path to the dotfiles repository")]
    public string? RepoPath { get; set; }
}

public class DoctorCommand : AsyncCommand<DoctorSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, DoctorSettings settings, CancellationToken cancellation)
    {
        var device = OsDetector.Detect();

        var header = new Rule("[bold yellow]System Info[/]")
        {
            Style = Style.Parse("dim"),
            Justification = Justify.Left,
        };
        AnsiConsole.Write(header);
        AnsiConsole.MarkupLine($"  [bold]OS:[/]       {device.Os} / {device.DistroId} {device.DistroVersion}");
        AnsiConsole.MarkupLine($"  [bold]PM:[/]       {device.PackageManager}");
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(new Style(new Color(99, 102, 241)))
            .AddColumn(new TableColumn("[bold]Package[/]").Centered())
            .AddColumn(new TableColumn("[bold]Category[/]").Centered())
            .AddColumn(new TableColumn("[bold]Status[/]").Centered());

        var total = Packages.All.Count;
        var installedCount = 0;
        var missing = new List<Package>();

        foreach (var pkg in Packages.All)
        {
            var ok = await PackageHelper.CheckInstalledAsync(pkg);
            table.AddRow(
                new Markup(pkg.DisplayName),
                PackageHelper.CategoryMarkup(pkg.Category),
                PackageHelper.StatusMarkup(ok));
            if (ok) installedCount++;
            else missing.Add(pkg);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var summaryColor = installedCount == total ? "green" : "yellow";
        AnsiConsole.MarkupLine($"  [bold {summaryColor}]{installedCount}/{total}[/] packages installed.");

        if (missing.Count > 0)
        {
            AnsiConsole.WriteLine();
            if (AnsiConsole.Confirm("Install missing packages?", false))
            {
                await PackageHelper.InstallMissingPackagesAsync(missing, device.PackageManager, device);
            }
        }

        return 0;
    }
}
