using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;

namespace binding.Internal;

internal sealed class DiffWorkingBeatmap : FlatWorkingBeatmap
{
    internal DiffWorkingBeatmap(IBeatmap beatmap) : base(beatmap) { }

    // Bypass beatmap conversion logic since the beatmap is already playable state to given ruleset and mods.
    public override IBeatmap GetPlayableBeatmap(IRulesetInfo ruleset, IReadOnlyList<Mod> mods, CancellationToken token) => Beatmap;
}
