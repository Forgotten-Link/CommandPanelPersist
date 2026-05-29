using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace CommandPanelPersist.Windows;

public class ConfigWindow : Window
{
    public ConfigWindow() : base("CommandPanelPersist")
    {
        Size = new Vector2(360, 120);
        SizeCondition = ImGuiCond.FirstUseEver;
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize;
    }

    public override void Draw()
    {
        var enabled = Plugin.Config.Enabled;
        if (ImGui.Checkbox("Keep Command Panel sticky", ref enabled))
        {
            Plugin.Config.Enabled = enabled;
            Plugin.Config.Save();
        }

        ImGui.TextDisabled("Prevents the panel from auto-closing on death, wipes,");
        ImGui.TextDisabled("and zone changes. Close & reopen the panel after");
        ImGui.TextDisabled("toggling off to fully revert.");
    }
}
