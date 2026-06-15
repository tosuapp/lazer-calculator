using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Difficulty;
using binding.Data;
using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using binding.Internal;

namespace binding;

[JSExport]
public class GradualDifficulty
{
    private readonly PlayBeatmap beatmap;
    private readonly GradualDifficultyEnumerator inner;

    internal GradualDifficulty(PlayBeatmap beatmap, GradualDifficultyEnumerator inner)
    {
        this.beatmap = beatmap;
        this.inner = inner;
    }

    public bool Advance()
    {
        return inner.Advance();
    }

    public void SkipToTime(double time)
    {
        inner.SkipToTime(time);
    }

    public void Skip(int offset)
    {
        inner.Skip(offset);
    }

    public void SkipToEnd()
    {
        inner.SkipToEnd();
    }

    public DifficultyAttrs CreateDifficultyAttrs()
    {
        return new(inner.CreateDifficultyAttributes());
    }

    /// <summary>
    /// Calculates the accuracy up to current sections based on hit results.
    /// </summary>=
    public double CalculateProgressiveAccuracy(ScoreInfoData data)
    {
        return AccuracyCalculator.Calculate(beatmap.Mode, inner.ProgressiveBeatmap, data.CreateStatistics(), beatmap.Mods);
    }

    /// <summary>
    /// Simulates a score with the given accuracy up to current sections.
    /// </summary>
    public ScoreInfoData CreateProgressiveScore(double accuracy)
    {
        return ScoreInfoData.FromScoreInfo(ScoreSimulator.CreateScoreInfo(beatmap.ruleset, inner.ProgressiveBeatmap, beatmap.Mods, accuracy));
    }

    /// <summary>
    /// Gets the strains of the current difficulty section.
    /// </summary>
    public StrainsData GetCurrentStrains() => StrainsData.FromSkills(inner.Skills);
}