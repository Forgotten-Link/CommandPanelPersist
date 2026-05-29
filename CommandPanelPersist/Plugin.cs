using CommandPanelPersist.Windows;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace CommandPanelPersist;

public sealed unsafe class Plugin : IDalamudPlugin
{
    public string Name => "CommandPanelPersist";
    private const string CommandName = "/cmdpanelpersist";

    // The Command Panel addon is internally named "QuickPanel".
    private const string AddonName = "QuickPanel";

    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] private static ICommandManager          Commands         { get; set; } = null!;
    [PluginService] private static IAddonLifecycle          AddonLifecycle   { get; set; } = null!;
    [PluginService] private static IChatGui                 ChatGui          { get; set; } = null!;
    [PluginService] private static IPluginLog               Log              { get; set; } = null!;

    internal static Configuration Config { get; private set; } = null!;

    private readonly WindowSystem _windowSystem = new("CommandPanelPersist");
    private readonly ConfigWindow _configWindow = new();

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        _windowSystem.AddWindow(_configWindow);

        AddonLifecycle.RegisterListener(AddonEvent.PostSetup, AddonName, OnQuickPanelSetup);

        Commands.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Stops the Command Panel from auto-closing on death, wipes, and zone changes. /cmdpanelpersist [on|off]"
        });

        PluginInterface.UiBuilder.Draw         += _windowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
        PluginInterface.UiBuilder.OpenMainUi   += ToggleConfigWindow;

        Log.Information("[CommandPanelPersist] Loaded.");
    }

    // When the Command Panel opens, set the auto-hide override bits the game's
    // UI manager checks before hiding it on load screens / death / cutscenes.
    // 0x16 = 0b0001_0110 — three auto-hide-exempt flags in AtkUnitBase.Flags1B4
    // (reverse-engineered; the official ClientStructs name has not been mapped yet).
    private void OnQuickPanelSetup(AddonEvent type, AddonArgs args)
    {
        if (!Config.Enabled) return;

        var addon = (AtkUnitBase*)args.Addon.Address;
        addon->Flags1B4 |= 0x16;
        addon->DisableFocusability = true;
    }

    private void ToggleConfigWindow() => _configWindow.Toggle();

    private void OnCommand(string command, string args)
    {
        switch (args.Trim().ToLowerInvariant())
        {
            case "on":
                Config.Enabled = true;
                Config.Save();
                ChatGui.Print("[CommandPanelPersist] Enabled.");
                break;

            case "off":
                Config.Enabled = false;
                Config.Save();
                ChatGui.Print("[CommandPanelPersist] Disabled. Close and reopen the panel to fully revert.");
                break;

            case "":
                ToggleConfigWindow();
                break;

            default:
                ChatGui.Print($"[CommandPanelPersist] {(Config.Enabled ? "ON" : "OFF")}");
                ChatGui.Print("  on / off — enable or disable");
                ChatGui.Print("  (no args) — open settings window");
                break;
        }
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw         -= _windowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;
        PluginInterface.UiBuilder.OpenMainUi   -= ToggleConfigWindow;

        _windowSystem.RemoveAllWindows();

        AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, AddonName, OnQuickPanelSetup);
        Commands.RemoveHandler(CommandName);
    }
}
