using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxPacstall: PluginBaseClass
{
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "PacStall",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"pacstall"}),
        SupportedPlatforms = { OSPlatform.Linux },
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true)
               && IsCommandAvailable("curl")
               && IsCommandAvailable("bash");
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "pacstall");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"pacstall -Qi {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacstall -I {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacstall -I {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacstall -R {packageName.GetPackage()}"),
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
        return CheckExitCode(
            ExecuteCommand($"pacstall -A {repository}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        throw new NotSupportedException("This package manager does not support removing repositories.");
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("pacstall -Up"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        return CheckExitCode(
            ExecuteCommand(
                "/bin/bash -c \"$(curl -fsSL https://git.io/JsADh || wget -q https://git.io/JsADh -O -)\""
            ),
            new[] { 0 }
        );
    }
}