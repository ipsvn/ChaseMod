using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using System.Runtime.InteropServices;

namespace ChaseMod.Utils.Memory;

public class CCSMatch
{
    private static nint MATCH_OFFSET = 0xF3C;

    // string search '#SFUIHUD_InfoPanel_Coop_DeployMissionBust', xref and takefunc before if statement containing + 0xF3C in
    // param_1 and value 1 in param_2, follow the last function called taking param_1 (just hope this offset doesn't change)
    // TODO: I can remove probably this sig later
    private static MemoryFunctionVoid<IntPtr> CCSMatch_UpdateTeamScores =
        new(
            ChaseModUtils.IsLinux
                ? @"\x55\x48\x89\xE5\x41\x56\x41\x55\x49\x89\xFD\xBF\x02\x00\x00\x00"
                : @"\x48\x89\x5C\x24\x2A\x48\x89\x74\x24\x2A\x48\x89\x7C\x24\x2A\x41\x56\x48\x83\xEC\x20\x48\x8B\xF9\xB9\x02\x00\x00\x00"
        );
    
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

        Marshal.StructureToPtr(marshallMatch, structOffset, true);
        CCSMatch_UpdateTeamScores.Invoke(structOffset);
    }
}
