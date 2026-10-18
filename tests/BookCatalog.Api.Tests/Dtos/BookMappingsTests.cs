using BookCatalog.Api.Dtos;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Api.Tests.Dtos;

public class BookMappingsTests
{
    [Fact]
    public void ToResponse_CopiesAllFields()
    {
        var book = CreateBook(7, "Dune", "Frank Herbert", 1965);

        var result = book.ToResponse();

        Assert.Equal("Dune", result.Title);
        Assert.Equal("Frank Herbert", result.Author);
        Assert.Equal(1965, result.Year);
    }

    [Fact]
    public void ToEntity_WithId_UsesGivenId()
    {
        var bookRequest = new BookRequest{Title="Dune", Author = "Frank Herbert", Year=1965};

        var result = bookRequest.ToEntity(5);

        Assert.Equal(5, result.Id);
        Assert.Equal("Dune", result.Title);
        Assert.Equal("Frank Herbert", result.Author);
        Assert.Equal(1965, result.Year);

    }
    [Fact]
    public void ToEntity_WithoutId_LeavesIdZero()
    {
        var bookRequest = new BookRequest{Title = "Dune", Author = "Frank Herbert", Year = 1965};

        var result = bookRequest.ToEntity();

        Assert.Equal(0, result.Id);
    }

    private static Book CreateBook(int id, string title, string? author = null, int? year = null) =>
        new() { Id= id, Title = title, Author = author, Year = year };
}