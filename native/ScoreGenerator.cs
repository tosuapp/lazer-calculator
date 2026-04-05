using System.Linq;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using tosu.pp.Data;
using tosu.pp.Internal;

namespace tosu.pp;

[JSExport]
public class ScoreGenerator
{
    private readonly Ruleset ruleset;
    private readonly Mod[] mods;
    private readonly IBeatmap playableBeatmap;

    internal ScoreGenerator(Ruleset ruleset, IBeatmap beatmap, Mod[] mods)
    {
        var workingBeatmap = new DiffWorkingBeatmap(beatmap);

        this.ruleset = ruleset;
        this.mods = mods;
        playableBeatmap = workingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo, mods);
    }

    private ScoreInfo createPerfectScoreInfo(IBeatmap playableBeatmap)
    {
        var scoreProcessor = ruleset.CreateScoreProcessor();
        scoreProcessor.Mods.Value = mods;
        scoreProcessor.ApplyBeatmap(playableBeatmap);

        return new ScoreInfo(playableBeatmap.BeatmapInfo, ruleset.RulesetInfo)
        {
            Passed = true,
            Accuracy = 1,
            Mods = mods,
            MaxCombo = scoreProcessor.MaximumCombo,
            Combo = scoreProcessor.MaximumCombo,
            TotalScore = scoreProcessor.MaximumTotalScore,
            Statistics = scoreProcessor.MaximumStatistics,
            MaximumStatistics = scoreProcessor.MaximumStatistics
        };
    }

    /// <summary>
    /// Create a perfect score with the current beatmap and mods, but only with the first `count` hitobjects.
    /// </summary>
    /// <param name="count"></param>
    public ScoreInfoData CreatePartialPerfectScore(int count) => ScoreInfoData.FromScoreInfo(
        createPerfectScoreInfo(
            new osu.Game.Beatmaps.Beatmap()
            {
                HitObjects = [.. playableBeatmap.HitObjects.Take(count)]
            }
        )
    );

    /// <summary>
    /// Create a perfect score with the current beatmap and mods.
    /// </summary>
    public ScoreInfoData CreatePerfectScore() => ScoreInfoData.FromScoreInfo(createPerfectScoreInfo(playableBeatmap));
}