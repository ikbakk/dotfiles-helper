using Spectre.Console;
using Spectre.Console.Cli;
using DotfilesCli.Commands;

AnsiConsole.Write(new FigletText("dotfiles").Centered().Color(new Spectre.Console.Color(99, 102, 241)));

var app = new CommandApp();

app.Configure(config =>
{
    config.Settings.ApplicationName = "dotfiles";

    config.AddCommand<InstallCommand>("install")
        .WithDescription("Install dependencies and stow dotfiles");

    config.AddCommand<DoctorCommand>("doctor")
        .WithDescription("Check which packages are installed vs missing");
});

return app.Run(args);
