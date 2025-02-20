using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ChaseMod;

public sealed class FreezeRingParticle
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; } = false;
    [JsonPropertyName("vpcfFile")] public string VpcfFile { get; set; } = "particles/example/freezering.vpcf";
    [JsonPropertyName("lifetime")] public float Lifetime { get; set; } = 0.35f;
}

public sealed class ChaseModConfig : BasePluginConfig
{
    [JsonPropertyName("ConfigVersion")]
    public override int Version { get; set; } = 4;

    [JsonPropertyName("enableKnifeHook")] public bool EnableKnifeHook { get; set; } = true;
    [JsonPropertyName("knifeDamageModify")] public bool KnifeDamageModify { get; set; } = true;
    [JsonPropertyName("knifeDamage")] public int KnifeDamage { get; set; } = 50;
    [JsonPropertyName("knifeCooldown")] public float KnifeCooldown { get; set; } = 2.0f;
    [JsonPropertyName("stunThrowTime")] public float StunThrowTime { get; set; } = 2.0f;
    [JsonPropertyName("stunFreezeTime")] public float StunFreezeTime { get; set; } = 15.0f;
    [JsonPropertyName("stunFreezeRadius")] public float StunFreezeRadius { get; set; } = 500f;
    [JsonPropertyName("stunSameTeam")] public bool StunSameTeam { get; set; } = false;

    [JsonPropertyName("enableTeamSwitchingConditions")] public bool EnableTeamSwitchingConditions { get; set; } = true;
    [JsonPropertyName("maxTerroristWinStreak")] public int MaxTerroristWinStreak { get; set; } = 5;

    [JsonPropertyName("enableKnifeDisabling")] public bool EnableKnifeDisabling { get; set; } = true;
    [JsonPropertyName("alwaysDisableTerroristKnife")] public bool AlwaysDisableTerroristKnife { get; set; } = false;

    [JsonPropertyName("ctStartFreezeTime")] public float RoundStartFreezeTime { get; set; } = 15.0f;
    [JsonPropertyName("enableFreezeTimeCountDownSound")] public bool EnableFreezeTimeCountDownSound { get; set; } = false;
    [JsonPropertyName("freezeTimeCountDownSoundPath")] public string FreezeTimeCountDownSoundPath { get; set; } = "sounds/player/playerping";

    [JsonPropertyName("absvelocityWorkaroundMultiplier")] public float absvelocityWorkaroundMultiplier { get; set; } = 1.0f;
    [JsonPropertyName("disableBoostTriggers")] public bool DisableBoostTriggers { get; set; } = true;

    [JsonPropertyName("freezeRingParticle")] public FreezeRingParticle FreezeRingParticle { get; set; } = new();
}