namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate command result (<c>modify</c> / <c>cancel</c> / <c>liquidate</c>).
/// </summary>
public sealed record CommandResult
{
    /// <summary>Gets the failure reason. Anything other than <see cref="Models.FailureReason.Success"/> is a failed command.</summary>
    public FailureReason? FailureReason { get; init; }

    /// <summary>Gets the failure text, when present.</summary>
    public string? FailureText { get; init; }

    /// <summary>Gets the command id, when present.</summary>
    public long? CommandId { get; init; }
}
