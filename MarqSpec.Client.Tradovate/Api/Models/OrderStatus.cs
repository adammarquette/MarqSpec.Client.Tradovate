namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate order status (<c>ordStatus</c>).
/// </summary>
public enum OrderStatus
{
    /// <summary>Unknown.</summary>
    Unknown = 0,

    /// <summary>Canceled.</summary>
    Canceled,

    /// <summary>Completed.</summary>
    Completed,

    /// <summary>Expired.</summary>
    Expired,

    /// <summary>Filled.</summary>
    Filled,

    /// <summary>Pending cancel.</summary>
    PendingCancel,

    /// <summary>Pending new.</summary>
    PendingNew,

    /// <summary>Pending replace.</summary>
    PendingReplace,

    /// <summary>Rejected.</summary>
    Rejected,

    /// <summary>Suspended.</summary>
    Suspended,

    /// <summary>Working.</summary>
    Working,
}
