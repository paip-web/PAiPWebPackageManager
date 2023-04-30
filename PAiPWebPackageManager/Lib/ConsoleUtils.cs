using System.Diagnostics;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Json;

namespace PAiPWebPackageManager.Lib;

public static class ConsoleUtils
{
    #region Version
    
    /// <summary>
    /// Get Current Version of Project
    /// </summary>
    /// <returns>Current Version</returns>
    public static string GetApplicationVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
    }
    
    #endregion
    
    #region Debug Environment

    /// <summary>
    /// Debug Variable
    /// </summary>
    public static bool IsDebug = false;
    
    /// <summary>
    /// Set Debug Environment
    /// </summary>
    /// <param name="debug">
    /// Does current environment need debug information?
    /// </param>
    public static void SetDebugEnvironment(bool debug = true)
    {
        IsDebug = debug;
    }
    
    /// <summary>
    /// Perform some action in debug environment and not check if it should be debug or not
    /// </summary>
    /// <param name="action">Action you want to take in Debug Environment</param>
    public static void WithDebugEnvironment(Action action)
    {
        StartWithDebugEnvironment();
        action();
        StopWithDebugEnvironment();
    }

    /// <summary>
    /// Cached Debug Variable for enforcing Debug Environment for specific segment of code
    /// </summary>
    private static bool _debugCache = IsDebug;
    /// <summary>
    /// This is first part of WithDebugEnvironment Segmented method.
    /// This segment is used to start enforcing debug environment.
    /// </summary>
    public static void StartWithDebugEnvironment()
    {
        _debugCache = IsDebug;
        IsDebug = true;
    }
    
    /// <summary>
    /// This is second part of WithDebugEnvironment Segmented method.
    /// This segment is used to stop enforcing debug environment.
    /// </summary>
    public static void StopWithDebugEnvironment()
    {
        IsDebug = _debugCache;
    }
    
    /// <summary>
    /// Do some action only in debug environment
    /// </summary>
    /// <param name="action">Action to do in debug environment</param>
    public static void DoInDebug(Action action)
    {
        if (!IsDebug) return;
        action();
    }

    #endregion

    #region Debug Logging - Basic Messages
    
    /// <summary>
    /// Prefix for Debug Messages
    /// </summary>
    private const string DebugPrefix = "DEBUG";

    /// <summary>
    /// Log Message in Debug Environment
    /// </summary>
    /// <param name="message">Message to Log</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLog<T>(T message)
    {
        DebugLogWriteLine(message, DebugPrefix);
    }

    /// <summary>
    /// Log Message in Debug Environment
    /// </summary>
    /// <param name="enabled">
    /// If true then log message
    /// </param>
    /// <param name="message">Message to Log</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLog<T>(bool enabled, T message)
    {
        if (!enabled) return;
        DebugLog<T>(message);
    }
    
    /// <summary>
    /// Log Message Line in Debug Environment
    /// </summary>
    /// <param name="message">Message to Log</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLogMessageLine<T>(T message)
    {
        DebugLogWriteLine(message, DebugPrefix);
    }

    /// <summary>
    /// Log Message without new line at the end in Debug Environment
    /// </summary>
    /// <param name="message">Message to Log</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLogMessage<T>(T message)
    {
        DebugLogWrite(message, DebugPrefix);
    }

    /// <summary>
    /// Log Message only in Debug Environment
    /// </summary>
    /// <param name="message">Message to Log</param>
    /// <param name="prefix">Prefix for the message if needed</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLogWriteLine<T>(T message, string? prefix = null)
    {
        if (!IsDebug) return;
        if (prefix is null)
        {
            AnsiConsole.MarkupLineInterpolated($"[maroon]{message}[/]");
        }
        else
        {
            AnsiConsole.MarkupLineInterpolated($"[maroon]{prefix}: {message}[/]");
        }
    }
    
    /// <summary>
    /// Log Message without new line at the end only in Debug Environment
    /// </summary>
    /// <param name="message">Message to Log</param>
    /// <param name="prefix">Prefix for the message if needed</param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLogWrite<T>(T message, string? prefix = null)
    {
        if (!IsDebug) return;
        if (prefix is null)
        {
            AnsiConsole.MarkupInterpolated($"[maroon]{message}[/]");
        }
        else
        {
            AnsiConsole.MarkupInterpolated($"[maroon]{prefix}: {message}[/]");
        }
    }

    /// <summary>
    /// Print Separator with Title only in Debug Environment
    /// </summary>
    /// <param name="title">
    /// Title to print
    /// </param>
    public static void DebugPrintSeparator(string? title = null)
    {
        if (!IsDebug) return;
        AnsiConsole.Write(title is null ? new Rule() : new Rule(title));
    }
    
    #endregion
    
    #region Debug Logging - Advanced Objects
    
    /// <summary>
    /// Log Object in Debug Environment
    /// </summary>
    /// <param name="objectToLog">Object to Log</param>
    /// <param name="title">Optional Title for the log</param>
    /// <typeparam name="T">
    /// Type of the object to log
    /// Note: this supports only types that could be serialized by Newtonsoft.Json
    /// </typeparam>
    public static void DebugLogObject<T>(T objectToLog, string? title = null)
    {
        if (!IsDebug) return;
        
        var json = new JsonText(
            Newtonsoft.Json.JsonConvert.SerializeObject(objectToLog)
        );

        AnsiConsole.Write(
            new Panel(json)
                .Header(string.IsNullOrWhiteSpace(title) ? "Debug Log Object" : $"Debug Log Object: {title}")
                .Expand()
                .RoundedBorder()
                .BorderColor(Color.Yellow)
        );
    }
    
    #endregion
    
    #region Print Utilities
    
    /// <summary>
    /// Print Success Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintSuccess(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[lime]{message}[/]");
    }
    
    /// <summary>
    /// Print Information Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintInfo(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[blue]{message}[/]");
    }
    
    /// <summary>
    /// Print Warning Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintWarning(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[yellow]{message}[/]");
    }
    
    /// <summary>
    /// Print Error Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintError(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
    }
    
    /// <summary>
    /// Print Question Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintQuestion(string message)
    {
        AnsiConsole.MarkupLineInterpolated($"[fuchsia]{message}[/]");
    }
    
    /// <summary>
    /// Print Message without new line at the end
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void Print(string message)
    {
        AnsiConsole.Markup(message);
    }
    
    /// <summary>
    /// Print Message
    /// </summary>
    /// <param name="message">
    /// Message to Print
    /// </param>
    public static void PrintLine(string message)
    {
        AnsiConsole.MarkupLine(message);
    }
    
    /// <summary>
    /// Print Separator
    /// </summary>
    public static void PrintSeparator()
    {
        AnsiConsole.Write(new Rule());
    }

    /// <summary>
    /// Print Separator with Title
    /// </summary>
    /// <param name="title">
    /// Title to print
    /// </param>
    public static void PrintSeparatorTitle(string title)
    {
        AnsiConsole.Write(new Rule(title));
    }

    /// <summary>
    /// Print Separator with Title
    /// </summary>
    /// <param name="title">
    /// Title to print
    /// </param>
    /// <param name="color">Color of Separator</param>
    public static void PrintSeparatorTitle(string title, Color color)
    {
        AnsiConsole.Write(
            new Rule(title)
                .RuleStyle(new Style(color))
        );
    }
    
    #endregion
    
    #region Print Utilities - Advanced

    /// <summary>
    /// Print Message without new line to Stderr
    /// </summary>
    /// <param name="message">Message to print</param>
    public static void PrintMessageToStderr(string message)
    {
        Console.Error.Write(message);
    }
    
    /// <summary>
    /// Print Message to Stderr
    /// </summary>
    /// <param name="message">Message to print</param>
    public static void PrintMessageLineToStderr(string message)
    {
        Console.Error.WriteLine(message);
    }
    
    #endregion

    #region App Banner

    /// <summary>
    /// Print App Banner
    /// </summary>
    public static void PrintAppBanner()
    {
        AnsiConsole.MarkupLineInterpolated(
            $"[navy]PAiP Web Package Manager - v{GetApplicationVersion()}[/]");
        AnsiConsole.WriteLine();
    }

    #endregion
    
    #region Questions
    
    /// <summary>
    /// Ask Yes or No Question
    /// </summary>
    /// <param name="question">
    /// Question to Ask
    /// </param>
    /// <returns>
    /// True - yes, False - no
    /// </returns>
    public static bool AskYesNoQuestion(string question)
    {
        return AnsiConsole.Confirm(question);
    }
    
    /// <summary>
    /// Ask if user wants to continue
    /// </summary>
    /// <returns>
    /// True - yes, False - no
    /// </returns>
    public static bool AskToContinue()
    {
        return AskYesNoQuestion("Do you want to continue?");
    }
    
    #endregion
    
    #region Verbose Print
    
    /// <summary>
    /// Verbose Variable
    /// </summary>
    public static bool IsVerbose => VerboseLevel > 0;

    /// <summary>
    /// Verbose Level Variable
    /// </summary>
    public static int VerboseLevel = 0;
    
    /// <summary>
    /// Set Verbosity Level
    /// </summary>
    /// <param name="level">Verbosity Level to set to</param>
    public static void SetVerboseLevel(int level)
    {
        VerboseLevel = level;
    }

    /// <summary>
    /// Print Message if Verbose is enabled
    /// </summary>
    /// <param name="message">Message to print</param>
    /// <typeparam name="T">Type of Message to Print</typeparam>
    public static void PrintVerbose<T>(T message)
    {
        if (!IsVerbose) return;
        AnsiConsole.MarkupLineInterpolated($"[grey]{message}[/]");
    }
    
    /// <summary>
    /// Print Message if Verbose is enabled and level is less than or equal to current verbosity level
    /// </summary>
    /// <param name="verboseLevel">Verbose Level of provided message</param>
    /// <param name="message">Message to print</param>
    /// <typeparam name="T">Type of Message to Print</typeparam>
    public static void PrintVerbose<T>(int verboseLevel, T message)
    {
        if (VerboseLevel < verboseLevel) return;
        PrintVerbose<T>(message);
    }

    /// <summary>
    /// Do Action if Verbose is enabled and level is less than or equal to current verbosity level
    /// </summary>
    /// <param name="verboseLevel">
    /// Verbose Level of provided action
    /// </param>
    /// <param name="action">
    /// Action to perform
    /// </param>
    public static void DoInVerbose(int verboseLevel, Action action)
    {
        if (!IsVerbose || VerboseLevel < verboseLevel) return;
        action();
    }

    #endregion
    
    #region Package Manager Output
    
    /// <summary>
    /// Print Package Manager Process Output
    /// </summary>
    /// <param name="process">
    /// Package Manager Process
    /// </param>
    /// <param name="cmd">
    /// Command String for Title
    /// </param>
    public static void PrintPackageManagerProcessOutput(Process process, string cmd)
    {
        var escapedCmd = Markup.Escape(cmd);
        PrintSeparatorTitle($"CMD - {escapedCmd}", Color.Blue);
        process.OutputDataReceived += (sender, e) => {
            Console.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) => {
            var errLine = e.Data;
            if (!string.IsNullOrWhiteSpace(errLine) && (!errLine.StartsWith("\r") || !errLine.StartsWith("\b")))
            {
                AnsiConsole.Markup("[yellow]STDERR:[/] ");
            }

            Console.WriteLine(errLine);
        };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.Exited += (sender, e) => {
            PrintSeparatorTitle($"Finished - {escapedCmd}", Color.Blue);
        };
    }
    
    #endregion
}

public static class ConsoleUtilsExtensions
{
    /// <summary>
    /// Debug Log Message Extension
    /// </summary>
    /// <param name="message">
    /// Message to log from the chain
    /// </param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    public static void DebugLogMessage<T>(this T message)
    {
        ConsoleUtils.DebugLog<T>(message);
    }

    /// <summary>
    /// Debug Log Message Extension
    /// </summary>
    /// <param name="message">
    /// Message to log from the chain
    /// </param>
    /// <typeparam name="T">
    /// Type of message
    /// Not this supports only types that are supported by Console.Write
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the message
    /// </returns>
    public static T DebugLog<T>(this T message)
    {
        ConsoleUtils.DebugLog<T>(message);
        return message;
    }

    /// <summary>
    /// Debug Log Object Extension
    /// </summary>
    /// <param name="obj">
    /// Object to Log from the chain
    /// </param>
    /// <param name="logTitle">
    /// Title for the log
    /// </param>
    /// <typeparam name="T">
    /// Type of the object to log
    /// Note: this supports only types that could be serialized by Newtonsoft.Json
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T DebugLogObj<T>(this T obj, string? logTitle = null)
    {
        ConsoleUtils.DebugLogObject<T>(obj, logTitle);
        return obj;
    }
    
    /// <summary>
    /// Debug Log Custom Object Extension
    /// </summary>
    /// <param name="obj">
    /// Object from the chain
    /// </param>
    /// <param name="action">
    /// Function that takes the object from the chain and returns the object you want to log
    /// </param>
    /// <param name="logTitle">
    /// Title for the log
    /// </param>
    /// <typeparam name="T">
    /// Type of the object from the chain
    /// </typeparam>
    /// <typeparam name="T2">
    /// Type of the object to log which is returned from action function
    /// Note: this supports only types that could be serialized by Newtonsoft.Json
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T CustomDebugLogObj<T,T2>(this T obj, Func<T,T2> action, string? logTitle = null)
    {
        ConsoleUtils.DebugLogObject<T2>(action(obj), logTitle);
        return obj;
    }
    
    /// <summary>
    /// With Debug Environment Extension
    /// </summary>
    /// <param name="obj">
    /// Object from the chain
    /// </param>
    /// <param name="action">
    /// Action to perform in Debug Environment
    /// </param>
    /// <typeparam name="T">
    /// Type of the object from the chain
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T WithDebugEnvironment<T>(this T obj, Action action)
    {
        ConsoleUtils.WithDebugEnvironment(action);
        return obj;
    }
    
    /// <summary>
    /// With Debug Environment Extension
    /// </summary>
    /// <param name="obj">
    /// Object from the chain
    /// </param>
    /// <param name="action">
    /// Action to perform in Debug Environment
    /// </param>
    /// <typeparam name="T">
    /// Type of the object from the chain
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T WithDebugEnvironment<T>(this T obj, Action<T> action)
    {
        ConsoleUtils.WithDebugEnvironment(() => action(obj));
        return obj;
    }
    
    /// <summary>
    /// Start With Enforcing Debug Environment Extension
    /// </summary>
    /// <param name="obj">
    /// Object from the chain
    /// </param>
    /// <typeparam name="T">
    /// Type of the object from the chain
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T StartWithDebugEnvironment<T>(this T obj)
    {
        ConsoleUtils.StartWithDebugEnvironment();
        return obj;
    }
    
    /// <summary>
    /// Stop With Enforcing Debug Environment Extension
    /// </summary>
    /// <param name="obj">
    /// Object from the chain
    /// </param>
    /// <typeparam name="T">
    /// Type of the object from the chain
    /// </typeparam>
    /// <returns>
    /// This is just a pass through for the object
    /// </returns>
    public static T StopWithDebugEnvironment<T>(this T obj)
    {
        ConsoleUtils.StopWithDebugEnvironment();
        return obj;
    }
}