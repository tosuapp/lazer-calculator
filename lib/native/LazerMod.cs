using System.Collections.Generic;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;

namespace binding;

[JSExport]
public struct LazerMod
{
    public string Acronym { get; set; }
    public Dictionary<string, object> Settings { get; set; }

    internal readonly Mod ToMod(Ruleset ruleset)
    {
        var inner = new ModImpl
        {
            Acronym = Acronym,
            Settings = Settings,
        };

        return inner.ToMod(ruleset);
    }

    class ModImpl : IConfiguredMod
    {
        public required string Acronym { get; init; }
        public required IReadOnlyDictionary<string, object> Settings { get; init; }
    }
}