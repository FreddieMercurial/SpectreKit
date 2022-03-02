using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PhantomKit.Extensions;
using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.Menus;
using PhantomMail.Windows;
using Serilog;
using Terminal.Gui;

namespace PhantomKit.Models.Commands;

/// <summary>
///     Abstract, mainly example class
/// </summary>
[HelpOption(template: "--help")]
public abstract class HostedGuiCommandBase : IGuiCommand
{
    public const bool DefaultDarkMode = true;

    /// <summary>
    ///     Must be assigned before using with parameterless constructor
    /// </summary>
    public static IServiceProvider? ServiceProvider;

    protected readonly bool IsBox10X = true;

    public HostedGuiCommandBase(bool run = true) : this(
        run: run,
        window: null,
        statusBar: null,
        menu: null)
    {
    }

    public HostedGuiCommandBase(bool run = true, PhantomKitWindow? window = null, PhantomKitStatusBar? statusBar = null,
        PhantomKitMainMenu? menu = null)
    {
        // assuming we are in an IHostedService and Host was set before instantiating this class
        var logger = (ServiceProvider!.GetService(serviceType: typeof(ILogger)) as ILogger)!;
        var configuration = (ServiceProvider.GetService(serviceType: typeof(IConfiguration)) as IConfiguration)!;
        var console = (ServiceProvider.GetService(serviceType: typeof(IConsole)) as IConsole)!;

        if (IGuiCommand.SingletonMade(t: this.GetType()))
            throw new Exception(message: $"{this.GetType()} already made");
        IGuiCommand.SetSingleton(instance: this);
        this.Instance = this;
        this.Logger = logger;
        this.Configuration = configuration;
        this.Console = console;
        Application.Init();
        Application.HeightAsBuffer = true;
        this.Window = window ?? new PhantomKitWindow();
        this.Menu = menu ?? new PhantomKitMainMenu(guiCommandType: this.GetType());
        this.StatusBar = statusBar ?? new PhantomKitStatusBar(guiCommandType: this.GetType());

        if (run)
            Application.Run(view: GuiUtilities.GetNewTopLevel(guiCommand: this));
    }

    protected HostedGuiCommandBase(ILogger logger, IConfiguration configuration, IConsole console, bool run = true,
        PhantomKitWindow? window = null, PhantomKitStatusBar? statusBar = null, PhantomKitMainMenu? menu = null)
        : this(logger: logger,
            configuration: configuration,
            console: console,
            run: run)
    {
        this.Window = window ?? new PhantomKitWindow();
        this.StatusBar = statusBar ?? new PhantomKitStatusBar(guiCommandType: this.GetType());
        this.Menu = menu ?? new PhantomKitMainMenu(guiCommandType: this.GetType());
    }

    public HostedGuiCommandBase(ILogger logger, IConfiguration configuration, IConsole console, bool run = true)
    {
        if (IGuiCommand.SingletonMade(t: this.GetType()))
            throw new Exception(message: $"{this.GetType()} already made");
        IGuiCommand.SetSingleton(instance: this);
        this.Instance = this;
        this.Logger = logger;
        this.Configuration = configuration;
        this.Console = console;
        this.Window = new PhantomKitWindow();
        this.Menu = new PhantomKitMainMenu(guiCommandType: this.GetType());
        this.StatusBar = new PhantomKitStatusBar(guiCommandType: this.GetType());

        if (run)
            Application.Run(view: GuiUtilities.GetNewTopLevel(guiCommand: this));
    }

    public HostedGuiCommandBase Instance { get; init; }
    public bool Running { get; private set; }

    public void SetRunning(bool value)
    {
        this.Running = value;
    }

    public abstract bool MouseEnabled { get; init; }
    public IConfiguration Configuration { get; init; }

    public IConsole Console { get; init; }
    public ILogger Logger { get; init; }

    public PhantomKitMainMenu Menu { get; init; }
    public PhantomKitStatusBar StatusBar { get; init; }
    public PhantomKitWindow? Window { get; init; }
    public bool DarkMode { get; set; } = DefaultDarkMode;

    public abstract int OnExecute(CommandLineApplication app);

    public void OnException(Exception ex)
    {
        GuiCommandExtensions.OnException(hostedGuiCommand: this,
            ex: ex);
    }

    public void OutputToConsole(string data)
    {
        GuiCommandExtensions.OutputToConsole(hostedGuiCommand: this,
            data: data);
    }

    public void OutputError(string message)
    {
        GuiCommandExtensions.OutputError(hostedGuiCommand: this,
            message: message);
    }

    public void UpdateTheme(HumanEditableTheme theme)
    {
        GuiCommandExtensions.UpdateTheme(hostedGuiCommand: this,
            theme: theme);
    }

    public void Copy()
    {
        GuiCommandExtensions.Copy(hostedGuiCommand: this);
    }

    public void Cut()
    {
        GuiCommandExtensions.Cut(hostedGuiCommand: this);
    }

    public void Paste()
    {
        GuiCommandExtensions.Paste(hostedGuiCommand: this);
    }

    public void SetTheme(HumanEditableTheme theme)
    {
        GuiCommandExtensions.SetTheme(hostedGuiCommand: this,
            theme: theme);
    }

    public void AskQuit()
    {
        GuiCommandExtensions.AskQuit(hostedGuiCommand: this);
    }

    public abstract void Dispose();
}