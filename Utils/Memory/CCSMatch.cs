﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Runtime.InteropServices;

namespace ChaseMod.Utils.Memory;

public class CCSMatch
{
    // to find offset:
    // xref convar mp_default_team_winner_no_objective, used in 2 if statements
    // with value 2 and 3 (t and ct), the first function inside this has first
    // argument like `+0xef4` and `1` as second argument. 0xef4 is the offset
    private static nint MATCH_OFFSET = 0xEEC;

    [StructLayout(LayoutKind.Sequential)]
    public struct MCCSMatch
    {
        public short m_totalScore;
        public short m_actualRoundsPlayed;
        public short m_nOvertimePlaying;
        public short m_ctScoreFirstHalf;
        public short m_ctScoreSecondHalf;
        public short m_ctScoreOvertime;
        public short m_ctScoreTotal;
        public short m_terroristScoreFirstHalf;
        public short m_terroristScoreSecondHalf;
        public short m_terroristScoreOvertime;
        public short m_terroristScoreTotal;
        public short unknown;
        public int m_phase;
    }

    public static void SwapTeamScores(CCSGameRules gameRules)
    {
        var structOffset = gameRules.Handle + MATCH_OFFSET;

        var marshallMatch = Marshal.PtrToStructure<MCCSMatch>(structOffset);

        var temp = marshallMatch.m_terroristScoreFirstHalf;
        marshallMatch.m_terroristScoreFirstHalf = marshallMatch.m_ctScoreFirstHalf;
        marshallMatch.m_ctScoreFirstHalf = temp;

        temp = marshallMatch.m_terroristScoreSecondHalf;
        marshallMatch.m_terroristScoreSecondHalf = marshallMatch.m_ctScoreSecondHalf;
        marshallMatch.m_ctScoreSecondHalf = temp;

        temp = marshallMatch.m_terroristScoreOvertime;
        marshallMatch.m_terroristScoreOvertime = marshallMatch.m_ctScoreOvertime;
        marshallMatch.m_ctScoreOvertime = temp;

        temp = marshallMatch.m_terroristScoreTotal;
        marshallMatch.m_terroristScoreTotal = marshallMatch.m_ctScoreTotal;
        marshallMatch.m_ctScoreTotal = temp;

        UpdateTeamScores(marshallMatch);
        Marshal.StructureToPtr(marshallMatch, structOffset, true);
    }

    public static void UpdateTeamScores(MCCSMatch match)
    {
        var teams = Utilities.FindAllEntitiesByDesignerName<CCSTeam>("cs_team_manager");

        var terrorists = teams.First(team => (CsTeam)team.TeamNum == CsTeam.Terrorist);
        var cts = teams.First(team => (CsTeam)team.TeamNum == CsTeam.CounterTerrorist);

        foreach(var team in teams)
        {
            var csTeam = (CsTeam) team.TeamNum;

            if(csTeam == CsTeam.Terrorist)
            {
                team.Score = match.m_terroristScoreTotal;
                team.ScoreFirstHalf = match.m_terroristScoreFirstHalf;
                team.ScoreSecondHalf = match.m_terroristScoreSecondHalf;
                team.ScoreOvertime = match.m_terroristScoreOvertime;
            }
            else if (csTeam == CsTeam.CounterTerrorist)
            {
                team.Score = match.m_ctScoreTotal;
                team.ScoreFirstHalf = match.m_ctScoreFirstHalf;
                team.ScoreSecondHalf = match.m_ctScoreSecondHalf;
                team.ScoreOvertime = match.m_ctScoreOvertime;
            }
            Utilities.SetStateChanged(team, "CTeam", "m_iScore");
            Utilities.SetStateChanged(team, "CCSTeam", "m_scoreFirstHalf");
            Utilities.SetStateChanged(team, "CCSTeam", "m_scoreSecondHalf");
            Utilities.SetStateChanged(team, "CCSTeam", "m_scoreOvertime");
        }
    }
}
