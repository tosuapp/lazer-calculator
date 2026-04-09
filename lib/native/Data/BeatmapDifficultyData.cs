using Microsoft.JavaScript.NodeApi;
using osu.Game.Beatmaps;

namespace binding;

[JSExport]
public readonly struct BeatmapDifficultyData
{
    /// <summary>
    /// The approach rate of the beatmap.
    /// </summary>
    public float ApproachRate { get; init; }

    /// <summary>
    /// The circle size of the beatmap.
    /// </summary>
    public float CircleSize { get; init; }

    /// <summary>
    /// The overall difficulty of the beatmap.
    /// </summary>
    public float OverallDifficulty { get; init; }

    /// <summary>
    /// The drain rate of the beatmap.
    /// </summary>
    public float DrainRate { get; init; }

    internal static BeatmapDifficultyData FromDifficulty(BeatmapDifficulty diff) => new()
    {
        ApproachRate = diff.ApproachRate,
        CircleSize = diff.CircleSize,
        OverallDifficulty = diff.OverallDifficulty,
        DrainRate = diff.DrainRate,
    };
}