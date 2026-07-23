using Spectre.Console;

namespace DotfilesCli.Infrastructure;

public static class SudoHandler
{
    public static bool EnsureSudo()
    {
        var exitCode = RunProcess("sudo", "-v", out _);
        if (exitCode == 0) return true;

        AnsiConsole.MarkupLine("[yellow]sudo[/] requires your password to install system packages.");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What should we do?")
                .AddChoices("Enter sudo password", "Skip system packages", "Show install commands and exit"));

        switch (choice)
        {
            case "Enter sudo password":
                exitCode = RunProcess("sudo", "-v", out _);
                if (exitCode == 0) return true;
                AnsiConsole.MarkupLine("[red]sudo authentication failed.[/]");
                return false;

            case "Skip system packages":
                return false;

            case "Show install commands and exit":
                return false;

            default:
                return false;
        }
    }

    public static void PrintCommands(IEnumerable<string> commands)
    {
        AnsiConsole.MarkupLine("\n[bold yellow]Run these commands manually:[/]");
        foreach (var cmd in commands)
            AnsiConsole.MarkupLine($"  [grey]$[/] {cmd}");
        AnsiConsole.WriteLine();
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
        output = proc.StandardOutput.ReadToEnd().Trim();
        return proc.ExitCode;
    }
}
