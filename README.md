# dotfiles-helper

CLI tool to bootstrap my dotfiles on a fresh machine.
Built with [Spectre.Console](https://spectreconsole.net/) on .NET.

```bash
dotfiles doctor     # Check what's installed vs missing
dotfiles install    # Install dependencies + stow dotfiles
```

## Fresh Install

On a new machine, I need this tool to install everything — but the tool itself needs .NET.
A standalone binary avoids the cycle:

```bash
# 1. Download the latest release (no .NET needed)
curl -fsSL https://github.com/ikbakk/dotfiles-helper/releases/latest/download/dotfiles-linux-x64.tar.gz \
  | tar xz -C ~/.local/bin

# 2. Run it
dotfiles install
```

> No release uploaded yet? Then build from source:
> ```bash
> # Requires .NET SDK (via mise or system)
> git clone git@github.com:ikbakk/dotfiles-helper.git ~/src/dotfiles-helper
> cd ~/src/dotfiles-helper
> dotnet publish -c Release -r linux-x64 --self-contained -o ~/.local/bin
> ```

## Usage

```bash
dotfiles doctor                # Scan all packages, show status
dotfiles install               # Full install flow
dotfiles install --repo ~/src/dotfiles  # Custom repo path
dotfiles install --skip-stow   # Only install deps, skip stow
```

## What it installs

| Category | Packages |
|----------|----------|
| System   | git, neovim, eza, bat, fd, fzf, tmux, fastfetch, stow, zsh, java, curl, wget |
| Runtime  | Go, Node.js, .NET SDK, Flutter (all via mise) |
| Tools    | Rust (rustup), starship, zoxide, atuin, mise, opencode |
