using System.CommandLine.Invocation;

namespace PAiPWebPackageManager.Lib;

public enum PwpmExitCode
{
    Success = 0,
    Failure = 1
}

public static class CliUtils
{
    public static void SetExitCode(InvocationContext invCtx, PwpmExitCode exitCode)
    {
        invCtx.ExitCode = (int)exitCode;
    }
}