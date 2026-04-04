using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Beatmaps;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using tosu.pp.Data;
using tosu.pp.Internal;
using Decoder = osu.Game.Beatmaps.Formats.Decoder;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;

namespace tosu.pp;

[JSExport]
public class Beatmap
{
    private readonly IBeatmap inner;
    private readonly Ruleset ruleset;

    /// <summary>
    /// The online ID of the current beatmap's ruleset. Also known as legacy gamemode ID.
    /// </summary>
    public int Mode => ruleset.RulesetInfo.OnlineID;

    /// <summary>
    /// Total number of hit objects in the beatmap.
    /// </summary>
    public int HitObjectCount => inner.HitObjects.Count;

    private Beatmap(IBeatmap inner, Ruleset ruleset)
    {
        this.inner = inner;
        this.ruleset = ruleset;
    }

    /// <summary>
    /// Perform beatmap conversion to another gamemode
    /// </summary>
    public Beatmap? Convert(int gameMode)
    {
        var ruleset = Rulesets.FromLegacyGameMode(gameMode);
        if (ruleset is null)
        {
            return null;
        }

        var converted = ruleset.CreateBeatmapConverter(inner).Convert();
        if (converted is null)
        {
            return null;
        }
        return new(converted, ruleset);
    }

    /// <summary>
    /// Get beatmap difficulty with mods applied
    /// </summary>
    public BeatmapDifficultyData GetBeatmapDifficulty(IEnumerable<string> mods)
    {
        var diff = inner.BeatmapInfo.Difficulty;
        foreach (var mod in mods)
        {
            var m = ruleset.CreateModFromAcronym(mod);
            if (m is IApplicableToDifficulty diffMod)
            {
                diffMod.ApplyToDifficulty(diff);
            }
        }

        return BeatmapDifficultyData.FromDifficulty(diff);
    }

    /// <summary>
    /// Create gradual difficulty calculator
    /// </summary>
    /// <param name="mods">mods to apply to the difficulty calculation.</param>
    public GradualDifficulty CreateGradualDifficultyCalculator(IEnumerable<string> mods)
    {
        return new GradualDifficulty(
            ruleset.CreateDifficultyCalculator(new DiffWorkingBeatmap(inner)).CreateGradualDifficulty(
                mods.Select(ruleset.CreateModFromAcronym).Where(mod => mod is not null)
            )
        );
    }

    /// <summary>
    /// Calculate difficulty
    /// </summary>
    public DifficultyAttrs CalculateDifficulty(IEnumerable<string> mods)
    {
        return new(ruleset.CreateDifficultyCalculator(new DiffWorkingBeatmap(inner)).Calculate(
            mods.Select(ruleset.CreateModFromAcronym).Where(mod => mod is not null)
        ));
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
                score.ToPerformanceScoreInfo(inner.BeatmapInfo, ruleset),
                attrs.Inner
            )
        );
    }

    /// <summary>
    /// Parse string osu file into Beatmap
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static Beatmap Parse(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        using var reader = new LineBufferedReader(new MemoryStream(bytes));
        var beatmap = Decoder.GetDecoder<OsuBeatmap>(reader).Decode(reader);
        var rulesetId = beatmap.BeatmapInfo.Ruleset.OnlineID;
        var ruleset = Rulesets.FromLegacyGameMode(rulesetId);
        if (ruleset is null)
        {
            throw new InvalidOperationException("Invalid ruleset: " + rulesetId);
        }

        return new(beatmap, ruleset);
    }
}