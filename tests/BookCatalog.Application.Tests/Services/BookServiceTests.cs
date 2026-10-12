using BookCatalog.Application.Abstractions;
using BookCatalog.Application.Services;
using BookCatalog.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BookCatalog.Application.Tests.Services;

public class BookServiceTests
{
    private readonly IBookRepository _repository = Substitute.For<IBookRepository>();
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _sut = new BookService(_repository, NullLogger<BookService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        _repository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await _sut.GetByIdAsync(5, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookExists_ReturnsBook()
    {
        var book = CreateBook(1);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Book?)book);

        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        Assert.Same(book, result);
    }


    [Fact]
    public async Task CreateAsync_ReturnsBookFromRepository()
    {
        var bookIn = CreateBook();
        var bookOut = CreateBook(7);
        _repository.CreateAsync(bookIn, Arg.Any<CancellationToken>()).Returns(bookOut);

        var result = await _sut.CreateAsync(bookIn, CancellationToken.None);

        Assert.Same(bookOut, result);
    }


    [Fact]
    public async Task UpdateAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        var book = CreateBook(1);
        _repository.UpdateAsync(book, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.UpdateAsync(book, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenBookExists_ReturnsTrue()
    {
        var book = CreateBook(1);
        _repository.UpdateAsync(book, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.UpdateAsync(book, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        _repository.DeleteAsync(1, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.DeleteAsync(1, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_PassesIdToRepository()
    {
        await _sut.DeleteAsync(5, CancellationToken.None);
        await _repository.Received(1).DeleteAsync(5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenBookExists_ReturnsTrue()
    {
        _repository.DeleteAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.DeleteAsync(1, CancellationToken.None);

        Assert.True(result);
    }

    private static Book CreateBook(int id = 0) => new() {Id = id, Title = "Vepkhistkaosani", Author = "Shota Rustaveli", Year = 1200};
}