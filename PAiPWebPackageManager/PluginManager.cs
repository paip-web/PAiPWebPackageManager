using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager;

public class PluginManager: IPlugin
{
    #region Plugins Lists

    /// <summary>
    /// Get All Available Plugins as List
    /// </summary>
    /// <returns>
    /// List of all available plugins on current machine
    /// </returns>
    public static List<IPlugin> GetAllAvailablePlugins()
    {
        return PluginsList
            .GetAllPlugins()
            .Where(plugin => plugin.IsSupported())
            .ToList();
    }
    
    /// <summary>
    /// Get All Available Plugins with supporting installation as List
    /// </summary>
    /// <returns>
    /// List of all available plugins with supporting installation on current machine
    /// </returns>
    public static List<IPlugin> GetAllAvailablePluginsWithInstall()
    {
        return PluginsList
            .GetAllPlugins()
            .Where(plugin => plugin.IsSupportedWithInstall())
            .ToList();
    }

    /// <summary>
    /// Get All Available Plugins as Dictionary
    /// </summary>
    /// <returns>
    /// Dictionary of all available plugins with supporting installation on current machine
    /// Where Key is name of Plugin and Value is Plugin Instance
    /// </returns>
    public static Dictionary<string, IPlugin> GetAllAvailablePluginsAsDictionary()
    {
        return PluginsList
            .GetAllPlugins()
            .Where(plugin => plugin.IsSupported())
            .ToDictionary(
                pluginForKey => pluginForKey.GetPluginMetadata().PluginName.ToLower(),
                pluginForValue => pluginForValue
            );
    }
    
    /// <summary>
    /// Get All Available Plugins with supporting installation as Dictionary
    /// </summary>
    /// <returns>
    /// Dictionary of all available plugins with supporting installation on current machine
    /// Where Key is name of Plugin and Value is Plugin Instance
    /// </returns>
    public static Dictionary<string, IPlugin> GetAllAvailablePluginsWithInstallAsDictionary()
    {
        return PluginsList
            .GetAllPlugins()
            .Where(plugin => plugin.IsSupportedWithInstall())
            .ToDictionary(
                pluginForKey => pluginForKey.GetPluginMetadata().PluginName.ToLower(),
                pluginForValue => pluginForValue
            );
    }
    
    #endregion
    
    #region Plugin Manager
    protected readonly Dictionary<string, IPlugin> Plugins;

    /// <summary>
    /// Constructor for Plugin Manager that will get all available plugins
    /// </summary>
    public PluginManager()
    {
        Plugins = GetAllAvailablePluginsAsDictionary();
    }

    /// <summary>
    /// Constructor for Plugin Manager that will get all available plugins from list provided
    /// </summary>
    /// <param name="plugins">List of plugins</param>
    /// <param name="withInstall">Does this should accept installation of package manager or no</param>
    public PluginManager(IEnumerable<IPlugin> plugins, bool withInstall = false)
    {
        Plugins = new List<IPlugin>(plugins)
            .Where(
                plugin => (withInstall ? plugin.IsSupportedWithInstall() : plugin.IsSupported())
            )
            .ToDictionary(
                pluginForKey => pluginForKey.GetPluginMetadata().PluginName.ToLower(),
                pluginForValue => pluginForValue
            );
    }

    /// <summary>
    /// Constructor for Plugin Manager that will get all available plugins from list provided
    /// </summary>
    /// <param name="plugins">List of plugin names</param>
    /// <param name="withInstall">Does this should accept installation of package manager or no</param>
    public PluginManager(IEnumerable<string> plugins, bool withInstall = false)
    {
        var availablePlugins = withInstall
            ? GetAllAvailablePluginsWithInstallAsDictionary()
            : GetAllAvailablePluginsAsDictionary();
        var approvedPlugins = new List<string>(plugins);
        
        Plugins = availablePlugins
            .Where(plugin => approvedPlugins.Contains(plugin.Key))
            .ToDictionary(
                pluginForKey => pluginForKey.Key,
                pluginForValue => pluginForValue.Value
            );
    }
    
    /// <summary>
    /// Get List of Plugins
    /// </summary>
    /// <returns>
    /// Dictionary of plugins where Key is name of plugin and Value is Plugin Instance
    /// </returns>
    public Dictionary<string, IPlugin> GetPlugins()
    {
        return Plugins;
    }

    /// <summary>
    /// Get List of Plugins
    /// </summary>
    /// <returns>
    /// Dictionary of plugins where Key is name of plugin and Value is Plugin Instance
    /// </returns>
    public Dictionary<string, IPlugin> GetPluginsDict()
    {
        return GetPlugins();
    }

    /// <summary>
    /// Get List of Plugins
    /// </summary>
    /// <returns>
    /// List of Plugin Instances
    /// </returns>
    public List<IPlugin> GetPluginsList()
    {
        return GetPlugins().Values.ToList();
    }
    
    /// <summary>
    /// Check if any plugins are available
    /// </summary>
    /// <returns>
    /// True if any plugins are available, False if not
    /// </returns>
    public bool ArePluginsAvailable()
    {
        return Plugins.Count > 0;
    }

    #endregion
    
    #region IPlugin - Metadata

    public PluginInformationRecord GetPluginMetadata()
    {
        throw new UnsupportedPluginManagerFunctionException(
            "IPlugin::GetPluginMetadata() without arguments is not supported by Plugin Manager"
        );
    }

    public PluginInformationRecord PluginMetadata
    {
        get => GetPluginMetadata();
        // ReSharper disable once ValueParameterNotUsed
        // Ignore this warning because this function just throws exception
        set => GetPluginMetadata();
    }

    public PluginInformationRecord? GetPluginMetadata(string pluginName)
    {
        return GetPlugins()
            .TryGetValue(pluginName, out var plugin)
            ? plugin.GetPluginMetadata()
            : null;
    }

    #endregion
    
    #region IPlugin - Supported

    public bool IsSupported()
    {
        return GetPlugins().All(pair => pair.Value.IsSupported());
    }
    
    public bool IsSupported(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsSupported();
    }

    public bool IsInstallSupported()
    {
        return GetPlugins().All(pair => pair.Value.IsInstallSupported());
    }

    public bool IsInstallSupported(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsInstallSupported();
    }

    public bool IsSupportedWithInstall()
    {
        return GetPlugins().All(pair => pair.Value.IsSupportedWithInstall());
    }

    public bool IsSupportedWithInstall(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsSupportedWithInstall();
    }
    
    #endregion

    #region IPlugin - IsPackageURI Supported
    
    public bool IsPackageUriSupported(string packageUri)
    {
        return GetPlugins().All(pair => pair.Value.IsPackageUriSupported(packageUri));
    }

    public bool IsPackageUriSupported(PackageUri packageUri)
    {
        return GetPlugins().All(pair => pair.Value.IsPackageUriSupported(packageUri));
    }
    
    public bool IsPackageUriSupported(string pluginName, string packageUri)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsPackageUriSupported(packageUri);
    }

    public bool IsPackageUriSupported(string pluginName, PackageUri packageUri)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsPackageUriSupported(packageUri);
    }
    
    public IPlugin? GetPluginForWhichPackageUriSupported(string packageUri)
    {
        return GetPlugins()
            .Where(pair => pair.Value.IsPackageUriSupported(packageUri))
            .Select(pair => pair.Value)
            .FirstOrDefault(defaultValue: null);
    }

    public IPlugin? GetPluginForWhichPackageUriSupported(PackageUri packageUri)
    {
        return GetPlugins()
            .Where(pair => pair.Value.IsPackageUriSupported(packageUri))
            .Select(pair => pair.Value)
            .FirstOrDefault(defaultValue: null);
    }
    
    #endregion

    #region IPlugin - Check If Package is Installed
    
    public bool IsPackageInstalled(string package)
    {
        return GetPlugins().All(pair => pair.Value.IsPackageInstalled(package));
    }

    public bool IsPackageInstalled(PackageUri package)
    {
        return GetPlugins().All(pair => pair.Value.IsPackageInstalled(package));
    }
    
    public bool IsPackageInstalled(string pluginName, string package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsPackageInstalled(package);
    }

    public bool IsPackageInstalled(string pluginName, PackageUri package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.IsPackageInstalled(package);
    }
    
    #endregion
    
    #region IPlugin - Package Management

    public bool InstallPackage(string package)
    {
        return GetPluginsList().Any(plugin => plugin.InstallPackage(package));
    }

    public bool InstallPackage(IEnumerable<string> package)
    {
        var packageList = new List<string>(package);
        return GetPluginsList().Any(plugin => plugin.InstallPackage(packageList));
    }

    public bool InstallPackage(PackageUri package)
    {
        return GetPluginsList().Any(plugin => plugin.InstallPackage(package));
    }

    public bool InstallPackage(IEnumerable<PackageUri> package)
    {
        var packageList = new List<PackageUri>(package);
        return GetPluginsList().Any(plugin => plugin.InstallPackage(packageList));
    }

    public bool InstallPackage(string pluginName, string package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.InstallPackage(package);
    }

    public bool InstallPackage(string pluginName, IEnumerable<string> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.InstallPackage(package);
    }

    public bool InstallPackage(string pluginName, PackageUri package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.InstallPackage(package);
    }

    public bool InstallPackage(string pluginName, IEnumerable<PackageUri> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.InstallPackage(package);
    }

    public bool UpdatePackage(string package)
    {
        return GetPluginsList().Any(plugin => plugin.UpdatePackage(package));
    }

    public bool UpdatePackage(IEnumerable<string> package)
    {
        var packageList = new List<string>(package);
        return GetPluginsList().Any(plugin => plugin.UpdatePackage(packageList));
    }

    public bool UpdatePackage(PackageUri package)
    {
        return GetPluginsList().Any(plugin => plugin.UpdatePackage(package));
    }

    public bool UpdatePackage(IEnumerable<PackageUri> package)
    {
        var packageList = new List<PackageUri>(package);
        return GetPluginsList().Any(plugin => plugin.UpdatePackage(packageList));
    }

    public bool UpdatePackage(string pluginName, string package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdatePackage(package);
    }

    public bool UpdatePackage(string pluginName, IEnumerable<string> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdatePackage(package);
    }

    public bool UpdatePackage(string pluginName, PackageUri package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdatePackage(package);
    }

    public bool UpdatePackage(string pluginName, IEnumerable<PackageUri> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdatePackage(package);
    }

    public bool UninstallPackage(string package)
    {
        return GetPluginsList().Any(plugin => plugin.UninstallPackage(package));
    }

    public bool UninstallPackage(IEnumerable<string> package)
    {
        var packageList = new List<string>(package);
        return GetPluginsList().Any(plugin => plugin.UninstallPackage(packageList));
    }

    public bool UninstallPackage(PackageUri package)
    {
        return GetPluginsList().Any(plugin => plugin.UninstallPackage(package));
    }

    public bool UninstallPackage(IEnumerable<PackageUri> package)
    {
        var packageList = new List<PackageUri>(package);
        return GetPluginsList().Any(plugin => plugin.UninstallPackage(packageList));
    }
    
    public bool UninstallPackage(string pluginName, string package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UninstallPackage(package);
    }

    public bool UninstallPackage(string pluginName, IEnumerable<string> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UninstallPackage(package);
    }

    public bool UninstallPackage(string pluginName, PackageUri package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UninstallPackage(package);
    }

    public bool UninstallPackage(string pluginName, IEnumerable<PackageUri> package)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UninstallPackage(package);
    }

    public bool UpdatePackageDatabase()
    {
        return UpdatePackageDatabase(all: false);
    }

    public bool UpdatePackageDatabase(bool all)
    {
        return all
            ? GetPlugins().All(pair => pair.Value.UpdatePackageDatabase())
            : GetPlugins().Any(plugin => plugin.Value.UpdatePackageDatabase());
    }
    
    public bool UpdatePackageDatabase(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdatePackageDatabase();
    }

    public bool AddPackageRepositoryToDatabase(string repository)
    {
        return GetPlugins().Any(plugin => plugin.Value.AddPackageRepositoryToDatabase(repository));
    }

    public bool AddPackageRepositoryToDatabase(string pluginName, string repository)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.AddPackageRepositoryToDatabase(repository);
    }

    public bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return GetPlugins().Any(plugin => plugin.Value.RemovePackageRepositoryFromDatabase(repository));
    }

    public bool RemovePackageRepositoryFromDatabase(string pluginName, string repository)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.RemovePackageRepositoryFromDatabase(repository);
    }
    
    #endregion
    
    #region IPlugin - Mass Update

    public bool UpdateAllCurrentPackages()
    {
        return GetPlugins().All(plugin => plugin.Value.UpdateAllCurrentPackages());
    }
    
    public bool UpdateAllCurrentPackages(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.UpdateAllCurrentPackages();
    }
    
    #endregion

    #region IPlugin - Package Manager Installation
    
    public bool InstallPackageManager()
    {
        return GetPlugins().All(plugin => plugin.Value.InstallPackageManager());
    }
    
    public bool InstallPackageManager(string pluginName)
    {
        return GetPlugins().TryGetValue(pluginName, out var plugin) && plugin.InstallPackageManager();
    }
    
    #endregion
}

public class UnsupportedPluginManagerFunctionException: Exception
{
    public UnsupportedPluginManagerFunctionException(string message): base(message)
    {
    }
}