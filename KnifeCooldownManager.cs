using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace ChaseMod;

internal class KnifeCooldownManager
{
    private readonly ChaseMod _plugin;
    private readonly Dictionary<CBasePlayerController, float> _invulnerablePlayers = new();

    public KnifeCooldownManager(ChaseMod chaseMod)
    {
        _plugin = chaseMod;
    }

    public void OnLoad()
    {
        if (_plugin.Config.EnableKnifeHook)
        {
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(CBaseEntity_TakeDamageOldFuncHook, HookMode.Pre);
        }
    }

    public void OnUnload()
    {
        if (_plugin.Config.EnableKnifeHook)
        {
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(CBaseEntity_TakeDamageOldFuncHook, HookMode.Pre);
        }
    }

    private HookResult CBaseEntity_TakeDamageOldFuncHook(DynamicHook hook)
    {
        var entity = hook.GetParam<CEntityInstance>(0);
        var info = hook.GetParam<CTakeDamageInfo>(1);

        if (!entity.IsValid || !info.Attacker.IsValid)
        {
            return HookResult.Continue;
        }

        if (entity.DesignerName != "player" || info.Attacker.Value?.DesignerName != "player")
        {
            return HookResult.Continue;
        }

        var attacker = info.Attacker.Value.As<CCSPlayerPawn>();

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

        // if attacked player is counter-terrorist or on the same team, ignore damage from knife
        if (controller.TeamNum == (byte)CsTeam.CounterTerrorist || controller.TeamNum == attacker.TeamNum)
        {
            return HookResult.Handled;
        }

        // if attacked player is not terrorist, handle normally?
        if (controller.TeamNum != (byte)CsTeam.Terrorist)
        {
            return HookResult.Continue;
        }

        if (_invulnerablePlayers.TryGetValue(controller, out float expiry))
        {
            if (expiry > Server.CurrentTime)
            {
                return HookResult.Handled;
            }
        }

        if (_plugin.Config.KnifeDamageModify)
        {
            info.Damage = _plugin.Config.KnifeDamage;
        }

        info.DamageFlags |= TakeDamageFlags_t.DFLAG_SUPPRESS_PHYSICS_FORCE;

        _invulnerablePlayers[controller] = Server.CurrentTime + _plugin.Config.KnifeCooldown;

        if (_plugin.Config.KnifeCooldown > 0.0f && pawn.Health - info.Damage > 0.0f)
        {
            var originalColor = pawn.Render;
            pawn.Render = Color.FromArgb(160, 192, 0, 0);
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

            new Timer(_plugin.Config.KnifeCooldown, () =>
            {
                if (!pawn.IsValid)
                {
                    return;
                }

                pawn.Render = originalColor;
                Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        return HookResult.Continue;
    }
}
