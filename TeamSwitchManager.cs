using CounterStrikeSharp.API.Core;
using ChaseMod.Utils;
using CounterStrikeSharp.API.Modules.Utils;
using ChaseMod.Utils.Memory;

namespace ChaseMod;

internal class TeamSwitchManager
{
    private bool _switchingTeams;
    private int _terroristWinstreak;
    
    private CsTeam _lastWinningTeam = CsTeam.Terrorist;
    private bool _shouldSwitchTeams;

    private readonly ChaseMod _plugin;

    public TeamSwitchManager(ChaseMod chaseMod)
    {
        _plugin = chaseMod;
    }

    public void Start()
    {
        // TODO: Combine this _lastWinningTeam check logic in separate function since we're duplicating the logic atm.
        _plugin.RegisterEventHandler<EventRoundEnd>((@event, info) =>
        {
            _lastWinningTeam = (CsTeam)@event.Winner;
            _shouldSwitchTeams = false;
            
            switch (_lastWinningTeam)
            {
                case CsTeam.CounterTerrorist:
                    ChaseModUtils.ChatAllPrefixed($"{ChatColors.Blue}CT {ChatColors.Grey}Win - Teams are being switched.");
                    _terroristWinstreak = 0;
                    _shouldSwitchTeams = true;
                    break;
                
                case CsTeam.Terrorist:
                    _terroristWinstreak++;
                    if (_plugin.Config.MaxTerroristWinStreak > 0 && _terroristWinstreak >= _plugin.Config.MaxTerroristWinStreak)
                    {
                        ChaseModUtils.ChatAllPrefixed($"{ChatColors.Yellow}T {ChatColors.Grey}Win - Teams are being switched due to winstreak. ({_terroristWinstreak} wins in a row)");
                        _terroristWinstreak = 0;
                        _shouldSwitchTeams = true;
                    }
                    else
                    {
                        ChaseModUtils.ChatAllPrefixed($"{ChatColors.Yellow}T {ChatColors.Grey}Win");
                    }
                    break;
            }
            
            return HookResult.Continue;
        });
        
        _plugin.RegisterEventHandler<EventRoundPrestart>((@event, info) =>
        {
            if (_shouldSwitchTeams)
            {
                SwitchTeams();
            }
            
            return HookResult.Continue;
        });

        _plugin.RegisterEventHandler<EventPlayerTeam>((@event, info) =>
        {
            if (_switchingTeams)
            {
                info.DontBroadcast = true;
            }

            return HookResult.Continue;
        }, HookMode.Pre);

        _plugin.RegisterListener<Listeners.OnMapEnd>(() =>
        {
            _terroristWinstreak = 0;
        });
    }
    
    private void SwitchTeams()
    {
        _switchingTeams = true;
        
        CCSMatch.SwapTeamScores(
            ChaseModUtils.GetGameRules()
        );

        foreach (var controller in ChaseModUtils.GetAllRealPlayers())
        {
            switch (controller.Team)
            {
                case CsTeam.CounterTerrorist:
                    controller.SwitchTeam(CsTeam.Terrorist);
                    break;
                
                case CsTeam.Terrorist:
                    controller.SwitchTeam(CsTeam.CounterTerrorist);
                    break;
            }
            controller.RemoveAllItemsOnNextRoundReset = true;
        }
        _switchingTeams = false;
    }
}
