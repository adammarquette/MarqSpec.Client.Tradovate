namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate product.
/// </summary>
/// <remarks>
/// <see cref="ValuePerPoint"/> and <see cref="TickSize"/> are exposed as Tradovate returns them.
/// Do not assume ProjectX's <c>PointValue = TickValue / TickSize</c>.
/// reference: documentation/INDEX.md (Product units)
/// </remarks>
public sealed record Product
{
    /// <summary>Gets the product id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the product name (e.g. <c>ES</c>).</summary>
    public required string Name { get; init; }

    /// <summary>Gets the currency id.</summary>
    public required long CurrencyId { get; init; }

    /// <summary>Gets the product type.</summary>
    public required ProductType ProductType { get; init; }

    /// <summary>Gets the description.</summary>
    public required string Description { get; init; }

    /// <summary>Gets the exchange id.</summary>
    public required long ExchangeId { get; init; }

    /// <summary>Gets the contract group id.</summary>
    public required long ContractGroupId { get; init; }

    /// <summary>Gets the product status.</summary>
    public required ProductStatus Status { get; init; }

    /// <summary>
    /// Gets the currency value of one full price point, as Tradovate's <c>valuePerPoint</c>.
    /// This is not a tick value.
    /// </summary>
    public required decimal ValuePerPoint { get; init; }

    /// <summary>Gets the price format type.</summary>
    public required PriceFormatType PriceFormatType { get; init; }

    /// <summary>Gets the price format precision.</summary>
    public required int PriceFormat { get; init; }

    /// <summary>Gets the product tick size.</summary>
    public required decimal TickSize { get; init; }

    /// <summary>Gets the listed months string, when present.</summary>
    public string? Months { get; init; }

    /// <summary>Gets a value indicating whether the product is secured, when present.</summary>
    public bool? IsSecured { get; init; }
}
