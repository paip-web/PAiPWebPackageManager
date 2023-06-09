﻿using System.Runtime.InteropServices;
using PAiPWebPackageManager.Command;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxSnap: PluginBaseClass
{
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Snapcraft",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = false,
        IsWslSupported = false,
        RequiredCommands = new List<string>(new []{"snap"}),
        SupportedPlatforms = { OSPlatform.Linux },
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        if (Executor.IsWsl())
        {
            return false;
        }
        
        return CheckBasicRequirements(ignoreCommands: true)
               && GetSupportedPackageManagers().ArePluginsAvailable();
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckAllBoolRequirements(new bool?[] {
            CheckIfPackageManagerUriIsSupported(packageUri, "snap"),
            CheckIfPackageManagerUriIsSupported(packageUri, "snapcraft"),
        });
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"snap list {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"snap install {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"snap refresh {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"snap remove {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        // Not Supported / Not Needed
        return true;
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        throw new NotSupportedException("This package manager does not support adding repositories.");
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        throw new NotSupportedException("This package manager does not support removing repositories.");
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("snap refresh"),
            new[] { 0 }
        );
    }
    
    private PluginManager? _pluginManagerInstance = null;
    
    private PluginManager GetSupportedPackageManagers()
    {
        _pluginManagerInstance ??= new PluginManager(new[] {
            "Pacman",
            "Apt",
            "Dnf",
            "Yum",
        });
        return _pluginManagerInstance;
    }

    public override bool InstallPackageManager()
    {
        var installed = false;
        string? installedWith = null;
        var supportedPackageManagers = GetSupportedPackageManagers();
        
        
        foreach (var (pluginName, plugin) in supportedPackageManagers.GetPluginsDict())
        {
            if (pluginName == "pacman" && plugin is PackageManagerPluginLinuxPacman pacmanPlugin)
            {
                if (!pacmanPlugin.InstallPackageFromAUR("snapd https://aur.archlinux.org/snapd.git"))
                {
                    continue;
                }
            }
            
            if (pluginName is "dnf" or "yum")
            {
                // Ignore Fedora which don't need this package
                if ((!File.Exists("/etc/fedora-release")) && !plugin.InstallPackage("epel-release"))
                {
                    // If RedHat Enterprise Linux or CentOS
                    // RHEL requires special package
                    // CentOS requires this package installed
                    if (File.Exists("/etc/redhat-release"))
                    {
                        AnsiConsole.WriteLine(
                            "[red]If you are using RedHat Enterprise Linux check docs for install epel-release: https://snapcraft.io/docs/installing-snap-on-red-hat[/red]"
                        );
                        continue;
                    }
                }
            }
            
            if (plugin.InstallPackage("snapd"))
            {
                installed = true;
                installedWith = pluginName;
                break;
            }
        }

        if (installed == false)
        {
            return false;
        }

        if (!CheckExitCode(
            ExecuteCommand("systemctl enable --now snapd.socket"),
            new[] { 0 }
        ))
        {
            return false;
        }

        if (
            installedWith is not null
            && new List<string>(new [] { "pacman", "dnf", "yum" }).Contains(installedWith)
        )
        {
            if (!File.Exists("/snap"))
            {
                File.CreateSymbolicLink("/snap", "/var/lib/snapd/snap");
            }
        }

        return true;
    }
}