using System.Runtime.InteropServices;
using PAiPWebPackageManager.PluginBase;

namespace PAiPWebPackageManager.Plugins;

public class PackageManagerPluginLinuxApk: PluginBaseClass
{
    protected new PluginInformationRecord PluginMetadata = new PluginInformationRecord()
    {
        PluginCategoryType = PluginCategoryEnum.Operating_System_x_Linux,
        PluginName = "Apk",
        DoesNotWorkWithAdmin = false,
        IsAdminNeeded = true,
        RequiredCommands = new List<string>(new []{"apk"}),
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
        return CheckIfPackageManagerUriIsSupported(packageUri, "apk");
    }

    public override bool IsPackageInstalled(PackageUri package)
    {
        return CheckExitCode(
            ExecuteCommand($"apk info -e {package.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool InstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"apk add {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"apk upgrade {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UninstallPackage(PackageUri packageName)
    {
        return CheckExitCode(
            ExecuteCommand($"apk del {packageName.GetPackage()}"),
            new[] { 0 }
        );
    }

    public override bool UpdatePackageDatabase()
    {
        return CheckExitCode(
            ExecuteCommand("apk update"),
            new[] { 0 }
        );
    }

    public override bool UpdateAllCurrentPackages()
    {
        return CheckExitCode(
            ExecuteCommand("apk upgrade"),
            new[] { 0 }
        );
    }

    public override bool AddPackageRepositoryToDatabase(string repository)
    {
        var file = File.Open("/etc/apk/repositories", FileMode.Append);
        if (!file.CanWrite)
        {
            return false;
        }
        var writer = new StreamWriter(file);
        writer.WriteLine(repository);
        writer.Close();
        file.Close();
        return true;
    }

    public override bool RemovePackageRepositoryFromDatabase(string repository)
    {
        var file = File.Open("/etc/apk/repositories", FileMode.Open);
        if (!file.CanWrite)
        {
            return false;
        }
        var reader = new StreamReader(file);
        var writer = new StreamWriter(file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == repository)
            {
                continue;
            }
            writer.WriteLine(line);
        }
        writer.Close();
        reader.Close();
        file.Close();
        return true;
    }

    public override bool InstallPackageManager()
    {
        throw new NotSupportedException("This package manager does not support installation.");
    }
}