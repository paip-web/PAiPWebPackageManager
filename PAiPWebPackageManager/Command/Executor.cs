﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using PAiPWebPackageManager.Lib;
using Spectre.Console;

namespace PAiPWebPackageManager.Command;

public static class Executor
{
    /// <summary>
    /// Is Dry Run
    /// </summary>
    private static bool _isDryRun = false;
    
    /// <summary>
    /// Set Dry Run
    /// </summary>
    /// <param name="dryRun">Is it dry run or no</param>
    public static void SetDryRun(bool dryRun)
    {
        _isDryRun = dryRun;
    }
    
    /// <summary>
    /// Check if current user is administrator
    /// </summary>
    /// <returns>
    /// True if user is administrator
    /// False if user is not administrator
    /// False if platform is not supported
    /// </returns>
    public static bool IsAdmin()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Get Current Identity
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            // Get Built In Admin Role
            var adminRole = System.Security.Principal.WindowsBuiltInRole.Administrator;
            // Get Current Principal from Identity
            var identityPrincipal = new System.Security.Principal.WindowsPrincipal(identity);
            // Check if Current Account is In Admin Role
            return identityPrincipal.IsInRole(adminRole);
        }

        if (!(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error: Can't check if user is admin on this platform[/]");
            return false;
        }
        
        // Linux / OS X
        var uid = UnixUserTools.GetUserId();
        var gid = UnixUserTools.GetGroupId();
        
        if (uid == 0 || gid == 0)
        {
            return true;
        } 
        return false;
    }

    /// <summary>
    /// Execute Command as Administrator
    /// </summary>
    /// <param name="command">
    /// Shell to use / Program to execute
    /// </param>
    /// <param name="arguments">
    /// Command to execute
    /// </param>
    /// <param name="dryRun">
    /// Is it dry run?
    /// </param>
    /// <exception cref="Exception"></exception>
    private static Process? ExecuteCommandAsAdmin(string command, string arguments, bool dryRun = false)
    {
        if (dryRun)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]Admin Command Execution Skipped (Dry Run): {command}[/]");
            return null;
        }

        Process cmd = new Process();
        cmd.StartInfo.FileName = command;
        cmd.StartInfo.Arguments = arguments;
        cmd.StartInfo.UseShellExecute = true;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            cmd.StartInfo.Verb = "RunAs";
        }
        else
        {
            if (IsCommandAvailable("sudo"))
            {
                cmd.StartInfo.FileName = $"sudo {command}";
            }
            else
            {
                throw new Exception("Command 'sudo' not found. Please install it and try again.");
            }
        }

        cmd.Start();
        cmd.WaitForExit();
        return cmd;
    }

    /// <summary>
    /// Execute Command
    /// </summary>
    /// <param name="shell">
    /// Shell to use / Program to execute
    /// </param>
    /// <param name="command">
    /// Command to execute
    /// </param>
    /// <param name="arguments">
    /// Arguments to pass to shell
    /// </param>
    /// <param name="dryRun">
    /// Is it dry run?
    /// </param>
    private static Process? ExecuteCommand(string shell, string command, string arguments = "", bool dryRun = false)
    {
        if (dryRun)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]Command Execution Skipped (Dry Run): {command}[/]");
            return null;
        }
        Process cmd = new Process();
        cmd.StartInfo.FileName = shell;
        cmd.StartInfo.Arguments = arguments;
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.RedirectStandardError = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();
        ConsoleUtils.PrintPackageManagerProcessOutput(cmd, $"{shell} {arguments} -> {command}");
        cmd.StandardInput.WriteLine(command);
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        return cmd;
    }
    
    /// <summary>
    /// Is Command Available
    /// </summary>
    /// <param name="command">Command to check</param>
    /// <returns>If command is available</returns>
    public static bool IsCommandAvailable(string command)
    {
        return ShellUtils.IsCommandAvailable(command);
    }
    
    /// <summary>
    /// Execute Command Cross Platform
    /// </summary>
    /// <param name="command">
    /// Command to Execute
    /// </param>
    /// <returns>
    /// Process Object with data about executed command
    /// </returns>
    public static Process? ExecuteCommandCrossPlatform(string command)
    {
        return ExecuteCommand(
            PlatformShell.GetBasicShellCommand(),
            command,
            PlatformShell.GetBasicShellArguments(),
            dryRun: _isDryRun
        );
    }
    
    /// <summary>
    /// Execute Command Cross Platform as Administrator
    /// </summary>
    /// <param name="command">
    /// Command to Execute as Administrator
    /// </param>
    /// <returns>
    /// Process Object with data about executed command
    /// </returns>
    public static Process? ExecuteCommandCrossPlatformAsAdmin(string command)
    {
        if (IsAdmin())
        {
            return ExecuteCommandCrossPlatform(command);
        }
        var escapedCommand = EscapeCommand(command);
        return ExecuteCommandAsAdmin(
            PlatformShell.GetBasicShellCommand(),
            $"{PlatformShell.GetBasicShellArguments()} {escapedCommand}",
            dryRun: _isDryRun
        );
    }

    /// <summary>
    /// Try to escape command for processing
    /// </summary>
    /// <param name="command">
    ///
    /// </param>
    /// <returns></returns>
    public static string EscapeCommand(string command)
    {
        return command.Contains('"') ? $"\'{command}\'" : $"\"{command}\"";
    }
    
    /// <summary>
    /// Check if it is Windows Subsystem for Linux
    /// </summary>
    /// <returns>
    /// True if it's run in WSL
    /// </returns>
    public static bool IsWsl()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return false;
        if (File.Exists("/proc/version"))
        {
            return File.ReadAllText("/proc/version").Contains("Microsoft");
        }
        if (File.Exists("/proc/sys/kernel/osrelease"))
        {
            return File.ReadAllText("/proc/sys/kernel/osrelease").Contains("Microsoft");
        }
        return false;
    }
    
    /// <summary>
    /// Execute Command in WSL
    /// </summary>
    /// <param name="command">
    /// Command to execute
    /// </param>
    /// <param name="distribution">
    /// Distribution to run in
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown in unsupported OS
    /// </exception>
    public static Process? ExecuteCommandInWsl(string command, string distribution = "Ubuntu")
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new NotSupportedException("Can't execute command in WSL on non-Windows OS");
        }
        return ExecuteCommandCrossPlatform($"wsl -d {distribution} {command}");
    }
    
    /// <summary>
    /// Check if command returned one of approved ExitCodes
    /// </summary>
    /// <param name="commandResult">
    /// Result of Command you want to check
    /// </param>
    /// <param name="approvedExitCodes">
    /// List of approved exit codes
    /// </param>
    /// <returns>
    /// Did command succeed or no
    /// </returns>
    public static bool CheckExitCode(Process? commandResult, IEnumerable<int>? approvedExitCodes)
    {
        if (commandResult is null)
        {
            return false;
        }

        if (commandResult.HasExited == false)
        {
            return false;
        }

        approvedExitCodes ??= new[] { 0 };
        
        return new List<int>(approvedExitCodes)
            .TrueForAll(approvedExitCode => commandResult.ExitCode == approvedExitCode);
    }
}