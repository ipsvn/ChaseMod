using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace ChaseMod.Utils.Memory;

public static class GrenadeFunctions
{
    // search 'smokegrenade_projectile', xref to function string used at top
    public static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, float, IntPtr> CSmokeGrenadeProjectile_CreateFunc =
        new(
            ChaseModUtils.IsLinux
                ? @"\x55\x4C\x89\xC1\x48\x89\xE5\x41\x57\x41\x56\x49\x89\xD6"
                : @"\x48\x89\x5C\x24\x2A\x48\x89\x6C\x24\x2A\x48\x89\x74\x24\x2A\x57\x41\x56\x41\x57\x48\x83\xEC\x50\x4C\x8B\xB4\x24"
        );
}
