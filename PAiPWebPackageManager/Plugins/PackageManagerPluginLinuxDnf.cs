using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxDnf: PluginBaseClass
{
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Dnf",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"dnf"}),
        SupportedPlatforms = { OSPlatform.Linux },
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return false;
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "dnf");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"dnf list installed {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"dnf install -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"dnf upgrade -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"dnf remove -y {packageName.GetPackage()}"),
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
        var result = ExecuteCommand($"dnf config-manager --add-repo {repository}");

        if (result?.HasExited == true && result?.ExitCode == 1)
        {
            AnsiConsole.WriteLine(
                "[red]Adding repository failed. Make sure that you installed: `dnf install 'dnf-command(config-manager)'`[/red]"
            );
        }
        
        return CheckExitCode(
            result,
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        var result = ExecuteCommand($"dnf config-manager --set-disabled {repository}");
        if (result?.HasExited == true && result?.ExitCode == 1)
        {
            AnsiConsole.WriteLine(
                "[red]Removing repository failed. Make sure that you installed: `dnf install 'dnf-command(config-manager)'`[/red]"
            );
        }
        var resultBool = CheckExitCode(
            result,
            new[] { 0 }
        );
        
        if (resultBool)
        {
            AnsiConsole.MarkupLineInterpolated(
                $"[red]Removing repository works as just disabling repository. If you want to completely remove it. Remove this file: /etc/yum.repos.d/{repository}.repo[/red]"
            );
        }

        return resultBool;
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("dnf upgrade -y"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        throw new NotSupportedException("This package manager does not support installation.");
    }
}