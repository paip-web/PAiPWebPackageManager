using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginWindowsScoop: PluginBaseClass
{
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Windows,
        PluginName = "Scoop",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = false,
        RequiredCommands = new List<string>(new []{"scoop"}),
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
        return CheckIfPackageManagerUriIsSupported(packageUri, "scoop");
    }

    /// <summary>
    /// This tries to get the package name from supported elements that could be in package name
    /// </summary>
    /// <param name="packageUri">
    /// Package URI
    /// </param>
    /// <returns>
    /// Null if package name can't be determined, otherwise package name
    /// </returns>
    private string? GetPackageName(PackageUri packageUri)
    {
        var packageString = packageUri.GetPackage();
        // Handle package names that are URLS
        if (packageString.Contains("://"))
        {
            // Can't get package name from that
            return null;
        }
        // Handle package names that are paths
        if (packageString.Contains(Path.PathSeparator) || packageString.Contains('\\') || packageString.Contains('/'))
        {
            // Can't get package name from that
            return null;
        }
        
        // Handle package names with buckets
        var packageStringWithoutBucket = packageString.Split("/").Length == 2
            ? packageString.Split("/")[1]
            : packageString;
        // Handle package names with versions
        var packageStringWithoutVersion = packageStringWithoutBucket.Split("@").Length == 2
            ? packageStringWithoutBucket.Split("@")[0]
            : packageStringWithoutBucket;

        return packageStringWithoutVersion;
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        var packageString = GetPackageName(package);

        if (packageString is null)
        {
            // Assume it's not installed
            return false;
        }
        
        // Check if package folder exists
        // This is workaround that scoop doesn't have a way to check if package is installed
        return Directory.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "scoop", "apps", packageString));
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"scoop install {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"scoop update {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"scoop uninstall {packageName.GetPackage()}"),
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
        return CheckExitCode(
            ExecuteCommand($"scoop bucket add {repository}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"scoop bucket rm {repository}"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("scoop update --all"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        return CheckExitCode(
            ExecuteCommand("powershell.exe -NoProfile -ExecutionPolicy Bypass -Command " +
                           "\"" +
                           "irm get.scoop.sh | iex" +
                           "\""),
            new[] { 0 }
        );
    }
}