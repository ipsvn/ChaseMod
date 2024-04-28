using CounterStrikeSharp.API.Core;

namespace ChaseMod.Commands;

public static class MiscCommands
{
    public static void AddCommands(BasePlugin plugin)
    {
        plugin.AddCommand("css_hns", "Shows the current version of the plugin", (client, args) =>
        {
            if (client == null || !client.IsValid)
            {
                return;
            }

            client.PrintToChat($"HnS ChaseMod v{plugin.ModuleVersion}");
        });
    }
}
