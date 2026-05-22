using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Beatmaps;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using binding.Data;
using binding.Internal;
using Decoder = osu.Game.Beatmaps.Formats.Decoder;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;

namespace binding;

/// <summary>
/// A beatmap with a ruleset and applied mods.
/// </summary>
[JSExport]
public class PlayBeatmap
{
    internal readonly IBeatmap inner;
    internal readonly Ruleset ruleset;

    internal Mod[] Mods;

    /// <summary>
    /// The online ID of the current beatmap's ruleset. Also known as legacy gamemode ID.
    /// </summary>
    public int Mode => ruleset.RulesetInfo.OnlineID;

    private PlayBeatmap(IBeatmap inner, Ruleset ruleset)
    {
        this.inner = inner;
        this.ruleset = ruleset;
        Mods = [];
    }

    /// <summary>
    /// Set beatmap mods.
    /// </summary>
    public void ApplyMods(LazerMod[] mods)
    {
        Mods = [.. mods.Select(m => m.ToMod(ruleset))];
    }

    /// <summary>
    /// Perform beatmap conversion to another gamemode.
    /// Applied mods will not be retained to returned beatmap.
    /// </summary>
    public PlayBeatmap? Convert(int gameMode)
    {
        var ruleset = Rulesets.FromLegacyGameMode(gameMode);
        if (ruleset is null)
        {
            return null;
        }

        var converter = ruleset.CreateBeatmapConverter(inner);
        if (!converter.CanConvert())
        {
            return null;
        }

        return new(converter.Convert(), ruleset);
    }

    /// <summary>
    /// Get the original beatmap difficulty without any mods applied.
    /// </summary>
    /// <returns></returns>
    public BeatmapDifficultyData GetOriginalBeatmapDifficulty() =>
        BeatmapDifficultyData.FromDifficulty(inner.BeatmapInfo.Difficulty);

    /// <summary>
    /// Get beatmap difficulty with current mods applied
    /// </summary>
    public BeatmapDifficultyData GetBeatmapDifficulty() =>
        BeatmapDifficultyData.FromDifficulty(
            ruleset.GetAdjustedDisplayDifficulty(
                inner.BeatmapInfo,
                Mods
            )
        );

    /// <summary>
    /// Create gradual difficulty calculator with current mods applied.
    /// </summary>
    public GradualDifficulty CreateGradualDifficulty()
    {
        return new GradualDifficulty(
            ruleset.CreateDifficultyCalculator(new DiffWorkingBeatmap(inner)).CreateGradualDifficulty(Mods)
        );
    }

    /// <summary>
    /// Calculate performance
    /// </summary>
    public PerformanceAttrsData CalculatePerformance(
        DifficultyAttrs attrs,
        ScoreInfoData score
    )
    {
        var calc = ruleset.CreatePerformanceCalculator();
        if (calc is null)
        {
            return default;
        }

        return PerformanceAttrsData.FromAttrs(
            calc.Calculate(
                score.ToPerformanceScoreInfo(this, ruleset),
                attrs.Inner
            )
        );
    }

    /// <summary>
    /// Parse string osu file into Beatmap
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static PlayBeatmap Parse(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        using var reader = new LineBufferedReader(new MemoryStream(bytes));
        IBeatmap beatmap = Decoder.GetDecoder<OsuBeatmap>(reader).Decode(reader);
        var rulesetId = beatmap.BeatmapInfo.Ruleset.OnlineID;
        var ruleset = Rulesets.FromLegacyGameMode(rulesetId) ?? throw new InvalidOperationException("Invalid ruleset: " + rulesetId);
        // Perform conversion from legacy beatmap.
        var converter = ruleset.CreateBeatmapConverter(beatmap);
        if (converter.CanConvert())
        {
            beatmap = converter.Convert();
        }

        return new(beatmap, ruleset);
    }
}