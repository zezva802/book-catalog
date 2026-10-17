using BookCatalog.Application.Abstractions;
using Microsoft.Extensions.Logging;
using BookCatalog.Domain.Entities;
using BookCatalog.Application.Common;
using BookCatalog.Application.Queries;

namespace BookCatalog.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookService> _logger;

    public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }

    public async Task<PagedResult<Book>> ListAsync(BookListQuery bookListQuery, CancellationToken cancellationToken)
    {
        return await _bookRepository.ListAsync(bookListQuery, cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogInformation("Book {BookId} not found", id);
            return null;
        }
        return book;
    }

    public async Task<Book> CreateAsync(Book book, CancellationToken cancellationToken)
    {
        var newBook = await _bookRepository.CreateAsync(book, cancellationToken);
        _logger.LogInformation("Created book {BookId}", newBook.Id);
        return newBook;
    }

    public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken)
    {
        var isUpdated = await _bookRepository.UpdateAsync(book, cancellationToken);
        if (!isUpdated)
        {
            _logger.LogInformation("Book {BookId} not found", book.Id);
            return false;
        }
        _logger.LogInformation("Updated book {BookId}", book.Id);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        bool isDeleted = await _bookRepository.DeleteAsync(id, cancellationToken);
        if (isDeleted)
        {
            _logger.LogInformation("Deleted book {BookId}", id);
        }
        else
        {
            _logger.LogInformation("Book {BookId} not found", id);
        }

        return isDeleted;
    }

}