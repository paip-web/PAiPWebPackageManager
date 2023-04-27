using System.Runtime.InteropServices;
using AutoConfigurator.Library.Blueprints.PackageManagerPlugins.Plugins;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxFlatpak: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Flatpak",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = false,
        RequiredCommands = new List<string>(new []{"flatpak"}),
        SupportedPlatforms = { OSPlatform.Linux },
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return false;
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "flatpak");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"flatpak info {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"flatpak install -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"flatpak update -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"flatpak uninstall -y {packageName.GetPackage()}"),
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
        if (repository.Split(" ").Length < 2)
        {
            AnsiConsole.WriteLine("[red]Flatpak repository have to have format: `name repository_url`[/red]");
            return false;
        } 
        
        return CheckExitCode(
            ExecuteCommand($"flatpak remote-add {repository}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"flatpak remote-delete {repository}"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("flatpak update -y"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        var flatpakPackageManagers = new List<PluginBaseClass>(new[]
            {
                new PackageManagerPluginLinuxApt(),
                new PackageManagerPluginLinuxDnf(),
                new PackageManagerPluginLinuxYum(),
                new PackageManagerPluginLinuxPacman(),
                new PackageManagerPluginLinuxApk(),
            });
        var supportedPackageManagers = flatpakPackageManagers
            .Where(plugin => plugin.IsSupported());
        foreach (var plugin in supportedPackageManagers)
        {
            if (plugin.InstallPackage("flatpak"))
            {
                break;
            }
        }

        // Add Default Repo
        return CheckExitCode(
            ExecuteCommand("flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo"),
            new[] { 0 }
        );
    }
}