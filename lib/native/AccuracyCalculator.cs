using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;

namespace tosu.pp;

/// <summary>
/// Provides methods to calculate accuracy for different gamemodes based on hit results and mods.
/// </summary>
[JSExport]
public static class AccuracyCalculator
{
    /// <summary>
    /// Calculate accuracy based on hit results and mods.
    /// The beatmap's gamemode is determined by its ruleset.
    /// From https://github.com/ppy/osu-tools/blob/master/PerformanceCalculatorGUI/RulesetHelper.cs
    /// </summary>
    public static double Calculate(Beatmap beatmap, Dictionary<HitResult, int> statistics, string[] mods)
        => Calculate(
            beatmap.ruleset.RulesetInfo.OnlineID,
            beatmap.inner,
            statistics,
            mods.Select(m => beatmap.ruleset.CreateModFromAcronym(m)).Where(m => m is not null).ToArray()!
        );

    internal static double Calculate(int ruleset, IBeatmap beatmap, Dictionary<HitResult, int> statistics, Mod[] mods)
        => ruleset switch
        {
            0 => GetOsuAccuracy(beatmap, statistics),
            1 => GetTaikoAccuracy(statistics),
            2 => GetCatchAccuracy(statistics),
            3 => GetManiaAccuracy(statistics, mods),
            _ => 0.0
        };

    private static double GetOsuAccuracy(IBeatmap beatmap, Dictionary<HitResult, int> statistics)
    {
        int countGreat = statistics[HitResult.Great];
        int countGood = statistics[HitResult.Ok];
        int countMeh = statistics[HitResult.Meh];
        int countMiss = statistics[HitResult.Miss];

        double total = 6 * countGreat + 2 * countGood + countMeh;
        double max = 6 * (countGreat + countGood + countMeh + countMiss);

        if (statistics.TryGetValue(HitResult.SliderTailHit, out int countSliderTailHit))
        {
            int countSliders = beatmap.HitObjects.Count(x => x is Slider);

            total += 3 * countSliderTailHit;
            max += 3 * countSliders;
        }

        if (statistics.TryGetValue(HitResult.LargeTickMiss, out int countLargeTicksMiss))
        {
            int countLargeTicks = beatmap.HitObjects.Sum(obj => obj.NestedHitObjects.Count(x => x is SliderTick or SliderRepeat));
            int countLargeTickHit = countLargeTicks - countLargeTicksMiss;

            total += 0.6 * countLargeTickHit;
            max += 0.6 * countLargeTicks;
        }

        return total / max;
    }

    private static double GetTaikoAccuracy(Dictionary<HitResult, int> statistics)
    {
        int countGreat = statistics[HitResult.Great];
        int countGood = statistics[HitResult.Ok];
        int countMiss = statistics[HitResult.Miss];
        int total = countGreat + countGood + countMiss;

        return (double)((2 * countGreat) + countGood) / (2 * total);
    }

    private static double GetCatchAccuracy(Dictionary<HitResult, int> statistics)
    {
        double hits = statistics[HitResult.Great] + statistics[HitResult.LargeTickHit] + statistics[HitResult.SmallTickHit];
        double total = hits + statistics[HitResult.Miss] + statistics[HitResult.SmallTickMiss];

        return hits / total;
    }

    private static double GetManiaAccuracy(Dictionary<HitResult, int> statistics, Mod[] mods)
    {
        int countPerfect = statistics[HitResult.Perfect];
        int countGreat = statistics[HitResult.Great];
        int countGood = statistics[HitResult.Good];
        int countOk = statistics[HitResult.Ok];
        int countMeh = statistics[HitResult.Meh];
        int countMiss = statistics[HitResult.Miss];

        int perfectWeight = mods.Any(m => m is ModClassic) ? 300 : 305;

        double total = (perfectWeight * countPerfect) + (300 * countGreat) + (200 * countGood) + (100 * countOk) + (50 * countMeh);
        double max = perfectWeight * (countPerfect + countGreat + countGood + countOk + countMeh + countMiss);

        return total / max;
    }
}