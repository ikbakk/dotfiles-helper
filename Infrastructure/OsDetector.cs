using DotfilesCli.Models;

namespace DotfilesCli.Infrastructure;

public static class OsDetector
{
    public static DeviceInfo Detect()
    {
        if (OperatingSystem.IsWindows())
            return new DeviceInfo { Os = OsKind.Windows, PackageManager = PackageManagerKind.Winget };

        if (OperatingSystem.IsMacOS())
            return new DeviceInfo { Os = OsKind.MacOs, PackageManager = PackageManagerKind.Brew };

        return DetectLinux();
    }

    private static DeviceInfo DetectLinux()
    {
        var distroId = "";
        var distroVersion = "";

        if (File.Exists("/etc/os-release"))
        {
            foreach (var line in File.ReadAllLines("/etc/os-release"))
            {
                if (line.StartsWith("ID="))
                    distroId = line["ID=".Length..].Trim('"');
                else if (line.StartsWith("VERSION_ID="))
                    distroVersion = line["VERSION_ID=".Length..].Trim('"');
            }
        }

        if (string.IsNullOrEmpty(distroId) && File.Exists("/etc/arch-release"))
        {
            distroId = "arch";
        }

        var pm = distroId switch
        {
            "fedora" => PackageManagerKind.Dnf,
            "rhel" or "centos" => PackageManagerKind.Dnf,
            "ubuntu" or "pop" or "debian" or "linuxmint" or "elementary" => PackageManagerKind.Apt,
            "arch" or "cachyos" or "endeavouros" or "manjaro" => PackageManagerKind.Pacman,
            _ => PackageManagerKind.Dnf,
        };

        return new DeviceInfo
        {
            Os = OsKind.Linux,
            DistroId = distroId,
            DistroVersion = distroVersion,
            PackageManager = pm,
            AvailableAurHelpers = pm == PackageManagerKind.Pacman ? DetectAurHelpers() : [],
        };
    }

    private static List<string> DetectAurHelpers()
    {
        var helpers = new List<string>(2);
        if (File.Exists("/usr/bin/paru") || File.Exists("/usr/local/bin/paru"))
            helpers.Add("paru");
        if (File.Exists("/usr/bin/yay") || File.Exists("/usr/local/bin/yay"))
            helpers.Add("yay");
        return helpers;
    }
}
