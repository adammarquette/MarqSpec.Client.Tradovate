namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate command <c>failureReason</c> values from swagger <c>CommandResult</c> / place results.
/// </summary>
public enum FailureReason
{
    /// <summary>The command succeeded.</summary>
    Success = 0,

    /// <summary>The account is closed.</summary>
    AccountClosed,

    /// <summary>Advanced trailing stop is unsupported.</summary>
    AdvancedTrailingStopUnsupported,

    /// <summary>Another command is already pending.</summary>
    AnotherCommandPending,

    /// <summary>Back-month trading is prohibited.</summary>
    BackMonthProhibited,

    /// <summary>The execution provider is not configured.</summary>
    ExecutionProviderNotConfigured,

    /// <summary>The execution provider is unavailable.</summary>
    ExecutionProviderUnavailable,

    /// <summary>The contract is invalid.</summary>
    InvalidContract,

    /// <summary>The price is invalid.</summary>
    InvalidPrice,

    /// <summary>The account is liquidation-only.</summary>
    LiquidationOnly,

    /// <summary>The account is liquidation-only before expiration.</summary>
    LiquidationOnlyBeforeExpiration,

    /// <summary>Maximum order quantity is not specified.</summary>
    MaxOrderQtyIsNotSpecified,

    /// <summary>Maximum order quantity was reached.</summary>
    MaxOrderQtyLimitReached,

    /// <summary>Maximum position limit is misconfigured.</summary>
    MaxPosLimitMisconfigured,

    /// <summary>Maximum position limit was reached.</summary>
    MaxPosLimitReached,

    /// <summary>Maximum total position limit was reached.</summary>
    MaxTotalPosLimitReached,

    /// <summary>A multiple-account plan is required.</summary>
    MultipleAccountPlanRequired,

    /// <summary>No quote is available.</summary>
    NoQuote,

    /// <summary>There is not enough liquidity.</summary>
    NotEnoughLiquidity,

    /// <summary>Another execution-related failure.</summary>
    OtherExecutionRelated,

    /// <summary>The parent order was rejected.</summary>
    ParentRejected,

    /// <summary>A risk check timed out.</summary>
    RiskCheckTimeout,

    /// <summary>The session is closed.</summary>
    SessionClosed,

    /// <summary>The command arrived too late.</summary>
    TooLate,

    /// <summary>Trading is locked.</summary>
    TradingLocked,

    /// <summary>Trailing-stop quantity cannot be modified this way.</summary>
    TrailingStopNonOrderQtyModify,

    /// <summary>The caller is unauthorized.</summary>
    Unauthorized,

    /// <summary>An unknown reason.</summary>
    UnknownReason,

    /// <summary>The operation is unsupported.</summary>
    Unsupported,
}
