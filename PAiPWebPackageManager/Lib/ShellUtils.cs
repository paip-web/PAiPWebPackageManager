using System.Runtime.InteropServices;

namespace PAiPWebPackageManager.Lib;

public static class ShellUtils
{
    /// <summary>
    /// Check if current platform is Windows
    /// </summary>
    /// <returns>
    /// True if current platform is Windows
    /// </returns>
    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
    
    /// <summary>
    /// Check if current platform is Linux
    /// </summary>
    /// <returns>
    /// True if current platform is Linux
    /// </returns>
    public static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    
    /// <summary>
    /// Check if current platform is Mac OS
    /// </summary>
    /// <returns>
    /// True if current platform is Mac OS
    /// </returns>
    public static bool IsMacOS()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }
    
    /// <summary>
    /// Check if current platform is POSIX
    /// </summary>
    /// <returns>
    /// True if current platform is POSIX
    /// </returns>
    public static bool IsPosix()
    {
        return IsLinux()
               || IsMacOS()
               || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
    }

    /// <summary>
    /// Get Executable Extensions
    /// </summary>
    /// <returns>
    /// Executable Extensions
    /// </returns>
    private static string[] GetExecutableExtensions()
    {
        if (IsWindows())
        {
            return new string[] { ".exe", ".cmd", ".bat" };
        }

        return new string[] { "" };
    }

    /// <summary>
    /// Expand Tilde to Home Directory
    /// </summary>
    /// <param name="path">
    /// Path to expand
    /// </param>
    /// <returns>
    /// Expanded path
    /// </returns>
    public static string ExpandTilde(string path)
    {
        if (path.Length == 0 || !path.StartsWith("~")) return path;
        if (path.Length == 1)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        if (path[1] == Path.DirectorySeparatorChar || path[1] == Path.AltDirectorySeparatorChar)
        {
            return Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                path[2..]
            );
        }

        return path;
    }

    /// <summary>
    /// Find Executable
    /// </summary>
    /// <param name="executableToFind">
    /// Executable to find
    /// </param>
    /// <returns>
    /// Executable path if found
    /// Null if not found
    /// </returns>
    public static string? FindExecutable(string executableToFind)
    {
        if (executableToFind.Length == 0) return null;
        var extensions = GetExecutableExtensions();

        string AddFileExt(string file, string extension) => Path.HasExtension(file) ? file : Path.ChangeExtension(file, extension);
        
        if ((IsPosix() && executableToFind.Contains('/')) || !IsPosix())
        {
            var foundInCurrentDir = extensions
                .ToList()
                .Select((ext) => AddFileExt(executableToFind, ext))
                .Where(Path.Exists)
                .Cast<string?>()
                .FirstOrDefault(defaultValue: null);
            if (foundInCurrentDir is not null)
            {
                return foundInCurrentDir;
            }
        }

        var path = Environment.GetEnvironmentVariable("PATH");
        if (path is null) return null;
        var candidatePaths = path.Split(Path.PathSeparator);
        if (candidatePaths.Length == 0) return null;

        return candidatePaths
            .ToList()
            .Where((candidate) => candidate.Length != 0)
            .Select((candidate) => {
                if (!IsWindows())
                {
                    return Path.Join(ExpandTilde(candidate), executableToFind);
                }

                if (candidate.StartsWith('"') && candidate.EndsWith('"'))
                {
                    return Path.Join(candidate.Substring(1, candidate.Length - 2), executableToFind);
                }

                return Path.Join(candidate, executableToFind);
            })
            .SelectMany((candidate) => extensions.Select((ext) => AddFileExt(candidate, ext)))
            .Where(Path.Exists)
            .FirstOrDefault(defaultValue: null);
    }
    
    /// <summary>
    /// Check if command is available
    /// </summary>
    /// <param name="command">
    /// Command to check
    /// </param>
    /// <returns>
    /// True if command is available
    /// </returns>
    public static bool IsCommandAvailable(string command)
    {
        return FindExecutable(command) is not null;
    }
}