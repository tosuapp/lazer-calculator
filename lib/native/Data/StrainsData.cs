
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JavaScript.NodeApi;
using Microsoft.Toolkit.HighPerformance;
using osu.Game.Rulesets.Catch.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mania.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty.Skills;
using osu.Game.Rulesets.Taiko.Difficulty.Skills;

namespace binding.Data;

[JSExport]
public struct StrainsData
{
    #region osu!
    public PeakStrains Aim { get; set; }
    public PeakStrains AimWithoutSliders { get; set; }
    public PeakStrains Flashlight { get; set; }
    public PeakStrains Speed { get; set; }
    #endregion

    #region osu! and osu!taiko
    public PeakStrains Reading { get; set; }
    #endregion

    #region osu!taiko
    public PeakStrains Color { get; set; }
    public PeakStrains Rhythm { get; set; }
    public PeakStrains Stamina { get; set; }
    #endregion

    #region osu!catch
    public PeakStrains Movement { get; set; }
    #endregion

    #region osu!mania
    public PeakStrains Strains { get; set; }
    #endregion

    // current strain section length is hardcoded to match with current impl because StrainSkill SectionLength is protected.
    // TODO:: use SectionLength value from StrainSkill.

    private void FromStrainSkill(StrainSkill skill)
    {
        var strains = skill.GetCurrentStrainPeaks().ToArray();
        switch (skill)
        {
            case Flashlight _:
                Flashlight = new PeakStrains(strains, 400);
                break;
            case Colour _:
                Color = new PeakStrains(strains, 400);
                break;
            case osu.Game.Rulesets.Taiko.Difficulty.Skills.Reading _:
                Reading = new PeakStrains(strains, 400);
                break;
            case Rhythm _:
                Rhythm = new PeakStrains(strains, 400);
                break;
            case Stamina _:
                Stamina = new PeakStrains(strains, 400);
                break;
            case Movement _:
                Movement = new PeakStrains(strains, 750);
                break;
            case Strain _:
                Strains = new PeakStrains(strains, 400);
                break;
        }
    }

    private void FromDefaultSkill(DifficultyHitObject[] difficultyHitObjects, Skill skill)
    {
        var objectStrains = skill.GetObjectDifficulties();
        var compatStrainSkill = new CompatStrainSkill(objectStrains);
        foreach (var difficultyHitObject in difficultyHitObjects)
        {
            compatStrainSkill.Process(difficultyHitObject);
        }

        var strains = new PeakStrains(compatStrainSkill.GetCurrentStrainPeaks().ToArray(), 400);
        switch (skill)
        {
            case Aim aim:
                if (aim.IncludeSliders)
                {
                    Aim = strains;
                }
                else
                {
                    AimWithoutSliders = strains;
                }
                break;
            case Speed _:
                Speed = strains;
                break;
            case osu.Game.Rulesets.Osu.Difficulty.Skills.Reading _:
                Reading = strains;
                break;
        }
    }

    private void FromSkill(DifficultyHitObject[] difficultyHitObjects, Skill skill)
    {
        switch (skill)
        {
            case StrainSkill strainSkill:
                FromStrainSkill(strainSkill);
                return;
            default:
                FromDefaultSkill(difficultyHitObjects, skill);
                return;
        }
    }

    internal static StrainsData FromEnumerator(GradualDifficultyEnumerator enumerator)
    {
        var data = new StrainsData();

        foreach (var skill in enumerator.Skills)
        {
            data.FromSkill(enumerator.DifficultyHitObjects, skill);
        }
        return data;
    }

    /// <summary>
    /// A wrapper skill for calculating alternative strain skill values from object difficulties.
    /// </summary>
    private class CompatStrainSkill(IReadOnlyList<double> objectStrains) : StrainSkill([])
    {
        private static readonly double STRAIN_DECAY_BASE = 0.15;
    
        private int currentIndex = 0;
        private double currentStrain = 0;

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current)
            => currentStrain * Math.Pow(STRAIN_DECAY_BASE, (time - current.Previous(0).StartTime) / 1000);

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            return currentStrain = objectStrains[currentIndex++];
        }
    }
}