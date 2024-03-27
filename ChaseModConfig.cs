using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ChaseMod;
public sealed class ChaseModConfig : BasePluginConfig
{
    [JsonPropertyName("Version")]
    public override int Version { get; set; } = 3;

    [JsonPropertyName("ctStartFreezeTime")] public float RoundStartFreezeTime { get; set; } = 15.0f;
    [JsonPropertyName("knifeDamage")] public int KnifeDamage { get; set; } = 50;
    [JsonPropertyName("knifeCooldown")] public float KnifeCooldown { get; set; } = 2.0f;
    [JsonPropertyName("stunThrowTime")] public float StunThrowTime { get; set; } = 2.0f;
    [JsonPropertyName("stunFreezeTime")] public float StunFreezeTime { get; set; } = 15.0f;
    [JsonPropertyName("stunFreezeRadius")] public float StunFreezeRadius { get; set; } = 500f;
    [JsonPropertyName("stunSameTeam")] public bool StunSameTeam { get; set; } = false;
    [JsonPropertyName("absvelocityWorkaroundMultiplier")] public float absvelocityWorkaroundMultiplier { get; set; } = 1.0f;
    [JsonPropertyName("maxTerroristWinStreak")] public int MaxTerroristWinStreak { get; set; } = 5;
    [JsonPropertyName("alwaysDisableTerroristKnife")] public bool AlwaysDisableTerroristKnife { get; set; } = false;
    [JsonPropertyName("enableCountDownSound")] public bool EnableCountDownSound { get; set; } = false;
    [JsonPropertyName("countDownSoundPath")] public string CountDownSoundPath { get; set; } = "sounds/player/playerping";
}