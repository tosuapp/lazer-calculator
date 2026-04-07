
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Mania.Scoring;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko.Scoring;

namespace tosu.pp.Internal;

[JSExport]
public static class HitWindows
{
    public static double GetGreatHitWindow(int mode, double od)
    {
        switch (mode)
        {
            case 0:
                {
                    var windows = new OsuHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(HitResult.Great);
                }

            case 1:
                {
                    var windows = new TaikoHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(HitResult.Great);
                }
            
            // No hit windows for catch

            case 3:
                {
                    var windows = new ManiaHitWindows();
                    windows.SetDifficulty(od);
                    return windows.WindowFor(HitResult.Great);
                }

            default: return 0;
        }
    }
}