using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Utils;

namespace ChaseMod.Utils;

public static class ChaseModUtils
{
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static CCSGameRules GetGameRules()
    {
        var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
        var gameRules = gameRulesEntities.First().GameRules;

        if (gameRules == null)
        {
            throw new Exception("Game rules not found!");
        }

        return gameRules;
    }

    public static bool IsRealPlayer(CCSPlayerController p)
    {
        return
            p.IsValid &&
            p.PlayerPawn.IsValid && p.PlayerPawn.Value != null &&
            p.PlayerPawn.Value.IsValid &&
            p.Connected == PlayerConnectedState.PlayerConnected &&
            !p.IsHLTV &&
            (
                p.Team == CsTeam.Terrorist || p.Team == CsTeam.CounterTerrorist
            );
    }

    public static List<CCSPlayerController> GetAllRealPlayers()
    {
        return Utilities.GetPlayers().Where(p => IsRealPlayer(p)).ToList();
    }

    public static void ChatPrefixed(CCSPlayerController player, string message)
    {
        player.PrintToChat($" {ChatColors.Magenta}HNS {ChatColors.Grey}| {message}");
    }
    public static void ChatAllPrefixed(string message)
    {
        Server.PrintToChatAll($" {ChatColors.Magenta}HNS {ChatColors.Grey}| {message}");
    }
}
