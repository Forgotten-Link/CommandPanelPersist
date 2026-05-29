using System;
using Dalamud.Configuration;

namespace CommandPanelPersist;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool Enabled { get; set; } = true;

    public void Save() => Plugin.PluginInterface.SavePluginConfig(this);
}
