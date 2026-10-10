using BookCatalog.Application.Abstractions;
using Microsoft.Extensions.Logging;
using BookCatalog.Domain.Entities;

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

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);
        return books;
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogInformation("Get - Not found book {BookId}", id);
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
            _logger.LogInformation("Update - Not found book {BookId}", book.Id);
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
            _logger.LogInformation("Delete - Not found book {BookId}", id);
        }

        return isDeleted;
    }

}