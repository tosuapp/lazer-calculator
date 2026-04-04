using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using System.Linq;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using osu.Game.Beatmaps;
using osu.Game.Scoring.Legacy;

namespace tosu.pp.Data;

[JSExport]
public readonly struct ScoreInfoData
{
    public required int TotalScore { get; init; }

    public required double Accuracy { get; init; }
    public required IEnumerable<string> Mods { get; init; }

    public required int MaxCombo { get; init; }
    public required int LargeTickHits { get; init; }
    public required int LargeTickMisses { get; init; }
    public required int SmallTickHits { get; init; }
    public required int SmallTickMisses { get; init; }
    public required int SliderEndHits { get; init; }
    public required int NGeki { get; init; }
    public required int NKatu { get; init; }
    public required int N300 { get; init; }
    public required int N100 { get; init; }
    public required int N50 { get; init; }
    public required int Misses { get; init; }

    internal ScoreInfo ToPerformanceScoreInfo(BeatmapInfo info, Ruleset ruleset)
    {
        var scoreInfo = new ScoreInfo(info)
        {
            TotalScore = TotalScore,
            LegacyTotalScore = TotalScore,
            Mods = Mods.Select(ruleset.CreateModFromAcronym).Where(mod => mod is not null).ToArray()!,
            MaxCombo = MaxCombo,
            Accuracy = Accuracy,
        };

        scoreInfo.SetCountGeki(NGeki);
        scoreInfo.SetCountKatu(NKatu);
        scoreInfo.SetCount300(N300);
        scoreInfo.SetCount100(N100);
        scoreInfo.SetCount50(N50);
        scoreInfo.SetCountMiss(Misses);
        return scoreInfo;
    }
}