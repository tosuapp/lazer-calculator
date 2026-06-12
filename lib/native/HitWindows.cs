using System;
using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Mania.Scoring;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko.Scoring;

namespace binding.Internal;

[JSExport]
public enum OsuHitResult
{
    None = HitResult.None,
    Miss = HitResult.Miss,
    Meh = HitResult.Meh,
    Ok = HitResult.Ok,
    Good = HitResult.Good,
    Great = HitResult.Great,
    Perfect = HitResult.Perfect,
    SmallTickMiss = HitResult.SmallTickMiss,
    SmallTickHit = HitResult.SmallTickHit,
    LargeTickMiss = HitResult.LargeTickMiss,
    LargeTickHit = HitResult.LargeTickHit,
    SmallBonus = HitResult.SmallBonus,
    LargeBonus = HitResult.LargeBonus,
    IgnoreMiss = HitResult.IgnoreMiss,
    IgnoreHit = HitResult.IgnoreHit,
    ComboBreak = HitResult.ComboBreak,
    SliderTailHit = HitResult.SliderTailHit,
    LegacyComboIncrease = HitResult.LegacyComboIncrease,
};


[JSExport]
public static class HitWindows
{
    public static Dictionary<OsuHitResult, double> All(int mode, double od)
    {
        osu.Game.Rulesets.Scoring.HitWindows windows = mode switch
        {
            0 => new OsuHitWindows(),
            1 => new TaikoHitWindows(),
            3 => new ManiaHitWindows(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };

        windows.SetDifficulty(od);

        var result = new Dictionary<OsuHitResult, double>();
        foreach (var (hitResult, length) in windows.GetAllAvailableWindows())
            result[(OsuHitResult)hitResult] = length;

        return result;
    }

    public static double GetGreatHitWindow(int mode, double od)
    {
        return GetHitWindow(mode, od, OsuHitResult.Great);
    }

    public static double GetHitWindow(int mode, double od, OsuHitResult hitResult)
    {
        var result = (HitResult)hitResult;
        switch (mode)
        {
            case 0:
                {
                    var windows = new OsuHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(result);
                }

            case 1:
                {
                    var windows = new TaikoHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(result);
                }

            // No hit windows for catch

            case 3:
                {
                    var windows = new ManiaHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(result);
                }

            default: return 0;
        }
    }
}