namespace PAiPWebPackageManager.PluginBase;

public interface IPlugin
{
    #region Metadata
    /// <summary>
    /// Get Plugin Metadata
    /// </summary>
    /// <returns>
    /// Get Plugin Metadata
    /// </returns>
    public PluginInformationRecord GetPluginMetadata();
    #endregion
    
    #region Supported
    /// <summary>
    /// Check if the plugin is supported on the current platform and current system
    /// </summary>
    /// <returns>True if this plugin is supported</returns>
    public bool IsSupported();
    /// <summary>
    /// Check if installation of package manager that plugin provides is supported on current system
    /// </summary>
    /// <returns>True if this plugins package manager have support for installing it on current system</returns>
    public bool IsInstallSupported();
    /// <summary>
    /// Check if the plugin is supported and if it's available for installation if needed.
    /// </summary>
    /// <returns>
    /// True if this plugin is supported and package manager is installed
    /// True if this plugin is not supported and package manager is not installed but it's available for installation
    /// </returns>
    public bool IsSupportedWithInstall();
    #endregion
    
    #region IsPackageURI Supported
    /// <summary>
    /// Check if Package URI is supported by this plugin
    /// </summary>
    /// <param name="packageUri">Package URI</param>
    /// <returns>If this plugin supports this package URI</returns>
    public bool IsPackageUriSupported(string packageUri);
    /// <summary>
    /// Check if Package URI is supported by this plugin
    /// </summary>
    /// <param name="packageUri">Package URI</param>
    /// <returns>If this plugin supports this package URI</returns>
    public bool IsPackageUriSupported(PackageUri packageUri);
    #endregion
    
    #region Check If Package is Installed
    /// <summary>
    /// Check if package is installed on the system
    /// </summary>
    /// <param name="package">Package to check</param>
    /// <returns>True if package is installed</returns>
    public bool IsPackageInstalled(string package);
    /// <summary>
    /// Check if package is installed on the system
    /// </summary>
    /// <param name="package">Package to check</param>
    /// <returns>True if package is installed</returns>
    public bool IsPackageInstalled(PackageUri package);
    #endregion
    
    #region Package Management
    /// <summary>
    /// Install Package
    /// </summary>
    /// <param name="package">Package to install</param>
    /// <returns>True if package got installed</returns>
    public bool InstallPackage(string package);
    /// <summary>
    /// Install Package
    /// </summary>
    /// <param name="package">Package to install</param>
    /// <returns>True if package got installed</returns>
    public bool InstallPackage(IEnumerable<string> package);
    /// <summary>
    /// Install Package
    /// </summary>
    /// <param name="package">Package to install</param>
    /// <returns>True if package got installed</returns>
    public bool InstallPackage(PackageUri package);
    /// <summary>
    /// Install Package
    /// </summary>
    /// <param name="package">Package to install</param>
    /// <returns>True if package got installed</returns>
    public bool InstallPackage(IEnumerable<PackageUri> package);

    /// <summary>
    /// Update Package
    /// </summary>
    /// <param name="package">Package to update</param>
    /// <returns>True if package got updated</returns>
    public bool UpdatePackage(string package);
    /// <summary>
    /// Update Package
    /// </summary>
    /// <param name="package">Package to update</param>
    /// <returns>True if package got updated</returns>
    public bool UpdatePackage(IEnumerable<string> package);
    /// <summary>
    /// Update Package
    /// </summary>
    /// <param name="package">Package to update</param>
    /// <returns>True if package got updated</returns>
    public bool UpdatePackage(PackageUri package);
    /// <summary>
    /// Update Package
    /// </summary>
    /// <param name="package">Package to update</param>
    /// <returns>True if package got updated</returns>
    public bool UpdatePackage(IEnumerable<PackageUri> package);

    /// <summary>
    /// Remove Package
    /// </summary>
    /// <param name="package">Package to remove</param>
    /// <returns>True if package got removed</returns>
    public bool UninstallPackage(string package);
    /// <summary>
    /// Remove Package
    /// </summary>
    /// <param name="package">Package to remove</param>
    /// <returns>True if package got removed</returns>
    public bool UninstallPackage(IEnumerable<string> package);
    /// <summary>
    /// Remove Package
    /// </summary>
    /// <param name="package">Package to remove</param>
    /// <returns>True if package got removed</returns>
    public bool UninstallPackage(PackageUri package);
    /// <summary>
    /// Remove Package
    /// </summary>
    /// <param name="package">Package to remove</param>
    /// <returns>True if package got removed</returns>
    public bool UninstallPackage(IEnumerable<PackageUri> package);
    #endregion
    
    #region Package Database Management
    /// <summary>
    /// Update Package Database
    /// </summary>
    /// <returns>
    /// True if package database got updated
    /// True if package manager doesn't support updating package database
    /// </returns>
    public bool UpdatePackageDatabase();

    /// <summary>
    /// Add Package Database Repository
    /// </summary>
    /// <param name="repository">Package Repository to Add</param>
    /// <returns>
    /// True if package repository got added
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// This exception is thrown if package manager doesn't support adding package repositories
    /// </exception>
    public bool AddPackageRepositoryToDatabase(string repository);
    /// <summary>
    /// Remove Package Database Repository
    /// </summary>
    /// <param name="repository">Package Repository to Remove</param>
    /// <returns>
    /// True if package repository got removed
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// This exception is thrown if package manager doesn't support removing package repositories
    /// </exception>
    public bool RemovePackageRepositoryFromDatabase(string repository);
    #endregion
    
    #region Mass Update
    /// <summary>
    /// Update all packages that are currently installed
    /// </summary>
    /// <returns>
    /// True if all packages got updated
    /// </returns>
    public bool UpdateAllCurrentPackages();
    #endregion

    #region Package Manager Installation
    /// <summary>
    /// Install Package Manager
    /// </summary>
    /// <returns>
    /// If package manager got installed
    /// </returns>
    public bool InstallPackageManager();
    #endregion
    
    #region FUTURE:
    #if false
    public void SearchPackage(string package);
    public void SearchPackage(PackageUri packageUri);
    public List<string> GetListOfInstalledPackages();
    #endif
    #endregion
}
