using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace ChaseMod;

internal class KnifeCooldownManager
{
    private readonly ChaseMod _plugin;
    private readonly Dictionary<CBasePlayerController, DateTime> _invulnerablePlayers = new();

    public KnifeCooldownManager(ChaseMod chaseMod)
    {
        _plugin = chaseMod;
    }

    public void EnableHooks()
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(CBaseEntity_TakeDamageOldFuncHook, HookMode.Pre);
    }

    public void DisableHooks()
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(CBaseEntity_TakeDamageOldFuncHook, HookMode.Pre);
    }

    private HookResult CBaseEntity_TakeDamageOldFuncHook(DynamicHook hook)
    {
        var entity = hook.GetParam<CEntityInstance>(0);
        var info = hook.GetParam<CTakeDamageInfo>(1);

        if (!entity.IsValid || !info.Attacker.IsValid)
        {
            return HookResult.Continue;
        }

        if (entity.DesignerName != "player" || info.Attacker.Value!.DesignerName != "player")
        {
            return HookResult.Continue;
        }

        var attacker = info.Attacker.Value!.As<CCSPlayerPawn>();

        var pawn = entity.As<CCSPlayerPawn>();
        var controller = pawn.OriginalController.Value!;

        if (attacker.WeaponServices == null || !attacker.WeaponServices.ActiveWeapon.IsValid)
        {
            return HookResult.Continue;
        }

        var weapon = attacker.WeaponServices.ActiveWeapon.Value!;

        if (weapon.DesignerName != "weapon_knife")
        {
            return HookResult.Continue;
        }

        // if attacked player is counter-terrorist, ignore damage from knife
        if (controller.TeamNum == (byte)CsTeam.CounterTerrorist)
        {
            return HookResult.Handled;
        }

        // if attacked player is not terrorist, handle normally?
        if (controller.TeamNum != (byte)CsTeam.Terrorist)
        {
            return HookResult.Continue;
        }

        if (_invulnerablePlayers.TryGetValue(controller, out DateTime expiry))
        {
            if (expiry > DateTime.Now)
            {
                return HookResult.Handled;
            }
        }

        info.Damage = _plugin.Config.KnifeDamage;
        info.DamageFlags |= TakeDamageFlags_t.DFLAG_SUPPRESS_PHYSICS_FORCE;
        _invulnerablePlayers[controller] = DateTime.Now.AddSeconds(_plugin.Config.KnifeCooldown);
        return HookResult.Continue;
    }
}
