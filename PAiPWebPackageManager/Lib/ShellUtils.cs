using System.Runtime.InteropServices;

namespace PAiPWebPackageManager.Lib;

public static class ShellUtils
{
    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
    
    public static bool IsLinux()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    
    public static bool IsMacOS()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }
    
    public static bool IsPosix()
    {
        return IsLinux()
               || IsMacOS()
               || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
    }
    
    public static string GetExecutableExtension()
    {
        if (IsWindows())
        {
            return ".exe";
        }

        return "";
    }
    
    public static string[] GetExecutableExtensions()
    {
        if (IsWindows())
        {
            return new string[] { ".exe", ".cmd", ".bat" };
        }

        return new string[] { "" };
    }

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

    public static string? FindExe(string exe)
    {
        if (exe.Length == 0) return null;
        var extensions = GetExecutableExtensions();

        string AddFileExt(string file, string extension) => Path.HasExtension(file) ? file : Path.ChangeExtension(file, extension);
        
        if ((IsPosix() && exe.Contains('/')) || !IsPosix())
        {
            var foundInCurrentDir = extensions
                .ToList()
                .Select((ext) => AddFileExt(exe, ext))
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

        var candidates = candidatePaths
            .ToList()
            .Where((candidate) => candidate.Length != 0)
            .Select((candidate) => {
                if (!IsWindows())
                {
                    return Path.Join(ExpandTilde(candidate), exe);
                }

                if (candidate.StartsWith('"') && candidate.EndsWith('"'))
                {
                    return Path.Join(candidate.Substring(1, candidate.Length - 2), exe);
                }

                return Path.Join(candidate, exe);
            });

        var realCandidates = candidates
            .SelectMany((candidate) => extensions.Select((ext) => AddFileExt(candidate, ext)))
            .Where(Path.Exists)
            .FirstOrDefault(defaultValue: null);

        return realCandidates;
    }
    
    
    public static bool IsCommandAvailable(string command)
    {
        return false;
    }
}