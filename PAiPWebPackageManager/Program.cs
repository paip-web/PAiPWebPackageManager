using PAiPWebPackageManager.Lib;

namespace PAiPWebPackageManager;

public class Program
{
    public static int Main(string[] args)
    {
        ConsoleUtils.PrintAppBanner();
        return PackageManagerCli.Run(args);
    }
}
