using System.CommandLine.Invocation;

namespace PAiPWebPackageManager.Lib;

public enum PwpmExitCode
{
    UnhandledError = -1,
    Success = 0,
    Failure = 1,
}

public static class CliUtils
{
    public static int GetExitCode(PwpmExitCode exitCodeEnum)
    {
        return (int)exitCodeEnum;
    }
    
    public static void SetExitCode(InvocationContext invCtx, PwpmExitCode exitCode)
    {
        invCtx.ExitCode = GetExitCode(exitCode);
    }
}