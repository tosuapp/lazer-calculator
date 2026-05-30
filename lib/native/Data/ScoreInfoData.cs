using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace binding.Data;

[JSExport]
public readonly struct ScoreInfoData
{
    public required long TotalScore { get; init; }

    public required bool LegacyScore { get; init; }

    public required double Accuracy { get; init; }

    public required int MaxCombo { get; init; }

    public required int SliderEndHits { get; init; }
    public required int ComboBreaks { get; init; }
    public required int IgnoreHits { get; init; }
    public required int IgnoreMisses { get; init; }
    public required int LargeBonuses { get; init; }
    public required int SmallBonuses { get; init; }
    public required int LargeTickHits { get; init; }
    public required int LargeTickMisses { get; init; }
    public required int SmallTickHits { get; init; }
    public required int SmallTickMisses { get; init; }
    public required int Perfects { get; init; }
    public required int Greats { get; init; }
    public required int Goods { get; init; }
    public required int Oks { get; init; }
    public required int Mehs { get; init; }
    public required int Misses { get; init; }

    private Dictionary<HitResult, int> CreateStatistics() => new Dictionary<HitResult, int>
    {
        [HitResult.SliderTailHit] = SliderEndHits,
        [HitResult.ComboBreak] = ComboBreaks,
        [HitResult.IgnoreHit] = IgnoreHits,
        [HitResult.IgnoreMiss] = IgnoreMisses,
        [HitResult.LargeBonus] = LargeBonuses,
        [HitResult.SmallBonus] = SmallBonuses,
        [HitResult.LargeTickHit] = LargeTickHits,
        [HitResult.LargeTickMiss] = LargeTickMisses,
        [HitResult.SmallTickHit] = SmallTickHits,
        [HitResult.SmallTickMiss] = SmallTickMisses,
        [HitResult.Perfect] = Perfects,
        [HitResult.Great] = Greats,
        [HitResult.Good] = Goods,
        [HitResult.Ok] = Oks,
        [HitResult.Meh] = Mehs,
        [HitResult.Miss] = Misses,
    };

    internal ScoreInfo ToPerformanceScoreInfo(PlayBeatmap beatmap, Ruleset ruleset)
    {
        ScoreInfo info = new(beatmap.inner.BeatmapInfo, ruleset.RulesetInfo)
        {
            TotalScore = TotalScore,
            Mods = beatmap.Mods,
            MaxCombo = MaxCombo,
            Accuracy = Accuracy,
            Statistics = CreateStatistics(),
        };
        if (LegacyScore)
        {
            info.IsLegacyScore = true;
            info.LegacyTotalScore = TotalScore;
        }

        return info;
    }

    internal static ScoreInfoData FromScoreInfo(ScoreInfo info)
    {
        return new ScoreInfoData
        {
            TotalScore = info.TotalScore,
            LegacyScore = info.IsLegacyScore,
            Accuracy = info.Accuracy,
            MaxCombo = info.MaxCombo,
            SliderEndHits = info.Statistics.GetValueOrDefault(HitResult.SliderTailHit),
            ComboBreaks = info.Statistics.GetValueOrDefault(HitResult.ComboBreak),
            IgnoreHits = info.Statistics.GetValueOrDefault(HitResult.IgnoreHit),
            IgnoreMisses = info.Statistics.GetValueOrDefault(HitResult.IgnoreMiss),
            LargeBonuses = info.Statistics.GetValueOrDefault(HitResult.LargeBonus),
            SmallBonuses = info.Statistics.GetValueOrDefault(HitResult.SmallBonus),
            LargeTickHits = info.Statistics.GetValueOrDefault(HitResult.LargeTickHit),
            LargeTickMisses = info.Statistics.GetValueOrDefault(HitResult.LargeTickMiss),
            SmallTickHits = info.Statistics.GetValueOrDefault(HitResult.SmallTickHit),
            SmallTickMisses = info.Statistics.GetValueOrDefault(HitResult.SmallTickMiss),
            Perfects = info.Statistics.GetValueOrDefault(HitResult.Perfect),
            Greats = info.Statistics.GetValueOrDefault(HitResult.Great),
            Goods = info.Statistics.GetValueOrDefault(HitResult.Good),
            Oks = info.Statistics.GetValueOrDefault(HitResult.Ok),
            Mehs = info.Statistics.GetValueOrDefault(HitResult.Meh),
            Misses = info.Statistics.GetValueOrDefault(HitResult.Miss),
        };
    }
}