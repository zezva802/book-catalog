using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Domain.Entities;

public class Book : IValidatableObject
{
    /// <summary>
    /// Id is automatically assigned by server.
    /// Any value sent is ignored.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the work.
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Title { get; set; }

    /// <summary>
    /// (Optional) Author of the work.
    /// Cannot be only whitespaces.
    /// </summary>
    [StringLength(100, MinimumLength = 2)]
    public string? Author { get; set; }

    /// <summary>
    /// Year the work was written. Negative numbers are BCE.
    /// Must be between -3500 and next year.
    /// </summary>
    [Range(-3500, int.MaxValue, ErrorMessage = "Year must be no earlier than 3500 BCE (-3500)")]
    public int? Year { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Author != null && string.IsNullOrWhiteSpace(Author))
        {
            yield return new ValidationResult("Author name can not be only spaces", [nameof(Author)]);
        }
        int allowedYear = DateTime.UtcNow.Year + 1;
        if (Year > allowedYear)
        {
            yield return new ValidationResult($"Year must be no later than {allowedYear}", [nameof(Year)]);
        }
    }

}