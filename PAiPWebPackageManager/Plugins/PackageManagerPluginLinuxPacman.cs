using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;
using Spectre.Console;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxPacman: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Pacman",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"pacman"}),
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
        return CheckIfPackageManagerUriIsSupported(packageUri, "pacman");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -Qi {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -Syu {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -Syu {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -R {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -Syy"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        if (repository.Split(" ").Length < 2)
        {
            AnsiConsole.WriteLine("[red]Pacman repository have to have format: `name repository_urls...`[/red]");
            return false;
        }

        var input = repository.Split(" ");
        var name = input[0];
        var urls = input[1..];
        
        using var file = File.Open("/etc/pacman.conf", FileMode.Append);
        if (!file.CanWrite)
        {
            return false;
        }
        using var writer = new StreamWriter(file);
        
        writer.WriteLine($"[{name}]");
        foreach (var url in urls)
        {
            writer.WriteLine($"Server = {url}");
        }
        writer.WriteLine();

        AnsiConsole.WriteLine("[red]If pacman repository need some specific GPG keys you have to install them yourself.[/red]");
        
        return UpdatePackageDatabase();
    }
    
    private static bool CheckIfLineIsNextRepository(string line)
    {
        return CheckAllBoolRequirements(new bool?[] {
            // New Section (repository)
            line.StartsWith("["),
            // Commented Repository
            line.StartsWith("#["),
        });
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        using var file = File.Open("/etc/pacman.conf", FileMode.Open);
        if (!file.CanWrite)
        {
            return false;
        }
        using var reader = new StreamReader(file);
        using var writer = new StreamWriter(file);
        var repositoryFound = false;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (repositoryFound && line is not null && !CheckIfLineIsNextRepository(line))
            {
                continue;
            }
            
            if (!repositoryFound && line == $"[{repository}]")
            {
                repositoryFound = true;
                continue;
            }
            
            writer.WriteLine(line);
        }
        return true;
    }
    
    /// <summary>
    /// API for installing GPG Keys into Pacman
    /// </summary>
    /// <param name="keys">GPG Keys to install</param>
    /// <returns>
    /// True if all keys were installed successfully
    /// False if at least one key was not installed successfully
    /// </returns>
    public bool InstallPackageRepositoryKeys(IEnumerable<string> keys)
    {
        return keys.All(InstallPackageRepositoryKeys);
    }

    /// <summary>
    /// API for installing GPG Keys into Pacman
    /// </summary>
    /// <param name="key">GPG Key to install</param>
    /// <returns>
    /// True if key was installed successfully
    /// </returns>
    public bool InstallPackageRepositoryKeys(string key)
    {
        return CheckAllBoolRequirements(new bool?[]
        {
            // import key
            CheckExitCode(
                ExecuteCommand($"pacman-key --recv-keys {key}"),
                new[] { 0 }
            ),
            // verify fingerprint
            CheckExitCode(
                ExecuteCommand($"pacman-key --finger {key}"),
                new[] { 0 }
            ),
            // sign imported key locally
            CheckExitCode(
                ExecuteCommand($"pacman-key --lsign-key {key}"),
                new[] { 0 }
            ),
        });
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand($"pacman -Syu"),
            new[] { 0 }
        );
    }

    public override bool InstallPackageManager()
    {
        throw new NotSupportedException("This package manager does not support installation.");
    }
}