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
        // Get all assemblies that could contain plugins
        var assemblies = new List<System.Reflection.Assembly>(new[] {
            System.Reflection.Assembly.GetExecutingAssembly(),
        });
        return assemblies
            // Get all types from all assemblies
            .SelectMany(assembly => assembly.GetTypes())
            // Ignore Base Class
            .Where(t => t != typeof(PluginBaseClass))
            // Only accept types assignable to base class
            .Where(t => typeof(PluginBaseClass).IsAssignableFrom(t))
            // Don't accept abstract classes
            .Where(t => !t.IsAbstract)
            // Only accept types with a parameterless constructor
            .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            // Create an instance of each type and add it as plugin
            .Select(Activator.CreateInstance)
            // Ignore nulls
            .Where(plugin => plugin is not null)
            // Cast to class instance PluginBaseClass
            .Cast<PluginBaseClass>()
            // Cast to interface IPlugin
            .Cast<IPlugin>()
            // Convert to list
            .ToList();
    }
}