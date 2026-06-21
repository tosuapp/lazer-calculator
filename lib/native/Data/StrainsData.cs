
using System;
using System.Linq;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Catch.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mania.Difficulty.Skills;
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

    private void FromSkill(Skill skill)
    {
        if (skill is StrainSkill strainSkill)
        {
            FromStrainSkill(strainSkill);
            return;
        }

        var strains = skill.GetObjectDifficulties().ToArray();
        switch (skill)
        {
            case Aim aim:
                if (aim.IncludeSliders)
                {
                    Aim = new PeakStrains(strains, 400);
                }
                else
                {
                    AimWithoutSliders = new PeakStrains(strains, 400);
                }
                break;
            case Speed _:
                Speed = new PeakStrains(strains, 400);
                break;
            case osu.Game.Rulesets.Osu.Difficulty.Skills.Reading _:
                Reading = new PeakStrains(strains, 400);
                break;
        }
    }

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

    internal static StrainsData FromSkills(Skill[] skills)
    {
        var data = new StrainsData();
        foreach (var skill in skills)
        {
            data.FromSkill(skill);
        }

        return data;
    }
}