using System;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Difficulty;
using binding.Data;
using binding.Internal;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;

namespace binding;

[JSExport]
public class GradualDifficulty : IDisposable
{
    private readonly Ruleset ruleset;
    private readonly Mod[] mods;
    private GradualDifficultyEnumerator? inner;

    private GradualDifficultyEnumerator Inner =>
        inner ?? throw new ObjectDisposedException(nameof(GradualDifficulty));

    internal GradualDifficulty(Ruleset ruleset, Mod[] mods, GradualDifficultyEnumerator inner)
    {
        this.ruleset = ruleset;
        this.mods = mods;
        this.inner = inner;
    }

    public bool Advance()
    {
        return Inner.Advance();
    }

    /// <summary>
    /// Skips hit objects up to the specified time.
    /// </summary>
    /// <param name="time">The time to skip to, in milliseconds.</param>
    /// <returns>The number of hit objects skipped.</returns>
    public int SkipToTime(double time)
    {
        return Inner.SkipToTime(time);
    }

    /// <summary>
    /// Skips the specified number of hit objects.
    /// </summary>
    /// <param name="offset">The number of hit objects to skip.</param>
    public void Skip(int offset)
    {
        Inner.Skip(offset);
    }

    /// <summary>
    /// Skips all remaining hit objects.
    /// </summary>
    public void SkipToEnd()
    {
        Inner.SkipToEnd();
    }

    public DifficultyAttrs CreateDifficultyAttrs()
    {
        return new(Inner.CreateDifficultyAttributes());
    }

    /// <summary>
    /// Calculates the accuracy up to current sections based on hit results.
    /// </summary>
    public double CalculateProgressiveAccuracy(ScoreInfoData data)
    {
        return AccuracyCalculator.Calculate(ruleset.RulesetInfo.OnlineID, Inner.ProgressiveBeatmap, data.CreateStatistics(), mods);
    }

    /// <summary>
    /// Simulates a score with the given accuracy up to current sections.
    /// Generated score only have hit results and accuracy.
    /// </summary>
    public ScoreInfoData CreateProgressiveScore(double accuracy)
    {
        return ScoreInfoData.FromScoreInfo(ScoreSimulator.CreateScoreInfo(ruleset, Inner.ProgressiveBeatmap, mods, accuracy));
    }

    /// <summary>
    /// Gets the strains of the current difficulty section.
    /// </summary>
    public StrainsData GetCurrentStrains() => StrainsData.FromEnumerator(Inner);

    /// <summary>
    /// Releases the underlying gradual difficulty enumerator and its progressive
    /// beatmap state.
    /// </summary>
    /// <remarks>
    /// The JS proxy created for this instance keeps the .NET object alive across the
    /// interop boundary, so the enumerator (which references the playable beatmap and
    /// every difficulty hit object/skill) would otherwise remain rooted for the module
    /// lifetime. Call this once the instance is no longer needed. The instance must not
    /// be used afterwards.
    /// </remarks>
    public void Dispose()
    {
        inner = null;
    }
}
