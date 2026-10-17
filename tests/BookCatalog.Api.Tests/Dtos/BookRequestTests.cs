using System.ComponentModel.DataAnnotations;
using BookCatalog.Api.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace BookCatalog.Api.Tests.Dtos;

public class BookRequestTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
    private static List<ValidationResult> Validate(object model)
    {
        var services = new ServiceCollection()
            .AddSingleton<TimeProvider>(new FakeTimeProvider(FixedNow))
            .BuildServiceProvider();

        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, services, items: null);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Validate_WhenRequestIsValid_ReturnsNoErrors()
    {
        var request = new BookRequest { Title = "Dune", Author = "Frank Herbert", Year = 1965 };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_ReturnsTitleError()
    {
        var request = new BookRequest { Title = "" };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookRequest.Title)));
    }

    [Fact]
    public void Validate_WhenTitleIsTooLong_ReturnsTitleError()
    {
        var request = new BookRequest { Title = new string('a', 201) };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookRequest.Title)));
    }

    [Fact]
    public void Validate_WhenTitleIsMax_ReturnsNoErrors()
    {
        var request = new BookRequest { Title = new string('a', 200) };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_WhenAuthorIsWhiteSpace_ReturnsAuthorError()
    {
        var request = new BookRequest { Title = "a", Author = "   " };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookRequest.Author)));
    }

    [Fact]
    public void Validate_WhenAuthorIsTooShort_ReturnsAuthorError()
    {
        var request = new BookRequest { Title = "a", Author = "a" };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookRequest.Author)));
    }

    [Fact]
    public void Validate_WhenYearIsMinimum_ReturnsNoErrors()
    {
        var request = new BookRequest { Title = "a", Year = -3500 };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_WhenYearIsBelowMinimum_ReturnsYearError()
    {
        var request = new BookRequest { Title = "a", Year = -3501 };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookRequest.Year)));
    }

    [Fact]
    public void Validate_WhenYearIsNextYear_ReturnsNoErrors()
    {
        var request = new BookRequest { Title = "a", Year = 2027 };
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Validate_WhenYearIsAfterNextYear_ReturnsYearError()
    {
        var request = new BookRequest { Title = "a", Year = 2028 };
        Assert.Contains(Validate(request), r => r.MemberNames.Contains(nameof(BookRequest.Year)));
    }
}