using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginSpecialNix: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_LinuxAndMacOS,
        PluginName = "Nix",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"nix-env", "nix-channel"}),
        SupportedPlatforms = { OSPlatform.Linux, OSPlatform.OSX },
    };
    
    public override bool IsSupported()
    {
        return CheckBasicRequirements();
    }

    public override bool IsInstallSupported()
    {
        return CheckBasicRequirements(ignoreCommands: true)
               && IsCommandAvailable("curl")
               && IsCommandAvailable("sh");
    }

    public override bool IsPackageUriSupported(PackageUri packageUri)
    {
        return CheckIfPackageManagerUriIsSupported(packageUri, "nix");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"nix-env -q {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"nix-env -iA {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"nix-env -uA {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"nix-env --uninstall {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand("nix-channel --update"),
            new[] { 0 }
        );
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
            ExecuteCommand($"nix-channel --add {repositoryUrl} {repositoryName}"),
            new[] { 0 }
        );
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        return CheckExitCode(
            ExecuteCommand($"nix-channel --remove {repository}"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("nix-channel --update"),
            new[] { 0 }
        ) && CheckExitCode(
            ExecuteCommand("nix-env -u '*'"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        return CheckExitCode(
            ExecuteCommand("curl -L https://nixos.org/nix/install | sh"),
            new[] { 0 }
        );
    }
}