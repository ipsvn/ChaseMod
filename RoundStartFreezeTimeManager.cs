using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using ChaseMod.Utils;

using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using CounterStrikeSharp.API.Modules.Cvars;

namespace ChaseMod;

internal class RoundStartFreezeTimeManager
{
    private readonly ChaseMod _plugin;
    private readonly PlayerFreezeManager _playerFreezeManager;
    private float _roundStartTime;
    private float? _normalFalldamageScale;

    private float FrozenUntilTime => _roundStartTime + _plugin.Config.RoundStartFreezeTime;
    private float FrozenTimeLeft => FrozenUntilTime - Server.CurrentTime;
    private string CountDownSoundPath => _plugin.Config.FreezeTimeCountDownSoundPath;
    private bool EnableCountDownSound => _plugin.Config.EnableFreezeTimeCountDownSound;

    private Timer? _countdownTimer;
    private Timer? _soundTimer;

    public RoundStartFreezeTimeManager(ChaseMod chaseMod, PlayerFreezeManager playerFreezeManager)
    {
        _plugin = chaseMod;
        _playerFreezeManager = playerFreezeManager;
    }

    public void Start()
    {
        _plugin.RegisterEventHandler<EventRoundFreezeEnd>((@event, info) =>
        {
            if (ChaseModUtils.GetGameRules().WarmupPeriod)
            {
                return HookResult.Continue;
            }

            _roundStartTime = Server.CurrentTime;

            if (_plugin.Config.RoundStartFreezeTime <= 0)
            {
                return HookResult.Continue;
            }

            SwitchFallDamage(false);

            foreach (var player in ChaseModUtils.GetAllRealPlayers().Where(p => p.Team == CsTeam.CounterTerrorist))
            {
                _playerFreezeManager.Freeze(player, _plugin.Config.RoundStartFreezeTime, 
                                            true, false, true);
            }

            if (_countdownTimer != null)
            {
                _countdownTimer.Kill();
                _countdownTimer = null;
            }

            if (_soundTimer != null)
            {
                _soundTimer?.Kill();
                _soundTimer = null;
            }

            if (EnableCountDownSound)
            {
                _soundTimer = _plugin.AddTimer(1.0f, PlaySoundTimer, 
                                               TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
            }

            _countdownTimer = _plugin.AddTimer(0.1f, CountdownTimerTick, 
                                               TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);

            return HookResult.Continue;
        });
    }

    private void CountdownTimerTick()
    {
        if (FrozenTimeLeft <= 0)
        {
            SwitchFallDamage(true);
            _countdownTimer?.Kill();
            _countdownTimer = null;
        }

        var text = FrozenTimeLeft > 0 ? $"Round begins in {FrozenTimeLeft:0.0} seconds!" : "Round start!";
        foreach (var player in ChaseModUtils.GetAllRealPlayers())
        {
            player.PrintToCenter(text);
        }
    }

    private void PlaySoundTimer()
    {
        if (!IsInFreezeTime())
        {
            _soundTimer?.Kill();
            _soundTimer = null;
        }

        foreach (var player in ChaseModUtils.GetAllRealPlayers())
        {
            player.ExecuteClientCommand($"play {CountDownSoundPath}");
        }
    }

    private void SwitchFallDamage(bool enabled)
    {
        if (enabled)
        {
            // convar SetValue isn't replicated on clients it seems...
            Server.ExecuteCommand($"sv_falldamage_scale {_normalFalldamageScale ?? 1}");
        }
        else
        {
            if (_normalFalldamageScale == null)
            {
                var falldamageScale = ConVar.Find("sv_falldamage_scale");
                if (falldamageScale != null)
                {
                    _normalFalldamageScale = falldamageScale.GetPrimitiveValue<float>();
                }
            }

            Server.ExecuteCommand($"sv_falldamage_scale 0");
        }
    }

    public bool IsInFreezeTime()
    {
        return FrozenTimeLeft > 0.0f;
    }
}
