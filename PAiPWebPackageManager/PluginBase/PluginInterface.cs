namespace PAiPWebPackageManager.PluginBase;

public interface PluginInterface
{
#region Supported
    public bool IsSupported();
    public bool IsInstallSupported();
#endregion
#region IsPackageURI Supported
    public bool IsPackageURISupported(string packageURI);
#endregion
#region Check If Package is Installed
    public bool IsPackageInstalled(string package);
#endregion
#region Package Management
    public bool InstallPackage(string package);
    public bool InstallPackage(List<string> packages);

    public bool UpdatePackage(string package);
    public bool UpdatePackage(List<string> packages);

    public bool UninstallPackage(string package);
    public bool UninstallPackage(List<string> packages);
#endregion
#region Package Database Management
    public bool UpdatePackageDatabase();

    public bool AddPackageDatabase(string database);
    public bool RemovePackageDatabase(string database);
#endregion
#region Mass Update
    public bool UpdateAllCurrentPackages();
#endregion
}
