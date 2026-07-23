namespace DotfilesCli.Models;

public enum OsKind
{
    Linux,
    MacOs,
    Windows,
}

public enum PackageManagerKind
{
    Dnf,
    Apt,
    Pacman,
    Brew,
    Choco,
    Winget,
}

public class DeviceInfo
{
    public OsKind Os { get; init; }
    public string DistroId { get; init; } = "";
    public string DistroVersion { get; init; } = "";
    public PackageManagerKind PackageManager { get; init; }
    public bool HasSudo { get; init; }
}

public class Package
{
    public string Id { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Category { get; init; } = "";
    public string CheckCommand { get; init; } = "";
    public Dictionary<PackageManagerKind, string[]> InstallCommands { get; init; } = [];
    public string? PostInstallMessage { get; init; }

    public bool NeedsSudo { get; init; }
}
