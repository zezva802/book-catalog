using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Api.Dtos;

public record BookRequest : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Title{ get; init; }

    [StringLength(100, MinimumLength = 2)]
    public string? Author {get; init;}

    [Range(-3500, int.MaxValue, ErrorMessage = "Year must be no earlier than 3500 BCE (-3500)")]
    public int? Year {get; init;}

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if(Author != null && string.IsNullOrWhiteSpace(Author))
        {
            yield return new ValidationResult("Author name can not be only spaces", [nameof(Author)]);
        }
        var timeProvider = context.GetService<TimeProvider>()
            ?? throw new InvalidOperationException("TimeProvider is not registered.");
        int allowedYear = timeProvider.GetUtcNow().Year + 1;
        if(Year > allowedYear)
        {
            yield return new ValidationResult($"Year must be no later than {allowedYear}", [nameof(Year)]);
        }
    }

}