using PAiPWebPackageManager.Lib;
using Spectre.Console;

namespace PAiPWebPackageManager;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            ConsoleUtils.PrintAppBanner();
            return PackageManagerCli.Run(args);
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return CliUtils.GetExitCode(PwpmExitCode.UnhandledError);
        }
    }
}
