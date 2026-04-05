using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using System.Linq;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Beatmaps;

namespace tosu.pp.Data;

[JSExport]
public readonly struct ScoreInfoData
{
    public required long TotalScore { get; init; }

    public required double Accuracy { get; init; }
    public required string[] Mods { get; init; }

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

    internal ScoreInfo ToPerformanceScoreInfo(BeatmapInfo info, Ruleset ruleset) => new(info, ruleset.RulesetInfo)
    {
        TotalScore = TotalScore,
        LegacyTotalScore = TotalScore,
        Mods = Mods.Select(ruleset.CreateModFromAcronym).Where(mod => mod is not null).ToArray()!,
        MaxCombo = MaxCombo,
        Accuracy = Accuracy,
        Statistics = CreateStatistics(),
    };

    internal static ScoreInfoData FromScoreInfo(ScoreInfo info)
    {
        return new ScoreInfoData
        {
            TotalScore = info.TotalScore,
            Accuracy = info.Accuracy,
            Mods = [.. info.Mods.Select(mod => mod.Acronym)],
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