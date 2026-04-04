
using System;
using System.Linq;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Catch.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mania.Difficulty.Skills;
using osu.Game.Rulesets.Osu.Difficulty.Skills;
using osu.Game.Rulesets.Taiko.Difficulty.Skills;

namespace tosu.pp.Data;

[JSExport]
public readonly struct StrainsData
{
    #region osu!
    public required Memory<double> Aim { get; init; }
    public required Memory<double> Flashlight { get; init; }
    public required Memory<double> Speed { get; init; }
    #endregion

    #region osu!taiko
    public required Memory<double> Color { get; init; }
    public required Memory<double> Rhythm { get; init; }
    public required Memory<double> Reading { get; init; }
    public required Memory<double> Stamina { get; init; }
    #endregion

    #region osu!catch
    public required Memory<double> Movement { get; init; }
    #endregion

    #region osu!mania
    public required Memory<double> Strains { get; init; }
    #endregion

    internal static StrainsData FromSkills(Skill[] skills) => new()
    {
        Aim = skills.OfType<Aim>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Flashlight = skills.OfType<Flashlight>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Speed = skills.OfType<Speed>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Color = skills.OfType<Colour>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Reading = skills.OfType<Reading>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Rhythm = skills.OfType<Rhythm>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Stamina = skills.OfType<Stamina>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Movement = skills.OfType<Movement>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
        Strains = skills.OfType<Strain>().FirstOrDefault()?.GetObjectStrains().ToArray() ?? [],
    };
}