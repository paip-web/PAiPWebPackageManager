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

    // Port of https://github.com/nim-lang/Nim/blob/version-1-6/lib/pure/os.nim#L1227 (os.findExe)
    public static string? FindExe(string exe)
    {
        if (exe.Length == 0) return null;
        var extensions = GetExecutableExtensions();
        
        // Check Current Dir
        var fileExists = (string filename) => {
            return Path.Exists(filename);
        };
        var expandTilde = (string filename) => {
            if (filename.Length == 0 || !filename.StartsWith("~")) return filename;
            if (filename.Length == 1)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            if (filename[1] == Path.DirectorySeparatorChar || filename[1] == Path.AltDirectorySeparatorChar)
            {
                return Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    filename[2..]
                );
            }

            return filename;
        };
        var addFileExt = (string exe2, string ext) => {
            if (Path.HasExtension(exe2))
            {
                return exe2;
            }

            return Path.ChangeExtension(exe2, ext);
        };
        var checkCurrentDir = () => {
            foreach (var ext in extensions)
            {
                var f = addFileExt(exe, ext);
                if (fileExists(f))
                {
                    return f;
                }
            }

            return null;
        };
        if (IsPosix())
        {
            if (exe.Contains('/'))
            {
                var r = checkCurrentDir();
                if (r is not null)
                {
                    return r;
                }
            }
        }
        else
        {
            var r = checkCurrentDir();
            if (r is not null)
            {
                return r;
            }
        }

        var path = Environment.GetEnvironmentVariable("PATH");
        if (path is null) return null;
        foreach (var candidate in path.Split(Path.PathSeparator))
        {
            if (candidate.Length == 0) continue;
            string x = "";
            if (IsWindows())
            {
                if (candidate.StartsWith('"') && candidate.EndsWith('"'))
                {
                    x = candidate.Substring(1, candidate.Length - 2);
                }
                else
                {
                    x = candidate;
                }
                x = Path.Join(x, exe);
            }
            else
            {
                x = Path.Join(expandTilde(candidate), exe);
            }

            foreach (var ext in extensions)
            {
                var x2 = addFileExt(x, ext);
                if (fileExists(x2))
                {
                    return x2;
                }
            }
        }

        return null;
    }
    
    
    public static bool IsCommandAvailable(string command)
    {
        return false;
    }
}