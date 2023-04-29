namespace PAiPWebPackageManager;

public record PackageManagementInvocationContext
{
    public bool SupportInstallationOfPm { get; init; } = false;
    public string? PackageManagerToUse { get; init; } = null;

    public PackageManagementInvocationContext()
    {
    }
    
    public PackageManagementInvocationContext(bool supportInstallationOfPm, string? packageManagerToUse)
    {
        SupportInstallationOfPm = supportInstallationOfPm;
        PackageManagerToUse = packageManagerToUse;
    }
}