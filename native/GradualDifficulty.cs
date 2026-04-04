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

    public TimedDifficultyAttrsData? CalculateNext()
    {
        var attrs = inner.CalculateNext();
        if (attrs == null)
            return null;

        return TimedDifficultyAttrsData.FromAttrs(attrs);
    }

    public void SkipToTime(double time)
    {
        inner.SkipToTime(time);
    }

    public void Skip(int offset)
    {
        inner.Skip(offset);
    }
}