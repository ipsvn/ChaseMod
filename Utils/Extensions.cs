using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace ChaseMod.Utils;

public static class Extensions
{
    public static float Distance(this Vector thisVector, Vector vector2)
    {
        var deltaX = vector2.X - thisVector.X;
        var deltaY = vector2.Y - thisVector.Y;
        var deltaZ = vector2.Z - thisVector.Z;

        var distanceSquared = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        var distance = (float)Math.Sqrt(distanceSquared);

        return distance;
    }

    public static void Set(this Vector thisVector, float x, float y, float z)
    {
        thisVector.X = x;
        thisVector.Y = y;
        thisVector.Z = z;
    }
    public static void Set(this Vector thisVector, Vector3 vector2)
        => Set(thisVector, vector2.X, vector2.Y, vector2.Z);
    public static void Set(this Vector thisVector, Vector vector2)
        => Set(thisVector, vector2.X, vector2.Y, vector2.Z);

    public static Vector3 ToManaged(this Vector thisVector) => new Vector3(thisVector.X, thisVector.Y, thisVector.Z);


    public static void DisableUntil(this CBasePlayerWeapon weapon, int? primaryTick, int? secondaryTick)
    {
        if (primaryTick != null)
        {
            weapon.NextPrimaryAttackTick = primaryTick.Value;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_nNextPrimaryAttackTick");
        }
        
        if (secondaryTick != null)
        {
            weapon.NextSecondaryAttackTick = secondaryTick.Value;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_nNextSecondaryAttackTick");
        }
    }
    
    public static void DisableUntil(this CBasePlayerWeapon weapon, int tick) => DisableUntil(weapon, tick, tick);

    public static void DisableAllWeaponsUntil(this CPlayer_WeaponServices weaponServices, int tick)
    {
        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            if (!weaponHandle.IsValid) continue;
            weaponHandle.Value?.DisableUntil(int.MaxValue);
        }
    }

    private static void SetMoveType(this CCSPlayerPawn pawn, MoveType_t moveType)
    {
        pawn.MoveType = moveType;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
        Schema.GetRef<MoveType_t>(pawn.Handle, "CBaseEntity", "m_nActualMoveType") = moveType;
    }
    
    public static void FreezePlayer(this CCSPlayerPawn pawn) => pawn.SetMoveType(MoveType_t.MOVETYPE_OBSOLETE);
    public static void UnfreezePlayer(this CCSPlayerPawn pawn) => pawn.SetMoveType(MoveType_t.MOVETYPE_WALK);
}
