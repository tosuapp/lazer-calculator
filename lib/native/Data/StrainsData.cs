
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
using osu.Game.Rulesets.Osu.Difficulty.Skills;
using osu.Game.Rulesets.Taiko.Difficulty.Skills;

namespace binding.Data;

[JSExport]
public struct StrainsData
{
    /// <summary>
    /// The minimum length of each object strain in milliseconds.
    /// </summary>
    private const double DEFAULT_STRAIN_SECTION_LENGTH = 400;

    #region osu!
    public ObjectStrains Aim { get; set; }
    public ObjectStrains AimWithoutSliders { get; set; }
    
    public ObjectStrains StdReading { get; set; }
    public PeakStrains Flashlight { get; set; }
    public ObjectStrains Speed { get; set; }
    #endregion

    #region osu!taiko
    public PeakStrains TaikoReading { get; set; }
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

    /// <summary>
    /// The times of each difficulty hit object and gap.
    /// </summary>
    public Memory<double> ObjectTimes { get; set; }
    
    /// <summary>
    /// The end time of the last object in the beatmap.
    /// </summary>
    public double LastObjectEndTime { get; set; }

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
                TaikoReading = new PeakStrains(strains, 400);
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
        List<double> mappedStrains = new(ObjectTimes.Length);

        double lastEndTime = -1;
        foreach (var (difficultyHitObject, strain) in difficultyHitObjects.Zip(skill.GetObjectDifficulties()))
        {
            // If the current hit object does not start after minimum strain section length, add 0 strain for the gap.
            if (lastEndTime != -1 && difficultyHitObject.StartTime - lastEndTime > DEFAULT_STRAIN_SECTION_LENGTH)
            {
                mappedStrains.Add(0);
            }

            mappedStrains.Add(strain);
            lastEndTime = difficultyHitObject.EndTime;
        }

        var strains = new ObjectStrains(mappedStrains.ToArray());
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
                StdReading = strains;
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
        var data = new StrainsData
        {
            ObjectTimes = ObjectTimesEnumerable(enumerator.DifficultyHitObjects).ToArray(),
            LastObjectEndTime = enumerator.DifficultyHitObjects.LastOrDefault()?.EndTime ?? 0
        };

        foreach (var skill in enumerator.Skills)
        {
            data.FromSkill(enumerator.DifficultyHitObjects, skill);
        }
        return data;
    }

    private static IEnumerable<double> ObjectTimesEnumerable(DifficultyHitObject[] difficultyHitObjects)
    {
        double lastEndTime = -1;
        foreach (var difficultyHitObject in difficultyHitObjects)
        {
            // If the current hit object does not start after minimum strain section length, yield the last end time with the section length added.
            if (lastEndTime != -1 && difficultyHitObject.StartTime - lastEndTime > DEFAULT_STRAIN_SECTION_LENGTH)
            {
                yield return lastEndTime + DEFAULT_STRAIN_SECTION_LENGTH;
            }

            yield return difficultyHitObject.StartTime;
            lastEndTime = difficultyHitObject.EndTime;
        }
    }
}