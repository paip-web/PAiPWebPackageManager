using System.CommandLine;
using System.CommandLine.Invocation;
using PAiPWebPackageManager.Command;
using PAiPWebPackageManager.Lib;

namespace PAiPWebPackageManager;

using Cli = System.CommandLine;
using CliCommand = System.CommandLine.Command;
using CliU = CliUtils;
using Con = ConsoleUtils;

public static class PackageManagerCli
{
    #region Helper Functions
    
    private static Option<string?> BuildStringOption(Option<string?> opt, string[] possibleOptions)
    {
        opt.FromAmong(possibleOptions);
        return opt;
    }
    
    #endregion
    #region Package Management Options
    private static readonly Option<bool> SupportInstallationOfPmOption = new Option<bool>(
        aliases: new []{ "--support-package-manager-installation", "--support-pm-install", "-SPM" },
        description: "Do package manager installation if needed.",
        getDefaultValue: () => false
    );
    private static readonly Option<string?> PackageManagerToUseOption = BuildStringOption(new Option<string?>(
        aliases: new[] { "--package-manager", "-pm" },
        description: "Specify package manager to use.",
        getDefaultValue: () => null
    ), PluginsList.GetAllPluginNames().ToArray());
    #endregion
    #region Global Options
    private static readonly Option<bool> DryRunOption = new Option<bool>(
        aliases: new []{ "--dry-run", "--what-if", "-D" },
        description: "Do not execute any commands, just print them to the console.",
        getDefaultValue: () => false
    );
    private static readonly Option<bool> VerboseOption = new Option<bool>(
        aliases: new []{ "--verbose", "-v"},
        description: "Show verbose messaging",
        getDefaultValue: () => false
    );
    private static readonly Option<bool> DebugOption = new Option<bool>(
        aliases: new []{ "--debug", "-d"},
        description: "Show debug messaging",
        getDefaultValue: () => false
    );
    #endregion
    #region Add Package Management Options
    private static CliCommand AddPackageManagementOptions(CliCommand cmd)
    {
        cmd.AddOption(SupportInstallationOfPmOption);
        cmd.AddOption(PackageManagerToUseOption);
        return cmd;
    }
    #endregion
    #region Option Handlers
    private static void HandlerForGlobalOptions(InvocationContext invCtx)
    {
        var isDryRun = invCtx.ParseResult.GetValueForOption(DryRunOption);
        var isVerbose = invCtx.ParseResult.GetValueForOption(VerboseOption);
        var isDebug = invCtx.ParseResult.GetValueForOption(DebugOption);
        
        ConsoleUtils.SetDebugEnvironment(isDebug);
        ConsoleUtils.SetVerboseLevel(isVerbose ? 1 : 0);
        Executor.SetDryRun(isDryRun);
        
        ConsoleUtils.DebugPrintSeparator("Global Options");
        ConsoleUtils.DebugLog(isDebug ? "Debug Mode Enabled" : "Debug Mode Disabled");
        ConsoleUtils.DebugLog(isVerbose ? "Verbose Mode Enabled" : "Verbose Mode Disabled");
        ConsoleUtils.DebugLog(isDryRun ? "Dry Run Mode Enabled" : "Dry Run Mode Disabled");
    }
    private static PackageManagementInvocationContext HandlerForPackageManagementOptions(InvocationContext invCtx)
    {
        var isInstallSupported = invCtx.ParseResult.GetValueForOption(SupportInstallationOfPmOption);
        var packageManager = invCtx.ParseResult.GetValueForOption(PackageManagerToUseOption);
        
        var context = new PackageManagementInvocationContext(isInstallSupported, packageManager);
        PluginManager.WithInstall = context.SupportInstallationOfPm;
        
        ConsoleUtils.DebugPrintSeparator("Package Management Options");
        ConsoleUtils.DebugLog(
            context.SupportInstallationOfPm 
                ? "Package Manager Installation is supported"
                : "Package Manager Installation is not supported"
        );
        ConsoleUtils.DebugLog(
            context.PackageManagerToUse is not null
                ? $"Package Manager is specified and it is: {context.PackageManagerToUse}"
                : "Package Manager is not specified"
        );

        return context;
    }
    #endregion
    #region Command Handlers
    private static void InstallCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("INSTALL");
    }
    private static void UpdateCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("UPDATE");
    }
    private static void UninstallCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("UNINSTALL");
    }
    private static void UpdatePackageDatabaseCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("UPDATE PACKAGE DB");
    }
    private static void AddPackageDatabaseCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("ADD PACKAGE DB");
    }
    private static void RemovePackageDatabaseCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("REMOVE PACKAGE DB");
    }
    private static void UpdateAllCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("UPDATE ALL");
    }
    private static void InstallPackageManagerCommandHandler(InvocationContext invCtx, PackageManagementInvocationContext pmInvCtx)
    {
        Con.DebugPrintSeparator("INSTALL PACKAGE MANAGER");
        
        var packageManager = invCtx.ParseResult.GetValueForOption(PackageManagerToUseOption);
        if (string.IsNullOrWhiteSpace(packageManager))
        {
            Con.PrintError("Package Manager is not specified, exiting...");
            CliU.SetExitCode(invCtx, PwpmExitCode.Failure);
            return;
        }
        
        PluginManager.WithInstall = true;
        PluginManager.GetPluginManager();
        
        Con.DebugPrintSeparator("Installation of Package Manager");
        
        if (!PluginManager.GetPluginManager().IsInstallSupported(pluginName: packageManager))
        {
            Con.PrintError("Package Manager is not supported, exiting...");
            CliU.SetExitCode(invCtx, PwpmExitCode.Failure);
            return;
        }

        Con.PrintInfo("Installing Package Manager...");
        if (!PluginManager.GetPluginManager().InstallPackageManager(packageManager))
        {
            Con.PrintError("Package Manager could not be installed, exiting...");
            CliU.SetExitCode(invCtx, PwpmExitCode.Failure);
            return;
        }
        Con.PrintInfo($"Finished installing Package Manager: {packageManager}.");
    }
    #endregion
    
    public static int Run(string[] args)
    {
        // Create Root Command
        var rootCommand = new RootCommand
        {
            Name = "pwpm",
            Description = "PAiP Web Package Manager is a tool to manage your packages installed through different package managers."
        };
        #region Run - Global Options
        // Add Global Options
        rootCommand.AddGlobalOption(DryRunOption);
        rootCommand.AddGlobalOption(VerboseOption);
        rootCommand.AddGlobalOption(DebugOption);
        #endregion

        #region Run - Subcommands
        // Install Command
        var installCommand = new CliCommand(
            name: "install",
            description: "Install a package"
        );
        installCommand = AddPackageManagementOptions(installCommand);
        rootCommand.AddCommand(installCommand);
        
        // Update Command
        var updateCommand = new CliCommand(
            name: "update",
            description: "Update a package"
        );
        updateCommand = AddPackageManagementOptions(updateCommand);
        rootCommand.AddCommand(updateCommand);
        
        // Uninstall Command
        var uninstallCommand = new CliCommand(
            name: "uninstall",
            description: "Uninstall a package"
        );
        uninstallCommand.AddAlias("remove");
        uninstallCommand = AddPackageManagementOptions(uninstallCommand);
        rootCommand.AddCommand(uninstallCommand);
        
        // Update Package Database Command
        var updatePackageDbCommand = new CliCommand(
            name: "update-package-db",
            description: "Update a package database"
        );
        updatePackageDbCommand = AddPackageManagementOptions(updatePackageDbCommand);
        rootCommand.AddCommand(updatePackageDbCommand);
        
        // Add Package Database Command
        var addPackageDbCommand = new CliCommand(
            name: "add-package-db",
            description: "Add a package database"
        );
        addPackageDbCommand = AddPackageManagementOptions(addPackageDbCommand);
        rootCommand.AddCommand(addPackageDbCommand);
        
        // Remove Package Database Command
        var removePackageDbCommand = new CliCommand(
            name: "remove-package-db",
            description: "Remove a package database"
        );
        removePackageDbCommand = AddPackageManagementOptions(removePackageDbCommand);
        rootCommand.AddCommand(removePackageDbCommand);
        
        // Update All Packages Command
        var updateAllCommand = new CliCommand(
            name: "update-all",
            description: "Update all installed packages"
        );
        updateAllCommand = AddPackageManagementOptions(updateAllCommand);
        rootCommand.AddCommand(updateAllCommand);
        
        // Update All Packages Command
        var installPackageManager = new CliCommand(
            name: "install-pm",
            description: "Install specified Package Manager if it supports it."
        );
        installPackageManager = AddPackageManagementOptions(installPackageManager);
        rootCommand.AddCommand(installPackageManager);
        #endregion
        
        #region Run - Handlers for Commands
        // Add Handlers
        rootCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            Con.DebugPrintSeparator("ROOT");
        });
        installCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            InstallCommandHandler(invCtx, packageManagementContext);
        });
        updateCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            UpdateCommandHandler(invCtx, packageManagementContext);
        });
        uninstallCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            UninstallCommandHandler(invCtx, packageManagementContext);
        });
        updatePackageDbCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            UpdatePackageDatabaseCommandHandler(invCtx, packageManagementContext);
        });
        addPackageDbCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            AddPackageDatabaseCommandHandler(invCtx, packageManagementContext);
        });
        removePackageDbCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            RemovePackageDatabaseCommandHandler(invCtx, packageManagementContext);
        });
        updateAllCommand.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            UpdateAllCommandHandler(invCtx, packageManagementContext);
        });
        installPackageManager.SetHandler((invCtx) =>
        {
            HandlerForGlobalOptions(invCtx);
            var packageManagementContext = HandlerForPackageManagementOptions(invCtx);
            InstallPackageManagerCommandHandler(invCtx, packageManagementContext);
        });
        #endregion

        #region Run - Help by default and Invoke CommandLine
        // If run without arguments run --help
        if (args.Length == 0)
        {
            return rootCommand.Invoke("--help");
        }

        // If run with arguments run with them
        return rootCommand.Invoke(args);
        #endregion
    }
}