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
using System.Threading;

namespace binding;

/// <summary>
/// A beatmap with a ruleset and applied mods.
/// </summary>
[JSExport]
public class PlayBeatmap
{
    private readonly FlatWorkingBeatmap workingBeatmap;

    internal readonly Ruleset ruleset;

    internal Mod[] Mods { get; private set; }

    /// <summary>
    /// The online ID of the current beatmap's ruleset. Also known as legacy gamemode ID.
    /// </summary>
    public int Mode => ruleset.RulesetInfo.OnlineID;

    private PlayBeatmap(FlatWorkingBeatmap workingBeatmap, Ruleset ruleset)
    {
        this.workingBeatmap = workingBeatmap;
        this.ruleset = ruleset;
        Mods = [];
    }

    /// <summary>
    /// Set beatmap mods.
    /// </summary>
    public void ApplyMods(LazerMod[] mods)
    {
        InvalidatePlayableBeatmap();
        Mods = [.. mods.Select(m => m.ToMod(ruleset))];
    }

    private IBeatmap? cachedPlayableBeatmap;

    /// <summary>
    /// Invalidate cached playable beatmap. Should be called when changing mods.
    /// </summary>
    private void InvalidatePlayableBeatmap() => cachedPlayableBeatmap = null;

    /// <summary>
    /// Lazily construct or get beatmap with current mods and ruleset applied.
    /// </summary>
    internal IBeatmap GetPlayableBeatmap()
    {
        if (cachedPlayableBeatmap != null)
        {
            return cachedPlayableBeatmap;
        }

        return cachedPlayableBeatmap = workingBeatmap.GetPlayableBeatmap(
            ruleset.RulesetInfo,
            Mods,
            CancellationToken.None
        );
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

        return new(workingBeatmap, ruleset);
    }

    /// <summary>
    /// Get the original beatmap difficulty without any mods applied.
    /// </summary>
    /// <returns></returns>
    public BeatmapDifficultyData GetOriginalBeatmapDifficulty() =>
        BeatmapDifficultyData.FromDifficulty(workingBeatmap.BeatmapInfo.Difficulty);

    /// <summary>
    /// Get beatmap difficulty with current mods applied
    /// </summary>
    public BeatmapDifficultyData GetBeatmapDifficulty() =>
        BeatmapDifficultyData.FromDifficulty(
            ruleset.GetAdjustedDisplayDifficulty(
                GetPlayableBeatmap().BeatmapInfo,
                Mods
            )
        );

    /// <summary>
    /// Create gradual difficulty calculator with current mods applied.
    /// </summary>
    public GradualDifficulty CreateGradualDifficulty()
    {
        return new GradualDifficulty(
            ruleset.CreateDifficultyCalculator(
                new FlatWorkingBeatmap(GetPlayableBeatmap())
            ).CreateGradualDifficulty(Mods)
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
        Beatmap beatmap = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
        var rulesetId = beatmap.BeatmapInfo.Ruleset.OnlineID;
        var ruleset = Rulesets.FromLegacyGameMode(rulesetId) ?? throw new InvalidOperationException("Invalid ruleset: " + rulesetId);

        return new(new(beatmap), ruleset);
    }
}
