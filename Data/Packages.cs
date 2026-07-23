using DotfilesCli.Models;

namespace DotfilesCli.Data;

public static class Packages
{
    public static readonly List<Package> All =
    [
        // ── System (dnf) ────────────────────────────────────────────
        new()
        {
            Id = "git", DisplayName = "Git", Category = "system",
            CheckCommand = "command -v git",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y git"],
                [PackageManagerKind.Apt] = ["apt install -y git"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm git"],
                [PackageManagerKind.Brew] = ["brew install git"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "neovim", DisplayName = "Neovim", Category = "system",
            CheckCommand = "command -v nvim",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y neovim"],
                [PackageManagerKind.Apt] = ["apt install -y neovim"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm neovim"],
                [PackageManagerKind.Brew] = ["brew install neovim"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "eza", DisplayName = "eza", Category = "system",
            CheckCommand = "command -v eza",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y eza"],
                [PackageManagerKind.Apt] = ["apt install -y eza"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm eza"],
                [PackageManagerKind.Brew] = ["brew install eza"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "bat", DisplayName = "bat", Category = "system",
            CheckCommand = "command -v bat",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y bat"],
                [PackageManagerKind.Apt] = ["apt install -y bat"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm bat"],
                [PackageManagerKind.Brew] = ["brew install bat"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "fdfind", DisplayName = "fd", Category = "system",
            CheckCommand = "command -v fdfind || command -v fd",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y fd-find"],
                [PackageManagerKind.Apt] = ["apt install -y fd-find"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm fd"],
                [PackageManagerKind.Brew] = ["brew install fd"],
            },
            NeedsSudo = true,
            PostInstallMessage = "On Fedora the binary is 'fdfind'; add 'alias fd=fdfind' to .zshrc if needed.",
        },
        new()
        {
            Id = "fzf", DisplayName = "fzf", Category = "system",
            CheckCommand = "command -v fzf",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y fzf"],
                [PackageManagerKind.Apt] = ["apt install -y fzf"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm fzf"],
                [PackageManagerKind.Brew] = ["brew install fzf"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "tmux", DisplayName = "tmux", Category = "system",
            CheckCommand = "command -v tmux",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y tmux"],
                [PackageManagerKind.Apt] = ["apt install -y tmux"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm tmux"],
                [PackageManagerKind.Brew] = ["brew install tmux"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "fastfetch", DisplayName = "fastfetch", Category = "system",
            CheckCommand = "command -v fastfetch",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y fastfetch"],
                [PackageManagerKind.Apt] = ["apt install -y fastfetch"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm fastfetch"],
                [PackageManagerKind.Brew] = ["brew install fastfetch"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "stow", DisplayName = "stow", Category = "system",
            CheckCommand = "command -v stow",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y stow"],
                [PackageManagerKind.Apt] = ["apt install -y stow"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm stow"],
                [PackageManagerKind.Brew] = ["brew install stow"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "unzip", DisplayName = "unzip", Category = "system",
            CheckCommand = "command -v unzip",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y unzip"],
                [PackageManagerKind.Apt] = ["apt install -y unzip"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm unzip"],
                [PackageManagerKind.Brew] = ["brew install unzip"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "zsh", DisplayName = "Zsh", Category = "system",
            CheckCommand = "command -v zsh",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y zsh"],
                [PackageManagerKind.Apt] = ["apt install -y zsh"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm zsh"],
                [PackageManagerKind.Brew] = ["brew install zsh"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "java", DisplayName = "Java (OpenJDK 21)", Category = "system",
            CheckCommand = "command -v java",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y java-21-openjdk"],
                [PackageManagerKind.Apt] = ["apt install -y openjdk-21-jdk"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm jdk21-openjdk"],
                [PackageManagerKind.Brew] = ["brew install openjdk@21"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "curl", DisplayName = "curl", Category = "system",
            CheckCommand = "command -v curl",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y curl"],
                [PackageManagerKind.Apt] = ["apt install -y curl"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm curl"],
                [PackageManagerKind.Brew] = ["brew install curl"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "wget", DisplayName = "wget", Category = "system",
            CheckCommand = "command -v wget",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y wget"],
                [PackageManagerKind.Apt] = ["apt install -y wget"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm wget"],
                [PackageManagerKind.Brew] = ["brew install wget"],
            },
            NeedsSudo = true,
        },

        // ── Runtimes (mise) ─────────────────────────────────────────
        new()
        {
            Id = "go", DisplayName = "Go", Category = "runtime",
            CheckCommand = "mise which go 2>/dev/null || command -v go",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["mise install go@latest", "mise use -g go@latest"],
                [PackageManagerKind.Apt] = ["mise install go@latest", "mise use -g go@latest"],
                [PackageManagerKind.Pacman] = ["mise install go@latest", "mise use -g go@latest"],
                [PackageManagerKind.Brew] = ["mise install go@latest", "mise use -g go@latest"],
            },
        },
        new()
        {
            Id = "node", DisplayName = "Node.js", Category = "runtime",
            CheckCommand = "mise which node 2>/dev/null || command -v node",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["mise install node@latest", "mise use -g node@latest"],
                [PackageManagerKind.Apt] = ["mise install node@latest", "mise use -g node@latest"],
                [PackageManagerKind.Pacman] = ["mise install node@latest", "mise use -g node@latest"],
                [PackageManagerKind.Brew] = ["mise install node@latest", "mise use -g node@latest"],
            },
        },
        new()
        {
            Id = "dotnet", DisplayName = ".NET SDK", Category = "runtime",
            CheckCommand = "mise which dotnet 2>/dev/null || command -v dotnet",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["mise install dotnet@latest", "mise use -g dotnet@latest"],
                [PackageManagerKind.Apt] = ["mise install dotnet@latest", "mise use -g dotnet@latest"],
                [PackageManagerKind.Pacman] = ["mise install dotnet@latest", "mise use -g dotnet@latest"],
                [PackageManagerKind.Brew] = ["mise install dotnet@latest", "mise use -g dotnet@latest"],
            },
        },
        new()
        {
            Id = "flutter", DisplayName = "Flutter", Category = "runtime",
            CheckCommand = "mise which flutter 2>/dev/null || command -v flutter",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["mise install flutter@latest", "mise use -g flutter@latest"],
                [PackageManagerKind.Apt] = ["mise install flutter@latest", "mise use -g flutter@latest"],
                [PackageManagerKind.Pacman] = ["mise install flutter@latest", "mise use -g flutter@latest"],
                [PackageManagerKind.Brew] = ["mise install flutter@latest", "mise use -g flutter@latest"],
            },
        },

        // ── Tools ───────────────────────────────────────────────────
        new()
        {
            Id = "rust", DisplayName = "Rust (rustup)", Category = "tool",
            CheckCommand = "command -v cargo",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] =
                [
                    "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y",
                ],
                [PackageManagerKind.Apt] =
                [
                    "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y",
                ],
                [PackageManagerKind.Pacman] =
                [
                    "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y",
                ],
                [PackageManagerKind.Brew] =
                [
                    "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y",
                ],
            },
        },
        new()
        {
            Id = "starship", DisplayName = "starship", Category = "tool",
            CheckCommand = "command -v starship",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y starship"],
                [PackageManagerKind.Apt] = ["curl -sS https://starship.rs/install.sh | sh -s -- -y"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm starship"],
                [PackageManagerKind.Brew] = ["brew install starship"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "zoxide", DisplayName = "zoxide", Category = "tool",
            CheckCommand = "command -v zoxide",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y zoxide"],
                [PackageManagerKind.Apt] = ["curl -sS https://raw.githubusercontent.com/ajeetdsouza/zoxide/main/install.sh | sh"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm zoxide"],
                [PackageManagerKind.Brew] = ["brew install zoxide"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "atuin", DisplayName = "atuin", Category = "tool",
            CheckCommand = "command -v atuin",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y atuin"],
                [PackageManagerKind.Apt] = ["bash <(curl --proto '=https' --tlsv1.2 -sSf https://setup.atuin.sh)"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm atuin"],
                [PackageManagerKind.Brew] = ["brew install atuin"],
            },
            NeedsSudo = true,
        },

        // ── Mise itself (mise should already be installed but just in case) ──
        new()
        {
            Id = "mise", DisplayName = "mise", Category = "tool",
            CheckCommand = "command -v mise",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["dnf install -y mise"],
                [PackageManagerKind.Apt] = ["curl https://mise.run | sh"],
                [PackageManagerKind.Pacman] = ["pacman -S --noconfirm mise"],
                [PackageManagerKind.Brew] = ["brew install mise"],
            },
            NeedsSudo = true,
        },
        new()
        {
            Id = "opencode", DisplayName = "opencode", Category = "tool",
            CheckCommand = "command -v opencode",
            InstallCommands =
            {
                [PackageManagerKind.Dnf] = ["curl -fsSL https://opencode.ai/install.sh | sh"],
                [PackageManagerKind.Apt] = ["curl -fsSL https://opencode.ai/install.sh | sh"],
                [PackageManagerKind.Pacman] = ["curl -fsSL https://opencode.ai/install.sh | sh"],
                [PackageManagerKind.Brew] = ["curl -fsSL https://opencode.ai/install.sh | sh"],
            },
            PostInstallMessage = "Add ~/.opencode/bin to your PATH if not already present.",
        },
    ];
}
