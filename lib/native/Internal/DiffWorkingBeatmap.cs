using System.Collections.Generic;
using System.IO;
using System.Threading;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Skinning;

namespace binding.Internal;

internal sealed class DiffWorkingBeatmap(IBeatmap beatmap) : WorkingBeatmap(beatmap.BeatmapInfo, null)
{
    private readonly IBeatmap beatmap = beatmap;

    protected override IBeatmap GetBeatmap() => beatmap;

    // Bypass beatmap conversion logic since the beatmap is already playable state to given ruleset and mods.
    public override IBeatmap GetPlayableBeatmap(IRulesetInfo ruleset, IReadOnlyList<Mod> mods, CancellationToken token) => beatmap;

    public override Texture GetBackground() => throw new System.NotImplementedException();
    public override Stream GetStream(string storagePath) => throw new System.NotImplementedException();
    protected override Track GetBeatmapTrack() => throw new System.NotImplementedException();
    protected override ISkin GetSkin() => throw new System.NotImplementedException();
}