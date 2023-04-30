using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxApt: PluginBaseClass
{
    private string _packageManagerCommand = "apt-get";
    
    public override PluginInformationRecord PluginMetadata { get; set; } = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Apt",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"dpkg", "add-apt-repository"}),
        SupportedPlatforms = { OSPlatform.Linux },
    };

    public override bool IsSupported()
    {
        if (!CheckBasicRequirements())
        {
            return false;
        }

        var packageManagerCommandTuple = PluginInformationRecord.RequireOneOfTheseCommands(
            new[] { "nala", "apt", "apt-get" }
        );

        if (packageManagerCommandTuple.error)
        {
            return false;
        }
        
        _packageManagerCommand = packageManagerCommandTuple.command;
        return true;
    }

    public override bool IsInstallSupported()
    {
        return false;
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckAllBoolRequirements(new bool?[] {
            CheckIfPackageManagerUriIsSupported(packageUri, "apt"),
            CheckIfPackageManagerUriIsSupported(packageUri, "nala"),
        });
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"dpkg --verify {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} install -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} upgrade -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} remove -y {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} update -y"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"add-apt-repository {repository}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"add-apt-repository --remove {repository}"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} update -y"),
            new[] { 0 }
        ) && CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} upgrade -y"),
            new[] { 0 }
        ) && CheckExitCode(
            ExecuteCommand($"{_packageManagerCommand} autoremove -y"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        throw new NotSupportedException("This package manager does not support installation.");
    }
}