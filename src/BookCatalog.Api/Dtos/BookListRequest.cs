using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Api.Dtos;

public record BookListRequest : IValidatableObject
{
    /// <summary>
    /// Only books whose title contains this text (case-insensitive)
    /// </summary>
    public string? Title {get; init;}

    /// <summary>
    /// Only books whose author contains this text (case-insensitive)
    /// </summary>
    public string? Author {get; init;}

    /// <summary>
    /// Only books published in or after this year (inclusive). Negative = BCE. Books without a year are excluded.
    /// </summary>
    public int? YearFrom {get; init;}

    /// <summary>
    /// Only books published in or before this year (inclusive). Negative = BCE. Books without a year are excluded.
    /// </summary>
    public int? YearTo {get; init;}

    /// <summary>
    /// Page number, starting at 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page {get; init;} = 1;

    /// <summary>
    /// Size of page, maximum: 100.
    /// </summary>
    [Range(1, 100)]
    public int PageSize {get; init;} = 10;

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if(YearFrom > YearTo)
        {
            yield return new ValidationResult("YearTo cannot be less than YearFrom.", [nameof(YearFrom), nameof(YearTo)]);
        }
    }
}