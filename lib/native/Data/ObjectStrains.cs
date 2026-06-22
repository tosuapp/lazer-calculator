
using System;
using Microsoft.JavaScript.NodeApi;

namespace binding.Data;

/// <summary>
/// Represents the strains values per difficulty hit object and gap.
/// </summary>
[JSExport]
public struct ObjectStrains
{
    internal ObjectStrains(Memory<double> strains)
    {
        Value = strains;
    }

    /// <summary>
    /// The strains of each difficulty hit object and gap.
    /// </summary>
    public Memory<double> Value { get; set; }
}