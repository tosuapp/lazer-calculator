using System;
using Microsoft.JavaScript.NodeApi;
using osu.Game.Rulesets.Difficulty;
using binding.Data;

namespace binding;

[JSExport]
[method: JSExport(false)]
public class DifficultyAttrs(DifficultyAttributes inner) : IDisposable
{
    [JSExport(false)]
    public DifficultyAttributes? Inner { get; private set; } = inner;

    internal DifficultyAttributes InnerAttrs =>
        Inner ?? throw new ObjectDisposedException(nameof(DifficultyAttrs));

    public DifficultyAttrsData GetData() => DifficultyAttrsData.FromAttrs(InnerAttrs);

    public void Dispose()
    {
        Inner = null;
    }
}
