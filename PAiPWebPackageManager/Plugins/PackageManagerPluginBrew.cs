using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginBrew: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_LinuxAndMacOS,
        PluginName = "Brew",
        DoesNotWorkWithAdmin = true,
        IsAdminNeeded = false,
        RequiredCommands = new List<string>(new []{"brew"}),
        SupportedPlatforms = { OSPlatform.Linux, OSPlatform.OSX},
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
            ExecuteCommand($"brew ls --versions {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew install {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew upgrade {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"brew uninstall {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand("brew update"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("brew update"),
            new[] { 0 }
        ) && CheckExitCode(
            ExecuteCommand("brew upgrade"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        throw new NotSupportedException("This package manager does not support adding repositories.");
    }
    
    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        throw new NotSupportedException("This package manager does not support removing repositories.");
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