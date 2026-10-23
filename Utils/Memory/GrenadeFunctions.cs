using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace ChaseMod.Utils.Memory;

public static class GrenadeFunctions
{
    // search 'smokegrenade_projectile', xref to function with string used at top
    public static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, CSmokeGrenadeProjectile> CSmokeGrenadeProjectile_CreateFunc =
        new(GameData.GetSignature("CSmokeGrenadeProjectile_CreateFunc"));
}
