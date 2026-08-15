namespace MarqSpec.Client.Tradovate.Configuration;

/// <summary>
/// Retry options for idempotent REST calls (reads, cancel, modify). Command endpoints are never retried.
/// </summary>
public sealed class RetryOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts. Default is 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay between retries. Default is 1 second.
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum delay between retries. Default is 30 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
}
