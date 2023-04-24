namespace PAiPWebPackageManager.PluginBase;

public abstract class PluginBase: IPlugin
{
    public abstract bool IsSupported();

    public abstract bool IsInstallSupported();

    public bool IsSupportedWithInstall()
    {
        return IsSupported() || IsInstallSupported();
    }

    public bool IsPackageUriSupported(string packageUri)
    {
        return IsPackageUriSupported(new PackageUri(packageUri));
    }

    public abstract bool IsPackageUriSupported(PackageUri packageUri);

    public bool IsPackageInstalled(string package)
    {
        return IsPackageInstalled(new PackageUri(package));
    }

    public abstract bool IsPackageInstalled(PackageUri package);

    public bool InstallPackage(string package)
    {
        return InstallPackage(new PackageUri(package));
    }

    public bool InstallPackage(IEnumerable<string> package)
    {
        return InstallPackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool InstallPackage(PackageUri package);

    public bool InstallPackage(IEnumerable<PackageUri> package)
    {
        return package.All(InstallPackage);
    }

    public bool UpdatePackage(string package)
    {
        return UpdatePackage(new PackageUri(package));
    }

    public bool UpdatePackage(IEnumerable<string> package)
    {
        return UpdatePackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool UpdatePackage(PackageUri package);

    public bool UpdatePackage(IEnumerable<PackageUri> package)
    {
        return package.All(InstallPackage);
    }

    public bool UninstallPackage(string package)
    {
        return UninstallPackage(new PackageUri(package));
    }

    public bool UninstallPackage(IEnumerable<string> package)
    {
        return UninstallPackage(
            new List<string>(package)
                .ConvertAll(
                    (packageName => new PackageUri(packageName))
                )
        );
    }

    public abstract bool UninstallPackage(PackageUri package);

    public bool UninstallPackage(IEnumerable<PackageUri> package)
    {
        return package.All(UninstallPackage);
    }

    public abstract bool UpdatePackageDatabase();

    public abstract bool AddPackageRepositoryToDatabase(string repository);

    public abstract bool RemovePackageRepositoryToDatabase(string repository);

    public abstract bool UpdateAllCurrentPackages();

    public abstract bool InstallPackageManager();
}