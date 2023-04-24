using System.Runtime.InteropServices;
using Spectre.Console;

namespace PAiPWebPackageManager.Command;

public enum ShellType
{
    #region Null Shell
    /// <summary>
    /// Invalid Shell / Null Shell
    /// </summary>
    Null = 0,
    #endregion
    #region Cross Platform Shells
    /// <summary>
    /// Cross platform PowerShell
    /// </summary>
    PowerShell = 1_000,
    /// <summary>
    /// Windows PowerShell
    /// </summary>
    WindowsPowerShell = 1_001,
    /// <summary>
    /// NuShell
    /// </summary>
    NuShell = 1_100,
    #endregion
    #region Shells
    /// <summary>
    /// Windows CMD
    /// </summary>
    Cmd = 2_000,
    /// <summary>
    /// Bash
    /// </summary>
    Bash = 2_100,
    /// <summary>
    /// ZSH
    /// </summary>
    Zsh = 2_200,
    /// <summary>
    /// Sh
    /// </summary>
    Sh = 2_300,
    #endregion
}

public static class PlatformShell
{
    /// <summary>
    /// Shell that is used for commands
    /// </summary>
    private static string _basicShell = "";
    /// <summary>
    /// Shell Type that is used for commands
    /// </summary>
    private static ShellType _basicShellType = ShellType.Null;
    
    /// <summary>
    /// Get Basic Shell
    /// </summary>
    /// <returns>
    /// Current System Basic Shell
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown if platform is not supported
    /// </exception>
    public static (string shellName, ShellType shellType) GetBasicShell()
    {
        if (!string.IsNullOrEmpty(_basicShell))
        {
            return (_basicShell, _basicShellType);
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows
            
            // Prefer PowerShell Core
            if (Executor.IsCommandAvailable("pwsh.exe"))
            {
                _basicShell = "pwsh.exe";
                _basicShellType = ShellType.WindowsPowerShell;
                return (_basicShell, _basicShellType);
            }
            
            // Fallback to CMD
            _basicShell = "cmd.exe";
            _basicShellType = ShellType.Cmd;
            return (_basicShell, _basicShellType);
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux
            
            // Prefer PowerShell Core
            if (Executor.IsCommandAvailable("pwsh"))
            {
                _basicShell = "pwsh";
                _basicShellType = ShellType.PowerShell;
                return (_basicShell, _basicShellType);
            }
            
            // Then Prefer Bash
            if (Executor.IsCommandAvailable("bash"))
            {
                _basicShell = "bash";
                _basicShellType = ShellType.Bash;
                return (_basicShell, _basicShellType);
            }
            // Then Prefer Zsh
            if (Executor.IsCommandAvailable("zsh"))
            {
                _basicShell = "zsh";
                _basicShellType = ShellType.Zsh;
                return (_basicShell, _basicShellType);
            }
            // Fallback to SH
            _basicShell = "sh";
            _basicShellType = ShellType.Sh;
            return (_basicShell, _basicShellType);
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // Mac OS X
            
            // Prefer PowerShell Core
            if (Executor.IsCommandAvailable("pwsh"))
            {
                _basicShell = "pwsh";
                _basicShellType = ShellType.PowerShell;
                return (_basicShell, _basicShellType);
            }
            
            // Prefer Bash
            if (Executor.IsCommandAvailable("bash"))
            {
                _basicShell = "bash";
                _basicShellType = ShellType.Bash;
                return (_basicShell, _basicShellType);
            }
            
            // Prefer Zsh
            if (Executor.IsCommandAvailable("zsh"))
            {
                _basicShell = "zsh";
                _basicShellType = ShellType.Zsh;
                return (_basicShell, _basicShellType);
            }
            
            // Fallback to ZSH or SH
            _basicShell = "sh";
            _basicShellType = ShellType.Sh;
            return (_basicShell, _basicShellType);
        }
        
        AnsiConsole.MarkupLineInterpolated($"[red]Error: Unsupported OS Platform[/]");
        throw new PlatformNotSupportedException("Unsupported OS Platform");
    }

    /// <summary>
    /// Get Basic Shell Command
    /// </summary>
    /// <returns>
    /// Basic Shell Command
    /// </returns>
    public static string GetBasicShellCommand()
    {
        return GetBasicShell().shellName;
    }

    /// <summary>
    /// Get Basic Shell Type
    /// </summary>
    /// <returns>
    /// Basic Shell Type
    /// </returns>
    public static ShellType GetBasicShellType()
    {
        return GetBasicShell().shellType;
    }

    /// <summary>
    /// Get Basic Shell Arguments
    /// </summary>
    /// <returns>
    /// Get Basic Shell Arguments to execute a command
    /// </returns>
    public static string GetBasicShellArguments()
    {
        // Interactive Mode
        return _basicShellType switch
        {
            ShellType.PowerShell => "-NoProfile",
            ShellType.WindowsPowerShell => "-NoProfile",
            ShellType.NuShell => "",
            ShellType.Cmd => "",
            ShellType.Bash => "",
            ShellType.Zsh => "",
            ShellType.Sh => "",
            ShellType.Null => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        // Command Mode
        // return _basicShellType switch
        // {
        //     ShellType.PowerShell => "-c",
        //     ShellType.WindowsPowerShell => "-c",
        //     ShellType.NuShell => "-c",
        //     ShellType.Cmd => "/c",
        //     ShellType.Bash => "-c",
        //     ShellType.Zsh => "-c",
        //     ShellType.Sh => "-c",
        //     ShellType.Null => throw new NotSupportedException(),
        //     _ => throw new ArgumentOutOfRangeException()
        // };
    }

    /// <summary>
    /// Get Executable Extension
    /// </summary>
    /// <returns>Current Platform Executable Extension</returns>
    public static string ExecutableExtension()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
    }
}