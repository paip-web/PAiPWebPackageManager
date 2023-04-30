using System.Runtime.InteropServices;
using PAiPWebPackageManager.Command;
using PAiPWebPackageManager.Lib;

namespace PAiPWebPackageManager.PluginBase;

// ReSharper disable InconsistentNaming
public enum PluginCategoryEnum
{
    Null,
    // OS
    Operating_System_x_Windows,
    Operating_System_x_Linux,
    Operating_System_x_MacOS,
    Operating_System_x_LinuxAndMacOS,
    // General Package Managers
    General_Package_Manager,
    // Programming Language Version Manager
    Programming_Language_Version_Manager,
    // Programming Language Package Managers
    Programming_Language_Package_Manager,
    // Cross Platform Package Managers
    Cross_Platform_Package_Manager,
    // Basic Git Manager
    Basic_Git_Package_Manager,
    // Shell Package Managers
    Shell_Package_Manager,
    // PAiP Web Package Manager
    PAiP_Web_Package_Manager,
}
// ReSharper enable InconsistentNaming

public record PluginInformationRecord
{
    /// <summary>
    /// Is Administrator Access needed for this plugin to work?
    /// </summary>
    public bool IsAdminNeeded = false;
    /// <summary>
    /// If true this plugin can't work on Administrator at all
    /// </summary>
    public bool DoesNotWorkWithAdmin = false;
    /// <summary>
    /// Is Windows Subsystem for Linux supported?
    /// </summary>
    public bool IsWslSupported = true;
    /// <summary>
    /// Supported Platforms of this plugin
    /// </summary>
    public List<OSPlatform> SupportedPlatforms = new List<OSPlatform>();
    /// <summary>
    /// Required Commands for this plugin to work
    /// </summary>
    public List<string> RequiredCommands = new List<string>();
    /// <summary>
    /// Category of this plugin
    /// </summary>
    public PluginCategoryEnum PluginCategoryType = PluginCategoryEnum.Null;
    /// <summary>
    /// Name of this plugin
    /// </summary>
    public string PluginName = "";

    #region Metadata Additional Fields
    public string PluginCategory => Enum
        .GetName(PluginCategoryType)
        ?.Replace("_x_", " - ")
        .Replace("_", " ") ?? "";
    public string PluginDescription =>
        string.IsNullOrEmpty(PluginCategory)
            ? PluginName
            : $"{PluginCategory} - {PluginName}";
    #endregion

    public static (string command, bool error) RequireOneOfTheseCommands(IEnumerable<string> commands)
    {
        var cmds = new List<string>(commands);
        if (cmds.Count < 1)
        {
            throw new ArgumentException("Commands is empty");
        }
        
        var availableCommands = cmds.Where(Executor.IsCommandAvailable).ToArray();
        return availableCommands.Length == 0 ? (cmds.First(), true) : (availableCommands.First(), false);
    }

    public bool CheckRequirements(
        bool ignoreCommands = false,
        bool ignoreNotWorkingWithAdmin = false
    )
    {
        var enableDebugLogMessages = false;
        ConsoleUtils.DebugLog(enableDebugLogMessages, $"Checking requirements for plugin: {PluginName}");
        if (!IsWslSupported && Executor.IsWsl())
        {
            ConsoleUtils.DebugLog(enableDebugLogMessages, "WSL is not supported");
            // WSL is not supported
            return false;
        }
        
        if (IsAdminNeeded && Executor.IsAdmin() == false)
        {
            ConsoleUtils.DebugLog(enableDebugLogMessages, "Administrator is needed");
            // Administrator is needed
            return false;
        }

        if (ignoreNotWorkingWithAdmin == false && DoesNotWorkWithAdmin && Executor.IsAdmin())
        {
            ConsoleUtils.DebugLog(enableDebugLogMessages, "Can't work with admin");
            // Can't work with admin
            return false;
        }
        
        if (SupportedPlatforms.Any(RuntimeInformation.IsOSPlatform) == false)
        {
            ConsoleUtils.DebugLog(enableDebugLogMessages, "Not supported platform");
            // Not supported platform
            return false;
        }

        if (ignoreCommands == false && RequiredCommands.TrueForAll(Executor.IsCommandAvailable) == false)
        {
            ConsoleUtils.DebugLog(enableDebugLogMessages, "System don't have required commands");
            // System don't have required commands
            return false;
        }
        
        ConsoleUtils.DebugLog(enableDebugLogMessages, $"Requirements for plugin: {PluginName} are ok");

        return true;
    }
};