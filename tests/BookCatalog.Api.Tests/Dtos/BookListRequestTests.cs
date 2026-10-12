using System.ComponentModel.DataAnnotations;
using BookCatalog.Api.Dtos;

namespace BookCatalog.Api.Tests.Dtos;

public class BookListRequestTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Validate_WithDefaults_ReturnsNoErrors()
    {
        var request = new BookListRequest();

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageIsBelowOne_ReturnsPageError(int page)
    {
        var request = new BookListRequest { Page = page };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookListRequest.Page)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    public void Validate_WhenPageIsAtLeastOne_ReturnsNoErrors(int page)
    {
        var request = new BookListRequest { Page = page };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WhenPageSizeIsOutOfRange_ReturnsPageSizeError(int pageSize)
    {
        var request = new BookListRequest { PageSize = pageSize };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookListRequest.PageSize)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_WhenPageSizeIsInRange_ReturnsNoErrors(int pageSize)
    {
        var request = new BookListRequest { PageSize = pageSize };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(2000, 1900)]
    [InlineData(-500, -700)]
    public void Validate_WhenYearFromIsAfterYearTo_ReturnsYearError(int yearFrom, int yearTo)
    {
        var request = new BookListRequest { YearFrom = yearFrom, YearTo = yearTo };

        var results = Validate(request);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(BookListRequest.YearFrom)));
    }

    [Theory]
    [InlineData(1900, 1900)]
    [InlineData(1900, 2000)]
    [InlineData(1900, null)]
    [InlineData(null, 1900)]
    [InlineData(null, null)]
    public void Validate_WhenYearRangeIsValidOrPartial_ReturnsNoErrors(int? yearFrom, int? yearTo)
    {
        var request = new BookListRequest { YearFrom = yearFrom, YearTo = yearTo };

        var results = Validate(request);

        Assert.Empty(results);
    }
}