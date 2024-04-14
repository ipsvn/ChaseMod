using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using ChaseMod.Utils.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using ChaseMod.Utils;
using CounterStrikeSharp.API;

namespace ChaseMod;

internal class NadeManager
{
    private readonly ChaseMod _plugin;
    private readonly PlayerFreezeManager _playerFreezeManager;
    private readonly RoundStartFreezeTimeManager _roundStartFreezeTimeManager;
    public NadeManager(
        ChaseMod chaseMod, PlayerFreezeManager playerFreezeManager,
        RoundStartFreezeTimeManager roundStartFreezeTimeManager)
    {
        _plugin = chaseMod;
        _playerFreezeManager = playerFreezeManager;
        _roundStartFreezeTimeManager = roundStartFreezeTimeManager;
    }

    public void EnableHooks()
    {
        GrenadeFunctions.CSmokeGrenadeProjectile_CreateFunc.Hook(CSmokeGrenadeProjectile_CreateHook, HookMode.Post);
        _plugin.RegisterEventHandler<EventPlayerBlind>((@event, info) =>
        {
            if (!ChaseModUtils.IsRealPlayer(@event.Attacker) || !ChaseModUtils.IsRealPlayer(@event.Userid))
            {
                return HookResult.Continue;
            }

            if (@event.Attacker.Team == @event.Userid.Team)
            {
                @event.Userid.PlayerPawn.Value!.BlindUntilTime = 0;
            }

            return HookResult.Continue;
        });
    }

    public void DisableHooks()
    {
        GrenadeFunctions.CSmokeGrenadeProjectile_CreateFunc.Unhook(CSmokeGrenadeProjectile_CreateHook, HookMode.Post);
    }

    private HookResult CSmokeGrenadeProjectile_CreateHook(DynamicHook hook)
    {
        ChaseMod.Logger.LogDebug("Freezenade thrown");

        var smoke = hook.GetReturn<CSmokeGrenadeProjectile>();
        smoke.NextThinkTick = -1;
        Utilities.SetStateChanged(smoke, "CBaseEntity", "m_nNextThinkTick");

        _plugin.AddTimer(_plugin.Config.StunThrowTime, () =>
        {
            Server.NextFrame(() =>
            {
                FreezeGrenadeExplode(smoke);
            });
        });

        return HookResult.Continue;
    }

    private void FreezeGrenadeExplode(CSmokeGrenadeProjectile smoke)
    {
        ChaseMod.Logger.LogDebug("Freezenade explode");

        if (!smoke.IsValid)
        {
            return;
        }

        if (_roundStartFreezeTimeManager.IsInFreezeTime())
        {
            smoke.Remove();
            return;
        }

        var smokeProjectileOrigin = smoke.AbsOrigin;
        if (smokeProjectileOrigin == null)
        {
            return;
        }

        var thrower = smoke.OwnerEntity;
        if (!thrower.IsValid || thrower.Value == null)
        {
            return;
        }

        var players = ChaseModUtils.GetAllRealPlayers();
        foreach (var player in players)
        {
            var pawn = player.PlayerPawn.Value!;
            if (pawn.LifeState != (byte)LifeState_t.LIFE_ALIVE)
            {
                continue;
            }

            if (!_plugin.Config.StunSameTeam && player.TeamNum == thrower.Value.TeamNum)
            {
                continue;
            }

            var playerOrigin = pawn.AbsOrigin;
            if (playerOrigin == null)
            {
                ChaseMod.Logger.LogWarning("Freezenade: other pawn has null AbsOrigin");
                continue;
            }

            var distance = playerOrigin.Distance(smokeProjectileOrigin);
            ChaseMod.Logger.LogDebug($"Distance between FreezeNade and {player.PlayerName} = {distance}");

            if (distance > _plugin.Config.StunFreezeRadius)
            {
                continue;
            }

            _playerFreezeManager.Freeze(player, _plugin.Config.StunFreezeTime, true, true, false);
        }

        smoke.Remove();
    }
}
