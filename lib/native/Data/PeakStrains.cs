
using System;
using Microsoft.JavaScript.NodeApi;

namespace binding.Data;

/// <summary>
/// Represents the strains value converted to time domain from object strains.
/// </summary>
[JSExport]
public struct PeakStrains
{
    internal PeakStrains(Memory<double> strains, int sectionLength)
    {
        Value = strains;
        SectionLength = sectionLength;
    }

    /// <summary>
    /// The peak strains of each section.
    /// </summary>
    public Memory<double> Value { get; set; }

    /// <summary>
    /// The miliseconds length of each peak strain section.
    /// </summary>
    public int SectionLength { get; set; }
}