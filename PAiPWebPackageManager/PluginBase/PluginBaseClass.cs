using System.Diagnostics;
using System.Runtime.InteropServices;
using PAiPWebPackageManager.Command;

namespace PAiPWebPackageManager.PluginBase;

public abstract class PluginBaseClass: IPlugin
{
    #region Plugin Interface

    protected PluginInformationRecord PluginMetadata = new PluginInformationRecord();

    public PluginInformationRecord GetPluginMetadata()
    {
        return PluginMetadata;
    }

    public abstract bool IsSupported();

    public abstract bool IsInstallSupported();

    public bool IsSupportedWithInstall()
    {
        return IsSupported() || IsInstallSupported();
    }

    public bool IsPackageUriSupported(string packageUri)
    {
        return IsPackageUriSupported(new PackageUri(packageUri));
    }

    public abstract bool IsPackageUriSupported(PackageUri packageUri);

    public bool IsPackageInstalled(string package)
    {
        return IsPackageInstalled(new PackageUri(package));
    }

    public abstract bool IsPackageInstalled(PackageUri package);

    public bool InstallPackage(string package)
    {
        return InstallPackage(new PackageUri(package));
    }

    public bool InstallPackage(IEnumerable<string> package)
    {
        return InstallPackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool InstallPackage(PackageUri package);

    public bool InstallPackage(IEnumerable<PackageUri> package)
    {
        return package.All(InstallPackage);
    }

    public bool UpdatePackage(string package)
    {
        return UpdatePackage(new PackageUri(package));
    }

    public bool UpdatePackage(IEnumerable<string> package)
    {
        return UpdatePackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool UpdatePackage(PackageUri package);

    public bool UpdatePackage(IEnumerable<PackageUri> package)
    {
        return package.All(InstallPackage);
    }

    public bool UninstallPackage(string package)
    {
        return UninstallPackage(new PackageUri(package));
    }

    public bool UninstallPackage(IEnumerable<string> package)
    {
        return UninstallPackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool UninstallPackage(PackageUri package);

    public bool UninstallPackage(IEnumerable<PackageUri> package)
    {
        return package.All(UninstallPackage);
    }

    public abstract bool UpdatePackageDatabase();

    public abstract bool AddPackageRepositoryToDatabase(string repository);

    public abstract bool RemovePackageRepositoryFromDatabase(string repository);

    public abstract bool UpdateAllCurrentPackages();

    public abstract bool InstallPackageManager();
    #endregion

    #region Utilities for Plugins
    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="command">
    /// Command to Execute
    /// </param>
    protected Process? ExecuteCommand(string command)
    {
        if (GetPluginMetadata().IsAdminNeeded)
        {
            return Executor.ExecuteCommandCrossPlatformAsAdmin(command);
        }

        return Executor.ExecuteCommandCrossPlatform(command);
    }
    
    /// <summary>
    /// Check if command is available
    /// </summary>
    /// <param name="command">
    /// Command to check
    /// </param>
    /// <returns>
    /// True if command exists
    /// </returns>
    protected static bool IsCommandAvailable(string command)
    {
        return Executor.IsCommandAvailable(command);
    }
    
    /// <summary>
    /// Check if platform is supported
    /// </summary>
    /// <param name="osPlatforms">
    /// Platform to check
    /// </param>
    /// <returns>
    /// True if platform is supported
    /// </returns>
    protected static bool IsPlatformSupported(OSPlatform[] osPlatforms)
    {
        return osPlatforms.All(RuntimeInformation.IsOSPlatform);
    }

    /// <summary>
    /// Check Basic Requirements of this plugin
    /// </summary>
    /// <returns>
    /// True if requirements are met
    /// </returns>
    protected bool CheckBasicRequirements(bool ignoreCommands = false)
    {
        return GetPluginMetadata().CheckRequirements(ignoreCommands);
    }

    /// <summary>
    /// This function checks if all specified arguments are true
    /// </summary>
    /// <param name="requirements">
    /// Requirements to fulfil (can be null if needed because of ?. operator)
    /// </param>
    /// <returns>
    /// If all requirements are true
    /// </returns>
    protected static bool CheckAllBoolRequirements(IEnumerable<bool?> requirements)
    {
        return new List<bool?>(requirements)
            .TrueForAll(v => v == true);
    }

    /// <summary>
    /// Check if command returned one of approved ExitCodes
    /// </summary>
    /// <param name="commandResult">
    /// Result of Command you want to check
    /// </param>
    /// <param name="approvedExitCodes">
    /// List of approved exit codes
    /// </param>
    /// <returns>
    /// Did command succeed or no
    /// </returns>
    protected static bool CheckExitCode(Process? commandResult, IEnumerable<int>? approvedExitCodes)
    {
        if (commandResult is null)
        {
            return false;
        }

        if (commandResult.HasExited == false)
        {
            return false;
        }

        approvedExitCodes ??= new[] { 0 };
        
        return new List<int>(approvedExitCodes)
            .TrueForAll(approvedExitCode => commandResult.ExitCode == approvedExitCode);
    }

    /// <summary>
    /// Check if PackageManagerUri is supported
    /// </summary>
    /// <param name="packageUri">
    /// Package URI
    /// </param>
    /// <param name="acceptedUri">
    /// Accepted URI
    /// </param>
    /// <returns>If it's accepted uri for this plugin</returns>
    protected static bool CheckIfPackageManagerUriIsSupported(PackageUri packageUri, string acceptedUri)
    {
        var managerUri = packageUri.GetManagerUri();
        if (managerUri == null)
        {
            return true;
        }

        return managerUri == acceptedUri;
    }
    #endregion
}