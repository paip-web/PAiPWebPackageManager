using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginWindowsChocolatey: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Windows,
        PluginName = "Chocolatey",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"choco"}),
        SupportedPlatforms = { OSPlatform.Windows },
    };

    private bool EnsureThatChocoFeatureEnchancedExitCodesIsEnabled()
    {
        // This Makes sure that useEnhancedExitCodes is enabled
        // This makes option to check if package is installed possible
        // With this enabled it's 0 if it's installed and 2 if it's not
        // Without it's just 0 in both cases
        return CheckExitCode(
            ExecuteCommand("choco feature enable -y --name=\"useEnhancedExitCodes\""),
            new[] { 0 }
        );
    }

    public override bool IsSupported()
    {
        return CheckBasicRequirements() && EnsureThatChocoFeatureEnchancedExitCodesIsEnabled();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true);
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "choco");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"choco info -lry {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"choco install -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"choco upgrade -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"choco uninstall -y {packageName.GetPackage()}"),
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
        var repositorySplit = repository.Split(" ");
        if (repositorySplit.Length < 2)
        {
            AnsiConsole.WriteLine("[red]Chocolatey repository have to have format: `name repository_url`[/red]");
            return false;
        }
        
        var repositoryName = repositorySplit[0];
        var repositoryUrl = repositorySplit[1];
        
        return CheckExitCode(
            ExecuteCommand($"choco source add -n=\"{repositoryName}\" -s=\"{repositoryUrl}\""),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"choco source remove -n=\"{repository}\""),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("choco upgrade -y all"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        return CheckExitCode(
            ExecuteCommand("powershell.exe -NoProfile -ExecutionPolicy Bypass -Command " +
                           "\"" +
                           "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072;" +
                           "iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))" +
                           "\""),
            new[] { 0 }
        );
    }
}