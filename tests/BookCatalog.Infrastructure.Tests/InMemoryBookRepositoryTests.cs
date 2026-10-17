using BookCatalog.Application.Abstractions;
using BookCatalog.Application.Queries;
using BookCatalog.Domain.Entities;
using BookCatalog.Infrastructure;

namespace BookCatalog.Infrastructure.Tests;

public class InMemoryBookRepositoryTests
{
    private readonly IBookRepository _sut = new InMemoryBookRepository(); 

    [Fact]
    public async Task ListAsync_WhenTitleFilterGiven_MatchesCaseInsensitiveSubstring()
    {
        await SeedAsync(
            CreateBook("Dune"),
            CreateBook("Dune Messiah"),
            CreateBook("The Hobbit"));

        var result = await _sut.ListAsync(Query(title: "DUNE"));

        Assert.Equal(new[] { "Dune", "Dune Messiah" }, result.Items.Select(b => b.Title));
        Assert.Equal(2, result.TotalCount);
    }
    [Fact]
    public async Task ListAsync_WithNoFilters_ReturnsAllBooksSortedByTitle()
    {
        await SeedAsync(
            CreateBook("The Hobbit"),
            CreateBook("Dune"),
            CreateBook("Beowulf")
        );
        var result = await _sut.ListAsync(Query());

        Assert.Equal(new[] {"Beowulf", "Dune", "The Hobbit"}, result.Items.Select(b => b.Title));
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenAuthorFilterGiven_ExcludesBooksWithoutAuthor()
    {
        await SeedAsync(
            CreateBook("Dune", "Frank Herbert"),
            CreateBook("Dune Messiah", "Frank Herbert"),
            CreateBook("The Hobbit")
        );

        var result = await _sut.ListAsync(Query(author: "frank"));

        Assert.Equal(new[] {"Dune", "Dune Messiah"}, result.Items.Select(b => b.Title));
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenYearFromGiven_IncludesBoundaryAndExcludesBooksWithoutYear()
    {
        await SeedAsync(
            CreateBook("Dune", year: 1965),
            CreateBook("Dune Messiah", year: 1969),
            CreateBook("Vepkhistkaosani", year: 1200),
            CreateBook("One Thousand and One Nights")
        );

        var result = await _sut.ListAsync(Query(yearFrom: 1965));

        Assert.Equal(new[] {"Dune", "Dune Messiah"}, result.Items.Select(b => b.Title));
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenTitleAndYearToGiven_ReturnsOnlyBooksMatchingBoth()
    {
        await SeedAsync(
            CreateBook("Dune", year: 1965),
            CreateBook("Dune Messiah", year: 1969),
            CreateBook("The Hobbit", year: 1937)
        );

        var result = await _sut.ListAsync(Query(title: "Dune", yearTo: 1965));

        Assert.Equal(new[] {"Dune"}, result.Items.Select(b => b.Title));
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenPaged_TotalCountIsAllMatchesNotPageSize()
    {
        await SeedAsync(
            CreateBook("A"),
            CreateBook("B"),
            CreateBook("C"),
            CreateBook("D"),
            CreateBook("E")
        );

        var result = await _sut.ListAsync(Query(pageSize: 2));

        Assert.Equal(2, result.Items.Count());
        Assert.Equal(5, result.TotalCount);

    }

    [Fact]
    public async Task ListAsync_WhenSecondPageRequested_ReturnsNextItems()
    {
        await SeedAsync(
            CreateBook("A"),
            CreateBook("B"),
            CreateBook("C"),
            CreateBook("D"),
            CreateBook("E")
        );

        var result = await _sut.ListAsync(Query(pageSize: 2, page: 2));

        Assert.Equal(new[] {"C", "D"}, result.Items.Select(b => b.Title));
        Assert.Equal(5, result.TotalCount);
        
    }

    [Fact]
    public async Task ListAsync_WhenPageIsBeyondEnd_ReturnsEmptyItemsWithTotalCount()
    {
        await SeedAsync(
            CreateBook("A"),
            CreateBook("B"),
            CreateBook("C")
        );

        var result = await _sut.ListAsync(Query(pageSize: 1, page: 4));

        Assert.Empty(result.Items);
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task ListAsync_WhenTitlesAreEqual_OrdersById()
    {
        await SeedAsync(
            CreateBook("Dune", year: 2021),
            CreateBook("Dune", year: 1965)
        );

        var result = await _sut.ListAsync(Query());

        Assert.Equal(new int?[] {2021, 1965}, result.Items.Select(b => b.Year));
    }

    private async Task SeedAsync(params Book[] books)
    {
        foreach (var book in books)
            await _sut.CreateAsync(book);
    }

    private static Book CreateBook(string title, string? author = null, int? year = null) =>
        new() { Title = title, Author = author, Year = year };

    private static BookListQuery Query(
        string? title = null, string? author = null, int? yearFrom = null, int? yearTo = null,
        int page = 1, int pageSize = 10) =>
        new(title, author, yearFrom, yearTo, page, pageSize);
}