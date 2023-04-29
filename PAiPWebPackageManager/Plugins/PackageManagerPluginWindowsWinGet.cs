using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginWindowsWinGet: PluginBaseClass
{
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Windows,
        PluginName = "WinGet",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new[] { "winget" }),
        SupportedPlatforms = { OSPlatform.Windows },
    };

    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true);
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckAllBoolRequirements(new bool?[] {
            CheckIfPackageManagerUriIsSupported(packageUri, "winget"),
            CheckIfPackageManagerUriIsSupported(packageUri, "win"),
            CheckIfPackageManagerUriIsSupported(packageUri, "windows"),
        });
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"winget list \"{package.GetPackage()}\""),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"winget install --accept-source-agreements --accept-package-agreements {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"winget upgrade --accept-source-agreements --accept-package-agreements {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"winget uninstall --accept-source-agreements --accept-package-agreements {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        // Not Supported / Not Needed
        
        return CheckExitCode(
            ExecuteCommand("winget source update"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        var repositorySplit = repository.Split(" ");
        if (repositorySplit.Length < 2)
        {
            AnsiConsole.WriteLine("[red]WinGet repository have to have format: `name repository_url`[/red]");
            return false;
        }
        
        var repositoryName = repositorySplit[0];
        var repositoryUrl = repositorySplit[1];
        
        return CheckExitCode(
            ExecuteCommand($"winget source add --name {repositoryName} {repositoryUrl}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"winget source remove --name {repository}"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("winget upgrade --all --accept-source-agreements --accept-package-agreements"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        // Note: this have to be run in Windows PowerShell
        return CheckExitCode(
            ExecuteCommand("powershell.exe -NoProfile -ExecutionPolicy Bypass -Command " +
                           "\"" +
                           "Get-AppxPackage Microsoft.DesktopAppInstaller " +
                           "| Foreach {Add-AppxPackage -DisableDevelopmentMode -Register \"$($_.InstallLocation)\\AppXManifest.xml\"" +
                           "\""),
            new[] { 0 }
        );
    }
}