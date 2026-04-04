using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Difficulty;
using tosu.pp.Data;

namespace tosu.pp;

[JSExport]
public class GradualDifficulty
{
    private readonly GradualDifficultyEnumerator inner;

    internal GradualDifficulty(GradualDifficultyEnumerator inner)
    {
        this.inner = inner;
    }

    public bool MoveNext()
    {
        return inner.MoveNext();
    }

    public DifficultyAttrs CreateDifficultyAttrs()
    {
        return new(inner.CreateDifficultyAttributes());
    }

    public void SkipToTime(double time)
    {
        inner.SkipToTime(time);
    }

    public void Skip(int offset)
    {
        inner.Skip(offset);
    }

    public void SkipToEnd()
    {
        while (inner.MoveNext()) { }
    }

    /// <summary>
    /// Gets the strains of the current difficulty section.
    /// </summary>
    public StrainsData GetCurrentStrains() => StrainsData.FromSkills(inner.Skills);
}