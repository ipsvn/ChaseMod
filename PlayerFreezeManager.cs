using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ChaseMod.Utils;

using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using Microsoft.Extensions.Logging;

namespace ChaseMod;

public record FrozenPlayer(
    float Time, 
    float StartTime,
    System.Numerics.Vector3 StoredVelocity, 
    Timer Timer
);

internal class PlayerFreezeManager
{
    private readonly ChaseMod _plugin;
    private readonly Dictionary<CCSPlayerController, FrozenPlayer> _frozenPlayers = new();
    
    public PlayerFreezeManager(ChaseMod chaseMod)
    {
        _plugin = chaseMod;
    }

    public void Freeze(CCSPlayerController controller, float time, bool showEffect, bool sendMessage, bool resetVelocity)
    {
        if (!ChaseModUtils.IsRealPlayer(controller)) return;
        var pawn = controller.PlayerPawn.Value!;

        ChaseMod.Logger.LogInformation($"Freeze player {pawn.Index}");

        var origVelocity = pawn.AbsVelocity.ToManaged();
        pawn.AbsVelocity.Set(0, 0, 0);

        if (_frozenPlayers.TryGetValue(controller, out var freezeState))
        {
            if (!resetVelocity) origVelocity = freezeState.StoredVelocity;
            freezeState.Timer.Kill();
        }

        pawn.FreezePlayer();

        var playerAlpha = pawn.Render.A;

        pawn.Render = Color.FromArgb(playerAlpha, 4, 58, 140);
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

        if (showEffect)
        {
            pawn.HealthShotBoostExpirationTime = Server.CurrentTime + time;
            Utilities.SetStateChanged(pawn, "CCSPlayerPawn", "m_flHealthShotBoostExpirationTime");
        }

        if (pawn.WeaponServices != null)
        {
            var activeWeaponHandle = pawn.WeaponServices.ActiveWeapon;
            if (activeWeaponHandle.IsValid)
                activeWeaponHandle.Value!.DisableUntil(Server.TickCount + (int)(0.5f + (time / Server.TickInterval)));
        }

        var timer = _plugin.AddTimer(
            time, () => Unfreeze(controller, sendMessage));

        _frozenPlayers[controller] = new FrozenPlayer(time, Server.CurrentTime, origVelocity, timer);

        if (sendMessage)
        {
            ChaseModUtils.ChatPrefixed(
                controller,
                $"{ChatColors.DarkRed}⚠{ChatColors.Grey} You have been {ChatColors.DarkRed}frozen{ChatColors.Grey}.");
        }
    }

    private void Unfreeze(CCSPlayerController controller, bool sendMessage)
    {
        if (!ChaseModUtils.IsRealPlayer(controller)) return;
        var pawn = controller.PlayerPawn.Value!;

        ChaseMod.Logger.LogInformation($"Unfreeze player {pawn.Index}");

        pawn.UnfreezePlayer();

        if (_frozenPlayers.TryGetValue(controller, out var frozenState))
        {
            pawn.AbsVelocity.Set(frozenState.StoredVelocity);
        }

        pawn.Render = Color.FromArgb(pawn.Render.A, 255, 255, 255);
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

        _frozenPlayers.Remove(controller);

        if (sendMessage)
        {
            ChaseModUtils.ChatPrefixed(
                controller,
                $"{ChatColors.Green}⚠{ChatColors.Grey} You are now {ChatColors.Green}unfrozen{ChatColors.Grey}.");
        }
    }

    public float? GetPlayerFreezeRemaining(CCSPlayerController controller)
    {
        if (_frozenPlayers.TryGetValue(controller, out var freezeState))
        {
            var freezeTime = Server.CurrentTime - freezeState.StartTime;
            var timeRemaining = freezeState.Time - freezeTime;
            return timeRemaining;
        }
        
        return null;
    }
}
