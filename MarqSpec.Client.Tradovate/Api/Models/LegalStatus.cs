namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate account legal status.
/// </summary>
public enum LegalStatus
{
    /// <summary>Individual.</summary>
    Individual = 0,

    /// <summary>Corporation.</summary>
    Corporation,

    /// <summary>General partnership.</summary>
    GP,

    /// <summary>IRA.</summary>
    IRA,

    /// <summary>Joint.</summary>
    Joint,

    /// <summary>LLC.</summary>
    LLC,

    /// <summary>LLP.</summary>
    LLP,

    /// <summary>Limited partnership.</summary>
    LP,

    /// <summary>Trust.</summary>
    Trust,
}
