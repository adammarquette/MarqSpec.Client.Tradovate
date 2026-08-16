namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate user status.
/// </summary>
public enum UserStatus
{
    /// <summary>Active.</summary>
    Active = 0,

    /// <summary>Closed.</summary>
    Closed,

    /// <summary>Initiated.</summary>
    Initiated,

    /// <summary>Temporarily locked.</summary>
    TemporaryLocked,

    /// <summary>Email is unconfirmed.</summary>
    UnconfirmedEmail,
}
