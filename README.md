# dotfiles-helper

Cross-platform CLI tool to bootstrap a new machine from your dotfiles.
Built with [Spectre.Console](https://spectreconsole.net/) on .NET.

```bash
dotfiles doctor     # Check what's installed vs missing
dotfiles install    # Install dependencies + stow dotfiles
```

## Usage

```bash
dotfiles doctor                # Scan all packages, show status
dotfiles install               # Full install flow
dotfiles install --repo ~/src/dotfiles  # Custom repo path
dotfiles install --skip-stow   # Only install deps, skip stow
```

## Features

- **OS detection** — Linux (Fedora, Debian, Arch, etc.), macOS, Windows
- **24+ packages** — system tools (dnf/apt/pacman/brew), runtimes (via mise), CLI tools
- **Smart sudo** — validates upfront, dumps commands if declined
- **Clean UI** — colored tables with rounded borders, progress bars, spinners
- **Extensible** — add packages in `Data/Packages.cs` with per-PM install commands
