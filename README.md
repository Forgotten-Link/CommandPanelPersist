# CommandPanelPersist

A small Dalamud plugin that keeps the in-game **Command Panel** open through deaths, wipes, instance resets, and zone changes.

By default the Command Panel auto-closes on load screens and when you die. This plugin flips the addon flags the UI manager checks before hiding it, so the panel just stays where you left it.

## Install

This plugin ships through a custom Dalamud repository.

1. In-game, run `/xlsettings` → **Experimental** → **Custom Plugin Repositories**.
2. Add this URL and hit the **+** button:
   ```
   https://raw.githubusercontent.com/Forgotten-Link/CommandPanelPersist/master/repo.json
   ```
3. Save, then open `/xlplugins` and search for **CommandPanelPersist**.

## Usage

| Command | Effect |
| --- | --- |
| `/cmdpanelpersist` | Open the settings window |
| `/cmdpanelpersist on` | Enable |
| `/cmdpanelpersist off` | Disable (close & reopen the panel to fully revert) |

Toggle the checkbox in the settings window or use the commands — that's it.

## Building

Requires the .NET 10 SDK and a working Dalamud install (`~/.xlcore/dalamud/Hooks/dev` on Linux, `%AppData%\XIVLauncher\addon\Hooks\dev` on Windows).

```sh
dotnet build --configuration Release
```

Output lands in `CommandPanelPersist/bin/x64/Release/CommandPanelPersist/`.

## License

[AGPL-3.0-or-later](LICENSE.md).
