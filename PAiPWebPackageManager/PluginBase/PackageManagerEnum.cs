namespace PAiPWebPackageManager.PluginBase;

// ReSharper disable InconsistentNaming

public enum PackageManagerEnum
{
    /// <summary>
    /// Null Package Manager
    /// </summary>
    Null,
    // Windows
    Windows_Category = 1_000,
    /// <summary>
    /// Windows - WinGet
    /// </summary>
    Windows_WinGet,
    /// <summary>
    /// Windows - Chocolatey
    /// </summary>
    Windows_Chocolatey,
    /// <summary>
    /// Windows - Scoop
    /// </summary>
    Windows_Scoop,
    // - Special
    /// <summary>
    /// Windows - NuGet
    /// </summary>
    Windows_NuGet,
    // - File Based
    Windows_Installer,
    // Linux
    Linux_Category = 2_000,
    /// <summary>
    /// Linux - Debian Based - Apt
    /// </summary>
    Linux_Apt,
    // Linux - Debian Based - Nala (Alternative to Apt) (PART of APT Plugin)
    // Linux_Nala,
    /// <summary>
    /// Linux - Suse Based - Yum
    /// </summary>
    Linux_Yum,
    /// <summary>
    /// Linux - Fedora Based - Dnf
    /// </summary>
    Linux_Dnf,
    /// <summary>
    /// Linux - Arch Based - Pacman
    /// </summary>
    Linux_Pacman,
    /// <summary>
    /// Linux - Debian Based - Pacstall (Alternative to Pacman but for Debian Based Distros)
    /// </summary>
    Linux_Pacstall,
    /// <summary>
    /// Linux - Alpine Based - Apk
    /// </summary>
    Linux_Apk,
    /// <summary>
    /// Linux - Flatpak
    /// </summary>
    Linux_Flatpak,
    /// <summary>
    /// Linux - Snap
    /// </summary>
    Linux_Snap,
    /// <summary>
    /// Linux - Homebrew
    /// </summary>
    Linux_Brew,
    // - File Based
    /// <summary>
    /// Linux - AppImage
    /// </summary>
    Linux_AppImage,
    /// <summary>
    /// Linux - Debian Based - dpkg
    /// </summary>
    Linux_Dpkg,
    /// <summary>
    /// Linux - Suse Based - rpm
    /// </summary>
    Linux_Rpm,
    // MacOs
    MacOS_Category = 3_000,
    /// <summary>
    /// MacOS - Homebrew
    /// </summary>
    MacOS_Brew,
    /// <summary>
    /// MacOS - Homebrew Cask
    /// </summary>
    MacOS_BrewCask,
    /// <summary>
    /// MacOS - MacPorts
    /// </summary>
    MacOS_MacPorts,
    // - File Based
    /// <summary>
    /// MacOS - App File
    /// </summary>
    MacOS_App,
    // Special
    Special_Category = 4_000,
    /// <summary>
    /// Python 3 - pip
    /// </summary>
    Special_Python3_Pip,
    /// <summary>
    /// Python 3 - Anaconda
    /// </summary>
    Special_Python3_Anaconda,
    /// <summary>
    /// Node.js - npm
    /// </summary>
    Special_NodeJS_Npm,
    /// <summary>
    /// Node.js - yarn
    /// </summary>
    Special_NodeJS_Yarn,
    /// <summary>
    /// Ruby - gem
    /// </summary>
    Special_Ruby_Gem,
    /// <summary>
    /// Rust - cargo
    /// </summary>
    Special_Rust_Cargo,
    /// <summary>
    /// Go - go
    /// </summary>
    Special_Go_Go,
    /// <summary>
    /// .NET - dotnet
    /// </summary>
    Special_DotNet_DotNet,
    /// <summary>
    /// PowerShell - Power Shell Modules
    /// </summary>
    Special_Powershell,
    /// <summary>
    /// Nix
    /// </summary>
    Special_Nix,
}