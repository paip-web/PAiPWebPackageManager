using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginMacOSMas: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Mas",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = false,
        RequiredCommands = new List<string>(new []{"mas"}),
        SupportedPlatforms = { OSPlatform.OSX },
    };

    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true)
            && GetSupportedPackageManagers().isSupported;
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckAllBoolRequirements(new bool?[] {
            CheckIfPackageManagerUriIsSupported(packageUri, "mas"),
            CheckIfPackageManagerUriIsSupported(packageUri, "apple-app-store"),
            CheckIfPackageManagerUriIsSupported(packageUri, "apple"),
            CheckIfPackageManagerUriIsSupported(packageUri, "apple-store"),
        });
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        var packageList = ExecuteCommand("mas list");
        if (packageList is null)
        {
            return false;
        }
        
        if (!CheckExitCode(packageList, new[] { 0 }))
        {
            return false;
        }
        
        var packageListString = packageList.StandardOutput.ReadToEnd();
        // Check if stdout have package name
        // To ensure that it's this package and not part of another package check with space at the end
        return packageListString.Contains($"{package.GetPackage()} ");
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"mas install {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"mas upgrade {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"mas uninstall {packageName.GetPackage()}"),
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
            ExecuteCommand($"mas upgrade"),
            new[] { 0 }
        );
    }
    

    private (PackageManagerPluginBrew plugin, bool isSupported) GetSupportedPackageManagers()
    {
        var brew = new PackageManagerPluginBrew();
        return (
            brew,
            brew.IsSupported()
        );
    }

    public override bool InstallPackageManager()
    {
        var (brew, isSupported) = GetSupportedPackageManagers();
        return isSupported && brew.InstallPackage("mas");
    }
}