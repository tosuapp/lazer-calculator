
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
    public Memory<double> Aim { get; set; }
    public Memory<double> AimWithoutSliders { get; set; }
    public Memory<double> Flashlight { get; set; }
    public Memory<double> Speed { get; set; }
    #endregion

    #region osu!taiko
    public Memory<double> Color { get; set; }
    public Memory<double> Rhythm { get; set; }
    public Memory<double> Reading { get; set; }
    public Memory<double> Stamina { get; set; }
    #endregion

    #region osu!catch
    public Memory<double> Movement { get; set; }
    #endregion

    #region osu!mania
    public Memory<double> Strains { get; set; }
    #endregion

    private void SetStrains(StrainSkill skill)
    {
        var strains = skill.GetCurrentStrainPeaks().ToArray();
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
            case Flashlight _:
                Flashlight = strains;
                break;
            case Speed _:
                Speed = strains;
                break;
            case Colour _:
                Color = strains;
                break;
            case Reading _:
                Reading = strains;
                break;
            case Rhythm _:
                Rhythm = strains;
                break;
            case Stamina _:
                Stamina = strains;
                break;
            case Movement _:
                Movement = strains;
                break;
            case Strain _:
                Strains = strains;
                break;
        }
    }

    internal static StrainsData FromSkills(Skill[] skills)
    {
        var data = new StrainsData();
        foreach (var skill in skills)
        {
            if (skill is StrainSkill strainSkill)
            {
                data.SetStrains(strainSkill);
            }
        }

        return data;
    }
}