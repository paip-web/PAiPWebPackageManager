using PAiPWebPackageManager.PluginBase;
using PAiPWebPackageManager.Plugins;

namespace PAiPWebPackageManager;

public static class PluginsList
{
    /// <summary>
    /// List of all the plugins
    /// </summary>
    /// <returns>List of plugins</returns>
    public static List<IPlugin> GetAllPlugins()
    {
        return new List<IPlugin>(new IPlugin[]
        {
            new PackageManagerPluginBrew(),
            new PackageManagerPluginLinuxApk(),
            new PackageManagerPluginLinuxDnf(),
            new PackageManagerPluginLinuxFlatpak(),
            new PackageManagerPluginLinuxPacman(),
            new PackageManagerPluginLinuxPacstall(),
            new PackageManagerPluginLinuxSnap(),
            new PackageManagerPluginLinuxYum(),
            new PackageManagerPluginMacOSBrewCask(),
            new PackageManagerPluginMacOSMas(),
            new PackageManagerPluginSpecialNix(),
            new PackageManagerPluginWindowsChocolatey(),
            new PackageManagerPluginWindowsScoop(),
            new PackageManagerPluginWindowsWinGet(),
        });
    }
}