using PAiPWebPackageManager.Command;
using PAiPWebPackageManager.Lib;

namespace PAiPWebPackageManager.SpecialScripts;

using CliU = CliUtils;
using Con = ConsoleUtils;
using Exe = Executor;

public static class SpecialScripts
{
    #region Script Index
    private static readonly Dictionary<string, Func<int>> Scripts = new Dictionary<string, Func<int>>()
    {
        { "ForceInstallDependenciesOfBrew", ForceInstallDependenciesOfBrew },
        { "ForceInstallDependenciesOfApt", ForceInstallDependenciesOfApt },
        { "ForceInstallDependenciesOfNixOnly", ForceInstallDependenciesOfNixOnly },
        { "ForceInstallDependenciesOfNixWithMakingGroup", ForceInstallDependenciesOfNixWithMakingGroup },
    };
    #endregion
    #region Script Runner
    public static int RunScript(string scriptName)
    {
        if (Scripts.TryGetValue(scriptName, out var script))
        {
            return script();
        }

        Con.PrintError("Script not found");
        return CliU.GetExitCode(PwpmExitCode.Failure);
    }
    
    public static string[] GetScriptNames()
    {
        return Scripts.Keys.ToArray();
    }
    #endregion
    
    #region Script List

    /// <summary>
    /// Script to install dependencies of brew
    ///
    /// Note: This script currently only uses apt
    /// Note: This script is just installing curl and git
    ///     (which are dependencies that brew needs to support installation)
    /// </summary>
    /// <returns>Exit Code</returns>
    private static int ForceInstallDependenciesOfBrew()
    {
        if (Exe.IsCommandAvailable("apt"))
        {
            if (!Exe.CheckExitCode(
                Exe.ExecuteCommandCrossPlatform("apt-get update"),
                new[] { 0 }
            ))
            {
                Con.PrintError("Failed to update using apt-get");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            if (!Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("apt-get install curl git -y"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to install git and curl using apt");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }

            return CliU.GetExitCode(PwpmExitCode.Success);
        }

        Con.PrintError("Failed to install dependencies");
        return CliU.GetExitCode(PwpmExitCode.Failure);
    }
    
    /// <summary>
    /// Script to install dependencies of apt
    ///
    /// Note: This script is just installing software-properties-common which contains add-apt-repository command
    ///     required for apt plugin to work
    /// </summary>
    /// <returns>Exit Code</returns>
    private static int ForceInstallDependenciesOfApt()
    {
        if (Exe.IsCommandAvailable("apt"))
        {
            if (!Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("apt-get update"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to update using apt-get");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            if (!Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("apt-get install software-properties-common -y"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to install software-properties-common using apt-get");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }

            return CliU.GetExitCode(PwpmExitCode.Success);
        }

        Con.PrintError("Failed to install dependencies");
        return CliU.GetExitCode(PwpmExitCode.Failure);
    }

    private static int ForceInstallDependenciesOfNixOnly()
    {
        return ForceInstallDependenciesOfNix(true);
    }
    
    private static int ForceInstallDependenciesOfNixWithMakingGroup()
    {
        return ForceInstallDependenciesOfNix(false);
    }

    /// <summary>
    /// Script to install dependencies of nix
    ///
    /// Note: This script is just installing sudo which is required for nix to create it's directory
    /// Note: This script is also trying to create a user named nixbld and a group named nixbld
    /// it only tries to do it if parameter ignoreGroup is false
    /// </summary>
    /// <param name="ignoreGroup">
    /// True if you want to ignore group creation and user creation
    /// </param>
    /// <returns>Exit Code</returns>
    private static int ForceInstallDependenciesOfNix(bool ignoreGroup = false)
    {
        if (Exe.IsCommandAvailable("apt"))
        {
            if (!Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("apt-get update"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to update using apt-get");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            if (!Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("apt-get install sudo -y"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to install sudo using apt-get");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            
            if (!ignoreGroup && !Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("useradd nixbld"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to add nixbld user");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            
            if (!ignoreGroup && !Exe.CheckExitCode(
                    Exe.ExecuteCommandCrossPlatform("gpasswd -a nixbld nixbld"),
                    new[] { 0 }
                ))
            {
                Con.PrintError("Failed to add nixbld user to nixbld group");
                return CliU.GetExitCode(PwpmExitCode.Failure);
            }
            
            return CliU.GetExitCode(PwpmExitCode.Success);
        }

        Con.PrintError("Failed to install dependencies");
        return CliU.GetExitCode(PwpmExitCode.Failure);
    }
    
    #endregion
}