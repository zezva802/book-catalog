using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Api.Entities;

public class Book : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [StringLength(100, MinimumLength = 2)]
    public string? Author { get; set; }

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