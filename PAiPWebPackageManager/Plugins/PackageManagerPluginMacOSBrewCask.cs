using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginMacOSBrewCask: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_MacOS,
        PluginName = "BrewCask",
        DoesNotWorkWithAdmin = true,
        IsAdminNeeded = false,
        RequiredCommands = new List<string>(new []{"brew"}),
        SupportedPlatforms = { OSPlatform.OSX},
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true)
               && IsCommandAvailable("curl")
               && IsCommandAvailable("bash");
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "brew");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"brew ls --versions --cask {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew install --cask {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew upgrade --cask {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew uninstall --cask {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand("brew update --cask"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("brew update --cask"),
            new[] { 0 }
        ) && CheckExitCode(
            ExecuteCommand("brew upgrade --cask"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"brew tap {repository}"),
            new[] { 0 }
        );
    }
    
    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"brew untap {repository}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        return CheckExitCode(
            ExecuteCommand(
                "/bin/bash -c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
            ),
            new[] { 0 }
        );
    }
}