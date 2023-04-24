using System.Runtime.InteropServices;
using Spectre.Console;

namespace PAiPWebPackageManager.Lib;

public static class UnixUserTools
{
    private static uint UnsupportedPlatformWarning()
    {
        AnsiConsole.MarkupLineInterpolated($"[red]Error: You tried to run UnixUserTools on a platform that is not supported.[/]");
        return uint.MaxValue;
    }
    
    [DllImport("libc")]
    private static extern uint getuid();
    [DllImport("libc")]
    private static extern uint getgid();
    
    public static uint GetUserId()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return getuid();
        }
        return UnsupportedPlatformWarning();
    }
    public static uint GetGroupId()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return getgid();
        }
        return UnsupportedPlatformWarning();
    }
}